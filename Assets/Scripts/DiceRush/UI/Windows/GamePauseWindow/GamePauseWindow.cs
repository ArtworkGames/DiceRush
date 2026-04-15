using StepanoffGames.DiceRush.UI.Components;
using StepanoffGames.Scenes.Signals;
using StepanoffGames.Signals;
using StepanoffGames.UI.Windows;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StepanoffGames.DiceRush.UI.Windows.GamePauseWindow
{
	public class GamePauseWindowParams : BaseWindowParams
	{
	}

	public class GamePauseWindow : BaseWindow<GamePauseWindowParams>
	{
		public static string PrefabName = "GamePauseWindow";

		[SerializeField] private TweenButton _continueButton;
		[SerializeField] private TweenButton _exitButton;

		private float _oldTimeScale;

		private bool isWindowOpened;

		override protected void BeforeOpen()
		{
			_oldTimeScale = Time.timeScale;
			Time.timeScale = 0f;
		}

		protected override void AfterOpen()
		{
			_continueButton.OnClick += OnContinueButtonClick;
			_exitButton.OnClick += OnExitButtonClick;

			isWindowOpened = true;
		}

		override protected void BeforeClose()
		{
			isWindowOpened = false;

			_continueButton.OnClick -= OnContinueButtonClick;
			_exitButton.OnClick -= OnExitButtonClick;
		}

		override protected void AfterClose()
		{
			Time.timeScale = _oldTimeScale;
		}

		private void Update()
		{
			if (isWindowOpened && Keyboard.current.escapeKey.wasPressedThisFrame)
			{
				CloseWindow();
			}
		}

		private void OnContinueButtonClick()
		{
			CloseWindow();
		}

		private void OnExitButtonClick()
		{
			SignalBus.Publish(new LoadSceneSignal("MainMenu"));
		}
	}
}
