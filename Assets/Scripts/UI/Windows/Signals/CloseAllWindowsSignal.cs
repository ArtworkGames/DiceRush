using StepanoffGames.Signals;

namespace StepanoffGames.UI.Windows.Signals
{
	public class CloseAllWindowsSignal : BaseSignal
	{
		public bool Immediately;

		public CloseAllWindowsSignal(bool immediately = false)
		{
			Immediately = immediately;
		}
	}
}
