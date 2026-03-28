using StepanoffGames.Signals;

namespace StepanoffGames.DiceRush.Game.Players.Signals
{
	public class PlayerTurnStartedSignal : BaseSignal
	{
		public PlayerController Player;

		public PlayerTurnStartedSignal(PlayerController player)
		{
			Player = player;
		}
	}
}
