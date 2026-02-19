using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.Signals;

namespace StepanoffGames.DiceRush.Game.Xp.Signals
{
	public class MoveXpChangedSignal : BaseSignal
	{
		public PlayerModel Player;

		public MoveXpChangedSignal(PlayerModel player)
		{
			Player = player;
		}
	}
}
