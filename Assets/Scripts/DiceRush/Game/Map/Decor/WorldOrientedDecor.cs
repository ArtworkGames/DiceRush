using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Map.Decor
{
	public class WorldOrientedDecor : MonoBehaviour
	{
		private void Start()
		{
			transform.eulerAngles = Vector3.zero;
		}
	}
}
