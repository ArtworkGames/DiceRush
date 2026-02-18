using StepanoffGames.Signals;

namespace StepanoffGames.Localization.Signals
{
	public class LocaleChangedSignal : BaseSignal
	{
		public string LocaleName;

		public LocaleChangedSignal(string localeName)
		{
			LocaleName = localeName;
		}
	}
}