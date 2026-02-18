using Cysharp.Threading.Tasks;
using UnityEngine;

namespace StepanoffGames.UI.Popups
{
	public enum PopupState
	{
		Closed,
		Opening,
		Opened,
		Closing,
		Cancelled
	}

	public class PopupBehaviour
	{
		public string PopupName;
		public bool Autoclosing;
		public BasePopup Popup;

		public PopupState State { get; private set; }

		public async UniTask OpenAsync(BasePopupParams @params, bool immediately)
		{
			if (State == PopupState.Cancelled)
			{
				Clear();
				return;
			}

			State = PopupState.Opening;
			Popup.SetParams(@params);
			await Popup.OpenAsync(immediately);
			State = PopupState.Opened;
		}

		public async UniTask CloseAsync(bool immediately)
		{
			if (State == PopupState.Closing || State == PopupState.Cancelled) return;
			if (Popup == null)
			{
				State = PopupState.Cancelled;
				return;
			}

			State = PopupState.Closing;
			await Popup.CloseAsync(immediately);
			State = PopupState.Closed;

			GameObject.Destroy(Popup.gameObject);

			Clear();
		}

		public void Clear()
		{
			Popup = null;
		}
	}
}
