using StepanoffGames.Signals;

namespace StepanoffGames.Sound.Signals
{
	public class ChangeMusicStateSignal : BaseSignal
	{
		public bool State;

		public ChangeMusicStateSignal(bool state)
		{
			State = state;
		}
	}
}
