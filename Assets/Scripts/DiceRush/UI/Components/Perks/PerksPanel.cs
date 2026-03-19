using StepanoffGames.DiceRush.Data.Models;
using System.Collections.Generic;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Components.Perks
{
	public class PerksPanel : MonoBehaviour
	{
		[SerializeField] private GameObject _sourcePerkItem;

		private Dictionary<PerkType, PerkIconItem> _perks;

		private void Awake()
		{
			_perks = new Dictionary<PerkType, PerkIconItem>();
			_sourcePerkItem.SetActive(false);
		}

		public void ShowPerks(PlayerModel player)
		{
			for (int i = 0; i < player.PerksSet.Perks.Count; i++)
			{
				if (player.PerksSet.Perks[i].Usage == PerkUsage.Multiple && !_perks.ContainsKey(player.PerksSet.Perks[i].Type))
				{
					AddPerk(player.PerksSet.Perks[i].Type);
				}
			}
		}

		public PerkIconItem AddPerk(PerkType perkType)
		{
			GameObject perkObject = Instantiate(_sourcePerkItem, _sourcePerkItem.transform.parent, false);
			perkObject.name = $"PerkItem ({perkType})";
			perkObject.SetActive(true);

			PerkIconItem perk = perkObject.GetComponent<PerkIconItem>();
			perk.Show(perkType);
			_perks.Add(perkType, perk);

			return perk;
		}

		public PerkIconItem GetPerkItem(PerkType perkType)
		{
			if (_perks.ContainsKey(perkType))
			{
				return _perks[perkType];
			}
			return null;
		}
	}
}
