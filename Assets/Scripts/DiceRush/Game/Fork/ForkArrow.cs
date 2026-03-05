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

		private LevelManager _level;

		private void Start()
		{
			_level = ServiceLocator.Get<LevelManager>();
		}

		private void OnDestroy()
		{
			_level = null;
		}

		public void Init(int id, Vector3 position, Vector3 cellCenter, Material material)
		{
			_id = id;

			transform.position = position;
			
			Vector3 direction = (position - cellCenter).normalized;
			float angle = Mathf.Atan2(-direction.z, direction.x) * Mathf.Rad2Deg;
			transform.localEulerAngles = new Vector3(0f, angle, 0f);

			_meshRenderer.material = material;
		}

		private void Update()
		{
			if (Mouse.current.leftButton.wasPressedThisFrame)
			{
				Vector2 mousePos = Mouse.current.position.ReadValue();
				Ray ray = _level.Camera.Camera.ScreenPointToRay(mousePos);

				if (Physics.Raycast(ray, out RaycastHit hit))
				{
					if (hit.collider.gameObject == _meshRenderer.gameObject)
					{
						OnSelect?.Invoke(this);
					}
				}
			}
		}
	}
}
