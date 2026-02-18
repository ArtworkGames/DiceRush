using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace StepanoffGames.UI.Windows.Behaviours
{
	public class CloseOtherWindowBehaviour : PopUpWindowBehaviour
	{
		public override async UniTask OpenAsync()
		{
			List<BaseWindowBehaviour> openedWindows = new List<BaseWindowBehaviour>();
			openedWindows.AddRange(Queue.OpenedWindows);

			for (var i = openedWindows.Count - 1; i >= 0; i--)
			{
				await openedWindows[i].CloseAsync();
			}
			Queue.OpenedWindows.Clear();

			await base.OpenAsync();
		}
	}
}