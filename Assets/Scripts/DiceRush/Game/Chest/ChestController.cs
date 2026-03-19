using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Deck;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.DiceRush.UI.Windows.ConfirmWindow;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using StepanoffGames.UI.Windows.Signals;
using System.Collections.Generic;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Chest
{
	public class ChestController : MonoBehaviour, IService
	{
		private void Awake()
		{
			ServiceLocator.Register(this);
		}

		private void OnDestroy()
		{
			ServiceLocator.Unregister<ChestController>();
		}

		public async UniTask Open(PlayerController player)
		{
			CardModel[] cards = GetCards(player);

			bool chestWindowClosed = false;
			SignalBus.Publish(new OpenWindowSignal(ChestWindow.PrefabName, new ChestWindowParams()
			{
				Cards = cards,
				OnSelect = (CardModel card) =>
				{
					//player.Model.Deck.AddCard(card);

					DeckController deckController = ServiceLocator.Get<DeckController>();
					deckController.AddCards(player, cards);

					//for (int i = 0; i < cards.Length; i++)
					//{
					//	player.Model.Deck.AddCard(cards[i]);
					//}

					chestWindowClosed = true;
				}
			}));

			await UniTask.WaitUntil(() => chestWindowClosed);
		}

		public void AddCards(PlayerController player)
		{
			CardModel[] cards = GetCards(player);

			//CardModel card = cards[Random.Range(0, cards.Length)];
			//player.Model.Deck.AddCard(card);

			DeckController deckController = ServiceLocator.Get<DeckController>();
			deckController.AddCards(player, cards);

			//for (int i = 0; i < cards.Length; i++)
			//{
			//	player.Model.Deck.AddCard(cards[i]);
			//}
		}

		private CardModel[] GetCards(PlayerController player)
		{
			CardModel[] availableDiceCards = CardModel.GetCards(CardKind.Dice);
			CardModel[] availableBagCards = CardModel.GetCards(CardKind.Bag);
			CardModel[] availableBattleCards = CardModel.GetCards(CardKind.Battle);

			List<CardModel> playerDiceCards = player.Model.Deck.GetCards(CardKind.Dice);
			List<CardModel> playerBagCards = player.Model.Deck.GetCards(CardKind.Bag);
			List<CardModel> playerBattleCards = player.Model.Deck.GetCards(CardKind.Battle);

			CardModel diceCard = SelectCard(availableDiceCards, playerDiceCards);
			CardModel bagCard = SelectCard(availableBagCards, playerBagCards);
			CardModel battleCard = SelectCard(availableBattleCards, playerBattleCards);

			CardModel[] cards = new CardModel[] { diceCard.Clone(), bagCard.Clone(), battleCard.Clone() };
			return cards;
		}

		public CardModel GetCard(PlayerController player)
		{
			List<CardModel> playerDiceCards = player.Model.Deck.GetCards(CardKind.Dice);
			List<CardModel> playerBagCards = player.Model.Deck.GetCards(CardKind.Bag);
			List<CardModel> playerBattleCards = player.Model.Deck.GetCards(CardKind.Battle);

			CardKind cardKind = CardKind.Undefined;
			if ((playerBattleCards.Count < playerDiceCards.Count && playerBattleCards.Count < playerBagCards.Count) ||
				(playerBattleCards.Count == playerDiceCards.Count && playerBattleCards.Count < playerBagCards.Count) ||
				(playerBattleCards.Count < playerDiceCards.Count && playerBattleCards.Count == playerBagCards.Count))
				cardKind = CardKind.Battle;
			else if ((playerDiceCards.Count < playerBagCards.Count && playerDiceCards.Count < playerBattleCards.Count) ||
					 (playerDiceCards.Count == playerBagCards.Count && playerDiceCards.Count < playerBattleCards.Count) ||
					 (playerDiceCards.Count == playerBagCards.Count && playerDiceCards.Count == playerBattleCards.Count))
				cardKind = CardKind.Dice;
			else
				cardKind = CardKind.Bag;

			Debug.Log($"[ChestController] GetCard: " +
				$"playerDiceCards.Count = {playerDiceCards.Count}, " +
				$"playerBagCards.Count = {playerBagCards.Count}, " +
				$"playerBattleCards.Count = {playerBattleCards.Count}, " +
				$"cardKind = {cardKind}");

			CardModel[] availableCards = new CardModel[0];
			List<CardModel> playerCards = null;
			switch (cardKind)
			{
				case CardKind.Dice:
					availableCards = CardModel.GetCards(CardKind.Dice);
					playerCards = playerDiceCards;
					break;
				case CardKind.Bag:
					availableCards = CardModel.GetCards(CardKind.Bag);
					playerCards = playerBagCards;
					break;
				case CardKind.Battle:
					availableCards = CardModel.GetCards(CardKind.Battle);
					playerCards = playerBattleCards;
					break;
			}

			CardModel card = SelectCard(availableCards, playerCards);
			return card.Clone();
		}

		private CardModel SelectCard(CardModel[] availableCards, List<CardModel> playerCards)
		{
			List<CardModel> selectedCards = new List<CardModel>();

			for (int i = 0; i < availableCards.Length; i++)
			{
				CardModel card = availableCards[i];
				bool exists = false;

				for (int j = 0; j < playerCards.Count; j++)
				{
					if (playerCards[j].Type == card.Type)
					{
						exists = true;
						break;
					}
				}

				if (!exists)
				{
					selectedCards.Add(card);
				}
			}

			CardModel selectedCard = null;
			if (selectedCards.Count > 0)
			{
				selectedCard = selectedCards[Random.Range(0, selectedCards.Count)];
			}
			else
			{
				selectedCard = availableCards[Random.Range(0, availableCards.Length)];
			}

			return selectedCard;
		}
	}
}
