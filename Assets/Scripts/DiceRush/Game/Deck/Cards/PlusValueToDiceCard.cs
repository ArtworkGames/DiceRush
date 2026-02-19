using StepanoffGames.DiceRush.Game.Players;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game
{
	public class PlusValueToDiceCard : DiceCard
	{
		[SerializeField] private int _delta;

		override protected int GetNewDiceValue(PlayerController player, int diceValue)
		{
			return diceValue + _delta;
		}
	}
}
