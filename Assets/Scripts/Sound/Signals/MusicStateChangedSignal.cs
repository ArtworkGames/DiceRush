using StepanoffGames.Signals;

namespace StepanoffGames.Sound.Signals
{
	public class MusicStateChangedSignal : BaseSignal
	{
		public bool State;

		public MusicStateChangedSignal(bool state)
		{
			State = state;
		}
	}
}
