using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Deck;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.Services;
using System.Threading;

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

		override public async UniTask<bool> Use(PlayerController player, CancellationToken ct)
		{
			return await Apply(player, ct);
		}

		override public async UniTask<bool> Apply(PlayerController player, CancellationToken ct)
		{
			DeckController deckController = ServiceLocator.Get<DeckController>();
			deckController.SetCardsPerOffer(player, player.Model.BaseCardsPerOffer + _delta);
			await UniTask.Yield(ct);
			return true;
		}
	}
}
