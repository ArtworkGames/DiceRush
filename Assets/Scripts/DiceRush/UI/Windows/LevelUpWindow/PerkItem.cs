using DG.Tweening;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.Localization;
using StepanoffGames.Services;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StepanoffGames.DiceRush.UI.Windows.ConfirmWindow
{
	public class PerkItem : MonoBehaviour
	{
		public Action<PerkItem> OnSelect;

		[SerializeField] private TMP_Text _title;
		[SerializeField] private TMP_Text _description;

		public PerkModel Model => _model;
		private PerkModel _model;

		private bool _canPress = true;

		private Tween moveTween;
		private Tween scaleTween;

		private void Start()
		{
			Button button = GetComponent<Button>();
			button.onClick.AddListener(OnButtonClick);
		}

		private void OnDestroy()
		{
			OnSelect = null;
			_model = null;

			moveTween?.Kill();
			scaleTween?.Kill();
		}

		public void SetModel(PerkModel model)
		{
			_model = model;

			LocalizationManager localizationManager = ServiceLocator.Get<LocalizationManager>();

			_title.text = localizationManager.GetString($"Perk:{model.Type}:Title");
			_description.text = localizationManager.GetString($"Perk:{model.Type}:Description");
		}

		public bool CanPress()
		{
			return _canPress;
		}

		public void OnButtonClick()
		{
			OnSelect?.Invoke(this);
		}
	}
}
