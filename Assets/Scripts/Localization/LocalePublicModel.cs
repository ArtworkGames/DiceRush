using System.Collections.Generic;

namespace StepanoffGames.Localization
{
	public class LocalePublicModel
	{
		private Dictionary<string, string> _strings;

		public LocalePublicModel(LocalePublicScheme scheme)
		{
			_strings = new Dictionary<string, string>();
			for (int i = 0; i < scheme.texts.Length; i++)
			{
				_strings.Add(scheme.texts[i].key, scheme.texts[i].value);
			}
		}

		public string GetString(string key)
		{
			if (string.IsNullOrEmpty(key) || (_strings == null))
				return string.Empty;

			if (!_strings.ContainsKey(key))
				return $"Locale [{key}] not found!";

			string str = _strings[key];

			//if (string.IsNullOrEmpty(str))
				//return $"Locale [{key}] is empty!";

			return str;
		}
	}
}
