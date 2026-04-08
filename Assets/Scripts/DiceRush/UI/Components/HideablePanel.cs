using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Components
{
	public class HideablePanel : MonoBehaviour
	{
		[SerializeField] private Vector2 _hideDelta;
		[SerializeField] private bool _initHidden;

		public bool IsShown => _isShown;
		private bool _isShown;

		private CanvasGroup _canvasGroup;

		private Vector3 shownPos;
		private Vector3 hiddenPos;
		private Tween showTween;
		private bool isDestroyed;

		private void Start()
		{
			shownPos = transform.localPosition;
			hiddenPos = new Vector3(
				shownPos.x + _hideDelta.x,
				shownPos.y + _hideDelta.y,
				shownPos.z);

			if (_initHidden)
			{
				transform.localPosition = hiddenPos;
			}

			_canvasGroup = GetComponent<CanvasGroup>();
			if (_canvasGroup != null)
			{
				_canvasGroup.interactable = false;
				_canvasGroup.blocksRaycasts = false;
			}
		}

		private void OnDestroy()
		{
			showTween?.Kill();
			isDestroyed = true;
		}

		public async UniTask Show(bool immediately, CancellationToken ct)
		{
			_isShown = true;
			showTween?.Kill();

			if (immediately)
			{
				transform.localPosition = shownPos;
				if (_canvasGroup != null)
				{
					_canvasGroup.interactable = true;
					_canvasGroup.blocksRaycasts = true;
				}
			}
			else
			{
				bool isShowCompleted = false;
				showTween = transform.DOLocalMove(shownPos, 0.3f)
					.SetEase(Ease.OutCubic)
					.OnComplete(() =>
					{
						isShowCompleted = true;
						if (_canvasGroup != null)
						{
							_canvasGroup.interactable = true;
							_canvasGroup.blocksRaycasts = true;
						}
					});
				await UniTask.WaitUntil(() => isShowCompleted || !_isShown || isDestroyed, cancellationToken: ct);
			}
		}

		public async UniTask Hide(bool immediately, CancellationToken ct)
		{
			_isShown = false;
			showTween?.Kill();

			if (_canvasGroup != null)
			{
				_canvasGroup.interactable = false;
				_canvasGroup.blocksRaycasts = false;
			}

			if (immediately)
			{
				transform.localPosition = hiddenPos;
			}
			else
			{
				bool isHideCompleted = false;
				showTween = transform.DOLocalMove(hiddenPos, 0.3f)
					.SetEase(Ease.OutCubic)
					.OnComplete(() =>
					{
						isHideCompleted = true;
					});
				await UniTask.WaitUntil(() => isHideCompleted || _isShown || isDestroyed, cancellationToken: ct);
			}
		}
	}
}
