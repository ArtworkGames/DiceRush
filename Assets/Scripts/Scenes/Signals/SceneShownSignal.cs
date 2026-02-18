using StepanoffGames.Signals;

namespace StepanoffGames.Scenes.Signals
{
	public class SceneShownSignal : BaseSignal
	{
		public string SceneName;

		public SceneShownSignal(string sceneName)
		{
			SceneName = sceneName;
		}
	}
}