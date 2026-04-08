using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Deck;
using StepanoffGames.DiceRush.Game.Map;
using StepanoffGames.DiceRush.Game.Perks;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.DiceRush.UI.Perks;
using StepanoffGames.DiceRush.UI.Popups.FlyingIconPopup;
using StepanoffGames.Services;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace StepanoffGames.DiceRush.Game.Chest
{
	public class ChestController : MonoBehaviour, IService
	{
		private GameManager _gameManager;
		private DeckController _deckController;
		private PerksManager _perksManager;

		private void Awake()
		{
			ServiceLocator.Register(this);
		}

		private void Start()
		{
			_gameManager = ServiceLocator.Get<GameManager>();
			_deckController = ServiceLocator.Get<DeckController>();
			_perksManager = ServiceLocator.Get<PerksManager>();
		}

		private void OnDestroy()
		{
			ServiceLocator.Unregister<ChestController>();

			_gameManager = null;
			_deckController = null;
			_perksManager = null;
		}

		public async UniTask Open(PlayerController player, CancellationToken ct)
		{
			CardModel[] cards = GetCards(player);

			//bool chestWindowClosed = false;
			//SignalBus.Publish(new OpenWindowSignal(ChestWindow.PrefabName, new ChestWindowParams()
			//{
			//	Cards = cards,
			//	OnSelect = (CardModel card) =>
			//	{
			//		//player.Model.Deck.AddCard(card);

			//		_deckController.AddCards(player, cards);

			//		//for (int i = 0; i < cards.Length; i++)
			//		//{
			//		//	player.Model.Deck.AddCard(cards[i]);
			//		//}

			//		chestWindowClosed = true;
			//	}
			//}));

			//await UniTask.WaitUntil(() => chestWindowClosed);


			Vector3 worldPos = ((Cell)player.Avatar.CurrentPoint).Drawer.ChestRewardPosition.position;
			Vector2 scrPos = _gameManager.Camera.Camera.WorldToScreenPoint(worldPos);
			worldPos = _gameManager.UICamera.ScreenToWorldPoint(scrPos);

			GameObject iconObject = await LoadPerkIcon(ct);
			iconObject.transform.position = worldPos;
			iconObject.transform.localPosition = new Vector3(iconObject.transform.localPosition.x, iconObject.transform.localPosition.y, 0f);

			FlyingPerkTarget flyingPerkTarget = _perksManager.GetFlyingPerkTarget(PerkType.Take3Cards);

			bool isCompleted = false;
			FlyingIconPopup.Show(iconObject, flyingPerkTarget.transform, null, () =>
			{
				_deckController.AddCards(player, cards);

				Destroy(iconObject);
				isCompleted = true;
			});

			await UniTask.WaitUntil(() => isCompleted, cancellationToken: ct);
		}

		private async UniTask<GameObject> LoadPerkIcon(CancellationToken ct)
		{
			string perkName = $"Take3CardsPerkIcon";
			string perkPath = $"UI/Perks/{perkName}.prefab";
			var handle = Addressables.LoadAssetAsync<GameObject>(perkPath);
			await UniTask.WaitUntil(() => handle.IsDone);

			GameObject iconObject = Instantiate(handle.Result, _perksManager.Panel.transform, false);
			iconObject.SetActive(false);
			iconObject.name = perkName;
			iconObject.transform.localScale = Vector3.one * 2.3f;

			return iconObject;
		}

		public void AddCards(PlayerController player)
		{
			CardModel[] cards = GetCards(player);
			_deckController.AddCards(player, cards);
		}

		private CardModel[] GetCards(PlayerController player)
		{
			CardModel[] availableDiceCards = CardModel.GetCards(CardKind.Dice);
			CardModel[] availableBagCards = CardModel.GetCards(CardKind.Bag);
			CardModel[] availableBattleCards = CardModel.GetCards(CardKind.Battle);

			List<CardModel> playerDiceCards = player.Model.Deck.GetCards(CardKind.Dice);
			List<CardModel> playerBagCards = player.Model.Deck.GetCards(CardKind.Bag);
			List<CardModel> playerBattleCards = player.Model.Deck.GetCards(CardKind.Battle);

			CardModel diceCard = SelectCard(player, availableDiceCards, playerDiceCards);
			CardModel bagCard = SelectCard(player, availableBagCards, playerBagCards);
			CardModel battleCard = SelectCard(player, availableBattleCards, playerBattleCards);

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

			CardModel card = SelectCard(player, availableCards, playerCards);
			return card.Clone();
		}

		private CardModel SelectCard(PlayerController player, CardModel[] availableCards, List<CardModel> playerCards)
		{
			List<CardModel> selectedCards = new List<CardModel>();

			for (int i = 0; i < availableCards.Length; i++)
			{
				CardModel card = availableCards[i];
				if (player.Model.Type == PlayerType.AI && card.Type == CardType.RerollDice) continue;

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
