using Cysharp.Threading.Tasks;
using DG.Tweening;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Map;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace StepanoffGames.DiceRush.Game.Players
{
	public class PlayerAvatar : MonoBehaviour
	{
		[SerializeField] private int _id;
		[SerializeField] private PlayerColor _color;

		public int Id => _id;
		public PlayerColor Color => _color;

		public MapPoint CurrentPoint => _currentPoint;
		private MapPoint _currentPoint;

		public PlayerSkin Skin => _skin;
		private PlayerSkin _skin;

		private float speed = 12f;

		private void Start()
		{
			LoadSkin().Forget();
		}

		private async UniTask LoadSkin()
		{
			string skinName = $"Knight_1_{_color}";
			string skinPath = $"Game/Players/{skinName}.prefab";
			var handle = Addressables.LoadAssetAsync<GameObject>(skinPath);
			await UniTask.WaitUntil(() => handle.IsDone);

			GameObject skinObject = Instantiate(handle.Result, transform, false);
			skinObject.name = skinName;
			skinObject.transform.localPosition = Vector3.zero;

			_skin = skinObject.GetComponent<PlayerSkin>();
		}

		public void SetToPosition(Vector3 pos, Cell cell)
		{
			transform.position = pos;
			_currentPoint = cell;
		}

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

		public async UniTask MoveToCurrentCellPlayerPosition(CancellationToken ct)
		{
			if (!(_currentPoint is Cell)) return;

			bool isMoveTween = true;
			if (_skin != null) _skin.ShowRun();

			Vector3 pos = ((Cell)_currentPoint).PlayerPositions[_id - 1].position;
			float time = Vector3.Distance(transform.position, pos) / speed;

			transform.DOMove(pos, time)
				.SetEase(Ease.Linear)
				.OnComplete(() =>
				{
					isMoveTween = false;
					if (_skin != null) _skin.ShowIdle();
				});

			await UniTask.WaitWhile(() => isMoveTween, cancellationToken: ct);
		}

		public async UniTask MoveToPoint(MapPoint point, CancellationToken ct)
		{
			bool isMoveTween = true;
			if (_skin != null) _skin.ShowRun();

			float time = Vector3.Distance(transform.position, point.transform.position) / speed;

			transform.DOMove(point.transform.position, time)
				.SetEase(Ease.Linear)
				.OnComplete(() =>
				{
					isMoveTween = false;
					if (_skin != null) _skin.ShowIdle();
				});

			await UniTask.WaitWhile(() => isMoveTween, cancellationToken: ct);
			_currentPoint = point;
		}
	}
}
