using StepanoffGames.Signals;
using UnityEngine;

namespace StepanoffGames.UI.Popups.Signals
{
	public class OpenPopupSignal : BaseSignal
	{
		public string PopupName;
		public Vector2 ScreenPos;
		public BasePopupParams Params;
		public bool Immediately;
		public bool CloseOther = true;
		public bool Autoclosing = false;
		public Transform Parent;

		public OpenPopupSignal(string popupName, Vector2 screenPos, bool immediately = false)
		{
			PopupName = popupName;
			ScreenPos = screenPos;
			Immediately = immediately;
		}

		public OpenPopupSignal(string popupName, Vector2 screenPos, BasePopupParams @params, bool immediately = false)
		{
			PopupName = popupName;
			ScreenPos = screenPos;
			Params = @params;
			Immediately = immediately;
		}
	}
}
