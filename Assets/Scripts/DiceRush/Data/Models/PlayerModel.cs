namespace StepanoffGames.DiceRush.Data.Models
{
	public enum PlayerType
	{
		HI,
		AI
	}

	public class PlayerModel
	{
		public PlayerType Type;

		public int MoveXp;
		public int XpMultiplier;
		public int TotalXp;
		public int Level;

		public DeckModel Deck => _deck;
		private DeckModel _deck;

		public bool IsXpAdditionCompleted;

		public PlayerModel(PlayerType type)
		{
			Type = type;

			_deck = new DeckModel();

			_deck.AddCard(CardModel.GetCard(CardType.RerollDice).Clone());
			//_deck.AddCard(new CardModel(CardKind.Dice, CardType.PlusOneToDice));
			//_deck.AddCard(new CardModel(CardKind.Dice, CardType.PlusTwoToDice));

			_deck.AddCard(CardModel.GetCard(CardType.RedealTile).Clone());
			//_deck.AddCard(new CardModel(CardKind.Tile, CardType.FlipTile));
			//_deck.AddCard(new CardModel(CardKind.Tile, CardType.FlipTile));
		}
	}
}
