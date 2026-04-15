using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Bag;
using StepanoffGames.DiceRush.Game.Deck.Cards;
using StepanoffGames.DiceRush.Game.Deck.Signals;
using StepanoffGames.DiceRush.Game.Map;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.DiceRush.UI.Deck;
using StepanoffGames.DiceRush.UI.Messages.Signals;
using StepanoffGames.DiceRush.UI.Popups.DiceAndTokenPopup;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using StepanoffGames.UI.Popups.Signals;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Deck
{
	public class DeckController : MonoBehaviour, IService
	{
		[SerializeField] private DeckPanel _panel;

		public DeckPanel Panel => _panel;

		private DataManager _dataManager;
		private BagController _bagController;

		private bool _showEmptyMessages = true;
		private bool _isDiceConfirmButtonAlwaysShown;
		private bool _isTokenConfirmButtonAlwaysShown;

		private void Awake()
		{
			ServiceLocator.Register(this);
		}

		private void Start()
		{
			_dataManager = ServiceLocator.Get<DataManager>();
			_bagController = ServiceLocator.Get<BagController>();
		}

		private void OnDestroy()
		{
			ServiceLocator.Unregister<DeckController>();

			_dataManager = null;
			_bagController = null;
		}

		public void SetCardsPerOffer(PlayerController player, int cardsPerOffer)
		{
			player.Model.CardsPerOffer = cardsPerOffer;
			SignalBus.Publish(new PlayerCardsPerOfferChangedSignal(player.Model));
		}

		public void AddCards(PlayerController player, CardModel[] cards)
		{
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

		public async UniTask<int> ConfirmDiceRoll(PlayerController player, int diceValue, CancellationToken ct)
		{
			return await ConfirmDiceRollInternal(player, diceValue, ct);
		}

		public async UniTask<int> ApplyDiceRoll(PlayerController player, int diceValue, CancellationToken ct)
		{
			return await ConfirmDiceRollInternal(player, diceValue, ct);
		}

		public async UniTask<int> ConfirmDiceRollInternal(PlayerController player, int diceValue, CancellationToken ct)
		{
			List<CardModel> cardModels = player.Model.Deck.GetCards(CardKind.Dice);
			int totalCardsCount = cardModels.Count;
			cardModels = GetCardsOffer(cardModels, player.Model.CardsPerOffer);

			if (cardModels == null || cardModels.Count == 0)
			{
				if (player.Model.Type == PlayerType.HI)
				{
					if (_isDiceConfirmButtonAlwaysShown)
					{
						ShowDiceDescriptionPopup();
						await _panel.SelectCard(cardModels, totalCardsCount, ct);
						SignalBus.Publish(new CloseAllPopupsSignal());
					}
					else
					{
						if (_showEmptyMessages)
						{
							SignalBus.Publish(new ShowMessageSignal("Message:NoCardsForDice"));
						}
						// ???
						await UniTask.WaitForSeconds(0.2f, cancellationToken: ct);
					}
				}
				return diceValue;
			}

			CardModel selectedCardModel = null;
			if (player.Model.Type == PlayerType.HI)
			{
				selectedCardModel = await _panel.SelectCard(cardModels, totalCardsCount, ct);
			}
			else
			{
				List<Card> cards = new List<Card>();
				for (int i = 0; i < cardModels.Count; i++) {
					cards.Add(GetCardByModel(cardModels[i]));
				}
				selectedCardModel = ((AIPlayerController)player).Brain.SelectCardForDice(diceValue, cards);
			}

			if (selectedCardModel != null)
			{
				RemoveCard(player, selectedCardModel);

				Card card = GetCardByModel(selectedCardModel);
				if (player.Model.Type == PlayerType.HI)
				{
					diceValue = await card.UseForDice(player, diceValue, ct);
				}
				else
				{
					diceValue = card.ApplyForDice(player, diceValue);
				}
			}
			else
			{
				// ???
				if (player.Model.Type == PlayerType.HI)
				{
					await UniTask.WaitForSeconds(0.5f, cancellationToken: ct);
				}
			}

			return diceValue;
		}

		public async UniTask<CellType> ConfirmTokenDraw(PlayerController player, CellType tileType, CancellationToken ct)
		{
			return await ConfirmTokenDrawInternal(player, tileType, ct);
		}

		public async UniTask<CellType> ApplyTokenDraw(PlayerController player, CellType tileType, CancellationToken ct)
		{
			return await ConfirmTokenDrawInternal(player, tileType, ct);
		}

		public async UniTask<CellType> ConfirmTokenDrawInternal(PlayerController player, CellType tileType, CancellationToken ct)
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
				if (player.Model.Type == PlayerType.HI)
				{
					if (_isTokenConfirmButtonAlwaysShown)
					{
						ShowTokenDescriptionPopup(tileType);
						await _panel.SelectCard(cardModels, totalCardsCount, ct);
						SignalBus.Publish(new CloseAllPopupsSignal());
					}
					else
					{
						if (_showEmptyMessages)
						{
							SignalBus.Publish(new ShowMessageSignal("Message:NoCardsForBag"));
						}
						// ???
						await UniTask.WaitForSeconds(0.2f, cancellationToken: ct);
					}
				}
				return tileType;
			}

			CardModel selectedCardModel = null;
			if (player.Model.Type == PlayerType.HI)
			{
				selectedCardModel = await _panel.SelectCard(cardModels, totalCardsCount, ct);
			}
			else
			{
			}

			if (selectedCardModel != null)
			{
				RemoveCard(player, selectedCardModel);

				Card card = GetCardByModel(selectedCardModel);
				if (player.Model.Type == PlayerType.HI)
				{
					tileType = await card.UseForToken(player, tileType, ct);
				}
				else
				{
					tileType = card.ApplyForToken(player, tileType);
				}
			}
			else
			{
				// ???
				if (player.Model.Type == PlayerType.HI)
				{
					await UniTask.WaitForSeconds(0.5f, cancellationToken: ct);
				}
			}

			return tileType;
		}

		public async UniTask PrepareForBattle(PlayerController player, CancellationToken ct)
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
				if (player.Model.Type == PlayerType.HI)
				{
					if (_showEmptyMessages)
					{
						SignalBus.Publish(new ShowMessageSignal("Message:NoCardsForBattle"));
					}
					// ???
					await UniTask.WaitForSeconds(0.2f, cancellationToken: ct);
				}
				return;
			}

			CardModel selectedCardModel = await _panel.SelectCard(cardModels, totalCardsCount, ct);

			if (selectedCardModel != null)
			{
				RemoveCard(player, selectedCardModel);

				Card card = GetCardByModel(selectedCardModel);
				await card.UseForBattle(player, ct);
			}
			else
			{
				await UniTask.WaitForSeconds(0.5f, cancellationToken: ct);
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

		public void SetShowEmptyMessages(bool show)
		{
			_showEmptyMessages = show;
		}

		public void SetDiceConfirmButtonAlwaysShown(bool show)
		{
			_isDiceConfirmButtonAlwaysShown = show;
		}

		public void SetTokenConfirmButtonAlwaysShown(bool show)
		{
			_isTokenConfirmButtonAlwaysShown = show;
		}

		private void ShowDiceDescriptionPopup()
		{
			if (!_dataManager.Profile.IsDiceDescriptionPopupShown)
			{
				_dataManager.Profile.IsDiceDescriptionPopupShown = true;
				DiceAndTokenPopup.Show(_bagController.Panel.BagButton.transform, DescriptionType.Dice);
			}
		}

		private void ShowTokenDescriptionPopup(CellType tileType)
		{
			if (tileType == CellType.Reward)
			{
				if (!_dataManager.Profile.IsRewardTokenDescriptionPopupShown)
				{
					_dataManager.Profile.IsRewardTokenDescriptionPopupShown = true;
					DiceAndTokenPopup.Show(_bagController.Panel.BagButton.transform, DescriptionType.RewardToken);
				}
			}
			else if (tileType == CellType.Enemy)
			{
				if (!_dataManager.Profile.IsEnemyTokenDescriptionPopupShown)
				{
					_dataManager.Profile.IsEnemyTokenDescriptionPopupShown = true;
					DiceAndTokenPopup.Show(_bagController.Panel.BagButton.transform, DescriptionType.EnemyToken);
				}
			}
			else if (tileType == CellType.MoveForward)
			{
				if (!_dataManager.Profile.IsMoveForwardTokenDescriptionPopupShown)
				{
					_dataManager.Profile.IsMoveForwardTokenDescriptionPopupShown = true;
					DiceAndTokenPopup.Show(_bagController.Panel.BagButton.transform, DescriptionType.MoveForwardToken);
				}
			}
			else if (tileType == CellType.MoveBackward)
			{
				if (!_dataManager.Profile.IsMoveBackwardTokenDescriptionPopupShown)
				{
					_dataManager.Profile.IsMoveBackwardTokenDescriptionPopupShown = true;
					DiceAndTokenPopup.Show(_bagController.Panel.BagButton.transform, DescriptionType.MoveBackwardToken);
				}
			}
			else if (tileType == CellType.Portal)
			{
				if (!_dataManager.Profile.IsPortalTokenDescriptionPopupShown)
				{
					_dataManager.Profile.IsPortalTokenDescriptionPopupShown = true;
					DiceAndTokenPopup.Show(_bagController.Panel.BagButton.transform, DescriptionType.PortalToken);
				}
			}
		}
	}
}
