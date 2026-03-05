using StepanoffGames.DiceRush.Data;
using StepanoffGames.DiceRush.Game;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.DiceRush.Game.Players.Signals;
using StepanoffGames.DiceRush.Game.Ranking.Signals;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Components.Ranking
{
	public class RankingPanel : MonoBehaviour
	{
		[SerializeField] private PlayerItem[] _playerItems;

		private void Start()
		{
			SignalBus.Subscribe<PlayerPlaceChangedSignal>(OnPlayerPlaceChanged);
			SignalBus.Subscribe<PlayerCellPassedSignal>(OnPlayerCellPassed);
			SignalBus.Subscribe<PlayerPortalPassedSignal>(OnPlayerPortalPassed);

			DataManager dataManager = ServiceLocator.Get<DataManager>();
			for (int i = 0; i < _playerItems.Length; i++)
			{
				_playerItems[i].Model = dataManager.Players[i];
			}
		}

		private void OnDestroy()
		{
			SignalBus.Unsubscribe<PlayerPlaceChangedSignal>(OnPlayerPlaceChanged);
			SignalBus.Unsubscribe<PlayerCellPassedSignal>(OnPlayerCellPassed);
			SignalBus.Unsubscribe<PlayerPortalPassedSignal>(OnPlayerPortalPassed);
		}

		private void OnPlayerPlaceChanged(PlayerPlaceChangedSignal signal)
		{
			for (int i = 0; i < _playerItems.Length; i++)
			{
				if (_playerItems[i].Model == signal.Player.Model)
				{
					bool up = signal.PrevPlace > signal.Place;
					_playerItems[i].MoveToPlace(signal.Place, up);
					_playerItems[i].UpdatePlace();
					break;
				}
			}
		}

		private void OnPlayerCellPassed(PlayerCellPassedSignal signal)
		{
			UpdateCellForPlayer(signal.Player);
		}

		private void OnPlayerPortalPassed(PlayerPortalPassedSignal signal)
		{
			UpdateCellForPlayer(signal.Player);
		}

		private void UpdateCellForPlayer(PlayerController player)
		{
			for (int i = 0; i < _playerItems.Length; i++)
			{
				if (_playerItems[i].Model == player.Model)
				{
					int cellIndex = ((Cell)player.Avatar.CurrentPoint).Index;
					_playerItems[i].UpdateCell(cellIndex);
					break;
				}
			}
		}
	}
}
