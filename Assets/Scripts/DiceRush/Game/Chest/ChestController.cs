using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.Robot.UI.Windows.ConfirmWindow;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using StepanoffGames.UI.Windows.Signals;
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

					for (int i = 0; i < cards.Length; i++)
					{
						player.Model.Deck.AddCard(cards[i]);
					}

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

			for (int i = 0; i < cards.Length; i++)
			{
				player.Model.Deck.AddCard(cards[i]);
			}
		}

		private CardModel[] GetCards(PlayerController player)
		{
			CardModel[] diceCards = CardModel.GetCards(CardKind.Dice);
			CardModel[] tileCards = CardModel.GetCards(CardKind.Tile);

			CardModel diceCard = diceCards[Random.Range(0, diceCards.Length)];
			CardModel tileCard = tileCards[Random.Range(0, tileCards.Length)];

			CardModel[] cards = new CardModel[] { diceCard.Clone(), tileCard.Clone() };
			return cards;
		}
	}
}
