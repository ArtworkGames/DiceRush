using StepanoffGames.DiceRush.Game;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Components.Bag
{
	public class BagToken : MonoBehaviour
	{
		[SerializeField] private GameObject _reward;
		[SerializeField] private GameObject _enemy;
		[SerializeField] private GameObject _moveForward;
		[SerializeField] private GameObject _moveBackward;
		[SerializeField] private GameObject _portal;

		public void UpdateView(CellType type)
		{
			_reward.SetActive(type == CellType.Reward);
			_enemy.SetActive(type == CellType.Enemy);
			_moveForward.SetActive(type == CellType.MoveForward);
			_moveBackward.SetActive(type == CellType.MoveBackward);
			_portal.SetActive(type == CellType.Portal1);
		}
	}
}
