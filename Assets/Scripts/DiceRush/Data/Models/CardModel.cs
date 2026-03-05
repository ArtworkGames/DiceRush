using System.Linq;

namespace StepanoffGames.DiceRush.Data.Models
{
	public enum CardKind
	{
		Undefined,
		Dice,
		Bag,
		Battle,
	}

	public enum CardType
	{
		Undefined,

		RerollDice,
		Plus1ToDice,
		Plus2ToDice,
		Minus1FromDice,
		Minus2FromDice,

		RedrawToken,

		Plus1ToHealth,
		Plus2ToHealth,
		Plus3ToHealth,
		Plus1ToDefense,
		Plus2ToDefense,
		Plus3ToDefense,
		Plus1ToAttack,
		Plus2ToAttack,
		Plus3ToAttack,
	}

	public class CardModel
	{
		public static CardModel[] AllCards = new CardModel[]
		{
			new CardModel(CardKind.Dice, CardType.RerollDice, 2),
			new CardModel(CardKind.Dice, CardType.Plus1ToDice, 2),
			new CardModel(CardKind.Dice, CardType.Plus2ToDice, 2),
			new CardModel(CardKind.Dice, CardType.Minus1FromDice, 2),
			new CardModel(CardKind.Dice, CardType.Minus2FromDice, 2),

			new CardModel(CardKind.Bag, CardType.RedrawToken, 2),

			new CardModel(CardKind.Battle, CardType.Plus1ToHealth, 2),
			new CardModel(CardKind.Battle, CardType.Plus2ToHealth, 2),
			new CardModel(CardKind.Battle, CardType.Plus3ToHealth, 2),
			new CardModel(CardKind.Battle, CardType.Plus1ToDefense, 2),
			new CardModel(CardKind.Battle, CardType.Plus2ToDefense, 2),
			new CardModel(CardKind.Battle, CardType.Plus3ToDefense, 2),
			new CardModel(CardKind.Battle, CardType.Plus1ToAttack, 2),
			new CardModel(CardKind.Battle, CardType.Plus2ToAttack, 2),
			new CardModel(CardKind.Battle, CardType.Plus3ToAttack, 2),
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
