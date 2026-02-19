using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.Signals;

namespace StepanoffGames.DiceRush.Game.Xp.Signals
{
	public class XpMultiplierChangedSignal : BaseSignal
	{
		public PlayerModel Player;

		public XpMultiplierChangedSignal(PlayerModel player)
		{
			Player = player;
		}
	}
}
