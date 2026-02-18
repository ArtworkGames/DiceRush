using StepanoffGames.Signals;

namespace StepanoffGames.Localization.Signals
{
	public class ChangeLocaleSignal : BaseSignal
	{
		public string LocaleName;

		public ChangeLocaleSignal(string localeName)
		{
			LocaleName = localeName;
		}
	}
}