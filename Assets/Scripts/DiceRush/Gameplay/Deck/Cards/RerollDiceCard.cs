namespace StepanoffGames.DiceRush.Gameplay
{
	public class RerollDiceCard : DiceCard
	{
		override protected int GetNewDiceValue(int diceValue)
		{
			return Level.Instance.Dice.GetValue();
		}
	}
}
