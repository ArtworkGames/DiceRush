using Cysharp.Threading.Tasks;
using DG.Tweening;
using StepanoffGames.UI.Windows.Animators;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Windows.Animators
{
	public class WindowFadeAnimator : BaseWindowAnimator
	{
		[SerializeField] private float _showDuration = 0.3f;
		[SerializeField] private float _hideDuration = 0.2f;

		private CanvasGroup canvasGroup;

		private bool shown;
		private Tween alphaTween;

		private void Awake()
		{
			canvasGroup = GetComponent<CanvasGroup>();
			if (canvasGroup == null)
			{
				canvasGroup = gameObject.AddComponent<CanvasGroup>();
			}
		}

		private void OnDestroy()
		{
			alphaTween?.Kill();
		}

		public override void Reset()
		{
			if (!Application.isPlaying) return;

			alphaTween?.Kill();
			shown = false;
			canvasGroup.alpha = 0.0f;
		}

		protected override async UniTask ShowOpenAnimationAsync(bool immediately = false)
		{
			alphaTween?.Kill();
			shown = false;

			if (immediately)
			{
				canvasGroup.alpha = 1.0f;
				shown = true;
			}
			else
			{
				canvasGroup.alpha = 0.0f;

				alphaTween = canvasGroup.DOFade(1.0f, _showDuration)
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
			alphaTween?.Kill();
			shown = true;

			if (immediately)
			{
				canvasGroup.alpha = 0.0f;
				shown = false;
			}
			else
			{
				alphaTween = canvasGroup.DOFade(0.0f, _hideDuration)
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
