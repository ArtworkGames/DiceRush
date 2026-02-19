using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.DiceRush.Game.Players.Signals;
using StepanoffGames.DiceRush.Game.Xp.Signals;
using StepanoffGames.Robot.UI.Windows.ConfirmWindow;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using StepanoffGames.UI.Windows.Signals;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Xp
{
	public class XpManager : MonoBehaviour, IService
	{
		private DataManager _dataManager;

		private float baseXp = 20f;
		private float power = 1.5f;

		private void Awake()
		{
			ServiceLocator.Register(this);
		}

		private void Start()
		{
			_dataManager = ServiceLocator.Get<DataManager>();

			SignalBus.Subscribe<TurnStartedSignal>(OnMoveStarted);
			SignalBus.Subscribe<TurnEndedSignal>(OnMoveEnded);

			SignalBus.Subscribe<PlayerMoveStartedSignal>(OnPlayerMoveStarted);
			SignalBus.Subscribe<PlayerMoveToPortalStartedSignal>(OnPlayerMoveToPortalStarted);
			SignalBus.Subscribe<PlayerCellPassedSignal>(OnPlayerCellPassed);
		}

		private void OnDestroy()
		{
			ServiceLocator.Unregister<XpManager>();

			_dataManager = null;

			SignalBus.Unsubscribe<TurnStartedSignal>(OnMoveStarted);
			SignalBus.Unsubscribe<TurnEndedSignal>(OnMoveEnded);

			SignalBus.Unsubscribe<PlayerMoveStartedSignal>(OnPlayerMoveStarted);
			SignalBus.Unsubscribe<PlayerMoveToPortalStartedSignal>(OnPlayerMoveToPortalStarted);
			SignalBus.Unsubscribe<PlayerCellPassedSignal>(OnPlayerCellPassed);
		}

		private void OnMoveStarted(TurnStartedSignal signal)
		{
			StartTurn();
		}

		private void OnMoveEnded(TurnEndedSignal signal)
		{
			EndTurn();
		}

		private void OnPlayerMoveStarted(PlayerMoveStartedSignal signal)
		{
			IncMultiplier(signal.Player.Model);
		}

		private void OnPlayerMoveToPortalStarted(PlayerMoveToPortalStartedSignal signal)
		{
			IncMultiplier(signal.Player.Model);
		}

		private void OnPlayerCellPassed(PlayerCellPassedSignal signal)
		{
			AddMoveXp(signal.Player.Model, 1);
		}

		private void StartTurn()
		{
			for (int i = 0; i < _dataManager.Players.Count; i++)
			{
				PlayerModel player = _dataManager.Players[i];

				player.XpMultiplier = 0;
				player.MoveXp = 0;

				player.IsXpAdditionCompleted = false;

				SignalBus.Publish(new XpMultiplierChangedSignal(_dataManager.Players[i]));
				SignalBus.Publish(new MoveXpChangedSignal(_dataManager.Players[i]));
			}
		}

		private void IncMultiplier(PlayerModel playerModel)
		{
			playerModel.XpMultiplier += 1;

			SignalBus.Publish(new XpMultiplierChangedSignal(playerModel));
		}

		private void AddMoveXp(PlayerModel playerModel, int xp)
		{
			if (xp == 0) return;

			playerModel.MoveXp += xp;

			SignalBus.Publish(new MoveXpChangedSignal(playerModel));
		}

		private void EndTurn()
		{
			for (int i = 0; i < _dataManager.Players.Count; i++)
			{
				PlayerModel player = _dataManager.Players[i];

				int xp = player.MoveXp * player.XpMultiplier;
				if (xp == 0f)
				{
					player.IsXpAdditionCompleted = true;
					continue;
				}

				player.TotalXp += xp;

				int oldLevel = player.Level;
				UpdateLevel(player);

				if (player.Type == PlayerType.AI)
				{
					int newLevels = player.Level - oldLevel;
					for (int j = 0; j < newLevels; j++)
					{
						AddCards(player);
					}

					player.IsXpAdditionCompleted = true;
				}

				SignalBus.Publish(new TotalXpChangedSignal(_dataManager.Players[i]));
			}
		}

		public async UniTask LevelUp(PlayerModel player)
		{
			CardModel[] cards = GetCards(player);

			bool levelUpWindowClosed = false;
			SignalBus.Publish(new OpenWindowSignal(LevelUpWindow.PrefabName, new LevelUpWindowParams()
			{
				Cards = cards,
				OnSelect = (CardModel card) =>
				{
					//player.Model.Deck.AddCard(card);

					for (int i = 0; i < cards.Length; i++)
					{
						player.Deck.AddCard(cards[i]);
					}

					levelUpWindowClosed = true;
				}
			}));

			await UniTask.WaitUntil(() => levelUpWindowClosed);
		}

		private void AddCards(PlayerModel player)
		{
			CardModel[] cards = GetCards(player);

			//CardModel card = cards[Random.Range(0, cards.Length)];
			//player.Model.Deck.AddCard(card);

			for (int i = 0; i < cards.Length; i++)
			{
				player.Deck.AddCard(cards[i]);
			}
		}

		private CardModel[] GetCards(PlayerModel player)
		{
			CardModel[] diceCards = CardModel.GetCards(CardKind.Dice);
			CardModel[] tileCards = CardModel.GetCards(CardKind.Tile);

			CardModel diceCard = diceCards[Random.Range(0, diceCards.Length)];
			CardModel tileCard = tileCards[Random.Range(0, tileCards.Length)];

			CardModel[] cards = new CardModel[] { diceCard.Clone(), tileCard.Clone() };
			return cards;
		}

		public async UniTask CheckXpAdditionCompleted()
		{
			bool isXpAdditionCompleted = true;
			do
			{
				isXpAdditionCompleted = true;
				for (int i = 0; i < _dataManager.Players.Count; i++)
				{
					PlayerModel player = _dataManager.Players[i];

					if (!player.IsXpAdditionCompleted)
					{
						isXpAdditionCompleted = false;
						break;
					}
				}

				await UniTask.WaitForSeconds(0.1f);
			}
			while (!isXpAdditionCompleted);
		}

		public int GetXpForLevel(int level)
		{
			int xp = (int)Mathf.Round(baseXp * Mathf.Pow(level, power));
			return xp;
		}

		private void UpdateLevel(PlayerModel playerModel)
		{
			int level = playerModel.Level;
			do
			{
				float levelXp = GetXpForLevel(level);
				if (playerModel.TotalXp < levelXp)
				{
					break;
				}
				else
				{
					level++;
				}
			}
			while (true);

			playerModel.Level = level;
		}
	}
}
