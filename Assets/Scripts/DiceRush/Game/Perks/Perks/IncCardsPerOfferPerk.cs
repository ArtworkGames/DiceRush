using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Players;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Perks.Perks
{
	public class IncCardsPerOfferPerk : Perk
	{
		private int _delta;

		public IncCardsPerOfferPerk(PerkModel model) : base(model)
		{
			switch (model.Type)
			{
				case PerkType.CardsPerOfferPlus1: _delta = 1; break;
				case PerkType.CardsPerOfferPlus2: _delta = 2; break;
				case PerkType.CardsPerOfferPlus3: _delta = 3; break;
			}
		}

		override public async UniTask Use(PlayerController player)
		{
			player.Model.CardsPerOffer = player.Model.BaseCardsPerOffer + _delta;
			await UniTask.Yield();
		}

		override public async UniTask Apply(PlayerController player)
		{
			player.Model.CardsPerOffer = player.Model.BaseCardsPerOffer + _delta;
			await UniTask.Yield();
		}
	}
}
