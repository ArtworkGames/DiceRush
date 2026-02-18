using StepanoffGames.Signals;

namespace StepanoffGames.UI.Windows.Signals
{
	public class CloseWindowSignal : BaseSignal
	{
		public string WindowName;
		public bool Immediately;

		public CloseWindowSignal(string windowName, bool immediately = false)
		{
			WindowName = windowName;
			Immediately = immediately;
		}
	}
}