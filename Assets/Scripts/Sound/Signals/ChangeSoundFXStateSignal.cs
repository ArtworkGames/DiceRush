using StepanoffGames.Signals;

namespace StepanoffGames.Sound.Signals
{
	public class ChangeSoundFXStateSignal : BaseSignal
	{
		public bool State;

		public ChangeSoundFXStateSignal(bool state)
		{
			State = state;
		}
	}
}
