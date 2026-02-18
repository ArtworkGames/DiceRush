using StepanoffGames.Localization;
using StepanoffGames.Localization.Signals;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using UnityEngine;

namespace StepanoffGames.UI.Components
{
	public class TextLocalizer : MonoBehaviour
	{
		[SerializeField] private string _localeKey;

		public string LocaleKey => _localeKey;

		private LocalizationManager _localizationManager;

		private string[] _params;
		private bool _isAlreadyLocalized;

		private void Awake()
		{
			_localizationManager = ServiceLocator.Get<LocalizationManager>();
			SignalBus.Subscribe<LocaleChangedSignal>(OnLocaleChanged);
		}

		private void Start()
		{
			if (_localizationManager != null && !_isAlreadyLocalized)
			{
				UpdateText();
			}
		}

		private void OnDestroy()
		{
			_localizationManager = null;
			SignalBus.Unsubscribe<LocaleChangedSignal>(OnLocaleChanged);
		}

		private void OnLocaleChanged(LocaleChangedSignal signal)
		{
			UpdateText();
		}

		public void Localize(string key, params string[] p)
		{
			_localeKey = key;
			_params = p;
			UpdateText();
		}

		public void SetParams(params string[] p)
		{
			_params = p;
			UpdateText();
		}

		public virtual void SetText(string txt)
		{
		}

		public virtual string GetText()
		{
			return "";
		}

		private void UpdateText()
		{
			if ((_localizationManager == null) || string.IsNullOrEmpty(_localeKey))
			{
				//Text.text = "";
				return;
			}

			string str = _localizationManager.GetString(_localeKey, _params);
			SetText(str);

			_isAlreadyLocalized = true;
		}
	}
}
