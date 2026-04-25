using StepanoffGames.Cameras.Signals;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StepanoffGames.DiceRush.Game.Fork
{
	public class ForkArrow : MonoBehaviour
	{
		public Action<ForkArrow> OnSelect;

		[SerializeField] private MeshRenderer _meshRenderer;

		public int Id => _id;
		private int _id;

		private GameManager _gameManager;

		private void Start()
		{
			_gameManager = ServiceLocator.Get<GameManager>();
		}

		private void OnDestroy()
		{
			_gameManager = null;
		}

		public void Init(int id, Vector3 position, Vector3 cellCenter, Material material)
		{
			_id = id;

			transform.position = position;
			
			Vector3 direction = (position - cellCenter).normalized;
			float angle = Mathf.Atan2(-direction.y, direction.x) * Mathf.Rad2Deg;
			transform.localEulerAngles = new Vector3(0f, 0f, -angle);

			//_meshRenderer.material = material;
		}

		private void Update()
		{
			//if (Mouse.current.leftButton.wasPressedThisFrame)
			//{
			//	Vector2 mousePos = Mouse.current.position.ReadValue();
			//	Ray ray = _gameManager.Camera.Camera.ScreenPointToRay(mousePos);

			//	if (Physics.Raycast(ray, out RaycastHit hit))
			//	{
			//		if (hit.collider.gameObject == _meshRenderer.gameObject)
			//		{
			//			OnSelect?.Invoke(this);
			//		}
			//	}
			//}
			if (Mouse.current.leftButton.wasPressedThisFrame)
			{
				Vector2 mousePos = Mouse.current.position.ReadValue();
				Vector2 worldPos = _gameManager.Camera.Camera.ScreenToWorldPoint(mousePos);

				RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
				if (hit.collider != null && hit.collider.gameObject == gameObject)
				{
					OnSelect?.Invoke(this);
				}
			}
		}
	}
}
