using StepanoffGames.Services;
using StepanoffGames.UI.Windows;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace StepanoffGames.UI.Components
{
	[RequireComponent(typeof(ScrollRect))]
	public class ScrollRectWheelScroller : MonoBehaviour
	{
		public Action OnWheelScroll;

		[SerializeField] private bool _stopIfOpenedWindowsAvailable = false;
		[SerializeField] private float _scrollDelta = 100f;

		private ScrollRect _scrollRect;
		private Camera _canvasCamera;
		private GraphicRaycaster _raycaster;

		private float scrollDelta;
		private float scrollSpeed;
		private float smoothTime;
		private float velocity;

		private WindowManager _windowManager;

		private void Start()
		{
			_scrollRect = GetComponent<ScrollRect>();
			_scrollRect.scrollSensitivity = 0.0f;

			_canvasCamera = GetComponentInParent<Canvas>().worldCamera;
			_raycaster = GetComponentInParent<GraphicRaycaster>();

			_windowManager = ServiceLocator.Get<WindowManager>();
		}

		private void OnDestroy()
		{
			_scrollRect = null;
			_canvasCamera = null;
			_raycaster = null;
			_windowManager = null;
		}

		private bool IsPointerOverScrollRect()
		{
			Vector2 mousePos = Mouse.current.position.ReadValue();

			bool isOver = RectTransformUtility.RectangleContainsScreenPoint(
				(RectTransform)_scrollRect.transform,
				mousePos,
				_canvasCamera
			);
			return isOver;
		}

		//private bool IsOverlapped()
		//{
		//	Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(_canvasCamera, _scrollRect.transform.position);

		//	PointerEventData eventData = new PointerEventData(EventSystem.current);
		//	eventData.position = screenPoint;

		//	List<RaycastResult> results = new List<RaycastResult>();
		//	_raycaster.Raycast(eventData, results);
		//	bool isOverlapped = false;

		//	foreach (var result in results)
		//	{
		//		if (result.gameObject == _scrollRect.gameObject)
		//			continue;

		//		if ((result.gameObject.transform as RectTransform != null) && (result.depth > 0))
		//		{
		//			isOverlapped = true;
		//			break;
		//		}
		//	}

		//	return isOverlapped;
		//}

		private void Update()
		{
			if (_stopIfOpenedWindowsAvailable && (_windowManager != null) && _windowManager.HasOpenWindow())
			{
				return;
			}

			if (IsPointerOverScrollRect())
			{
				scrollSpeed = 0.0f;
				if (_scrollRect.viewport.GetHeight() < _scrollRect.content.GetHeight())
				{
					scrollSpeed = 100.0f / (_scrollRect.content.GetHeight() - _scrollRect.viewport.GetHeight());
				}

				float scroll = 0f;
				if (Mouse.current != null)
				{
					scroll = Mouse.current.scroll.ReadValue().y;
				}

				//float scroll = Input.GetAxis("Mouse ScrollWheel");
				scroll *= Time.deltaTime * _scrollDelta;
				if (scroll != 0f)
				{
					scrollDelta += scroll * scrollSpeed;
				}

				//if (Input.GetMouseButtonDown(0))
				if (Mouse.current.leftButton.wasPressedThisFrame)
				{
					scrollDelta = 0.0f;
				}
			}

			if (scrollDelta != 0.0f)
			{
				float newPos = _scrollRect.verticalNormalizedPosition + scrollDelta;
				smoothTime = 0.2f;
				if (newPos < 0.0f)
				{
					if (scrollDelta != 0.0f) newPos = 0.0f;
				}
				if (newPos > 1.0f)
				{
					if (scrollDelta != 0.0f) newPos = 1.0f;
				}

				_scrollRect.verticalNormalizedPosition = newPos;
				scrollDelta = Mathf.SmoothDamp(scrollDelta, 0.0f, ref velocity, smoothTime);

				OnWheelScroll?.Invoke();
			}
		}
	}
}
