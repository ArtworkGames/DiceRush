using StepanoffGames.Signals;

namespace StepanoffGames.DiceRush.Game.Players.Signals
{
	public class PlayerMoveStartedSignal : BaseSignal
	{
		public PlayerController Player;

		public PlayerMoveStartedSignal(PlayerController player)
		{
			Player = player;
		}
	}
}
