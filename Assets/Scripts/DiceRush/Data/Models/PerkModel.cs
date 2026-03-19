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
		Battle,
		Opponents,
		Cards,
	}

	public enum PerkType
	{
		Undefined,

		FirstMultiplierX3,
		XpBonusForEachMultiplier,
		OneCardForMultiplierX5,

		CardsPerOfferPlus1,
		CardsPerOfferPlus2,
		CardsPerOfferPlus3,

		IncreaseFirstDefenseBy1,
		Restore1HealthAfterVictory,

		OpponentsSkipMove,
		SwitchPlacesWithFirstOpponent,
		OpponentsStartWithBackwardMove,

		Take3Cards,
	}

	public enum PerkUsage
	{
		Undefined,
		OneTime,
		OneTimeSave,
		Multiple,
	}

	public class PerkModel
	{
		public static PerkModel[] AllPerks = new PerkModel[]
		{
			new PerkModel(PerkKind.Xp, PerkType.FirstMultiplierX3, PerkUsage.Multiple),
			new PerkModel(PerkKind.Xp, PerkType.XpBonusForEachMultiplier, PerkUsage.Multiple, PerkType.FirstMultiplierX3),
			new PerkModel(PerkKind.Xp, PerkType.OneCardForMultiplierX5, PerkUsage.Multiple, PerkType.XpBonusForEachMultiplier),

			new PerkModel(PerkKind.Deck, PerkType.CardsPerOfferPlus1, PerkUsage.OneTimeSave),
			new PerkModel(PerkKind.Deck, PerkType.CardsPerOfferPlus2, PerkUsage.OneTimeSave, PerkType.CardsPerOfferPlus1),
			new PerkModel(PerkKind.Deck, PerkType.CardsPerOfferPlus3, PerkUsage.OneTimeSave, PerkType.CardsPerOfferPlus2),

			new PerkModel(PerkKind.Battle, PerkType.IncreaseFirstDefenseBy1, PerkUsage.Multiple, 0),
			new PerkModel(PerkKind.Battle, PerkType.Restore1HealthAfterVictory, PerkUsage.Multiple, PerkType.IncreaseFirstDefenseBy1),

			//new PerkModel(PerkKind.Opponents, PerkType.OpponentsSkipMove, PerkUsage.OneTime),
			//new PerkModel(PerkKind.Opponents, PerkType.SwitchPlacesWithFirstOpponent, PerkUsage.OneTime),
			//new PerkModel(PerkKind.Opponents, PerkType.OpponentsStartWithBackwardMove, PerkUsage.OneTime),

			new PerkModel(PerkKind.Cards, PerkType.Take3Cards, PerkUsage.OneTime),
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
		public PerkUsage Usage;
		public PerkType RequiredType;

		public PerkModel(PerkKind kind, PerkType type, PerkUsage usage, PerkType requiredType = PerkType.Undefined)
		{
			Kind = kind;
			Type = type;
			Usage = usage;
			RequiredType = requiredType;
		}

		public PerkModel Clone()
		{
			PerkModel card = new PerkModel(Kind, Type, Usage, RequiredType);
			return card;
		}
	}
}
