using Cysharp.Threading.Tasks;
using DG.Tweening;
using StepanoffGames.UI.Windows.Animators;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Windows.Animators
{
	public class CenterContentAnimator : BaseWindowAnimator
	{
		protected CanvasGroup canvasGroup;

		protected float showDuration = 0.15f;
		protected float hideDuration = 0.15f;
		protected float minScale = 0.9f;

		private bool shown;
		private Tween scaleTween;
		private Tween alphaTween;

		protected virtual void Awake()
		{
			canvasGroup = GetComponent<CanvasGroup>();
			if (canvasGroup == null)
			{
				canvasGroup = gameObject.AddComponent<CanvasGroup>();
			}
		}

		private void OnDestroy()
		{
			scaleTween?.Kill();
			alphaTween?.Kill();
		}

		public override void Reset()
		{
			if (!Application.isPlaying) return;

			scaleTween?.Kill();
			alphaTween?.Kill();
			shown = false;
			transform.localScale = Vector3.one * minScale;
			canvasGroup.alpha = 0.0f;
		}

		protected override async UniTask ShowOpenAnimationAsync(bool immediately = false)
		{
			scaleTween?.Kill();
			alphaTween?.Kill();
			shown = false;

			if (immediately)
			{
				transform.localScale = Vector3.one;
				canvasGroup.alpha = 1.0f;
				shown = true;
			}
			else
			{
				transform.localScale = Vector3.one * minScale;
				canvasGroup.alpha = 0.0f;

				scaleTween = transform.DOScale(Vector3.one, showDuration)
					.SetUpdate(true)
					.SetEase(Ease.OutCubic);

				alphaTween = canvasGroup.DOFade(1.0f, showDuration)
					.SetUpdate(true)
					.SetEase(Ease.OutCubic)
					.OnComplete(() =>
					{
						shown = true;
					});
			}
			await UniTask.WaitWhile(() => gameObject.activeSelf && !shown);
		}

		protected override async UniTask ShowCloseAnimationAsync(bool immediately = false)
		{
			scaleTween?.Kill();
			alphaTween?.Kill();
			shown = true;

			if (immediately)
			{
				transform.localScale = Vector3.one * minScale;
				canvasGroup.alpha = 0.0f;
				shown = false;
			}
			else
			{
				scaleTween = transform.DOScale(Vector3.one * minScale, hideDuration)
					.SetUpdate(true)
					.SetEase(Ease.OutCubic);

				alphaTween = canvasGroup.DOFade(0.0f, hideDuration)
					.SetUpdate(true)
					.SetEase(Ease.OutCubic)
					.OnComplete(() =>
					{
						shown = false;
					});
			}
			await UniTask.WaitWhile(() => gameObject.activeSelf && shown);
		}
	}
}