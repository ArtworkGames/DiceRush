using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.UI.Components;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Deck
{
	public class CardView : MonoBehaviour
	{
		[SerializeField] private TextLocalizer _textLocalizer;

		public CardModel Model => _model;
		private CardModel _model;

		public void SetModel(CardModel cardModel)
		{
			_model = cardModel;

			string key = $"Card:{_model.Type}";
			_textLocalizer.Localize(key);
		}

		private void OnDestroy()
		{
			_model = null;
		}
	}
}
