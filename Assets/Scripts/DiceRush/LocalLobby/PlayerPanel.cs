using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.UI.Components;
using StepanoffGames.UI.Components;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace StepanoffGames.DiceRush.LocalLobby
{
	public class PlayerPanel : MonoBehaviour
	{
		public Action OnPlayerTypeChanged;

		[SerializeField] private Transform _imageParent;
		[Space]
		[SerializeField] private TMPTextLocalizer _nameLocalizer;
		[Space]
		[SerializeField] private SelectableButton _hiButton;
		[SerializeField] private SelectableButton _aiButton;
		[SerializeField] private SelectableButton _noButton;

		private PlayerColor _playerColor;
		private PlayerType _playerType;

		public void Init(int playerId, PlayerColor playerColor, PlayerType playerType)
		{
			_playerColor = playerColor;
			_playerType = playerType;

			_nameLocalizer.SetParams(playerId.ToString());

			_hiButton.OnClick += OnHIButtonClick;
			_aiButton.OnClick += OnAIButtonClick;
			_noButton.OnClick += OnNoButtonClick;

			UpdateButtons();

			LoadImage().Forget();
		}

		private async UniTask LoadImage()
		{
			string imageName = $"{_playerColor}Player";
			string imagePath = $"Windows/BattleWindow/Players/{imageName}.prefab";
			var handle = Addressables.LoadAssetAsync<GameObject>(imagePath);
			await UniTask.WaitUntil(() => handle.IsDone);

			GameObject imageObject = Instantiate(handle.Result, _imageParent, false);
			imageObject.name = imageName;
		}

		private void UpdateButtons()
		{
			_hiButton.Selected = _playerType == PlayerType.HI;
			_aiButton.Selected = _playerType == PlayerType.AI;
			_noButton.Selected = _playerType == PlayerType.Undefined;
		}

		private void OnHIButtonClick()
		{
			_playerType = PlayerType.HI;
			UpdateButtons();

			OnPlayerTypeChanged?.Invoke();
		}

		private void OnAIButtonClick()
		{
			_playerType = PlayerType.AI;
			UpdateButtons();

			OnPlayerTypeChanged?.Invoke();
		}

		private void OnNoButtonClick()
		{
			_playerType = PlayerType.Undefined;
			UpdateButtons();

			OnPlayerTypeChanged?.Invoke();
		}

		public PlayerModel GetPlayerModel()
		{
			PlayerModel playerModel = null;
			if (_playerType != PlayerType.Undefined)
			{
				playerModel = new PlayerModel(_nameLocalizer.GetText(), _playerColor, _playerType);
			}
			return playerModel;
		}
	}
}
