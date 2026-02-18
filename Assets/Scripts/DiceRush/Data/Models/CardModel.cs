namespace StepanoffGames.DiceRush.Data.Models
{
	public enum CardKind
	{
		Undefined = 0,
		Dice = 1,
		Tile = 2,
		Fight = 3,
	}

	public enum CardType
	{
		Undefined = 0,

		RerollDice = 1,
		PlusOneToDice = 2,
		PlusTwoToDice = 3,

		RedealTile = 4,
		FlipTile = 5,
	}

	public class CardModel
	{
		public CardKind Kind => _kind;
		public CardType Type => _type;

		private CardKind _kind;
		private CardType _type;

		public CardModel(CardKind kind, CardType type)
		{
			_kind = kind;
			_type = type;
		}
	}
}
