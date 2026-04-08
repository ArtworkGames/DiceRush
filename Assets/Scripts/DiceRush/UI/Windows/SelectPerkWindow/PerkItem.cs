using Cysharp.Threading.Tasks;
using DG.Tweening;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Perks;
using StepanoffGames.DiceRush.UI.Components;
using StepanoffGames.DiceRush.UI.Perks;
using StepanoffGames.DiceRush.UI.Popups.FlyingIconPopup;
using StepanoffGames.Localization;
using StepanoffGames.Services;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace StepanoffGames.DiceRush.UI.Windows.SelectPerkWindow
{
	public class PerkItem : TweenButton
	{
		public Action<PerkItem> OnSelect;

		[Space]
		[SerializeField] private CanvasGroup _canvasGroup;
		[SerializeField] private Transform _showContent;
		[Space]
		[SerializeField] private Transform _icon;
		[SerializeField] private TMP_Text _title;
		[SerializeField] private TMP_Text _description;

		public PerkModel Model => _model;
		private PerkModel _model;

		private GameObject iconObject;

		private Tween showTween;
		private Tween alphaTween;
		private Tween iconScaleTween;
		private Tween moveTween;
		private bool isDestroyed;

		private void Start()
		{
			Button button = GetComponent<Button>();
		}

		override protected void OnDestroy()
		{
			base.OnDestroy();

			isDestroyed = true;

			OnSelect = null;
			_model = null;

			showTween?.Kill();
			alphaTween?.Kill();
			iconScaleTween?.Kill();
			moveTween?.Kill();
		}

		public async void Show(int index, PerkModel model)
		{
			_model = model;

			_canvasGroup.alpha = 0f;
			//_canvasGroup.interactable = false;
			//_canvasGroup.blocksRaycasts = false;

			await LoadIcon();
			if (isDestroyed) return;

			LocalizationManager localizationManager = ServiceLocator.Get<LocalizationManager>();
			_title.text = localizationManager.GetString($"Perk:{model.Type}:Title");
			_description.text = localizationManager.GetString($"Perk:{model.Type}:Description");

			_showContent.localScale = Vector3.zero;
			showTween = _showContent.DOScale(1f, 0.25f)
				.SetDelay(index * 0.05f)
				.SetEase(Ease.OutCubic)
				.OnStart(() =>
				{
					_canvasGroup.alpha = 1f;
				})
				.OnComplete(() =>
				{
					//_canvasGroup.interactable = true;
					//_canvasGroup.blocksRaycasts = true;
				});
		}

		private async UniTask LoadIcon()
		{
			string perkName = $"{_model.Type}PerkIcon";
			string perkPath = $"UI/Perks/{perkName}.prefab";
			var handle = Addressables.LoadAssetAsync<GameObject>(perkPath);
			await UniTask.WaitUntil(() => handle.IsDone);
			if (isDestroyed) return;

			iconObject = Instantiate(handle.Result, _icon, false);
			iconObject.name = perkName;
			iconObject.transform.localScale = Vector3.one * 2.3f;
		}

		override public void DoClick()
		{
			OnSelect?.Invoke(this);
		}

		public void Hide()
		{
			_canvasGroup.interactable = false;
			_canvasGroup.blocksRaycasts = false;

			alphaTween?.Kill();
			alphaTween = _canvasGroup.DOFade(0f, 0.2f)
				.SetEase(Ease.OutCubic);

			showTween?.Kill();
			showTween = _showContent.DOScale(0.5f, 0.2f)
				.SetEase(Ease.OutCubic);
		}

		public void FlyToIconsPanel()
		{
			_canvasGroup.interactable = false;
			_canvasGroup.blocksRaycasts = false;

			CanvasGroup iconCanvasGroup = _icon.GetComponent<CanvasGroup>();
			iconCanvasGroup.ignoreParentGroups = true;

			alphaTween?.Kill();
			alphaTween = _canvasGroup.DOFade(0f, 0.2f)
				.SetEase(Ease.OutCubic);

			PerksManager perksManager = ServiceLocator.Get<PerksManager>();
			PerkIconItem perkIconItem = perksManager.Panel.AddPerk(_model.Type);

			FlyingIconPopup.Show(iconObject, perkIconItem.transform,
				() =>
				{
					iconObject.SetActive(false);
				},
				() =>
				{
					perkIconItem.ShowIcon();
				}, true);
		}

		public void FlyToTarget()
		{
			_canvasGroup.interactable = false;
			_canvasGroup.blocksRaycasts = false;

			CanvasGroup iconCanvasGroup = _icon.GetComponent<CanvasGroup>();
			iconCanvasGroup.ignoreParentGroups = true;

			alphaTween?.Kill();
			alphaTween = _canvasGroup.DOFade(0f, 0.2f)
				.SetEase(Ease.OutCubic);

			PerksManager perksManager = ServiceLocator.Get<PerksManager>();
			FlyingPerkTarget flyingPerkTarget = perksManager.GetFlyingPerkTarget(_model.Type);

			if (flyingPerkTarget != null)
			{
				FlyingIconPopup.Show(iconObject, flyingPerkTarget.transform,
					() =>
					{
						iconObject.SetActive(false);
					},
					() =>
					{
						//perkIconItem.ShowIcon();
					}, true);
			}
		}
	}
}
