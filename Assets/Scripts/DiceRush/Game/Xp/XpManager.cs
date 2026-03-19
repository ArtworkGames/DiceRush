using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Perks;
using StepanoffGames.DiceRush.Game.Players.Signals;
using StepanoffGames.DiceRush.Game.Xp.Signals;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Xp
{
	public class XpManager : MonoBehaviour, IService
	{
		private DataManager _dataManager;
		private LevelManager _levelManager;
		private PerksManager _perksManager;

		private float baseXp = 20f;
		private float power = 1.5f;

		private void Awake()
		{
			ServiceLocator.Register(this);
		}

		private void Start()
		{
			_dataManager = ServiceLocator.Get<DataManager>();
			_levelManager = ServiceLocator.Get<LevelManager>();
			_perksManager = ServiceLocator.Get<PerksManager>();

			SignalBus.Subscribe<TurnStartedSignal>(OnMoveStarted);
			SignalBus.Subscribe<TurnEndedSignal>(OnMoveEnded);

			SignalBus.Subscribe<PlayerMoveStartedSignal>(OnPlayerMoveStarted);
			SignalBus.Subscribe<PlayerCellPassedSignal>(OnPlayerCellPassed);
			SignalBus.Subscribe<PlayerPortalPassedSignal>(OnPlayerPortalPassed);
		}

		private void OnDestroy()
		{
			ServiceLocator.Unregister<XpManager>();

			_dataManager = null;
			_levelManager = null;
			_perksManager = null;

			SignalBus.Unsubscribe<TurnStartedSignal>(OnMoveStarted);
			SignalBus.Unsubscribe<TurnEndedSignal>(OnMoveEnded);

			SignalBus.Unsubscribe<PlayerMoveStartedSignal>(OnPlayerMoveStarted);
			SignalBus.Unsubscribe<PlayerCellPassedSignal>(OnPlayerCellPassed);
			SignalBus.Unsubscribe<PlayerPortalPassedSignal>(OnPlayerPortalPassed);
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
			IncMovesCount(signal.Player.Model);
			IncMultiplier(signal.Player.Model);
		}

		private void OnPlayerPortalPassed(PlayerPortalPassedSignal signal)
		{
			IncMovesCount(signal.Player.Model);
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

				player.MovesCount = 0;
				player.MoveXp = 0;
				player.XpMultiplier = 0;

				player.IsXpAdditionCompleted = false;

				SignalBus.Publish(new XpMultiplierChangedSignal(_dataManager.Players[i]));
				SignalBus.Publish(new MoveXpChangedSignal(_dataManager.Players[i]));
			}
		}

		public void IncMovesCount(PlayerModel playerModel)
		{
			playerModel.MovesCount += 1;
		}

		public void IncMultiplier(PlayerModel playerModel)
		{
			playerModel.XpMultiplier += 1;

			SignalBus.Publish(new XpMultiplierChangedSignal(playerModel));
		}

		public void AddMoveXp(PlayerModel playerModel, int xp)
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
						LevelUp(player).Forget();
					}

					player.IsXpAdditionCompleted = true;
				}

				SignalBus.Publish(new TotalXpChangedSignal(_dataManager.Players[i]));
			}
		}

		public async UniTask LevelUp(PlayerModel player)
		{
			if (player.Type == PlayerType.HI)
			{
				await _perksManager.SelectPerk(player);
			}
            else
            {
				_perksManager.AddPerk(player).Forget();
			}
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
