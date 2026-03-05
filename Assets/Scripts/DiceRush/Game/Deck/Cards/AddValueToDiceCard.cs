using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Players;

namespace StepanoffGames.DiceRush.Game.Deck.Cards
{
	public class AddValueToDiceCard : Card
	{
		private int _delta;

		public AddValueToDiceCard(CardModel model) : base(model)
		{
			switch (model.Type)
			{
				case CardType.Plus1ToDice: _delta = 1; break;
				case CardType.Plus2ToDice: _delta = 2; break;
				case CardType.Minus1FromDice: _delta = -1; break;
				case CardType.Minus2FromDice: _delta = -2; break;
			}
		}

		override public async UniTask<int> UseForDice(PlayerController player, int diceValue)
		{
			diceValue = diceValue + _delta;

			await UniTask.Yield();
			return diceValue;
		}

		override public int ApplyForDice(PlayerController player, int diceValue)
		{
			return diceValue + _delta;
		}
	}
}
