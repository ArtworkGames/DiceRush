using System;

namespace StepanoffGames.Localization
{
	[Serializable]
	public class TextPublicScheme
	{
		public string key;
		public string value;
	}

	[Serializable]
	public class LocalePublicScheme
	{
		public TextPublicScheme[] texts;
	}
}
