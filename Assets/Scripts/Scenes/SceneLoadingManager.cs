using Cysharp.Threading.Tasks;
using StepanoffGames.Services;
using StepanoffGames.Scenes.Signals;
using StepanoffGames.Signals;
using StepanoffGames.UI.Windows.Signals;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using StepanoffGames.Cameras.Signals;

namespace StepanoffGames.Scenes
{
	public class SceneLoadingManager : MonoBehaviour, IService
	{
		[SerializeField] private Camera _camera;
		[Space]
		[SerializeField] private ScenesFade _fade;
		[SerializeField] private Image _fadeImage;
		//[SerializeField] private GameObject _background;
		//[SerializeField] private GameObject _logo;

		public Camera Camera => _camera;

		public string CurrentSceneName => _currentSceneName;

		private string previousSceneName = "";
		private string _currentSceneName = "";

		private void Awake()
		{
			ServiceLocator.Register(this);
			SignalBus.Subscribe<LoadSceneSignal>(OnLoadScene);
		}

		private void OnDestroy()
		{
			SignalBus.Unsubscribe<LoadSceneSignal>(OnLoadScene);
			ServiceLocator.Unregister<SceneLoadingManager>();
		}

		private void Start()
		{
			_ = _fade.HideAsync();
		}

		private void OnLoadScene(LoadSceneSignal signal)
		{
			previousSceneName = _currentSceneName;
			_currentSceneName = signal.SceneName;

			LoadSceneAsync();
		}

		private async void LoadSceneAsync()
		{
			//if (!(string.IsNullOrEmpty(previousSceneName) && _currentSceneName.Equals("Lobby")))
			//{
			//	//_logo.SetActive(false);
			//}

			await _fade.ShowAsync();

			SignalBus.Publish(new CloseAllWindowsSignal(true));
			SignalBus.Publish(new ClearCamerasSignal());

			AsyncOperation operation = SceneManager.LoadSceneAsync("Empty");
			await UniTask.WaitUntil(() => operation.isDone);

			Time.timeScale = 1.0f;

			operation = Resources.UnloadUnusedAssets();
			await UniTask.WaitUntil(() => operation.isDone);

			GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);

			await UniTask.WaitForEndOfFrame(this);

			operation = SceneManager.LoadSceneAsync(_currentSceneName, LoadSceneMode.Single);
			await UniTask.WaitUntil(() => operation.isDone);

			//if (!(string.IsNullOrEmpty(previousSceneName) && _currentSceneName.Equals("Lobby")))
			//{
			//	//_logo.SetActive(false);
			//}

			await _fade.HideAsync();

			//_fadeImage.color = Color.black;
			//_background.SetActive(false);
			//_logo.SetActive(false);

			SignalBus.Publish(new SceneShownSignal(_currentSceneName));
		}
	}
}
