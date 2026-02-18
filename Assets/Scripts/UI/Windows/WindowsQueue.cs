using StepanoffGames.UI.Windows.Behaviours;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StepanoffGames.UI.Windows
{
	public class WindowsQueue
	{
		public Dictionary<string, BaseWindow> WindowInstances = new Dictionary<string, BaseWindow>();
		public List<BaseWindowBehaviour> Queue = new List<BaseWindowBehaviour>();
		public List<BaseWindowBehaviour> OpenedWindows = new List<BaseWindowBehaviour>();

		public void AddWindowInstance(string name, BaseWindow window)
		{
			WindowInstances.Add(name, window);

			BaseWindowBehaviour queueItem = Queue.FirstOrDefault(x => x.WindowName == name);
			if (queueItem != null)
			{
				queueItem.Window = WindowInstances[name];
			}
		}

		public BaseWindow GetWindowInstance(string name)
		{
			if (WindowInstances.ContainsKey(name))
				return WindowInstances[name];
			return null;
		}

		public void RemoveWindowInstance(string name)
		{
			Object.Destroy(WindowInstances[name].gameObject);
			WindowInstances.Remove(name);
		}

		public BaseWindowBehaviour GetOpenedWindow(string name)
		{
			if ((OpenedWindows.Count == 0) || OpenedWindows.All(x => x.WindowName != name))
			{
				return null;
			}
			return OpenedWindows.First(x => x.WindowName == name);
		}

		public bool IsWindowOpened(string name)
		{
			return OpenedWindows.Any(x => (x.WindowName == name) && (x.State != WindowState.Closing) && (x.State != WindowState.Closed));
		}

		public BaseWindowBehaviour GetWindowFromQueue<T>() where T : BaseWindow
		{
			return Queue.FirstOrDefault(x => x.Window is T);
		}
	}
}