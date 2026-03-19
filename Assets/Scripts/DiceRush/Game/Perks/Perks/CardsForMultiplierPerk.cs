using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Chest;
using StepanoffGames.DiceRush.Game.Deck;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.Services;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Perks.Perks
{
	public class CardsForMultiplierPerk : Perk
	{
		private int _xpMultiplier;
		//private int _cardsCount;

		public CardsForMultiplierPerk(PerkModel model) : base(model)
		{
			switch (model.Type)
			{
				case PerkType.OneCardForMultiplierX5:
					_xpMultiplier = 5;
					//_cardsCount = 1;
					break;
			}
		}

		override public async UniTask<bool> Use(PlayerController player)
		{
			return await Apply(player);
		}

		override public async UniTask<bool> Apply(PlayerController player)
		{
			if (player.Model.XpMultiplier == _xpMultiplier)
			{
				Debug.Log($"[CardsEveryNMovesPerk] Apply: XpMultiplier = {player.Model.XpMultiplier}");
				ChestController chestController = ServiceLocator.Get<ChestController>();
				CardModel card = chestController.GetCard(player);

				DeckController deckController = ServiceLocator.Get<DeckController>();
				deckController.AddCards(player, new CardModel[] { card });
				return true;
			}
			await UniTask.Yield();
			return false;
		}
	}
}
