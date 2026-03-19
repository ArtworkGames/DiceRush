using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.Signals;

namespace StepanoffGames.DiceRush.Game.Deck.Signals
{
	public class PlayerCardsPerOfferChangedSignal : BaseSignal
	{
		public PlayerModel Player;

		public PlayerCardsPerOfferChangedSignal(PlayerModel player)
		{
			Player = player;
		}
	}
}
