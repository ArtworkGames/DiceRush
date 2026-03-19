using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.Signals;

namespace StepanoffGames.DiceRush.Game.Deck.Signals
{
	public class PlayerCardsChangedSignal : BaseSignal
	{
		public PlayerModel Player;

		public PlayerCardsChangedSignal(PlayerModel player)
		{
			Player = player;
		}
	}
}
