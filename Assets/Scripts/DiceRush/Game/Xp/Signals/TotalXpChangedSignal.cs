using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.Signals;

namespace StepanoffGames.DiceRush.Game.Xp.Signals
{
	public class TotalXpChangedSignal : BaseSignal
	{
		public PlayerModel Player;

		public TotalXpChangedSignal(PlayerModel player)
		{
			Player = player;
		}
	}
}
