using StepanoffGames.Localization;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using StepanoffGames.Signals;
using StepanoffGames.Scenes.Signals;
//using StepanoffGames.LiteralKnight.Data.Private;
//using StepanoffGames.LiteralKnight.Data.Public;

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
			//_initializationWorker.Register(new PrivateDataManager());

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
