using Cysharp.Threading.Tasks;

namespace StepanoffGames.UI.Windows.Behaviours
{
	public enum WindowState
	{
		Closed,
		Opening,
		Opened,
		Closing,
		Hiding,
		Hidden,
		Showing
	}

	public class BaseWindowBehaviour
	{
		public WindowsQueue Queue;

		public string WindowName;
		public BaseWindow Window;
		public BaseWindowParams Params;

		public WindowState State { get; private set; }

		public bool OpenImmediately;
		public bool CloseImmediately;
		public bool ShowImmediately;
		public bool HideImmediately;
		public bool CancelOpening;

		public virtual bool CanOpenOverOtherWindows { get; private set; } = false;

		public virtual async UniTask OpenAsync()
		{
			State = WindowState.Opening;
			Window.transform.SetAsLastSibling();
			Window.SetParams(Params);

			await Window.OpenAsync(OpenImmediately);
			State = WindowState.Opened;
			OpenImmediately = false;
		}

		public virtual async UniTask CloseAsync()
		{
			if (State == WindowState.Closing) return;

			State = WindowState.Closing;
			await Window.CloseAsync(CloseImmediately);
			State = WindowState.Closed;
			CloseImmediately = false;
		}

		public virtual async UniTask ShowAsync()
		{
			State = WindowState.Showing;
			await Window.ShowAsync(ShowImmediately);
			State = WindowState.Opened;
			ShowImmediately = false;
		}

		public virtual async UniTask HideAsync()
		{
			State = WindowState.Hiding;
			await Window.HideAsync(HideImmediately);
			State = WindowState.Hidden;
			HideImmediately = false;
		}
	}
}
