using StepanoffGames.Signals;

namespace StepanoffGames.UI.Windows.Signals
{
	public class WindowClosedSignal : BaseSignal
	{
		public string WindowName { get; private set; }

		public WindowClosedSignal(string windowName)
		{
			WindowName = windowName;
		}
	}
}