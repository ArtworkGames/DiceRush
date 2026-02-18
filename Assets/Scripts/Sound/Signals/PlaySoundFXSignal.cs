using StepanoffGames.Signals;

namespace StepanoffGames.Sound.Signals
{
	public class PlaySoundFXSignal : BaseSignal
	{
		public string ClipName;
		public bool Loop;
		public float Delay;

		public PlaySoundFXSignal(string clipName, bool loop = false, float delay = 0.0f)
		{
			ClipName = clipName;
			Loop = loop;
			Delay = delay;
		}
	}
}
