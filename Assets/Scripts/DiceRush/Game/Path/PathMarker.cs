using DG.Tweening;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Map;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Path
{
	public class PathMarker : MonoBehaviour
	{
		//[SerializeField] private MeshRenderer _markerMeshRenderer;
		//[SerializeField] private MeshRenderer _projectionMeshRenderer;

		[SerializeField] private Transform _marker;
		[Space]
		[SerializeField] private GameObject _redMarker;
		[SerializeField] private GameObject _blueMarker;
		[SerializeField] private GameObject _greenMarker;
		[SerializeField] private GameObject _yellowMarker;

		public Cell Cell => _cell;

		private Cell _cell;

		private void OnDestroy()
		{
			_cell = null;
		}

		public void Init(Cell cell, PlayerColor color)
		{
			_cell = cell;
			transform.position = cell.transform.position;

			_redMarker.SetActive(color == PlayerColor.Red);
			_blueMarker.SetActive(color == PlayerColor.Blue);
			_greenMarker.SetActive(color == PlayerColor.Green);
			_yellowMarker.SetActive(color == PlayerColor.Yellow);

			_marker.localScale = Vector3.zero;
			_marker.DOScale(1.5f, 0.5f)
				.SetEase(Ease.OutBack);

			//_projectionMeshRenderer.transform.localScale = Vector3.zero;
			//_projectionMeshRenderer.transform.DOScale(0.7f, 0.5f)
			//	.SetEase(Ease.OutQuad);

			//_markerMeshRenderer.material = material;
			//_projectionMeshRenderer.material = material;
		}

		public void Select()
		{
			_marker.DOScale(2f, 0.5f)
				.SetEase(Ease.OutBack);
			//_projectionMeshRenderer.transform.DOScale(1f, 0.5f)
			//	.SetEase(Ease.OutQuad);
		}

		public void Hide(bool destroyOnComplete = false)
		{
			//_cell = null;

			_marker.DOScale(0f, 0.3f)
				.SetEase(Ease.OutCubic)
				.OnComplete(() =>
				{
					if (destroyOnComplete)
						Destroy(gameObject);
				});
			//_projectionMeshRenderer.transform.DOScale(0f, 0.5f)
				//.SetEase(Ease.OutQuad);
		}

		private void Update()
		{
			//Vector3 pos = new Vector3(0f, 1.5f + 0.5f * Mathf.Sin(Time.time * 4f), 0f);
			//_markerMeshRenderer.transform.localPosition = pos;
			//_markerMeshRenderer.transform.Rotate(new Vector3(0f, Time.deltaTime * Mathf.Rad2Deg * 2f, 0f));
		}
	}
}
