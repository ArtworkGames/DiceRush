using System.Collections.Generic;
using System.Linq;

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

		public List<CardModel> GetCards(CardKind kind)
		{
			return _cards.Where(c => c.Kind == kind).ToList();
		}

		public CardModel GetCard(CardType type)
		{
			return _cards.First(c => c.Type == type);
		}
	}
}
