using StepanoffGames.Signals;

namespace StepanoffGames.DiceRush.Game.Players.Signals
{
	public class PlayerTurnEndedSignal : BaseSignal
	{
		public PlayerController Player;

		public PlayerTurnEndedSignal(PlayerController player)
		{
			Player = player;
		}
	}
}
