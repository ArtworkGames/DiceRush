using StepanoffGames.DiceRush.Data.Models;
using System.Collections.Generic;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Components.Perks
{
	public class PerksPanel : MonoBehaviour
	{
		[SerializeField] private GameObject _sourcePerk;

		private List<PerksPanelItem> _perks;

		private void Awake()
		{
			_sourcePerk.SetActive(false);
			_perks = new List<PerksPanelItem>();
		}

		public void ShowPerks(PlayerModel player)
		{
			List<PerkType> perkTypes = new List<PerkType>();
			for (int i = 0; i < player.PerksSet.Perks.Count; i++)
			{
				perkTypes.Add(player.PerksSet.Perks[i].Type);
			}

			HidePerks();

			for (int i = 0; i < perkTypes.Count; i++)
			{
				AddPerk(i, perkTypes[i]);
			}
		}

		private void AddPerk(int index, PerkType perkType)
		{
			GameObject perkObject = Instantiate(_sourcePerk, _sourcePerk.transform.parent, false);
			perkObject.name = "Perk" + index;
			perkObject.SetActive(true);

			PerksPanelItem perk = perkObject.GetComponent<PerksPanelItem>();
			perk.UpdateView(perkType);
			_perks.Add(perk);
		}

		public void HidePerks()
		{
			for (int i = 0; i < _perks.Count; i++)
			{
				Destroy(_perks[i].gameObject);
			}
			_perks.Clear();
		}
	}
}
