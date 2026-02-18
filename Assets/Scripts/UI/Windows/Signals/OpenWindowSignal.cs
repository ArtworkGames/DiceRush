using StepanoffGames.Signals;
using System;

namespace StepanoffGames.UI.Windows.Signals
{
	public class OpenWindowSignal : BaseSignal
	{
		public string WindowName;
		public BaseWindowParams Params;
		public bool Immediately;
		public Type BehaviourType;

		public OpenWindowSignal(string windowName, bool immediately = false)
		{
			WindowName = windowName;
			Immediately = immediately;
		}

		public OpenWindowSignal(string windowName, BaseWindowParams @params, bool immediately = false)
		{
			WindowName = windowName;
			Params = @params;
			Immediately = immediately;
		}
	}
}