using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace StepanoffGames.DiceRush.UI.Components
{
	public enum TweenButtonMode
	{
		Focusable,
		Pressable
	}

	public class TweenButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
	{
		public Action OnClick;

		[SerializeField] protected Transform _content;

		public Transform Content => _content;

		protected TweenButtonMode mode = TweenButtonMode.Pressable;

		protected float focusTime = 0.2f;
		protected float unfocusTime = 0.15f;
		protected float pressTime = 0.15f;

		protected float focusedScale = 1.2f;
		protected float pressedScale = 0.9f;

		private bool isFocused = false;
		private bool isPressed = false;
		private bool isClicked = false;

		private Tween scaleTween;

		virtual protected void OnDestroy()
		{
			scaleTween?.Kill();
		}

		virtual public void OnPointerEnter(PointerEventData eventData)
		{
			isFocused = true;

			if (!isPressed)
			{
				scaleTween?.Kill();
				scaleTween = _content.DOScale(focusedScale, focusTime)
					.SetUpdate(true)
					.SetEase(Ease.OutBack);
			}
		}

		virtual public void OnPointerExit(PointerEventData eventData)
		{
			isFocused = false;
			isPressed = false;
			isClicked = false;

			scaleTween?.Kill();
			scaleTween = _content.DOScale(1f, unfocusTime)
				.SetUpdate(true)
				.SetEase(Ease.OutBack);
		}

		virtual public void OnPointerDown(PointerEventData eventData)
		{
			if (mode != TweenButtonMode.Pressable) return;

			isPressed = true;
			isClicked = true;

			scaleTween?.Kill();
			scaleTween = _content.DOScale(pressedScale, pressTime)
				.SetUpdate(true)
				.SetEase(Ease.OutCubic);
		}

		virtual public void OnPointerUp(PointerEventData eventData)
		{
			if (mode != TweenButtonMode.Pressable) return;

			isPressed = false;

			if (isFocused)
			{
				OnPointerEnter(eventData);
			}
			else
			{
				OnPointerExit(eventData);
			}
		}

		virtual public void OnPointerClick(PointerEventData eventData)
		{
			if (mode != TweenButtonMode.Pressable) return;

			if (isClicked)
			{
				DoClick();
			}
			isClicked = false;
		}

		virtual public void DoClick()
		{
			OnClick?.Invoke();
		}
	}
}
