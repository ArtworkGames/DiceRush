using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Map
{
	public class MapSection : MonoBehaviour
	{
		[SerializeField] private Cell _enterCell;
		[SerializeField] private Cell _exitCell;

		public Cell EnterCell => _enterCell;
		public Cell ExitCell => _exitCell;
	}
}
