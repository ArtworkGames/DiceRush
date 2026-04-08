using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.UI.Components;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Perks.DescriptionPopup
{
	public class PerkDescriptionPopup : MonoBehaviour
	{
		[SerializeField] private GameObject _content;
		[Space]
		[SerializeField] private TMPTextLocalizer _titleLocalizer;
		[SerializeField] private TMPTextLocalizer _descriptionLocalizer;

		private void Start()
		{
			Hide();
		}

		public void Show()
		{
			_content.SetActive(true);
		}

		public void Hide()
		{
			_content.SetActive(false);
		}

		public void SetPerkType(PerkType type)
		{
			_titleLocalizer.Localize($"Perk:{type}:Title");
			_descriptionLocalizer.Localize($"Perk:{type}:Description");
		}
	}
}
