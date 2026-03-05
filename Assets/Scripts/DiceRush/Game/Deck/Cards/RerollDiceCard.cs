using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Dice;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.Services;

namespace StepanoffGames.DiceRush.Game.Deck.Cards
{
	public class RerollDiceCard : Card
	{
		public RerollDiceCard(CardModel model) : base(model)
		{
		}

		override public async UniTask<int> UseForDice(PlayerController player, int diceValue)
		{
			DiceController dice = ServiceLocator.Get<DiceController>();

			dice.Confirm();
			await UniTask.NextFrame();

			diceValue = await dice.Roll(player);

			return diceValue;
		}

		override public int ApplyForDice(PlayerController player, int diceValue)
		{
			DiceController dice = ServiceLocator.Get<DiceController>();
			diceValue = dice.GetValue(player);

			return diceValue;
		}
	}
}
