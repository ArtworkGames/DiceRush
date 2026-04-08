using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Components
{
	public class SelectableButton : TweenButton
	{
		[SerializeField] protected GameObject _selection;

		private bool _selected = false;
		public bool Selected
		{
			get
			{
				return _selected;
			}
			set
			{
				_selected = value;
				UpdateSelection();
			}
		}

		private CanvasGroup _canvasGroup;
		private CanvasGroup CanvasGroup
		{
			get
			{
				if (_canvasGroup == null)
				{
					_canvasGroup = GetComponent<CanvasGroup>();
					if (_canvasGroup == null)
					{
						_canvasGroup = gameObject.AddComponent<CanvasGroup>();
					}
				}
				return _canvasGroup;
			}
		}

		private void UpdateSelection()
		{
			CanvasGroup.interactable = !_selected;
			CanvasGroup.blocksRaycasts = !_selected;
			_selection.SetActive(_selected);
		}
	}
}

