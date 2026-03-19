using TMPro;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game
{
	public class CellDrawer : MonoBehaviour
	{
		[SerializeField] private GameObject _reward;
		[SerializeField] private GameObject _enemy;
		[SerializeField] private GameObject _moveBackward;
		[SerializeField] private GameObject _moveForward;
		[SerializeField] private GameObject _portal;
		[Space]
		[SerializeField] private TMP_Text _indexText;

		public void Show(Cell cell)
		{
			_reward.SetActive(cell.Type == CellType.Reward);
			_enemy.SetActive(cell.Type == CellType.Enemy);
			_moveBackward.SetActive(cell.Type == CellType.MoveBackward);
			_moveForward.SetActive(cell.Type == CellType.MoveForward);
			_portal.SetActive(cell.Type == CellType.Portal);

			bool isUsed = cell.IsUsed;
			if (_reward.activeSelf) UpdateUsed(true, _reward);
			if (_enemy.activeSelf) UpdateUsed(isUsed, _enemy);
			if (_moveBackward.activeSelf) UpdateUsed(isUsed, _moveBackward);
			if (_moveForward.activeSelf) UpdateUsed(isUsed, _moveForward);
			if (_portal.activeSelf) UpdateUsed(isUsed, _portal);

			_indexText.text = cell.Index.ToString();
		}

		private void UpdateUsed(bool isUsed, GameObject sprite)
		{
			SpriteRenderer spriteRenderer = sprite.GetComponent<SpriteRenderer>();
			Color c = spriteRenderer.color;
			c.a = isUsed ? 0.5f : 1f;
			spriteRenderer.color = c;
		}
	}
}
