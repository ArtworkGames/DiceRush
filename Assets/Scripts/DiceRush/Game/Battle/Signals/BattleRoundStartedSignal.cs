using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.Signals;

namespace StepanoffGames.DiceRush.Game.Battle.Signals
{
	public class BattleRoundStartedSignal : BaseSignal
	{
		public PlayerController Player;
		public EnemyModel Enemy;

		public BattleRoundStartedSignal(PlayerController player, EnemyModel enemy)
		{
			Player = player;
			Enemy = enemy;
		}
	}
}
