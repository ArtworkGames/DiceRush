using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Players;

namespace StepanoffGames.DiceRush.Game.Deck.Cards
{
	public class BattleStatsCard : Card
	{
		private int _healthDelta;
		private int _defenseDelta;
		private int _attackDelta;

		public BattleStatsCard(CardModel model) : base(model)
		{
			_healthDelta = 0;
			_defenseDelta = 0;
			_attackDelta = 0;

			switch (model.Type)
			{
				case CardType.Plus1ToHealth: _healthDelta = 1; break;
				case CardType.Plus2ToHealth: _healthDelta = 2; break;
				case CardType.Plus3ToHealth: _healthDelta = 3; break;

				case CardType.Plus1ToDefense: _defenseDelta = 1; break;
				case CardType.Plus2ToDefense: _defenseDelta = 2; break;
				case CardType.Plus3ToDefense: _defenseDelta = 3; break;

				case CardType.Plus1ToAttack: _attackDelta = 1; break;
				case CardType.Plus2ToAttack: _attackDelta = 2; break;
				case CardType.Plus3ToAttack: _attackDelta = 3; break;
			}
		}

		override public async UniTask UseForBattle(PlayerController player)
		{
			player.Model.Health += _healthDelta;
			player.Model.Defense += _defenseDelta;
			player.Model.Attack += _attackDelta;

			await UniTask.Yield();
		}

		override public void ApplyForBattle(PlayerController player)
		{
			player.Model.Health += _healthDelta;
			player.Model.Defense += _defenseDelta;
			player.Model.Attack += _attackDelta;
		}
	}
}
