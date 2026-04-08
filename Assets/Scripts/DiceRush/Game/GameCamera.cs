using Cysharp.Threading.Tasks;
using DG.Tweening;
using StepanoffGames.DiceRush.Game.Map;
using StepanoffGames.DiceRush.Game.Path;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.Services;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace StepanoffGames.DiceRush.Game
{
	public class GameCamera : MonoBehaviour
	{
		[SerializeField] private Camera _camera;
		[Space]
		[SerializeField] private bool _invert = false;

		public Camera Camera => _camera;

		private float focusOnPlayerDistance = -15f;
		private float focusOnPlayerTime = 1f;

		private float focusOnCellDistance = -15f;
		private float focusOnCellTime = 1f;

		private float focusOnFrontOfPlayerDistance = -40f;
		private float focusOnFrontOfPlayerTime = 1f;

		private PathController _pathController;

		private bool isDragging;
		private Vector2 lastMousePosition;
		private Plane dragPlane;
		private Tween moveTween;

		private Vector3 positionDelta = new Vector3(0f, 0f, -2f);

		private void Start()
		{
			_pathController = ServiceLocator.Get<PathController>();

			dragPlane = new Plane(Vector3.up, Vector3.zero);
		}

		private void OnDestroy()
		{
			moveTween?.Kill();

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
				moveTween?.Kill();
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

		public async UniTask FocusOnPlayer(PlayerAvatar player, CancellationToken ct)
		{
			Vector3 pos = player.transform.position;
			await Move(pos, focusOnPlayerDistance, focusOnPlayerTime, ct);
		}

		public async UniTask FocusOnCell(Cell cell, CancellationToken ct)
		{
			Vector3 pos = cell.transform.position;
			await Move(pos, focusOnCellDistance, focusOnCellTime, ct);
		}

		public async UniTask FocusOnPathMarkers(PlayerAvatar player, CancellationToken ct)
		{
			//Vector3 pos = player.CurrentPoint.transform.position;
			//int count = 1;
			//for (int i = 0; i < _pathController.Markers.Count; i++)
			//{
			//	if (_pathController.Markers[i] != null && _pathController.Markers[i].Cell != null)
			//	{
			//		pos += _pathController.Markers[i].Cell.transform.position;
			//		count++;
			//	}
			//}
			//pos /= count;

			Vector3 playerPos = player.CurrentPoint.transform.position;
			float minX = playerPos.x;
			float maxX = playerPos.x;
			float minY = playerPos.y;
			float maxY = playerPos.y;
			float minZ = playerPos.z;
			float maxZ = playerPos.z;

			for (int i = 0; i < _pathController.Markers.Count; i++)
			{
				if (_pathController.Markers[i] != null && _pathController.Markers[i].Cell != null)
				{
					Vector3 markerPos = _pathController.Markers[i].Cell.transform.position;
					minX = Mathf.Min(minX, markerPos.x);
					maxX = Mathf.Max(maxX, markerPos.x);
					minY = Mathf.Min(minY, markerPos.y);
					maxY = Mathf.Max(maxY, markerPos.y);
					minZ = Mathf.Min(minZ, markerPos.z);
					maxZ = Mathf.Max(maxZ, markerPos.z);
				}
			}

			Vector3 pos = new Vector3(
				Mathf.Lerp(minX, maxX, 0.5f),
				Mathf.Lerp(minY, maxY, 0.5f),
				Mathf.Lerp(minZ, maxZ, 0.5f));

			await Move(pos, focusOnFrontOfPlayerDistance, focusOnFrontOfPlayerTime, ct);
		}

		private async UniTask Move(Vector3 pos, float distance, float time, CancellationToken ct)
		{
			if (isDragging) return;
			pos += positionDelta;

			moveTween?.Kill();
			bool isMoveTween = true;
			moveTween = transform.DOMove(pos, time)
				.SetEase(Ease.InOutCubic)
				.OnComplete(() =>
				{
					isMoveTween = false;
				});

			await UniTask.WaitWhile(() => isMoveTween, cancellationToken: ct);
		}
	}
}
