using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.Localization;
using StepanoffGames.Services;
using TMPro;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Components.Perks
{
	public class PerksPanelItem : MonoBehaviour
	{
		[SerializeField] private TMP_Text _title;

		public void UpdateView(PerkType perkType)
		{
			LocalizationManager localizationManager = ServiceLocator.Get<LocalizationManager>();

			_title.text = localizationManager.GetString($"Perk:{perkType}:Title");
		}
	}
}
