using StepanoffGames.Cameras.Signals;
using StepanoffGames.DiceRush.UI.Components;
using StepanoffGames.Scenes.Signals;
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

		private void Awake()
		{
			SignalBus.Publish(new AddCamerasSignal(_cameras));
		}

		private void Start()
		{
			_campaignButton.OnClick += OnCampaignButtonClick;
			_localMultiplayerButton.OnClick += OnLocalMultiplayerButtonClick;
		}

		private void OnCampaignButtonClick()
		{
		}

		private void OnLocalMultiplayerButtonClick()
		{
			SignalBus.Publish(new LoadSceneSignal("LocalLobby"));
		}
	}
}
