using StepanoffGames.Signals;

namespace StepanoffGames.DiceRush.Game.Players.Signals
{
	public class PlayerStateChangedSignal : BaseSignal
	{
		public PlayerController Player;

		public PlayerStateChangedSignal(PlayerController player)
		{
			Player = player;
		}
	}
}
