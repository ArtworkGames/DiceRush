using Cysharp.Threading.Tasks;
using StepanoffGames.Initialization;
using StepanoffGames.Localization.Signals;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using System;
using UnityEngine;

namespace StepanoffGames.Localization
{
	public class LocaleDescription
	{
		public SystemLanguage SystemLanguage;
		public string Name;
		public string Code;
	}

	public class LocalizationManager : BaseInitializable, IService
	{
		private const string LOCALE_KEY = "Locale";

		public LocaleDescription[] Locales => _locales;
		public LocaleDescription Locale => _locale;
		
		private LocaleDescription[] _locales = new LocaleDescription[]
		{
			new LocaleDescription() { SystemLanguage = SystemLanguage.German, Name = "Deutsch", Code = "de" },
			new LocaleDescription() { SystemLanguage = SystemLanguage.English, Name = "English", Code = "en" },
			new LocaleDescription() { SystemLanguage = SystemLanguage.Spanish, Name = "Español", Code = "es" },
			new LocaleDescription() { SystemLanguage = SystemLanguage.French, Name = "Français", Code = "fr" },
			new LocaleDescription() { SystemLanguage = SystemLanguage.Italian, Name = "Italiano", Code = "it" },
			//new LocaleDescription() { Name = "日本語", Code = "ja" },
			//new LocaleDescription() { Name = "한국어", Code = "ko" },
			new LocaleDescription() { SystemLanguage = SystemLanguage.Polish, Name = "Polski", Code = "pl" },
			new LocaleDescription() { SystemLanguage = SystemLanguage.Portuguese, Name = "Português", Code = "pt" },
			new LocaleDescription() { SystemLanguage = SystemLanguage.Russian, Name = "Русский", Code = "ru" },
			new LocaleDescription() { SystemLanguage = SystemLanguage.Turkish, Name = "Türkçe", Code = "tr" },
			//new LocaleDescription() { Name = "中文", Code = "zh" },
		};
		private LocaleDescription _locale;

		private LocalePublicModel _model;

		public LocalizationManager()
		{
			ServiceLocator.Register(this);
			SignalBus.Subscribe<ChangeLocaleSignal>(OnChangeLocale);
		}

		override public async UniTask InitializeAsync()
		{
			string localeCode = "";
			if (PlayerPrefs.HasKey(LOCALE_KEY))
			{
				localeCode = PlayerPrefs.GetString(LOCALE_KEY);
			}

			if (string.IsNullOrEmpty(localeCode))
			{
				localeCode = "en";
				for (int i = 0; i < _locales.Length; i++)
				{
					if (_locales[i].SystemLanguage == Application.systemLanguage)
					{
						localeCode = _locales[i].Code;
						break;
					}
				}
			}

			LoadLocale(localeCode);

			await UniTask.Yield();
		}

		//private async UniTask LoadLocaleAsync(string localeCode)
		private void LoadLocale(string localeCode)
		{
			try
			{
				_locale = GetLocale(localeCode);
				PlayerPrefs.SetString(LOCALE_KEY, _locale.Code);

				//ResourceRequest request = Resources.LoadAsync(GetDataPath(_locale.Code));
				//await UniTask.WaitUntil(() => request.isDone);
				//TextAsset textAsset = (TextAsset)request.asset;

				TextAsset textAsset = (TextAsset)Resources.Load(GetDataPath(_locale.Code));
				LocalePublicScheme scheme = JsonUtility.FromJson<LocalePublicScheme>(textAsset.text);
				_model = new LocalePublicModel(scheme);

				SignalBus.Publish(new LocaleChangedSignal(_locale.Code));
			}
			catch (Exception e)
			{
				Debug.LogError("Error reading asset \"" + GetDataPath(_locale.Code) + "\": " + e.Message);
			}
		}

		private string GetDataPath(string localeCode)
		{
			return $"Locales/{localeCode}";
		}

		private LocaleDescription GetLocale(string localeCode)
		{
			for (int i = 0; i < _locales.Length; i++)
			{
				if (_locales[i].Code == localeCode)
				{
					return _locales[i];
				}
			}
			return _locales[0];
		}

		private void OnChangeLocale(ChangeLocaleSignal signal)
		{
			//LoadLocaleAsync(signal.LocaleName);
			LoadLocale(signal.LocaleName);
		}

		public string GetString(string key, params string[] p)
		{
			if (_model == null) return "";

			string str = _model.GetString(key);
			if ((p != null) && (p.Length > 0))
			{
				str = string.Format(str, p);
			}
			return str;
		}
	}
}
