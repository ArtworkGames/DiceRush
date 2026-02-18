using UnityEngine;

namespace StepanoffGames.DiceRush.Gameplay
{
	public class PlusValueToDiceCard : DiceCard
	{
		[SerializeField] private int _delta;

		override protected int GetNewDiceValue(int diceValue)
		{
			return diceValue + _delta;
		}
	}
}
