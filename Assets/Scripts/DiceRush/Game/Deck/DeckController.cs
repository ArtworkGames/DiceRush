using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Bag;
using StepanoffGames.DiceRush.Game.Deck.Cards;
using StepanoffGames.DiceRush.Game.Deck.Signals;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.DiceRush.UI.Components.Deck;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Deck
{
	public class DeckController : MonoBehaviour, IService
	{
		[SerializeField] private DeckPanel _panel;

		private BagController _bagController;

		private void Awake()
		{
			ServiceLocator.Register(this);
		}

		private void Start()
		{
			_bagController = ServiceLocator.Get<BagController>();
		}

		private void OnDestroy()
		{
			ServiceLocator.Unregister<DeckController>();

			_bagController = null;
		}

		public void SetCardsPerOffer(PlayerController player, int cardsPerOffer)
		{
			player.Model.CardsPerOffer = cardsPerOffer;
			SignalBus.Publish(new PlayerCardsPerOfferChangedSignal(player.Model));
		}

		public void AddCards(PlayerController player, CardModel[] cards)
		{
			Debug.Log($"[DeckController] AddCards: {cards.Length}");
			for (int i = 0; i < cards.Length; i++)
			{
				player.Model.Deck.AddCard(cards[i]);
			}
			SignalBus.Publish(new PlayerCardsChangedSignal(player.Model));
		}

		public void RemoveCard(PlayerController player, CardModel card)
		{
			player.Model.Deck.RemoveCard(card);
			SignalBus.Publish(new PlayerCardsChangedSignal(player.Model));
		}

		public async UniTask<int> ConfirmDiceRoll(PlayerController player, int diceValue)
		{
			_panel.DeckButton.SetPlayer(player.Model);
			return await ConfirmDiceRollInternal(player, true, diceValue);
		}

		public async UniTask<int> ApplyDiceRoll(PlayerController player, int diceValue)
		{
			return await ConfirmDiceRollInternal(player, false, diceValue);
		}

		public async UniTask<int> ConfirmDiceRollInternal(PlayerController player, bool isHuman, int diceValue)
		{
			List<CardModel> cardModels = player.Model.Deck.GetCards(CardKind.Dice);
			int totalCardsCount = cardModels.Count;
			cardModels = GetCardsOffer(cardModels, player.Model.CardsPerOffer);

			if (cardModels == null || cardModels.Count == 0)
			{
				return diceValue;
			}

			CardModel selectedCardModel = null;
			if (player.Model.Type == PlayerType.HI)
			{
				selectedCardModel = await _panel.SelectCard(cardModels, totalCardsCount);
			}
			else
			{
			}

			if (selectedCardModel != null)
			{
				RemoveCard(player, selectedCardModel);

				Card card = GetCardByModel(selectedCardModel);
				if (isHuman)
				{
					diceValue = await card.UseForDice(player, diceValue);
				}
				else
				{
					diceValue = card.ApplyForDice(player, diceValue);
				}
			}
			else
			{
				// ???
				if (isHuman)
				{
					await UniTask.WaitForSeconds(0.5f);
				}
			}

			// ???
			if (!isHuman)
			{
				await UniTask.WaitForSeconds(1f);
			}

			return diceValue;
		}

		public async UniTask<CellType> ConfirmTokenDraw(PlayerController player, CellType tileType)
		{
			_panel.DeckButton.SetPlayer(player.Model);
			return await ConfirmTokenDrawInternal(player, true, tileType);
		}

		public async UniTask<CellType> ApplyTokenDraw(PlayerController player, CellType tileType)
		{
			return await ConfirmTokenDrawInternal(player, false, tileType);
		}

		public async UniTask<CellType> ConfirmTokenDrawInternal(PlayerController player, bool isHuman, CellType tileType)
		{
			List<CardModel> rawCardModels = player.Model.Deck.GetCards(CardKind.Bag);
			int totalCardsCount = rawCardModels.Count;

			List<CardModel> cardModels = new List<CardModel>();
			for (int i = 0; i < rawCardModels.Count; i++)
			{
				if (rawCardModels[i].Type == CardType.ReplaceTokenWithEnemy &&
					_bagController.CurrentCellType == CellType.Enemy)
				{
					continue;
				}
				cardModels.Add(rawCardModels[i]);
			}

			cardModels = GetCardsOffer(cardModels, player.Model.CardsPerOffer);

			if (cardModels == null || cardModels.Count == 0)
			{
				return tileType;
			}

			CardModel selectedCardModel = null;
			if (isHuman)
			{
				selectedCardModel = await _panel.SelectCard(cardModels, totalCardsCount);
			}
			else
			{
			}

			if (selectedCardModel != null)
			{
				RemoveCard(player, selectedCardModel);

				Card card = GetCardByModel(selectedCardModel);
				if (isHuman)
				{
					tileType = await card.UseForToken(player, tileType);
				}
				else
				{
					tileType = card.ApplyForToken(player, tileType);
				}
			}
			else
			{
				// ???
				if (isHuman)
				{
					await UniTask.WaitForSeconds(0.5f);
				}
			}

			// ???
			if (!isHuman)
			{
				await UniTask.WaitForSeconds(1f);
			}

			return tileType;
		}

		public async UniTask PrepareForBattle(PlayerController player)
		{
			List<CardModel> rawCardModels = player.Model.Deck.GetCards(CardKind.Battle);
			int totalCardsCount = rawCardModels.Count;

			List<CardModel> cardModels = new List<CardModel>();
			for (int i = 0; i < rawCardModels.Count; i++)
			{
				if ((rawCardModels[i].Type == CardType.Plus1ToHealth ||
					rawCardModels[i].Type == CardType.Plus2ToHealth ||
					rawCardModels[i].Type == CardType.Plus3ToHealth) &&
					player.Model.Health == player.Model.MaxHealth)
				{
					continue;
				}
				cardModels.Add(rawCardModels[i]);
			}

			cardModels = GetCardsOffer(cardModels, player.Model.CardsPerOffer);

			if (cardModels == null || cardModels.Count == 0)
			{
				return;
			}

			CardModel selectedCardModel = await _panel.SelectCard(cardModels, totalCardsCount);

			if (selectedCardModel != null)
			{
				RemoveCard(player, selectedCardModel);

				Card card = GetCardByModel(selectedCardModel);
				await card.UseForBattle(player);
			}
			else
			{
				await UniTask.WaitForSeconds(0.5f);
			}
		}

		private List<CardModel> GetCardsOffer(List<CardModel> cardModels, int cardsPerOffer)
		{
			List<CardModel> shuffled = cardModels
				.OrderBy(_ => Random.value)
				.ToList();

			List<CardModel> result = new List<CardModel>();
			HashSet<CardType> usedTypes = new HashSet<CardType>();

			for (int i = 0; i < shuffled.Count; i++)
			{
				CardModel card = shuffled[i];
				if (!usedTypes.Contains(card.Type))
				{
					result.Add(card);
					usedTypes.Add(card.Type);

					if (result.Count == cardsPerOffer)
						return result;
				}
			}

			// если уникальных не хватило — добираем из оставшихся
			for (int i = 0; i < shuffled.Count; i++)
			{
				CardModel card = shuffled[i];
				if (!result.Contains(card))
					result.Add(card);

				if (result.Count == cardsPerOffer)
					break;
			}

			return result;
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

				case CardType.ReplaceTokenWithEnemy: card = new ReplaceTokenWithEnemyCard(cardModel); break;
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
	}
}
