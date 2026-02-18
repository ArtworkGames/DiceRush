using StepanoffGames.Signals;

namespace StepanoffGames.Sound.Signals
{
	public class StopSoundFXSignal : BaseSignal
	{
		public string ClipName;

		public StopSoundFXSignal(string clipName)
		{
			ClipName = clipName;
		}
	}
}
