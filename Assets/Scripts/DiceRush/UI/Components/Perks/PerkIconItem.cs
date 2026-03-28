using Cysharp.Threading.Tasks;
using DG.Tweening;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.UI.Popups.PerkDescriptionPopup;
using StepanoffGames.Signals;
using StepanoffGames.UI.Popups.Signals;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;

namespace StepanoffGames.DiceRush.UI.Components.Perks
{
	public class PerkIconItem : TweenButton// MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		//[SerializeField] private Transform _icon;

		public PerkType Type => _type;
		private PerkType _type;

		public GameObject IconObject => _iconObject;

		private GameObject _iconObject;

		private Tween widthTween;
		//private Tween scaleTween;

		private void Awake()
		{
			mode = TweenButtonMode.Focusable;
		}

		override protected void OnDestroy()
		{
			base.OnDestroy();

			widthTween?.Kill();
			//scaleTween?.Kill();
		}

		public void Init(PerkType type)
		{
			_type = type;
			LoadIcon(true).Forget();
			//_iconObject.SetActive(false);
			((RectTransform)transform).SetWidth(140f);
		}

		public void Show(PerkType type)
		{
			_type = type;

			float width = 0f;
			((RectTransform)transform).SetWidth(width);

			LoadIcon(false).Forget();

			//_icon.localScale = Vector3.zero;

			widthTween = DOTween.To(() => width, x => width = x, 140f, 0.5f)
				.SetEase(Ease.OutCubic)
				.SetUpdate(true)
				.OnUpdate(() =>
				{
					((RectTransform)transform).SetWidth(width);
				});

			//scaleTween = _icon.DOScale(1f, 0.3f)
			//	.SetDelay(0.2f)
			//	.SetEase(Ease.OutBack)
			//	.OnStart(() =>
			//	{
			//		iconObject.SetActive(true);
			//	});
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
			PerkDescriptionPopup.Show(this);

			base.OnPointerEnter(eventData);

			//scaleTween?.Kill();
			//scaleTween = _icon.DOScale(1.2f, 0.2f)
			//	.SetEase(Ease.OutBack);
		}

		override public void OnPointerExit(PointerEventData eventData)
		{
			SignalBus.Publish(new CloseAllPopupsSignal()
			{
				CloseAutoclosingPopups = false
			});

			base.OnPointerExit(eventData);

			//scaleTween?.Kill();
			//scaleTween = _icon.DOScale(1f, 0.15f)
			//	.SetEase(Ease.OutCubic);
		}
	}
}
