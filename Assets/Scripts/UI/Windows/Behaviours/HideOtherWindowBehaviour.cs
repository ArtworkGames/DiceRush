using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace StepanoffGames.UI.Windows.Behaviours
{
	public class HideOtherWindowBehaviour : BaseWindowBehaviour
	{
		public override bool CanOpenOverOtherWindows => true;

		private readonly List<BaseWindowBehaviour> _hiddenWindow = new List<BaseWindowBehaviour>();

		public override async UniTask OpenAsync()
		{
			List<BaseWindowBehaviour> openedWindows = new List<BaseWindowBehaviour>();
			openedWindows.AddRange(Queue.OpenedWindows);

			for (int i = openedWindows.Count - 1; i >= 0; i--)
			{
				//if (behaviour.State == WindowState.Opening)
				//{
				//	await UniTask.WaitWhile(() => behaviour.State == WindowState.Opening);
				//}
				//if (openedWindows[i].State == WindowState.Showing)
				//{
				//	await UniTask.WaitWhile(() => openedWindows[i].State == WindowState.Showing);
				//}
				//if (openedWindows[i].State == WindowState.Opened)
				//{
				//	await openedWindows[i].HideAsync();
				//	_hiddenWindow.Add(openedWindows[i]);
				//}

				await openedWindows[i].HideAsync();
				_hiddenWindow.Add(openedWindows[i]);
			}

			await base.OpenAsync();
		}

		public override async UniTask CloseAsync()
		{
			await base.CloseAsync();

			for (int i = 0; i < _hiddenWindow.Count; i++)
			{
				BaseWindowBehaviour item = _hiddenWindow[i];
				await item.ShowAsync();
			}

			_hiddenWindow.Clear();
		}

	}
}
