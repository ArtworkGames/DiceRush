using UnityEngine;
using UnityEngine.InputSystem;

namespace StepanoffGames.DiceRush.Game
{
	public class InputManager : MonoBehaviour
	{
		private bool isPressed;
		//private bool isDrag;
		//private Vector2 touchPos;
		//private Vector2 prevTouchPos;
		//private Vector2 currTouchPos;
		//private float touchTime;

		private IInputReceiver pressedHUDInputReceiver;

		private void Update()
		{
			if (Mouse.current.leftButton.wasPressedThisFrame)
			{
				isPressed = true;

				pressedHUDInputReceiver = GetHUDInputReceiver();
				if (pressedHUDInputReceiver != null)
				{
					pressedHUDInputReceiver.OnPress();
				}
			}

			if (isPressed && Mouse.current.leftButton.wasReleasedThisFrame)
			{
				IInputReceiver releasedHUDInputReceiver = GetHUDInputReceiver();
				if ((releasedHUDInputReceiver != null) && (releasedHUDInputReceiver == pressedHUDInputReceiver))
				{
					pressedHUDInputReceiver.OnClick();
					pressedHUDInputReceiver = null;
				}
				else
				{
					if (pressedHUDInputReceiver != null)
					{
						pressedHUDInputReceiver.OnRelease();
						pressedHUDInputReceiver = null;
					}
				}

				isPressed = false;
				//isDrag = false;
			}

			//if (isTouch)
			//{
			//	touchTime = Time.time;
			//}
		}

		private IInputReceiver GetHUDInputReceiver()
		{
			Vector2 mousePos = Mouse.current.position.ReadValue();
			Ray ray = Level.Instance.HUDCamera.ScreenPointToRay(mousePos);

			RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction);

			if (hits.Length > 0)
			{
				for (int i = 0; i < hits.Length; i++)
				{
					IInputReceiver inputReceiver = hits[i].collider.GetComponent<IInputReceiver>();
					if ((inputReceiver != null) && inputReceiver.CanPress())
					{
						return inputReceiver;
					}
				}
			}

			return null;
		}
	}
}
