using StepanoffGames.DiceRush.Game;
using StepanoffGames.DiceRush.UI.Components.Perks;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using StepanoffGames.UI.Components;
using StepanoffGames.UI.Popups;
using StepanoffGames.UI.Popups.Signals;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Popups.PerkDescriptionPopup
{
	public class PerkDescriptionPopupParams : BasePopupParams
	{
		public PerkIconItem PerksPanelItem;
	}

	public class PerkDescriptionPopup : BasePopup<PerkDescriptionPopupParams>
	{
		public static void Show(PerkIconItem perksPanelItem)
		{
			LevelManager levelManager = ServiceLocator.Get<LevelManager>();

			Vector3 worldPos = perksPanelItem.transform.position;
			Vector2 scrPos = levelManager.UICamera.WorldToScreenPoint(worldPos);

			SignalBus.Publish(new OpenPopupSignal(PrefabName, scrPos)
			{
				Params = new PerkDescriptionPopupParams()
				{
					PerksPanelItem = perksPanelItem,
				},
				CloseOther = false,
				Autoclosing = false
			});
		}

		public static string PrefabName = "PerkDescriptionPopup";

		[SerializeField] private TMPTextLocalizer _titleLocalizer;
		[SerializeField] private TMPTextLocalizer _descriptionLocalizer;

		override protected void BeforeOpen()
		{
			_titleLocalizer.Localize($"Perk:{Params.PerksPanelItem.Type}:Title");
			_descriptionLocalizer.Localize($"Perk:{Params.PerksPanelItem.Type}:Description");
		}
	}
}
