using StepanoffGames.DiceRush.UI.Components;
using StepanoffGames.Scenes.Signals;
using StepanoffGames.Signals;
using StepanoffGames.UI.Windows;
using UnityEngine;

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

		override protected void BeforeOpen()
		{
			_oldTimeScale = Time.timeScale;
			Time.timeScale = 0f;
		}

		protected override void AfterOpen()
		{
			_continueButton.OnClick += OnContinueButtonClick;
			_exitButton.OnClick += OnExitButtonClick;
		}

		override protected void BeforeClose()
		{
			_continueButton.OnClick -= OnContinueButtonClick;
			_exitButton.OnClick -= OnExitButtonClick;
		}

		override protected void AfterClose()
		{
			Time.timeScale = _oldTimeScale;
		}

		private void OnContinueButtonClick()
		{
			Debug.Log($"[GamePauseWindow] OnContinueButtonClick");
			CloseWindow();
		}

		private void OnExitButtonClick()
		{
			SignalBus.Publish(new LoadSceneSignal("MainMenu"));
		}
	}
}
