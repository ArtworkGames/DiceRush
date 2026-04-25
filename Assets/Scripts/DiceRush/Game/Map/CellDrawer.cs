using TMPro;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Map
{
	public class CellDrawer : MonoBehaviour
	{
		[SerializeField] private GameObject _rewardOn;
		[SerializeField] private GameObject _enemyOn;
		[SerializeField] private GameObject _moveBackwardOn;
		[SerializeField] private GameObject _moveForwardOn;
		[SerializeField] private GameObject _portalOn;
		[Space]
		[SerializeField] private GameObject _rewardOff;
		[SerializeField] private GameObject _enemyOff;
		[SerializeField] private GameObject _moveBackwardOff;
		[SerializeField] private GameObject _moveForwardOff;
		[SerializeField] private GameObject _portalOff;
		[Space]
		[SerializeField] private Transform _chestRewardPosition;
		[Space]
		[SerializeField] private TMP_Text _indexText;

		public Transform ChestRewardPosition => _chestRewardPosition;

		public void Show(Cell cell)
		{
			bool isUsed = cell.IsUsed;

			_rewardOn.SetActive(cell.Type == CellType.Reward && !isUsed);
			_enemyOn.SetActive(cell.Type == CellType.Enemy && !isUsed);
			_moveBackwardOn.SetActive(cell.Type == CellType.MoveBackward && !isUsed);
			_moveForwardOn.SetActive(cell.Type == CellType.MoveForward && !isUsed);
			_portalOn.SetActive(cell.Type == CellType.Portal && !isUsed);

			_rewardOff.SetActive(cell.Type == CellType.Reward && isUsed);
			_enemyOff.SetActive(cell.Type == CellType.Enemy && isUsed);
			_moveBackwardOff.SetActive(cell.Type == CellType.MoveBackward && isUsed);
			_moveForwardOff.SetActive(cell.Type == CellType.MoveForward && isUsed);
			_portalOff.SetActive(cell.Type == CellType.Portal && isUsed);

			//if (_reward.activeSelf) UpdateUsed(true, _reward);
			//if (_enemy.activeSelf) UpdateUsed(isUsed, _enemy);
			//if (_moveBackward.activeSelf) UpdateUsed(isUsed, _moveBackward);
			//if (_moveForward.activeSelf) UpdateUsed(isUsed, _moveForward);
			//if (_portal.activeSelf) UpdateUsed(isUsed, _portal);

			_indexText.text = cell.Index.ToString();
		}

		private void UpdateUsed(bool isUsed, GameObject sprite)
		{
			SpriteRenderer spriteRenderer = sprite.GetComponent<SpriteRenderer>();
			Color c = spriteRenderer.color;
			c.a = isUsed ? 0.5f : 1f;
			spriteRenderer.color = c;

			//MeshRenderer meshRenderer = sprite.GetComponent<MeshRenderer>();
			//Material material = meshRenderer.material;
			//Color c = material.color;
			//c.a = isUsed ? 0.5f : 1f;
			//material.color = c;
			//meshRenderer.material = material;
		}
	}
}
