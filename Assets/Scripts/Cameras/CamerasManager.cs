using StepanoffGames.Cameras.Signals;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace StepanoffGames.Cameras
{
	public class CamerasManager : MonoBehaviour, IService
	{
		[SerializeField] private Camera _windowsCamera;

		protected void Awake()
		{
			DontDestroyOnLoad(gameObject);

			ServiceLocator.Register(this);

			SignalBus.Subscribe<AddCamerasSignal>(OnAddCameras);
			SignalBus.Subscribe<ClearCamerasSignal>(OnClearCameras);
		}

		private void OnDestroy()
		{
			SignalBus.Unsubscribe<AddCamerasSignal>(OnAddCameras);
			SignalBus.Unsubscribe<ClearCamerasSignal>(OnClearCameras);

			ServiceLocator.Unregister<CamerasManager>();
		}

		private void OnAddCameras(AddCamerasSignal signal)
		{
			var windowsCamData = _windowsCamera.GetUniversalAdditionalCameraData();
			windowsCamData.renderType = CameraRenderType.Overlay;

			SetAsBaseCamera(signal.Cameras[0]);
			var baseCamData = signal.Cameras[0].GetUniversalAdditionalCameraData();

			for (int i = 1; i < signal.Cameras.Count; i++)
			{
				var camera = signal.Cameras[i];

				var overlayCamData = camera.GetUniversalAdditionalCameraData();
				overlayCamData.renderType = CameraRenderType.Overlay;

				baseCamData.cameraStack.Add(camera);
			}

			baseCamData.cameraStack.Add(_windowsCamera);
		}

		private void OnClearCameras(ClearCamerasSignal signal)
		{
			SetAsBaseCamera(_windowsCamera);
		}

		private void SetAsBaseCamera(Camera camera)
		{
			camera.backgroundColor = Color.black;
			camera.clearFlags = CameraClearFlags.SolidColor;

			var baseCamData = camera.GetUniversalAdditionalCameraData();
			baseCamData.renderType = CameraRenderType.Base;
			baseCamData.cameraStack.Clear();
		}
	}
}
