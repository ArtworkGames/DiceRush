using System.Linq;

namespace StepanoffGames.DiceRush.Data.Models
{
	public enum PerkKind
	{
		Undefined,
		Xp,
		Dice,
		Bag,
		Deck,
		Fight,
		Opponents,
		Cards,
	}

	public enum PerkType
	{
		Undefined,

		FirstMultiplierX3,
		OneCardEvery3Move,
		XpBonusForMultiplierX4,
		InstantCardEvery5Move,

		CardsPerOfferPlus1,
		CardsPerOfferPlus2,
		CardsPerOfferPlus3,

		HitPointsPlus1,
		HitPointsPlus2,
		HitPointsPlus3,

		OpponentsSkipMove,
		SwitchPlacesWithFirstOpponent,
		OpponentsStartWithBackwardMove,

		Take3Cards,
	}

	public class PerkModel
	{
		public static PerkModel[] AllPerks = new PerkModel[]
		{
			new PerkModel(PerkKind.Xp, PerkType.FirstMultiplierX3, false, 2),
			new PerkModel(PerkKind.Xp, PerkType.OneCardEvery3Move, false, 4),
			new PerkModel(PerkKind.Xp, PerkType.XpBonusForMultiplierX4, false, 6),
			new PerkModel(PerkKind.Xp, PerkType.InstantCardEvery5Move, false, 8),

			new PerkModel(PerkKind.Deck, PerkType.CardsPerOfferPlus1, false, 0),
			new PerkModel(PerkKind.Deck, PerkType.CardsPerOfferPlus2, false, 0, PerkType.CardsPerOfferPlus1),
			new PerkModel(PerkKind.Deck, PerkType.CardsPerOfferPlus3, false, 0, PerkType.CardsPerOfferPlus2),

			//new PerkModel(PerkKind.Fight, PerkType.HitPointsPlus1),
			//new PerkModel(PerkKind.Fight, PerkType.HitPointsPlus2),
			//new PerkModel(PerkKind.Fight, PerkType.HitPointsPlus3),

			new PerkModel(PerkKind.Opponents, PerkType.OpponentsSkipMove, true),
			new PerkModel(PerkKind.Opponents, PerkType.SwitchPlacesWithFirstOpponent, true),
			new PerkModel(PerkKind.Opponents, PerkType.OpponentsStartWithBackwardMove, true),

			new PerkModel(PerkKind.Cards, PerkType.Take3Cards, true),
		};

		public static PerkModel[] GetPerks(PerkKind kind)
		{
			return AllPerks.Where(c => c.Kind == kind).ToArray();
		}

		public static PerkModel GetPerk(PerkType type)
		{
			return AllPerks.First(c => c.Type == type);
		}

		public PerkKind Kind;
		public PerkType Type;
		public bool IsSingleUse;
		public int StartFromLevel;
		public PerkType RequiredType;

		public PerkModel(PerkKind kind, PerkType type, bool isSingleUse = false, int startFromLevel = 0, PerkType requiredType = PerkType.Undefined)
		{
			Kind = kind;
			Type = type;
			IsSingleUse = isSingleUse;
			StartFromLevel = startFromLevel;
			RequiredType = requiredType;
		}

		public PerkModel Clone()
		{
			PerkModel card = new PerkModel(Kind, Type, IsSingleUse, StartFromLevel, RequiredType);
			return card;
		}
	}
}
