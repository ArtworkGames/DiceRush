using StepanoffGames.Signals;

namespace StepanoffGames.DiceRush.Game.Players.Signals
{
	public class PlayerPortalPassedSignal : BaseSignal
	{
		public PlayerController Player;

		public PlayerPortalPassedSignal(PlayerController player)
		{
			Player = player;
		}
	}
}
