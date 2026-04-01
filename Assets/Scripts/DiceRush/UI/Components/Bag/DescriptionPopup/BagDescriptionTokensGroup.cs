using StepanoffGames.DiceRush.UI.Components.Deck.DescriptionPopup;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace StepanoffGames.DiceRush.UI.Components.Bag.DescriptionPopup
{
	public class BagDescriptionTokensGroup : TweenButton
	{
		[Space]
		[SerializeField] private GameObject _sourceToken;
		//[Space]
		//[SerializeField] private DeckDescriptionPopup _descriptionPopup;

		private List<GameObject> _tokens = new List<GameObject>();

		private void Awake()
		{
			mode = TweenButtonMode.Focusable;

			_sourceToken.SetActive(false);
		}

		public void ShowTokens(int count)
		{
			ClearTokens();

			gameObject.SetActive(count > 0);

			for (int i = 0; i < count; i++)
			{
				AddToken();
			}
		}

		private void ClearTokens()
		{
			for (int i = 0; i < _tokens.Count; i++)
			{
				Destroy(_tokens[i]);
			}
			_tokens.Clear();
		}

		private void AddToken()
		{
			GameObject tokenObject = Instantiate(_sourceToken, _sourceToken.transform.parent, false);
			tokenObject.name = $"Token";
			tokenObject.SetActive(true);

			_tokens.Add(tokenObject);
		}

		override public void OnPointerEnter(PointerEventData eventData)
		{
			//_descriptionPopup.Show();

			base.OnPointerEnter(eventData);
		}

		override public void OnPointerExit(PointerEventData eventData)
		{
			//_descriptionPopup.Hide();

			base.OnPointerExit(eventData);
		}
	}
}
