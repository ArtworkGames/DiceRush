using Cysharp.Threading.Tasks;

namespace StepanoffGames.DiceRush.Gameplay
{
	public class DiceCard : DeckCard
	{
		override public async UniTask<int> UseForDice(int diceValue)
		{
			diceValue = GetNewDiceValue(diceValue);

			await UniTask.Yield();
			return diceValue;
		}

		virtual protected int GetNewDiceValue(int diceValue)
		{
			return diceValue;
		}
	}
}