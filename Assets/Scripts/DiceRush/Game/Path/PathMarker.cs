using DG.Tweening;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Path
{
	public class PathMarker : MonoBehaviour
	{
		[SerializeField] private MeshRenderer _markerMeshRenderer;
		[SerializeField] private MeshRenderer _projectionMeshRenderer;

		public Cell Cell => _cell;

		private Cell _cell;

		private void OnDestroy()
		{
			_cell = null;
		}

		public void Init(Cell cell, Material material)
		{
			_cell = cell;
			transform.position = cell.transform.position;

			_markerMeshRenderer.transform.localScale = Vector3.zero;
			_markerMeshRenderer.transform.DOScale(0.7f, 0.5f)
				.SetEase(Ease.OutQuad);

			_projectionMeshRenderer.transform.localScale = Vector3.zero;
			//_projectionMeshRenderer.transform.DOScale(0.7f, 0.5f)
			//	.SetEase(Ease.OutQuad);

			_markerMeshRenderer.material = material;
			_projectionMeshRenderer.material = material;
		}

		public void Select()
		{
			_markerMeshRenderer.transform.DOScale(1f, 0.5f)
				.SetEase(Ease.OutQuad);
			//_projectionMeshRenderer.transform.DOScale(1f, 0.5f)
			//	.SetEase(Ease.OutQuad);
		}

		public void Hide(bool destroyOnComplete = false)
		{
			//_cell = null;

			_markerMeshRenderer.transform.DOScale(0f, 0.5f)
				.SetEase(Ease.OutQuad)
				.OnComplete(() =>
				{
					if (destroyOnComplete)
						Destroy(gameObject);
				});
			_projectionMeshRenderer.transform.DOScale(0f, 0.5f)
				.SetEase(Ease.OutQuad);
		}

		private void Update()
		{
			Vector3 pos = new Vector3(0f, 2.5f + 0.5f * Mathf.Sin(Time.time * 4f), 0f);
			_markerMeshRenderer.transform.localPosition = pos;
			_markerMeshRenderer.transform.Rotate(new Vector3(0f, Time.deltaTime * Mathf.Rad2Deg * 2f, 0f));
		}
	}
}
