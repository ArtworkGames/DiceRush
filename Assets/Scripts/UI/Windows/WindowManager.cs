using Cysharp.Threading.Tasks;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using StepanoffGames.UI.Windows.Behaviours;
using StepanoffGames.UI.Windows.Signals;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace StepanoffGames.UI.Windows
{
	public class WindowManager : MonoBehaviour, IService
	{
		[SerializeField] private GameObject _uiRoot;
		[SerializeField] private Transform _windowsParent;

		private WindowsQueue _queue;

		private bool _openingLock;

		protected void Awake()
		{
			DontDestroyOnLoad(_uiRoot);

			ServiceLocator.Register(this);

			SignalBus.Subscribe<OpenWindowSignal>(OnOpenWindow);
			SignalBus.Subscribe<CloseWindowSignal>(OnCloseWindow);
			SignalBus.Subscribe<CloseAllWindowsSignal>(OnCloseAllWindows);

			_queue = new WindowsQueue();
		}

		private void OnDestroy()
		{
			SignalBus.Unsubscribe<OpenWindowSignal>(OnOpenWindow);
			SignalBus.Unsubscribe<CloseWindowSignal>(OnCloseWindow);
			SignalBus.Unsubscribe<CloseAllWindowsSignal>(OnCloseAllWindows);

			ServiceLocator.Unregister<WindowManager>();
		}

		public bool HasOpenWindow()
		{
			return _queue.OpenedWindows.Count > 0;
		}

		public bool HasWindowInQueue()
		{
			return _queue.Queue.Count > 0;
		}

		public T GetOpenedWindow<T>() where T : BaseWindow
		{
			if (HasOpenWindow() == false)
			{
				return default;
			}

			BaseWindowBehaviour result = _queue.OpenedWindows.FirstOrDefault(x => x.Window is T);
			if (result == default)
			{
				return default;
			}

			return (T)result.Window;
		}

		private void OnOpenWindow(OpenWindowSignal signal)
		{
			if ((signal.BehaviourType != null) && !signal.BehaviourType.IsSubclassOf(typeof(BaseWindowBehaviour)))
			{
				Debug.LogError($"BehaviourType {signal.BehaviourType} must be subclass of {typeof(BaseWindowBehaviour)}");
				return;
			}

			if (_queue.Queue.Any(x => x.WindowName == signal.WindowName))
			{
				return;
			}

			BaseWindowBehaviour behaviour = CreateBehaviour(signal.BehaviourType, signal.WindowName, signal.Params, signal.Immediately, signal.Parent);
			OpenWindowAsync(behaviour);
		}

		private BaseWindowBehaviour CreateBehaviour(Type behavioursType, string windowName, BaseWindowParams @params, bool immediately, Transform parent)
		{
			BaseWindowBehaviour behaviour;
			if (behavioursType == null)
			{
				behaviour = new BaseWindowBehaviour();
			}
			else
			{
				behaviour = (BaseWindowBehaviour)Activator.CreateInstance(behavioursType);
			}

			behaviour.Queue = _queue;
			behaviour.WindowName = windowName;
			behaviour.Params = @params;
			behaviour.OpenImmediately = immediately;
			behaviour.Parent = parent;
			return behaviour;
		}

		private async void OpenWindowAsync(BaseWindowBehaviour behaviour)
		{
			_queue.Queue.Add(behaviour);

			behaviour.Window = _queue.GetWindowInstance(behaviour.WindowName);
			if (behaviour.Window == null)
			{
				await InstantiateWindowAsync(behaviour);
			}

			TryOpenNextAsync().Forget();
		}

		private async UniTask InstantiateWindowAsync(BaseWindowBehaviour behaviour)
		{
			string prefabName = GetPrefabPath(behaviour.WindowName);
			var handle = Addressables.LoadAssetAsync<GameObject>(prefabName);
			await UniTask.WaitUntil(() => handle.IsDone);

			Transform parent = _windowsParent;
            if (behaviour.Parent != null)
            {
				parent = behaviour.Parent;
			}

            GameObject instance = Instantiate(handle.Result, parent);
			instance.name = behaviour.WindowName;
			instance.SetActive(false);

			BaseWindow window = instance.GetComponent<BaseWindow>();
			behaviour.Window = window;

			_queue.AddWindowInstance(behaviour.WindowName, window);
		}

		private string GetPrefabPath(string name)
		{
			return $"Windows/{name}.prefab";
		}

		private async UniTask TryOpenNextAsync()
		{
			if (_queue.Queue.Count == 0 || _openingLock)
			{
				return;
			}

			_openingLock = true;

			await UniTask.Yield();

			bool openedAny = false;

			List<BaseWindowBehaviour> queueClone = new List<BaseWindowBehaviour>();
			queueClone.AddRange(_queue.Queue);

			foreach (BaseWindowBehaviour behaviour in queueClone)
			{
				if (behaviour.Window == null)
				{
					continue;
				}

				if (_queue.OpenedWindows.Count > 0 && !behaviour.CanOpenOverOtherWindows)
				{
					continue;
				}

				if (behaviour.CancelOpening)
				{
					_queue.Queue.Remove(behaviour);
					continue;
				}

				await behaviour.OpenAsync();

				_queue.OpenedWindows.Add(behaviour);
				_queue.Queue.Remove(behaviour);

				SignalBus.Publish(new WindowOpenedSignal(behaviour.WindowName));

				if (behaviour.CancelOpening)
				{
					behaviour.Window.CloseWindow();
				}

				openedAny = true;
				break;
			}

			_openingLock = false;

			if (openedAny)
			{
				TryOpenNextAsync().Forget();
			}
		}

		private void OnCloseWindow(CloseWindowSignal signal)
		{
			if (_queue.Queue.Any(x => x.WindowName == signal.WindowName))
			{
				BaseWindowBehaviour windowBehaviour = _queue.Queue.Find(x => x.WindowName == signal.WindowName);
				windowBehaviour.CancelOpening = true;
				return;
			}

			BaseWindowBehaviour behaviour = _queue.GetOpenedWindow(signal.WindowName);
			BaseWindow window = behaviour?.Window;
			if (window == null || behaviour.State != WindowState.Opened)
			{
				return;
			}

			behaviour.CloseImmediately = signal.Immediately;
			CloseWindowAsync(behaviour);
		}

		private async void CloseWindowAsync(BaseWindowBehaviour behaviour)
		{
			_queue.OpenedWindows.Remove(behaviour);
			await behaviour.CloseAsync();
			//_queue.OpenedWindows.Remove(behaviour);
			_queue.RemoveWindowInstance(behaviour.WindowName);

			SignalBus.Publish(new WindowClosedSignal(behaviour.WindowName));

			await TryOpenNextAsync();
		}

		private void OnCloseAllWindows(CloseAllWindowsSignal signal)
		{
			List<BaseWindowBehaviour> openedWindowsClone = new List<BaseWindowBehaviour>();
			openedWindowsClone.AddRange(_queue.OpenedWindows);

			foreach (BaseWindowBehaviour behaviour in openedWindowsClone)
			{
				behaviour.Window.CloseWindow(signal.Immediately);
			}
		}
	}
}
