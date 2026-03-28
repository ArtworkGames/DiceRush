using Cysharp.Threading.Tasks;
using DG.Tweening;
using StepanoffGames.DiceRush.Game.Path;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace StepanoffGames.DiceRush.Game
{
	public class LevelCamera : MonoBehaviour
	{
		[SerializeField] private Camera _camera;
		[Space]
		//[SerializeField] private float _dragSpeed = 20f;
		[SerializeField] private bool _invert = false;

		public Camera Camera => _camera;

		//private float focusOnPlayerHeight = 2f;
		private float focusOnPlayerDistance = -15f;
		private float focusOnPlayerTime = 1f;

		//private float focusOnCellHeight = 2f;
		private float focusOnCellDistance = -15f;
		private float focusOnCellTime = 1f;

		//private float focusOnFrontOfPlayerHeight = 5f;
		private float focusOnFrontOfPlayerDistance = -40f;
		private float focusOnFrontOfPlayerTime = 1f;

		//private float focusOnBackOfPlayerHeight = 5f;
		//private float focusOnBackOfPlayerDistance = -40f;
		//private float focusOnBackOfPlayerTime = 1f;

		private PathController _pathController;

		private bool isDragging;
		private Vector2 lastMousePosition;
		//private Vector3 lastWorldPointOnPlane;
		private Plane dragPlane;
		//private Vector3 destPosition;

		private void Start()
		{
			_pathController = ServiceLocator.Get<PathController>();

			dragPlane = new Plane(Vector3.up, Vector3.zero);
		}

		private void OnDestroy()
		{
			_pathController = null;
		}

		private void Update()
		{
			if (Mouse.current == null) return;

			Vector2 mousePosition = Mouse.current.position.ReadValue();

			if ((mousePosition - lastMousePosition).sqrMagnitude < 0.01f)
				return;

			if (Mouse.current.leftButton.wasPressedThisFrame)
			{
				if (IsPointerOverUI())
				{
					isDragging = false;
					return;
				}

				isDragging = true;
				lastMousePosition = mousePosition;
			}

			if (Mouse.current.leftButton.wasReleasedThisFrame)
			{
				isDragging = false;
			}

			if (!isDragging || !Mouse.current.leftButton.isPressed)
				return;

			if (!TryGetPointOnDragPlane(lastMousePosition, out Vector3 lastWorldPoint))
				return;

			if (!TryGetPointOnDragPlane(mousePosition, out Vector3 currentWorldPoint))
				return;

			Vector3 delta = lastWorldPoint - currentWorldPoint;
			delta.y = 0f;

			if (_invert)
				delta = -delta;

			transform.position += delta;

			lastMousePosition = mousePosition;
		}

		private bool TryGetPointOnDragPlane(Vector2 screenPosition, out Vector3 worldPoint)
		{
			Ray ray = _camera.ScreenPointToRay(screenPosition);

			if (dragPlane.Raycast(ray, out float enter))
			{
				worldPoint = ray.GetPoint(enter);
				return true;
			}

			worldPoint = default;
			return false;
		}

		private bool IsPointerOverUI()
		{
			if (EventSystem.current == null)
				return false;

			return EventSystem.current.IsPointerOverGameObject();
		}

		public async UniTask FocusOnPlayer(PlayerAvatar player)
		{
			Vector3 pos = player.transform.position;
			//pos += new Vector3(0f, focusOnPlayerHeight, 0f);

			await Move(pos, focusOnPlayerDistance, focusOnPlayerTime);
		}

		public async UniTask FocusOnCell(Cell cell)
		{
			Vector3 pos = cell.transform.position;
			//pos += new Vector3(0f, focusOnCellHeight, 0f);

			await Move(pos, focusOnCellDistance, focusOnCellTime);
		}

		public async UniTask FocusOnPathMarkers(PlayerAvatar player)
		{
			Vector3 pos = player.CurrentPoint.transform.position;
			int count = 1;
			for (int i = 0; i < _pathController.Markers.Count; i++)
			{
				if (_pathController.Markers[i] != null && _pathController.Markers[i].Cell != null)
				{
					pos += _pathController.Markers[i].Cell.transform.position;
					count++;
				}
			}
			pos /= count;
			//pos += new Vector3(0f, focusOnFrontOfPlayerHeight, 0f);

			await Move(pos, focusOnFrontOfPlayerDistance, focusOnFrontOfPlayerTime);
		}

		//public async UniTask FocusOnFrontOfPlayer(Player player)
		//{
		//	Cell currCell = player.CurrentCell;
		//	Vector3 pos = currCell.transform.position;
		//	int count = 1;
		//	for (int i = 0; i < 6; i++)
		//	{
		//		if (currCell.NextCells.Length > 0 && currCell.NextCells[0].Cell != null)
		//		{
		//			currCell = currCell.NextCells[0].Cell;
		//			pos += currCell.transform.position;
		//			count++;
		//		}
		//	}
		//	pos /= count;
		//	pos += new Vector3(0f, focusOnFrontOfPlayerHeight, 0f);

		//	await Move(pos, focusOnFrontOfPlayerDistance, focusOnFrontOfPlayerTime);
		//}

		//public async UniTask FocusOnBackOfPlayer(Player player)
		//{
		//	Cell currCell = player.CurrentCell;
		//	Vector3 pos = currCell.transform.position;
		//	int count = 1;
		//	for (int i = 0; i < 6; i++)
		//	{
		//		if (currCell.PrevCells.Length > 0 && currCell.PrevCells[0].Cell != null)
		//		{
		//			currCell = currCell.PrevCells[0].Cell;
		//			pos += currCell.transform.position;
		//			count++;
		//		}
		//	}
		//	pos /= count;
		//	pos += new Vector3(0f, focusOnBackOfPlayerHeight, 0f);

		//	await Move(pos, focusOnBackOfPlayerDistance, focusOnBackOfPlayerTime);
		//}

		private async UniTask Move(Vector3 pos, float distance, float time)
		{
			bool isMoveTween = true;

			transform.DOMove(pos, time)
				.SetEase(Ease.InOutCubic)
				.OnComplete(() =>
				{
					isMoveTween = false;
				});
			//_camera.transform.DOLocalMove(new Vector3(0f, 0f, distance), time)
			//	.SetEase(Ease.InOutCubic)
			//	.OnComplete(() =>
			//	{
			//	});

			await UniTask.WaitWhile(() => isMoveTween);
		}
	}
}
