using Cysharp.Threading.Tasks;
using System.Globalization;
using UnityEngine;

namespace StepanoffGames.Initialization
{
	public class AppSettingsManager : BaseInitializable
	{
		override public async UniTask InitializeAsync()
		{
			GL.Clear(false, true, Color.black);

			CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
			CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

			Application.targetFrameRate = 120;

#if UNITY_ANDROID
			Application.runInBackground = true;
			Input.multiTouchEnabled = false;
			//QualitySettings.vSyncCount = 0;
			QualitySettings.SetQualityLevel(QualitySettings.names.Length - 1);
#elif UNITY_WEBGL
			//Application.targetFrameRate = 60;
			Application.runInBackground = true;
			Input.multiTouchEnabled = false;
			//QualitySettings.vSyncCount = 0;
			QualitySettings.SetQualityLevel(QualitySettings.names.Length - 1);
#endif

			await UniTask.Yield(PlayerLoopTiming.Update);
		}
	}
}
