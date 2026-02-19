using System.Linq;

namespace StepanoffGames.DiceRush.Data.Models
{
	public enum CardKind
	{
		Undefined,
		Dice,
		Tile,
		Fight,
	}

	public enum CardType
	{
		Undefined,

		RerollDice,
		PlusOneToDice,
		PlusTwoToDice,

		FlipTile,
		RedealTile,
	}

	public class CardModel
	{
		public static CardModel[] AllCards = new CardModel[]
		{
			new CardModel(CardKind.Dice, CardType.PlusOneToDice, 2),
			new CardModel(CardKind.Dice, CardType.PlusTwoToDice, 2),
			new CardModel(CardKind.Dice, CardType.RerollDice, 2),

			new CardModel(CardKind.Tile, CardType.FlipTile, 2),
			new CardModel(CardKind.Tile, CardType.FlipTile, 2),
			new CardModel(CardKind.Tile, CardType.RedealTile, 2),
		};

		public static CardModel[] GetCards(CardKind kind)
		{
			return AllCards.Where(c => c.Kind == kind).ToArray();
		}

		public static CardModel GetCard(CardType type)
		{
			return AllCards.First(c => c.Type == type);
		}

		public CardKind Kind;
		public CardType Type;
		public int Cost;

		public CardModel(CardKind kind, CardType type, int cost)
		{
			Kind = kind;
			Type = type;
			Cost = cost;
		}

		public CardModel Clone()
		{
			CardModel card = new CardModel(Kind, Type, Cost);
			return card;
		}
	}
}
