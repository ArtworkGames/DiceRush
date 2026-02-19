using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Game.Players;

namespace StepanoffGames.DiceRush.Game
{
	public class DiceCard : DeckCard
	{
		override public async UniTask<int> UseForDice(PlayerController player, int diceValue)
		{
			diceValue = GetNewDiceValue(player, diceValue);

			await UniTask.Yield();
			return diceValue;
		}

		virtual protected int GetNewDiceValue(PlayerController player, int diceValue)
		{
			return diceValue;
		}
	}
}