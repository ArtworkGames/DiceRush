using Cysharp.Threading.Tasks;
using DG.Tweening;
using StepanoffGames.DiceRush.Data.Models;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace StepanoffGames.DiceRush.UI.Components.Deck
{
	public class CardButton : TweenButton
	{
		public Action<CardButton> OnSelect;

		[SerializeField] protected CanvasGroup _canvasGroup;
		[Space]
		[SerializeField] protected GameObject _back;
		[SerializeField] protected GameObject _front;

		public CardModel Model => _model;
		private CardModel _model;

		public bool IsShown => _isShown;
		private bool _isShown;

		private float hideDelay;
		private Vector3 hidePos;

		private Tween moveTween;
		private Tween scaleTween;
		private Tween flipTween;

		override protected void OnDestroy()
		{
			base.OnDestroy();

			moveTween?.Kill();
			scaleTween?.Kill();
			flipTween?.Kill();

			_model = null;
		}

		override public void OnClick()
		{
			OnSelect?.Invoke(this);
		}

		public void Show(CardModel cardModel, float showDelay, float hideDelay, Vector3 destPos)
		{
			_model = cardModel;

			this.hideDelay = hideDelay;

			LoadCard().Forget();

			//hidePos = transform.localPosition;

			//transform.localScale = Vector3.one * 0.75f;

			//moveTween?.Kill();
			//moveTween = transform.DOLocalMove(destPos, 0.3f)
			//	.SetDelay(showDelay)
			//	.SetEase(Ease.OutBack)
			//	.OnComplete(() =>
			//	{
			//		_isShown = true;
			//	});

			hidePos = new Vector3(destPos.x, -1600f, 0f);

			float duration = 0.4f;// 0.33f;

			_canvasGroup.interactable = false;
			_canvasGroup.blocksRaycasts = false;

			_back.SetActive(false);
			_front.SetActive(false);

			transform.localScale = Vector3.one * 0.5f;

			moveTween = transform.DOLocalMove(destPos, duration)
				.SetDelay(showDelay)
				.SetEase(Ease.InOutQuad)
				.OnComplete(() =>
				{
					_isShown = true;
				});

			scaleTween = transform.DOScale(0.75f, duration)
				.SetDelay(showDelay)
				.SetEase(Ease.OutBack);

			flipTween = _back.transform.DOScaleX(0f, duration / 3f)
				.SetDelay(showDelay + duration / 3f)
				.SetEase(Ease.InCubic)
				.OnStart(() =>
				{
					_back.SetActive(true);
				})
				.OnComplete(() =>
				{
					_back.SetActive(false);
					_back.transform.localScale = Vector3.one;

					_front.transform.localScale = new Vector3(0f, 1f, 1f);
					_front.SetActive(true);

					flipTween = _front.transform.DOScaleX(1f, duration / 3f)
						.SetEase(Ease.OutCubic);
				});
		}

		private async UniTask LoadCard()
		{
			string cardName = $"{_model.Type}Card";
			string cardPath = $"UI/Deck/{cardName}.prefab";
			var handle = Addressables.LoadAssetAsync<GameObject>(cardPath);
			await UniTask.WaitUntil(() => handle.IsDone);

			GameObject cardObject = Instantiate(handle.Result, _front.transform, false);
			cardObject.name = cardName;
			cardObject.transform.localScale = Vector3.one;
			cardObject.transform.localPosition = Vector3.zero;

			CardView cardView = cardObject.GetComponent<CardView>();
			cardView.SetModel(_model);
		}

		public void EnableButton()
		{
			_canvasGroup.interactable = true;
			_canvasGroup.blocksRaycasts = true;
		}

		public void Hide()
		{
			_canvasGroup.interactable = false;
			_canvasGroup.blocksRaycasts = false;

			moveTween?.Kill();
			moveTween = transform.DOLocalMove(hidePos, 0.2f)
				.SetDelay(hideDelay)
				.SetEase(Ease.InQuad)
				.OnComplete(() =>
				{
					Destroy(gameObject);
				});

			//float duration = 0.4f;// 0.33f;

			//moveTween = transform.DOLocalMove(hidePos, duration)
			//	.SetDelay(hideDelay)
			//	.SetEase(Ease.InOutQuad)
			//	.OnComplete(() =>
			//	{
			//		Destroy(gameObject);
			//	});

			//scaleTween = transform.DOScale(0.5f, duration)
			//	.SetDelay(hideDelay)
			//	.SetEase(Ease.OutBack);

			//flipTween = _front.transform.DOScaleX(0f, duration / 3f)
			//	.SetDelay(hideDelay)
			//	.SetEase(Ease.InCubic)
			//	.OnComplete(() =>
			//	{
			//		_front.SetActive(false);
			//		_front.transform.localScale = Vector3.one;

			//		_back.transform.localScale = new Vector3(0f, 1f, 1f);
			//		_back.SetActive(true);

			//		flipTween = _back.transform.DOScaleX(1f, duration / 3f)
			//			.SetEase(Ease.OutCubic);
			//	});
		}

		public async UniTask ShowSelected()
		{
			_canvasGroup.interactable = false;
			_canvasGroup.blocksRaycasts = false;

			bool tweenCompleted = false;

			moveTween?.Kill();
			moveTween = transform.DOLocalMoveY(0f, 0.5f)
				.SetEase(Ease.InOutCubic)
				.OnComplete(() =>
				{
					tweenCompleted = true;
				});

			await UniTask.WaitUntil(() => tweenCompleted);
		}

		public async UniTask HideSelected()
		{
			bool tweenCompleted = false;

			scaleTween?.Kill();
			scaleTween = transform.DOScale(0f, 0.5f)
				.SetEase(Ease.OutCubic)
				.OnComplete(() =>
				{
					tweenCompleted = true;
					Destroy(gameObject);
				});

			await UniTask.WaitUntil(() => tweenCompleted);
		}
	}
}
