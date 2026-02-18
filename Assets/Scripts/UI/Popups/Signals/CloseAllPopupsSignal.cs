using StepanoffGames.Signals;

namespace StepanoffGames.UI.Popups.Signals
{
	public class CloseAllPopupsSignal : BaseSignal
	{
		public bool Immediately;
		public bool CloseAutoclosingPopups = true;

		public CloseAllPopupsSignal(bool immediately = false)
		{
			Immediately = immediately;
		}
	}
}
