using Cysharp.Threading.Tasks;
using StepanoffGames.Cameras.Signals;
using StepanoffGames.DiceRush.Data;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Map;
using StepanoffGames.DiceRush.UI.Components;
using StepanoffGames.Scenes.Signals;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StepanoffGames.DiceRush.LocalLobby
{
	public class LocalLobbyManager : MonoBehaviour
	{
		[SerializeField] private List<Camera> _cameras;
		[Space]
		[SerializeField] private TweenButton _backButton;
		[Space]
		[SerializeField] private SelectableButton _easyDifficultyButton;
		[SerializeField] private SelectableButton _mediumDifficultyButton;
		[SerializeField] private SelectableButton _hardDifficultyButton;
		[Space]
		[SerializeField] private SelectableButton _shortLengthButton;
		[SerializeField] private SelectableButton _mediumLengthButton;
		[SerializeField] private SelectableButton _longLengthButton;
		[Space]
		[SerializeField] private GameObject _sourcePlayerPanel;
		[Space]
		[SerializeField] private TweenButton _playButton;
		[SerializeField] private CanvasGroup _playButtonCanvasGroup;

		private AIBrainType _aiBrainType = AIBrainType.Medium;
		private MapLength _mapLength = MapLength.Medium;

		private List<PlayerPanel> _panels;

		private bool isHotKeysLocked;

		private void Awake()
		{
			SignalBus.Publish(new AddCamerasSignal(_cameras));

			_sourcePlayerPanel.SetActive(false);
		}

		private async void Start()
		{
			_backButton.OnClick += OnBackButtonClick;

			_easyDifficultyButton.OnClick += OnEasyDifficultyButtonClick;
			_mediumDifficultyButton.OnClick += OnMediumDifficultyButtonClick;
			_hardDifficultyButton.OnClick += OnHardDifficultyButtonClick;

			_shortLengthButton.OnClick += OnShortLengthButtonClick;
			_mediumLengthButton.OnClick += OnMediumLengthButtonClick;
			_longLengthButton.OnClick += OnLongLengthButtonClick;

			UpdateDifficultyButtons();
			UpdateLengthButtons();

			_panels = new List<PlayerPanel>();
			AddPanel(1, PlayerColor.Red, PlayerType.HI);
			AddPanel(2, PlayerColor.Blue, PlayerType.AI);
			AddPanel(3, PlayerColor.Green, PlayerType.AI);
			AddPanel(4, PlayerColor.Yellow, PlayerType.AI);

			_playButton.OnClick += OnPlayButtonClick;

			isHotKeysLocked = true;
			await UniTask.WaitForSeconds(1f);
			isHotKeysLocked = false;
		}

		private void AddPanel(int playerId, PlayerColor playerColor, PlayerType playerType)
		{
			GameObject panelObject = Instantiate(_sourcePlayerPanel, _sourcePlayerPanel.transform.parent, false);
			panelObject.SetActive(true);

			PlayerPanel panel = panelObject.GetComponent<PlayerPanel>();
			panel.Init(playerId, playerColor, playerType);
			panel.OnPlayerTypeChanged += OnPlayerTypeChanged;
			_panels.Add(panel);
		}

		private void UpdateDifficultyButtons()
		{
			_easyDifficultyButton.Selected = _aiBrainType == AIBrainType.Easy;
			_mediumDifficultyButton.Selected = _aiBrainType == AIBrainType.Medium;
			_hardDifficultyButton.Selected = _aiBrainType == AIBrainType.Hard;
		}

		private void UpdateLengthButtons()
		{
			_shortLengthButton.Selected = _mapLength == MapLength.Short;
			_mediumLengthButton.Selected = _mapLength == MapLength.Medium;
			_longLengthButton.Selected = _mapLength == MapLength.Long;
		}

		private void OnEasyDifficultyButtonClick()
		{
			_aiBrainType = AIBrainType.Easy;
			UpdateDifficultyButtons();
		}

		private void OnMediumDifficultyButtonClick()
		{
			_aiBrainType = AIBrainType.Medium;
			UpdateDifficultyButtons();
		}

		private void OnHardDifficultyButtonClick()
		{
			_aiBrainType = AIBrainType.Hard;
			UpdateDifficultyButtons();
		}

		private void OnShortLengthButtonClick()
		{
			_mapLength = MapLength.Short;
			UpdateLengthButtons();
		}

		private void OnMediumLengthButtonClick()
		{
			_mapLength = MapLength.Medium;
			UpdateLengthButtons();
		}

		private void OnLongLengthButtonClick()
		{
			_mapLength = MapLength.Long;
			UpdateLengthButtons();
		}

		private void OnPlayerTypeChanged()
		{
			List<PlayerModel> playerModels = GetPlayerModels();

			bool isPlayButtonEnabled = playerModels.Count > 0;

			_playButtonCanvasGroup.alpha = isPlayButtonEnabled ? 1f : 0.5f;
			_playButtonCanvasGroup.interactable = isPlayButtonEnabled;
			_playButtonCanvasGroup.blocksRaycasts = isPlayButtonEnabled;
		}

		private List<PlayerModel> GetPlayerModels()
		{
			List<PlayerModel> playerModels = new List<PlayerModel>();
			for (int i = 0; i < _panels.Count; i++)
			{
				PlayerModel playerModel = _panels[i].GetPlayerModel();
				if (playerModel != null)
				{
					playerModel.AIBrainType = _aiBrainType;
					playerModels.Add(playerModel);
				}
			}
			return playerModels;
		}

		private void OnPlayButtonClick()
		{
			List<PlayerModel> playerModels = GetPlayerModels();
			if (playerModels.Count == 0) return;

			DataManager dataManager = ServiceLocator.Get<DataManager>();
			dataManager.SetPlayers(playerModels);
			MapController.MapLength = _mapLength;

			isHotKeysLocked = true;
			SignalBus.Publish(new LoadSceneSignal("Game"));
		}

		private void OnBackButtonClick()
		{
			isHotKeysLocked = true;
			SignalBus.Publish(new LoadSceneSignal("MainMenu"));
		}

		private void Update()
		{
			if (!isHotKeysLocked)
			{
				if (Keyboard.current.escapeKey.wasPressedThisFrame)
				{
					isHotKeysLocked = true;
					OnBackButtonClick();
				}
			}
		}
	}
}
