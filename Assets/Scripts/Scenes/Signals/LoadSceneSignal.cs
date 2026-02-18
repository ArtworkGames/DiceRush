using StepanoffGames.Signals;

namespace StepanoffGames.Scenes.Signals
{
	public class LoadSceneSignal : BaseSignal
	{
		public string SceneName;

		public LoadSceneSignal(string sceneName)
		{
			SceneName = sceneName;
		}
	}
}