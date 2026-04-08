using Cysharp.Threading.Tasks;
using DG.Tweening;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.UI.Components;
using StepanoffGames.DiceRush.UI.Perks.DescriptionPopup;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;

namespace StepanoffGames.DiceRush.UI.Perks
{
	public class PerkIconItem : TweenButton
	{
		[Space]
		[SerializeField] private PerkDescriptionPopup _descriptionPopup;

		public PerkType Type => _type;
		private PerkType _type;

		public GameObject IconObject => _iconObject;

		private GameObject _iconObject;

		private Tween widthTween;

		private void Awake()
		{
			mode = TweenButtonMode.Focusable;
		}

		override protected void OnDestroy()
		{
			base.OnDestroy();

			widthTween?.Kill();
		}

		public void Init(PerkType type)
		{
			_type = type;
			if (_descriptionPopup != null)
				_descriptionPopup.SetPerkType(type);

			LoadIcon(true).Forget();
			((RectTransform)transform).SetWidth(140f);
		}

		public void Show(PerkType type)
		{
			_type = type;
			if (_descriptionPopup != null)
				_descriptionPopup.SetPerkType(type);

			float width = 0f;
			((RectTransform)transform).SetWidth(width);

			LoadIcon(false).Forget();

			widthTween = DOTween.To(() => width, x => width = x, 140f, 0.5f)
				.SetEase(Ease.OutCubic)
				.SetUpdate(true)
				.OnUpdate(() =>
				{
					((RectTransform)transform).SetWidth(width);
				});
		}

		private async UniTask LoadIcon(bool iconObjectActive)
		{
			string perkName = $"{_type}PerkIcon";
			string perkPath = $"UI/Perks/{perkName}.prefab";
			var handle = Addressables.LoadAssetAsync<GameObject>(perkPath);
			await UniTask.WaitUntil(() => handle.IsDone);

			_iconObject = Instantiate(handle.Result, _content, false);
			_iconObject.name = perkName;
			_iconObject.SetActive(iconObjectActive);
		}

		public void ShowIcon()
		{
			_iconObject.SetActive(true);
		}

		override public void OnPointerEnter(PointerEventData eventData)
		{
			if (_descriptionPopup != null)
				_descriptionPopup.Show();

			base.OnPointerEnter(eventData);
		}

		override public void OnPointerExit(PointerEventData eventData)
		{
			if (_descriptionPopup != null)
				_descriptionPopup.Hide();

			base.OnPointerExit(eventData);
		}
	}
}
