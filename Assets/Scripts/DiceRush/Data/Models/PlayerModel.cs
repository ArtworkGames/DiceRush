namespace StepanoffGames.DiceRush.Data.Models
{
	public class PlayerModel
	{
		public DeckModel Deck => _deck;

		private DeckModel _deck;

		public PlayerModel()
		{
			_deck = new DeckModel();

			_deck.AddCard(new CardModel(CardKind.Dice, CardType.RerollDice));
			_deck.AddCard(new CardModel(CardKind.Dice, CardType.PlusOneToDice));
			_deck.AddCard(new CardModel(CardKind.Dice, CardType.PlusTwoToDice));

			_deck.AddCard(new CardModel(CardKind.Tile, CardType.RedealTile));
			_deck.AddCard(new CardModel(CardKind.Tile, CardType.FlipTile));
			_deck.AddCard(new CardModel(CardKind.Tile, CardType.FlipTile));
		}
	}
}
