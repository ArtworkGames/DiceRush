using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.Signals;

namespace StepanoffGames.DiceRush.Game.Battle.Signals
{
	public class PlayerWonBattleSignal : BaseSignal
	{
		public PlayerController Player;
		public EnemyModel Enemy;

		public PlayerWonBattleSignal(PlayerController player, EnemyModel enemy)
		{
			Player = player;
			Enemy = enemy;
		}
	}
}
