using StepanoffGames.Cameras.Signals;
using StepanoffGames.DiceRush.Data;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game;
using StepanoffGames.DiceRush.UI.Components;
using StepanoffGames.Scenes.Signals;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using System.Collections.Generic;
using UnityEngine;

namespace StepanoffGames.DiceRush.MainMenu
{
	public class MainMenuManager : MonoBehaviour
	{
		[SerializeField] private List<Camera> _cameras;
		[Space]
		[SerializeField] private TweenButton _campaignButton;
		[SerializeField] private TweenButton _localMultiplayerButton;

		private DataManager _dataManager;

		private void Awake()
		{
			SignalBus.Publish(new AddCamerasSignal(_cameras));
		}

		private void Start()
		{
			_dataManager = ServiceLocator.Get<DataManager>();

			_campaignButton.OnClick += OnCampaignButtonClick;
			_localMultiplayerButton.OnClick += OnLocalMultiplayerButtonClick;
		}

		private void OnDestroy()
		{
			_dataManager = null;
		}

		private void OnCampaignButtonClick()
		{
			List<PlayerModel> players = new List<PlayerModel>();
			players.Add(new PlayerModel("Player 1", PlayerColor.Red, PlayerType.HI));
			players.Add(new PlayerModel("Player 2", PlayerColor.Blue, PlayerType.AI, AIBrainType.Easy));
			players.Add(new PlayerModel("Player 3", PlayerColor.Green, PlayerType.AI, AIBrainType.Easy));
			players.Add(new PlayerModel("Player 4", PlayerColor.Yellow, PlayerType.AI, AIBrainType.Easy));
			_dataManager.SetPlayers(players);

			GameManager.GameMode = GameMode.Tutorial;
			SignalBus.Publish(new LoadSceneSignal("Game"));
		}

		private void OnLocalMultiplayerButtonClick()
		{
			GameManager.GameMode = GameMode.LocalMultiplayer;
			SignalBus.Publish(new LoadSceneSignal("LocalLobby"));
		}
	}
}
