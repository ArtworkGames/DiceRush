using StepanoffGames.Signals;

namespace StepanoffGames.Sound.Signals
{
	public class SoundFXStateChangedSignal : BaseSignal
	{
		public bool State;

		public SoundFXStateChangedSignal(bool state)
		{
			State = state;
		}
	}
}
