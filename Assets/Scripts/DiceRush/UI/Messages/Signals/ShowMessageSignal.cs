using StepanoffGames.Signals;

namespace StepanoffGames.DiceRush.UI.Messages.Signals
{
	public class ShowMessageSignal : BaseSignal
	{
		public string TextKey;
		public string[] Params;

		public ShowMessageSignal(string textKey, params string[] p)
		{
			TextKey = textKey;
			Params = p;
		}
	}
}
