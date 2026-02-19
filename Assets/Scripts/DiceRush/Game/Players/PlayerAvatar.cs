using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Players
{
	public class PlayerAvatar : MonoBehaviour
	{
		[SerializeField] private int _id;

		public int Id => _id;
		public MapPoint CurrentPoint => _currentPoint;

		private MapPoint _currentPoint;

		private float speed = 12f;

		public void SetToCellCenterPosition(Cell cell)
		{
			transform.position = cell.transform.position;
			_currentPoint = cell;
		}

		public void SetToCellPlayerPosition(Cell cell)
		{
			transform.position = cell.PlayerPositions[_id - 1].position;
			_currentPoint = cell;
		}

		public async UniTask MoveToCurrentCellPlayerPosition()
		{
			if (!(_currentPoint is Cell)) return;

			bool isMoveTween = true;

			Vector3 pos = ((Cell)_currentPoint).PlayerPositions[_id - 1].position;
			float time = Vector3.Distance(transform.position, pos) / speed;

			transform.DOMove(pos, time)
				.SetEase(Ease.Linear)
				.OnComplete(() =>
				{
					isMoveTween = false;
				});

			await UniTask.WaitWhile(() => isMoveTween);
		}

		public async UniTask MoveToPoint(MapPoint point)
		{
			bool isMoveTween = true;

			float time = Vector3.Distance(transform.position, point.transform.position) / speed;

			transform.DOMove(point.transform.position, time)
				.SetEase(Ease.Linear)
				.OnComplete(() =>
				{
					isMoveTween = false;
				});

			await UniTask.WaitWhile(() => isMoveTween);
			_currentPoint = point;
		}
	}
}
