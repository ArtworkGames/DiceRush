using StepanoffGames.Signals;

namespace StepanoffGames.DiceRush.Game.Players.Signals
{
	public class PlayerCellPassedSignal : BaseSignal
	{
		public PlayerController Player;

		public PlayerCellPassedSignal(PlayerController player)
		{
			Player = player;
		}
	}
}
