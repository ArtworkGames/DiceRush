using StepanoffGames.Signals;

namespace StepanoffGames.UI.Popups.Signals
{
	public class ClosePopupSignal : BaseSignal
	{
		public BasePopup Popup;
		public bool Immediately;

		public ClosePopupSignal(BasePopup popup, bool immediately = false)
		{
			Popup = popup;
			Immediately = immediately;
		}
	}
}
