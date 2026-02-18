using UnityEngine;

namespace StepanoffGames.DiceRush.Gameplay
{
	public class CellDrawer : MonoBehaviour
	{
		//[SerializeField] private GameObject _regular;
		//[SerializeField] private GameObject _skipMove;
		//[SerializeField] private GameObject _extraMove;
		//[SerializeField] private GameObject _moveTo;
		//[Space]
		[SerializeField] private GameObject _reward;
		[SerializeField] private GameObject _enemy;
		[SerializeField] private GameObject _moveBackward;
		[SerializeField] private GameObject _moveForward;
		[SerializeField] private GameObject _portal;

		public void Show(Cell cell)
		{
			//Debug.Log($"Show: {cell.Type}");

			//_regular.SetActive(cell.Type == CellType.Regular);
			//_skipMove.SetActive(cell.Type == CellType.SkipMove);
			//_extraMove.SetActive(cell.Type == CellType.ExtraMove);
			//_moveTo.SetActive(cell.Type == CellType.MoveToForward || cell.Type == CellType.MoveToBackward);

			_reward.SetActive(cell.Type == CellType.Reward);
			_enemy.SetActive(cell.Type == CellType.Enemy);
			_moveBackward.SetActive(cell.Type == CellType.MoveBackward);
			_moveForward.SetActive(cell.Type == CellType.MoveForward);
			_portal.SetActive(cell.Type == CellType.Portal1);

			//if ((cell.Type == CellType.MoveToForward || cell.Type == CellType.MoveToBackward) &&
			//	cell.MoveToPoint != null)
			//{
			//	Vector3 direction = (cell.MoveToPoint.transform.position - cell.transform.position).normalized;
			//	float angle = Mathf.Atan2(-direction.z, direction.x) * Mathf.Rad2Deg;
			//	transform.localEulerAngles = new Vector3(
			//		transform.localEulerAngles.x,
			//		angle,
			//		transform.localEulerAngles.z);
			//}
		}
	}
}
