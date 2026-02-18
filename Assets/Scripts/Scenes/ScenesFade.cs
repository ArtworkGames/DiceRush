using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace StepanoffGames.Scenes
{
	public class ScenesFade : MonoBehaviour
	{
		[SerializeField] private CanvasGroup _canvasGroup;

		private float showDuration = 0.2f;
		private float hideDuration = 0.2f;

		private bool shown;
		private Tween alphaTween;

		private void Awake()
		{
			_canvasGroup.alpha = 1.0f;
		}

		private void OnDestroy()
		{
			alphaTween?.Kill();
		}

		public async UniTask ShowAsync(bool immediately = false)
		{
			alphaTween?.Kill();
			shown = false;

			_canvasGroup.interactable = true;
			_canvasGroup.blocksRaycasts = true;

			if (immediately)
			{
				_canvasGroup.alpha = 1.0f;
				shown = true;
			}
			else
			{
				_canvasGroup.alpha = 0.0f;

				alphaTween = _canvasGroup.DOFade(1.0f, showDuration)
					.SetUpdate(true)
					.SetEase(Ease.OutCubic)
					.OnComplete(() =>
					{
						shown = true;
					});
			}
			await UniTask.WaitWhile(() => !shown);
		}

		public async UniTask HideAsync(bool immediately = false)
		{
			alphaTween?.Kill();
			shown = true;

			if (immediately)
			{
				_canvasGroup.alpha = 0.0f;
				_canvasGroup.interactable = false;
				_canvasGroup.blocksRaycasts = false;
				shown = false;
			}
			else
			{
				alphaTween = _canvasGroup.DOFade(0.0f, hideDuration)
					.SetUpdate(true)
					.SetEase(Ease.InCubic)
					.OnComplete(() =>
					{
						_canvasGroup.interactable = false;
						_canvasGroup.blocksRaycasts = false;
						shown = false;
					});
			}
			await UniTask.WaitWhile(() => shown);
		}
	}
}
