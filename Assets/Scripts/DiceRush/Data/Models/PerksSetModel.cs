using System.Collections.Generic;
using System.Linq;

namespace StepanoffGames.DiceRush.Data.Models
{
	public class PerksSetModel
	{
		public List<PerkModel> Perks => _perks;
		private List<PerkModel> _perks;

		public PerksSetModel()
		{
			_perks = new List<PerkModel>();
		}

		public void AddPerk(PerkModel perk)
		{
			_perks.Add(perk);
		}

		public void RemovePerk(PerkModel perk)
		{
			_perks.Remove(perk);
		}

		public List<PerkModel> GetPerks(PerkKind kind)
		{
			return _perks.Where(c => c.Kind == kind).ToList();
		}

		public PerkModel GetPerk(PerkType type)
		{
			PerkModel perk = _perks.FirstOrDefault(c => c.Type == type);
			if (perk == default) return null;
			return perk;
		}
	}
}
