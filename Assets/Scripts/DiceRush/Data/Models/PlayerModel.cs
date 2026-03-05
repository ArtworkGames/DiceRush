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

		public int BaseAttack = 5;
		public int BaseDefense = 3;

		public int Health = 20;
		public int Attack = 5;
		public int Defense = 3;

		public int Place;
		public int PrevPlace;
		public int CellIndex;

		public int MoveXp;
		public int XpMultiplier;
		public int TotalXp;
		public int Level;

		public int BaseCardsPerOffer = 2;
		public int CardsPerOffer = 2;

		public DeckModel Deck => _deck;
		private DeckModel _deck;

		public PerksSetModel PerksSet => _perksSet;
		private PerksSetModel _perksSet;

		public bool IsXpAdditionCompleted;

		public PlayerModel(PlayerType type)
		{
			Type = type;

			_deck = new DeckModel();

			_deck.AddCard(CardModel.GetCard(CardType.RerollDice).Clone());
			_deck.AddCard(CardModel.GetCard(CardType.RedrawToken).Clone());
			_deck.AddCard(CardModel.GetCard(CardType.Plus1ToDefense).Clone());

			_perksSet = new PerksSetModel();
		}
	}
}
