using System.Collections.Generic;

namespace StepanoffGames.DiceRush.Data.Models
{
	public class DeckModel
	{
		public List<CardModel> Cards => _cards;

		private List<CardModel> _cards;

		public DeckModel()
		{
			_cards = new List<CardModel>();
		}

		public void AddCard(CardModel card)
		{
			_cards.Add(card);
		}

		public void RemoveCard(CardModel card)
		{
			_cards.Remove(card);
		}
	}
}
