using StepanoffGames.DiceRush.UI.Bag.DescriptionPopup;
using StepanoffGames.DiceRush.UI.Components;
using UnityEngine;
using UnityEngine.EventSystems;

namespace StepanoffGames.DiceRush.UI.Bag
{
	public class BagButton : TweenButton
	{
		[Space]
		[SerializeField] private CanvasGroup _canvasGroup;
		[Space]
		[SerializeField] private BagDescriptionPopup _descriptionPopup;

		public BagDescriptionPopup DescriptionPopup => _descriptionPopup;

		private void Awake()
		{
			mode = TweenButtonMode.Focusable;
			Hide();
		}

		public void Show()
		{
			_canvasGroup.interactable = true;
			_canvasGroup.blocksRaycasts = true;
		}

		public void Hide()
		{
			_canvasGroup.interactable = false;
			_canvasGroup.blocksRaycasts = false;
			_descriptionPopup.Hide();
		}

		override public void OnPointerEnter(PointerEventData eventData)
		{
			_descriptionPopup.Show();

			base.OnPointerEnter(eventData);
		}

		override public void OnPointerExit(PointerEventData eventData)
		{
			_descriptionPopup.Hide();

			base.OnPointerExit(eventData);
		}
	}
}
