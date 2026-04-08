using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Map;
using StepanoffGames.DiceRush.Game.Players;
using System.Threading;

namespace StepanoffGames.DiceRush.Game.Deck.Cards
{
	public class Card
	{
		public CardModel Model => _model;
		private CardModel _model;

		public Card(CardModel model)
		{
			_model = model;
		}

		virtual public async UniTask<int> UseForDice(PlayerController player, int diceValue, CancellationToken ct)
		{
			await UniTask.Yield(ct);
			return diceValue;
		}

		virtual public int ApplyForDice(PlayerController player, int diceValue)
		{
			return diceValue;
		}

		virtual public async UniTask<CellType> UseForToken(PlayerController player, CellType tileType, CancellationToken ct)
		{
			await UniTask.Yield(ct);
			return tileType;
		}

		virtual public CellType ApplyForToken(PlayerController player, CellType tileType)
		{
			return tileType;
		}

		virtual public async UniTask UseForBattle(PlayerController player, CancellationToken ct)
		{
			await UniTask.Yield(ct);
		}

		virtual public void ApplyForBattle(PlayerController player)
		{
		}
	}
}
