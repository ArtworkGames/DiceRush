using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Messages
{
	public class MessageRow : MonoBehaviour
	{
		public Action<MessageRow> OnHide;

		[SerializeField] private CanvasGroup _canvasGroup;
		[SerializeField] private TMP_Text _text;

		private Tween alphaTween;
		private Tween moveTween;

		private void OnDestroy()
		{
			moveTween?.Kill();
		}

		public void Show(string text)
		{
			_text.text = text;

			//_canvasGroup.alpha = 0f;
			//alphaTween = _canvasGroup.DOFade(1f, 0.3f)
			//	.SetEase(Ease.OutQuad)
			//	.OnComplete(() =>
			//	{
			//		alphaTween = _canvasGroup.DOFade(0f, 0.3f)
			//			.SetDelay(0.4f)
			//			.SetEase(Ease.OutQuad)
			//			.OnComplete(() =>
			//			{
			//				Hide();
			//			});
			//	});
			alphaTween = _canvasGroup.DOFade(0f, 0.3f)
				.SetDelay(0.7f)
				.SetEase(Ease.OutQuad)
				.OnComplete(() =>
				{
					Hide();
				});
		}

		private void Hide()
		{
			OnHide?.Invoke(this);
			Destroy(gameObject);
		}

		public void MoveUp()
		{
			moveTween = transform.DOLocalMoveY(transform.localPosition.y + 120f, 0.3f)
				.SetEase(Ease.OutCubic);
		}
	}
}
