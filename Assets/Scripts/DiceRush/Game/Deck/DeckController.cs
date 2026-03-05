using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Deck.Cards;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.DiceRush.UI.Components.Deck;
using StepanoffGames.Services;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Deck
{
	public class DeckController : MonoBehaviour, IService
	{
		[SerializeField] private DeckPanel _panel;

		private void Awake()
		{
			ServiceLocator.Register(this);
		}

		private void OnDestroy()
		{
			ServiceLocator.Unregister<DeckController>();
		}

		public async UniTask<int> ConfirmDiceRoll(PlayerController player, int diceValue)
		{
			List<CardModel> cardModels = player.Model.Deck.GetCards(CardKind.Dice);
			int totalCardsCount = cardModels.Count;
			cardModels = GetCardsOffer(cardModels, player.Model.CardsPerOffer);

			if (cardModels == null || cardModels.Count == 0)
			{
				return diceValue;
			}

			CardModel selectedCardModel = await _panel.SelectCard(cardModels, totalCardsCount);

			if (selectedCardModel != null)
			{
				player.Model.Deck.RemoveCard(selectedCardModel);

				Card card = GetCardByModel(selectedCardModel);
				diceValue = await card.UseForDice(player, diceValue);
			}
			else
			{
				await UniTask.WaitForSeconds(0.5f);
			}

			return diceValue;
		}

		public async UniTask<int> ApplyDiceRoll(PlayerController player, int diceValue)
		{
			List<CardModel> cardModels = player.Model.Deck.GetCards(CardKind.Dice);
			cardModels = GetCardsOffer(cardModels, player.Model.CardsPerOffer);

			if (cardModels == null || cardModels.Count == 0)
			{
				return diceValue;
			}

			CardModel selectedCardModel = null;

			if (selectedCardModel != null)
			{
				player.Model.Deck.RemoveCard(selectedCardModel);

				Card card = GetCardByModel(selectedCardModel);
				diceValue = card.ApplyForDice(player, diceValue);
			}

			await UniTask.WaitForSeconds(1f);

			return diceValue;
		}

		public async UniTask<CellType> ConfirmTokenDraw(PlayerController player, CellType tileType)
		{
			List<CardModel> cardModels = player.Model.Deck.GetCards(CardKind.Bag);
			int totalCardsCount = cardModels.Count;
			cardModels = GetCardsOffer(cardModels, player.Model.CardsPerOffer);

			if (cardModels == null || cardModels.Count == 0)
			{
				return tileType;
			}

			CardModel selectedCardModel = await _panel.SelectCard(cardModels, totalCardsCount);

			if (selectedCardModel != null)
			{
				player.Model.Deck.RemoveCard(selectedCardModel);

				Card card = GetCardByModel(selectedCardModel);
				tileType = await card.UseForToken(player, tileType);
			}
			else
			{
				await UniTask.WaitForSeconds(0.5f);
			}

			return tileType;
		}

		public async UniTask<CellType> ApplyTokenDraw(PlayerController player, CellType tileType)
		{
			List<CardModel> cardModels = player.Model.Deck.GetCards(CardKind.Bag);
			cardModels = GetCardsOffer(cardModels, player.Model.CardsPerOffer);

			if (cardModels == null || cardModels.Count == 0)
			{
				return tileType;
			}

			CardModel selectedCardModel = null;

			if (selectedCardModel != null)
			{
				player.Model.Deck.RemoveCard(selectedCardModel);

				Card card = GetCardByModel(selectedCardModel);
				tileType = card.ApplyForToken(player, tileType);
			}

			await UniTask.WaitForSeconds(1f);

			return tileType;
		}

		public async UniTask PrepareForBattle(PlayerController player)
		{
			List<CardModel> cardModels = player.Model.Deck.GetCards(CardKind.Battle);
			int totalCardsCount = cardModels.Count;
			cardModels = GetCardsOffer(cardModels, player.Model.CardsPerOffer);

			if (cardModels == null || cardModels.Count == 0)
			{
				return;
			}

			CardModel selectedCardModel = await _panel.SelectCard(cardModels, totalCardsCount);

			if (selectedCardModel != null)
			{
				player.Model.Deck.RemoveCard(selectedCardModel);

				Card card = GetCardByModel(selectedCardModel);
				await card.UseForBattle(player);
			}
			else
			{
				await UniTask.WaitForSeconds(0.5f);
			}
		}

		private Card GetCardByModel(CardModel cardModel)
		{
			Card card = null;
			switch (cardModel.Type)
			{
				case CardType.RerollDice: card = new RerollDiceCard(cardModel); break;
				case CardType.Plus1ToDice:
				case CardType.Plus2ToDice:
				case CardType.Minus1FromDice:
				case CardType.Minus2FromDice: card = new AddValueToDiceCard(cardModel); break;

				case CardType.RedrawToken: card = new RedrawTokenCard(cardModel); break;

				case CardType.Plus1ToHealth:
				case CardType.Plus2ToHealth:
				case CardType.Plus3ToHealth:
				case CardType.Plus1ToDefense:
				case CardType.Plus2ToDefense:
				case CardType.Plus3ToDefense:
				case CardType.Plus1ToAttack:
				case CardType.Plus2ToAttack:
				case CardType.Plus3ToAttack: card = new BattleStatsCard(cardModel); break;
			}
			return card;
		}

		public List<CardModel> GetCardsOffer(List<CardModel> cardModels, int carrdsPerOffer)
		{
			var shuffled = cardModels
				.OrderBy(_ => Random.value)
				.ToList();

			var result = new List<CardModel>();
			var usedTypes = new HashSet<CardType>();

			foreach (var card in shuffled)
			{
				if (!usedTypes.Contains(card.Type))
				{
					result.Add(card);
					usedTypes.Add(card.Type);

					if (result.Count == carrdsPerOffer)
						return result;
				}
			}

			// если уникальных не хватило — добираем из оставшихся
			foreach (var card in shuffled)
			{
				if (result.Count == carrdsPerOffer)
					break;

				if (!result.Contains(card))
					result.Add(card);
			}

			return result;
		}
	}
}
