using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Map;
using StepanoffGames.DiceRush.Game.Players.Signals;
using StepanoffGames.Signals;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Players
{
	public class PlayerStateIcon : MonoBehaviour
	{
		[SerializeField] private PlayerAvatar _playerAvatar;
		[Space]
		[SerializeField] private Transform _content;
		[Space]
		[SerializeField] private GameObject _waitingIcon;
		[SerializeField] private GameObject _rollDiceIcon;
		[SerializeField] private GameObject _confirmDiceIcon;
		[SerializeField] private GameObject _drawTokenIcon;
		[SerializeField] private GameObject _confirmTokenIcon;
		[SerializeField] private GameObject _selectDirectionIcon;
		[SerializeField] private GameObject _openChestIcon;
		[SerializeField] private GameObject _battleIcon;
		[SerializeField] private GameObject _moveToPortalIcon;
		[SerializeField] private GameObject _endTurnIcon;
		[SerializeField] private GameObject _finishIcon;
		[Space]
		[SerializeField] private GameObject[] _confirmDiceValues;
		[SerializeField] private GameObject[] _confirmTokenValues;

		private void Start()
		{
			SignalBus.Subscribe<PlayerStateChangedSignal>(OnPlayerStateChanged);

			UpdateIcon(PlayerState.Undefined, 1, CellType.Empty);
		}

		private void OnDestroy()
		{
			SignalBus.Unsubscribe<PlayerStateChangedSignal>(OnPlayerStateChanged);
		}

		public void SetPlayerAvatar(PlayerAvatar playerAvatar)
		{
			_playerAvatar = playerAvatar;
		}

		private void OnPlayerStateChanged(PlayerStateChangedSignal signal)
		{
			if (signal.Player.Avatar != _playerAvatar) return;

			UpdateIcon(signal.Player.Model.State, signal.Player.LastDiceValue, signal.Player.LastCellType);
		}

		private void UpdateIcon(PlayerState playerState, int lastDiceValue, CellType lastCellType)
		{
			_content.gameObject.SetActive(
				playerState != PlayerState.Undefined &&
				playerState != PlayerState.MoveForward &&
				playerState != PlayerState.MoveBackward &&
				playerState != PlayerState.MoveToPosition &&
				playerState != PlayerState.CountXp);

			_waitingIcon.SetActive(playerState == PlayerState.Waiting);
			_rollDiceIcon.SetActive(playerState == PlayerState.RollDice);
			_confirmDiceIcon.SetActive(playerState == PlayerState.ConfirmDice);
			_drawTokenIcon.SetActive(playerState == PlayerState.DrawToken);
			_confirmTokenIcon.SetActive(playerState == PlayerState.ConfirmToken);
			_selectDirectionIcon.SetActive(playerState == PlayerState.SelectDirection);
			_openChestIcon.SetActive(playerState == PlayerState.OpenChest);
			_battleIcon.SetActive(playerState == PlayerState.Battle);
			_moveToPortalIcon.SetActive(playerState == PlayerState.MoveToPortal);
			_endTurnIcon.SetActive(playerState == PlayerState.EndTurn);
			_finishIcon.SetActive(playerState == PlayerState.Finish);

			for (int i = 0; i < _confirmDiceValues.Length; i++)
			{
				_confirmDiceValues[i].SetActive(lastDiceValue == (i + 1));
			}

			for (int i = 0; i < _confirmTokenValues.Length; i++)
			{
				_confirmTokenValues[i].SetActive(lastCellType.ToString() == _confirmTokenValues[i].name);
			}
		}
	}
}
