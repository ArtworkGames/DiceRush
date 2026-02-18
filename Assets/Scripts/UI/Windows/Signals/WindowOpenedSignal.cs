using StepanoffGames.Signals;

namespace StepanoffGames.UI.Windows.Signals
{
	public class WindowOpenedSignal : BaseSignal
	{
		public string WindowName { get; private set; }

		public WindowOpenedSignal(string windowName)
		{
			WindowName = windowName;
		}
	}
}