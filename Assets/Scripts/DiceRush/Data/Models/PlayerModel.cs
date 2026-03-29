namespace StepanoffGames.DiceRush.Data.Models
{
	public enum PlayerType
	{
		HI,
		AI
	}

	public enum PlayerColor
	{
		Red,
		Blue,
		Green,
		Yellow,
	}

	public enum PlayerState
	{
		Undefined,
		Waiting,
		RollDice,
		ConfirmDice,
		MoveForward,
		MoveBackward,
		SelectDirection,
		DrawToken,
		ConfirmToken,
		OpenChest,
		Battle,
		MoveToPortal,
		MoveToPosition,
		CountXp,
		EndTurn,
		Finish,
	}

	public class PlayerModel
	{
		public string Name;

		public PlayerType Type;
		public PlayerState State;

		public int MaxHealth = 20;
		public int BaseDefense = 3;
		public int BaseAttack = 5;

		public int Health = 20;
		public int Defense = 3;
		public int Attack = 5;

		public int ExtraDefense;
		public int ExtraAttack;
		public int BattleRound;

		public int Place;
		public int PrevPlace;
		public int CellIndex;

		public int MovesCount;
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

		public bool IsTotalXpCounted;
		public bool IsFinished;

		public PlayerModel(string name, PlayerType type)
		{
			Name = name;
			Type = type;

			_deck = new DeckModel();

			_deck.AddCard(CardModel.GetCard(CardType.RerollDice).Clone());
			_deck.AddCard(CardModel.GetCard(CardType.RedrawToken).Clone());
			_deck.AddCard(CardModel.GetCard(CardType.Plus1ToDefense).Clone());

			_perksSet = new PerksSetModel();

			//_perksSet.AddPerk(PerkModel.GetPerk(PerkType.IncreaseFirstDefenseBy1).Clone());
		}
	}
}
