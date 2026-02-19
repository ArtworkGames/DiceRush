using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Dice;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.Services;

namespace StepanoffGames.DiceRush.Game
{
	public class RerollDiceCard : DiceCard
	{
		//override protected int GetNewDiceValue(PlayerController player, int diceValue)
		//{
		//	DiceController dice = ServiceLocator.Get<DiceController>();
		//	return dice.GetValue();
		//}

		override public async UniTask<int> UseForDice(PlayerController player, int diceValue)
		{
			DiceController dice = ServiceLocator.Get<DiceController>();

			if (player.Model.Type == PlayerType.HI)
			{
				dice.Confirm();

				await UniTask.NextFrame();

				diceValue = await dice.Roll(player);
			}
			else
			{
				diceValue = dice.GetValue(player);
			}

			return diceValue;
		}
	}
}
