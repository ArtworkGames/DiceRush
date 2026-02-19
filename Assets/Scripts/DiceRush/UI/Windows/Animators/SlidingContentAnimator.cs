using Cysharp.Threading.Tasks;
using DG.Tweening;
using StepanoffGames.UI.Windows.Animators;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Windows.Animators
{
	public class SlidingContentAnimator : BaseWindowAnimator
	{
		[SerializeField] private Vector2 _hiddenDelta;
		[SerializeField] private bool _useAlpha;

		private float showDuration = 0.3f;
		private Ease showEase = Ease.OutBack;
		private float hideDuration = 0.2f;
		private Ease hideEase = Ease.OutCubic;

		private CanvasGroup _canvasGroup;

		private bool inited;
		private Vector2 shownPosition;
		private Vector2 hiddenPosition;
		private bool shown;
		private Tween moveTween;
		private Tween alphaTween;

		private void Awake()
		{
			if (_useAlpha)
			{
				_canvasGroup = GetComponent<CanvasGroup>();
				if (_canvasGroup == null)
				{
					_canvasGroup = gameObject.AddComponent<CanvasGroup>();
				}
			}
		}

		private void OnDestroy()
		{
			moveTween?.Kill();
			alphaTween?.Kill();
		}

		public override void Reset()
		{
			if (!Application.isPlaying) return;

			moveTween?.Kill();
			alphaTween?.Kill();
			shown = false;
		}

		protected override async UniTask ShowOpenAnimationAsync(bool immediately = false)
		{
			if (!inited)
			{
				inited = true;
				shownPosition = transform.localPosition;
				hiddenPosition = shownPosition + _hiddenDelta;
			}

			moveTween?.Kill();
			alphaTween?.Kill();
			shown = false;

			if (immediately)
			{
				transform.localPosition = shownPosition;
				if (_canvasGroup != null)
				{
					_canvasGroup.alpha = 1f;
				}
				shown = true;
			}
			else
			{
				transform.localPosition = hiddenPosition;

				moveTween = transform.DOLocalMove(shownPosition, showDuration)
					.SetUpdate(true)
					.SetEase(showEase)
					.OnComplete(() =>
					{
						shown = true;
					});

				if (_canvasGroup != null)
				{
					_canvasGroup.alpha = 0f;

					alphaTween = _canvasGroup.DOFade(1f, showDuration)
						.SetUpdate(true)
						.SetEase(showEase);
				}
			}
			await UniTask.WaitWhile(() => gameObject.activeSelf && !shown);
		}

		protected override async UniTask ShowCloseAnimationAsync(bool immediately = false)
		{
			if (!inited)
			{
				inited = true;
				shownPosition = transform.localPosition;
				hiddenPosition = shownPosition + _hiddenDelta;
			}

			moveTween?.Kill();
			alphaTween?.Kill();
			shown = true;

			if (immediately)
			{
				transform.localPosition = hiddenPosition;
				if (_canvasGroup != null)
				{
					_canvasGroup.alpha = 0f;
				}
				shown = false;
			}
			else
			{
				moveTween = transform.DOLocalMove(hiddenPosition, hideDuration)
					.SetUpdate(true)
					.SetEase(hideEase)
					.OnComplete(() =>
					{
						shown = false;
					});

				if (_canvasGroup != null)
				{
					alphaTween = _canvasGroup.DOFade(0f, hideDuration)
						.SetUpdate(true)
						.SetEase(showEase);
				}
			}
			await UniTask.WaitWhile(() => gameObject.activeSelf && shown);
		}
	}
}
