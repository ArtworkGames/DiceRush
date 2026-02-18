using Cysharp.Threading.Tasks;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using StepanoffGames.UI.Popups.Signals;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.InputSystem.OnScreen.OnScreenStick;

namespace StepanoffGames.UI.Popups
{
	public class PopupManager : MonoBehaviour, IService
	{
		[SerializeField] private Camera _camera;
		[SerializeField] private Transform _popupsParent;

		public Camera Camera => _camera;

		private List<PopupBehaviour> _popupBehaviours;

		protected void Awake()
		{
			ServiceLocator.Register(this);

			SignalBus.Subscribe<OpenPopupSignal>(OnOpenPopup);
			SignalBus.Subscribe<ClosePopupSignal>(OnClosePopup);
			SignalBus.Subscribe<CloseAllPopupsSignal>(OnCloseAllPopups);

			_popupBehaviours = new List<PopupBehaviour>();
		}

		private void OnDestroy()
		{
			SignalBus.Unsubscribe<OpenPopupSignal>(OnOpenPopup);
			SignalBus.Unsubscribe<ClosePopupSignal>(OnClosePopup);
			SignalBus.Unsubscribe<CloseAllPopupsSignal>(OnCloseAllPopups);

			ServiceLocator.Unregister<PopupManager>();
		}

		private void OnOpenPopup(OpenPopupSignal signal)
		{
			if (signal.CloseOther)
			{
				List<PopupBehaviour> behavioursForClose = _popupBehaviours.FindAll(x => !x.Autoclosing);
				for (int i = 0; i < behavioursForClose.Count; i++)
				{
					_popupBehaviours.Remove(behavioursForClose[i]);
					behavioursForClose[i].CloseAsync(signal.Immediately).Forget();
				}
			}

			OpenPopupAsync(signal);
		}

		private async void OpenPopupAsync(OpenPopupSignal signal)
		{
			PopupBehaviour behaviour = new PopupBehaviour();
			behaviour.PopupName = signal.PopupName;
			behaviour.Autoclosing = signal.Autoclosing;
			
			Transform parent = signal.Parent != null ? signal.Parent : _popupsParent;
			Vector3 worldPos = _camera.ScreenToWorldPoint(signal.ScreenPos);
			worldPos.z = parent.position.z;
			BasePopupParams @params = signal.Params;
			bool openImmediately = signal.Immediately;

			_popupBehaviours.Add(behaviour);

			string prefabName = GetPrefabPath(behaviour.PopupName);
			ResourceRequest request = Resources.LoadAsync(prefabName);

			await UniTask.WaitUntil(() => request.isDone);

			if (behaviour.State == PopupState.Cancelled)
			{
				behaviour.Clear();
				_popupBehaviours.Remove(behaviour);
				return;
			}

			GameObject prefab = (GameObject)request.asset;
			GameObject instance = Instantiate(prefab, parent, false);
			instance.name = behaviour.PopupName;
			instance.transform.position = worldPos;
			instance.SetActive(false);

			BasePopup popup = instance.GetComponent<BasePopup>();
			behaviour.Popup = popup;

			await behaviour.OpenAsync(@params, openImmediately);
		}

		private string GetPrefabPath(string name)
		{
			return $"Popups/{name}";
		}

		private void OnClosePopup(ClosePopupSignal signal)
		{
			PopupBehaviour behaviour = _popupBehaviours.FirstOrDefault(x => x.Popup == signal.Popup);
			if (behaviour != default)
			{
				_popupBehaviours.Remove(behaviour);
				behaviour.CloseAsync(signal.Immediately).Forget();
			}
		}

		private void OnCloseAllPopups(CloseAllPopupsSignal signal)
		{
			List<PopupBehaviour> behaviours = new List<PopupBehaviour>(_popupBehaviours);
			for (int i = 0; i < behaviours.Count; i++)
			{
				if (signal.CloseAutoclosingPopups || !behaviours[i].Autoclosing)
				{
					_popupBehaviours.Remove(behaviours[i]);
					behaviours[i].CloseAsync(signal.Immediately).Forget();
				}
			}
		}
	}
}
