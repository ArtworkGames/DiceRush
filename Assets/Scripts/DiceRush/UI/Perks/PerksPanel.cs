using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Players.Signals;
using StepanoffGames.DiceRush.UI.Components;
using StepanoffGames.Signals;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Perks
{
	public class PerksPanel : MonoBehaviour
	{
		[SerializeField] private HideablePanel _hideablePanel;
		[Space]
		[SerializeField] private GameObject _sourcePerkItem;

		private PlayerModel _player;

		private Dictionary<PerkType, PerkIconItem> _perks;

		private CancellationTokenSource cts;

		private void Awake()
		{
			_perks = new Dictionary<PerkType, PerkIconItem>();
			_sourcePerkItem.SetActive(false);
		}

		private void Start()
		{
			SignalBus.Subscribe<PlayerTurnStartedSignal>(OnPlayerTurnStarted);
			SignalBus.Subscribe<PlayerTurnEndedSignal>(OnPlayerTurnEnded);
		}

		private void OnDestroy()
		{
			cts?.Cancel();
			cts?.Dispose();
			cts = null;

			SignalBus.Unsubscribe<PlayerTurnStartedSignal>(OnPlayerTurnStarted);
			SignalBus.Unsubscribe<PlayerTurnEndedSignal>(OnPlayerTurnEnded);
		}

		private async UniTask Show(CancellationToken ct)
		{
			await _hideablePanel.Show(false, ct);
		}

		private async UniTask Hide(CancellationToken ct)
		{
			await _hideablePanel.Hide(false, ct);
		}

		public async void SetPlayer(PlayerModel player)
		{
			if (_player == player) return;

			cts?.Cancel();
			cts?.Dispose();
			cts = new CancellationTokenSource();

			if (_hideablePanel.IsShown)
			{
				await Hide(cts.Token);
			}

			_player = player;

			ShowPerks();

			await Show(cts.Token);
		}

		public async void ClearPlayer()
		{
			_player = null;
			if (_hideablePanel.IsShown)
			{
				cts?.Cancel();
				cts?.Dispose();
				cts = new CancellationTokenSource();

				await Hide(cts.Token);
			}
		}

		private void OnPlayerTurnStarted(PlayerTurnStartedSignal signal)
		{
			if (signal.Player.Model.Type != PlayerType.HI) return;
			SetPlayer(signal.Player.Model);
		}

		private void OnPlayerTurnEnded(PlayerTurnEndedSignal signal)
		{
			if (signal.Player.Model == _player)
			{
				ClearPlayer();
			}
		}

		private void ShowPerks()
		{
			List<PerkType> keys = new List<PerkType>(_perks.Keys);
			for (int i = 0; i < keys.Count; i++)
			{
				Destroy(_perks[keys[i]].gameObject);
			}
			_perks.Clear();

			for (int i = 0; i < _player.PerksSet.Perks.Count; i++)
			{
				if (_player.PerksSet.Perks[i].Usage == PerkUsage.Multiple)
				{
					AddPerk(_player.PerksSet.Perks[i].Type, true);
				}
			}
		}

		public PerkIconItem AddPerk(PerkType perkType, bool immediately = false)
		{
			GameObject perkObject = Instantiate(_sourcePerkItem, _sourcePerkItem.transform.parent, false);
			perkObject.name = $"PerkItem ({perkType})";
			perkObject.SetActive(true);

			PerkIconItem perk = perkObject.GetComponent<PerkIconItem>();

			if (immediately) perk.Init(perkType);
			else perk.Show(perkType);

			_perks.Add(perkType, perk);

			return perk;
		}

		public PerkIconItem GetPerkItem(PerkType perkType)
		{
			if (_perks.ContainsKey(perkType))
			{
				return _perks[perkType];
			}
			return null;
		}
	}
}
