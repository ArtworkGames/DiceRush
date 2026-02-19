using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data;
using StepanoffGames.Localization;
using StepanoffGames.Scenes.Signals;
using StepanoffGames.Signals;
using UnityEngine;
using UnityEngine.UI;

namespace StepanoffGames.Initialization
{
	public class GameLoadingManager : MonoBehaviour
	{
		[SerializeField] private Slider _progressSlider;

		private InitializationWorker _initializationWorker;

		private async void Start()
		{
			_progressSlider.value = 0.0f;

			await UniTask.NextFrame();

			_initializationWorker = new InitializationWorker();

			_initializationWorker.Register(new AppSettingsManager());
			_initializationWorker.Register(new LocalizationManager());
			//_initializationWorker.Register(new PublicDataManager());
			_initializationWorker.Register(new DataManager());

			await _initializationWorker.InitializeAllAsync();

			SignalBus.Publish(new LoadSceneSignal("Game"));
		}

		private void Update()
		{
			if (_initializationWorker != null)
			{
				float progress = _initializationWorker.GetInitializationProgress();
				if (!float.IsNaN(progress))
				{
					progress = Mathf.Clamp01(progress);
					_progressSlider.value = progress;
				}
			}
		}
	}
}
