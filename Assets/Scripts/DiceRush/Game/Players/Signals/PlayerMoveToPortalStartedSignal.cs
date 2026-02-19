using StepanoffGames.Signals;

namespace StepanoffGames.DiceRush.Game.Players.Signals
{
	public class PlayerMoveToPortalStartedSignal : BaseSignal
	{
		public PlayerController Player;

		public PlayerMoveToPortalStartedSignal(PlayerController player)
		{
			Player = player;
		}
	}
}
