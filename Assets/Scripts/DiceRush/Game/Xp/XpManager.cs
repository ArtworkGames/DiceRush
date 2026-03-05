using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Perks;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.DiceRush.Game.Players.Signals;
using StepanoffGames.DiceRush.Game.Xp.Signals;
using StepanoffGames.DiceRush.UI.Windows.ConfirmWindow;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using StepanoffGames.UI.Windows.Signals;
using System.Collections.Generic;
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
			IncMultiplier(signal.Player.Model);
		}

		private void OnPlayerPortalPassed(PlayerPortalPassedSignal signal)
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
						AddPerk(player).Forget();
					}

					player.IsXpAdditionCompleted = true;
				}

				SignalBus.Publish(new TotalXpChangedSignal(_dataManager.Players[i]));
			}
		}

		public async UniTask LevelUp(PlayerModel player)
		{
			List<PerkModel> perks = GetPerksOffer(player);
			if (perks.Count == 0) return;

			bool levelUpWindowClosed = false;
			PerkModel selectedPerk = null;

			SignalBus.Publish(new OpenWindowSignal(LevelUpWindow.PrefabName, new LevelUpWindowParams()
			{
				Perks = perks,
				OnSelect = (PerkModel perk) =>
				{
					selectedPerk = perk;
					levelUpWindowClosed = true;
				}
			}));

			await UniTask.WaitUntil(() => levelUpWindowClosed);

			if (!selectedPerk.IsSingleUse)
			{
				player.PerksSet.AddPerk(selectedPerk);
				_perksManager.ShowPerks(player);
			}

			PlayerController playerController = _levelManager.GetPlayer(player);
			await _perksManager.UsePerk(playerController, selectedPerk);
		}

		private async UniTask AddPerk(PlayerModel player)
		{
			List<PerkModel> perks = GetPerksOffer(player);
			if (perks.Count == 0) return;

			PerkModel selectedPerk = perks[Random.Range(0, perks.Count)];

			if (!selectedPerk.IsSingleUse)
			{
				player.PerksSet.AddPerk(selectedPerk);
			}

			PlayerController playerController = _levelManager.GetPlayer(player);
			await _perksManager.ApplyPerk(playerController, selectedPerk);
		}

		private List<PerkModel> GetPerksOffer(PlayerModel player)
		{
			List<PerkModel> perks = _perksManager.GetPerksOffer(player);
			return perks;
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
