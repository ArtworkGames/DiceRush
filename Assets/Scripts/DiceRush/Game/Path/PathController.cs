using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.Services;
using System.Collections.Generic;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Path
{
	public class PathController : MonoBehaviour, IService
	{
		[SerializeField] private GameObject _sourceMarker;
		[SerializeField] private Material[] _playerMaterials;

		public List<PathMarker> Markers => _markers;
		private List<PathMarker> _markers = new List<PathMarker>();

		private void Awake()
		{
			ServiceLocator.Register(this);

			_sourceMarker.SetActive(false);
		}

		private void OnDestroy()
		{
			ServiceLocator.Unregister<PathController>();
		}

		public void ShowMarkersInFrontOfPlayer(PlayerAvatar player)
		{
			List<Cell> cells = new List<Cell>();
			for (int i = 0; i < player.CurrentPoint.NextPoints.Count; i++)
			{
				AddCellAndNext(player.CurrentPoint.NextPoints[i], 1, 6, false, ref cells);
			}

			ShowMarkers(cells, _playerMaterials[player.Id - 1]);
		}

		public void ShowMarkersInBackOfPlayer(PlayerAvatar player)
		{
			List<Cell> cells = new List<Cell>();
			for (int i = 0; i < player.CurrentPoint.PrevPoints.Count; i++)
			{
				AddCellAndPrev(player.CurrentPoint.PrevPoints[i], 1, 6, false, ref cells);
			}

			ShowMarkers(cells, _playerMaterials[player.Id - 1]);
		}

		public void ShowDiceValueInFrontOfPlayer(PlayerAvatar player, int diceValue, int direction = -1)
		{
			List<Cell> cells = new List<Cell>();
			for (int i = 0; i < player.CurrentPoint.NextPoints.Count; i++)
			{
				if (direction == -1 || direction == i)
				{
					AddCellAndNext(player.CurrentPoint.NextPoints[i], 1, diceValue, true, ref cells);
				}
			}

			UpdateMarkers(cells, _playerMaterials[player.Id - 1]);
		}

		public void ShowDiceValueInBackOfPlayer(PlayerAvatar player, int diceValue, int direction = -1)
		{
			List<Cell> cells = new List<Cell>();
			for (int i = 0; i < player.CurrentPoint.PrevPoints.Count; i++)
			{
				if (direction == -1 || direction == i)
				{
					AddCellAndPrev(player.CurrentPoint.PrevPoints[i], 1, diceValue, true, ref cells);
				}
			}

			UpdateMarkers(cells, _playerMaterials[player.Id - 1]);
		}

		private void AddCellAndNext(MapPoint point, int count, int maxCount, bool onlyLast, ref List<Cell> cells)
		{
			if (point is Cell)
			{
				if (!cells.Contains((Cell)point))
				{
					if (onlyLast)
					{
						if (count == maxCount)
						{
							cells.Add((Cell)point);
						}
					}
					else
					{
						cells.Add((Cell)point);
					}
				}

				count++;
				if (count > maxCount) return;
			}

			for (int i = 0; i < point.NextPoints.Count; i++)
			{
				AddCellAndNext(point.NextPoints[i], count, maxCount, onlyLast, ref cells);
			}
		}

		private void AddCellAndPrev(MapPoint point, int count, int maxCount, bool onlyLast, ref List<Cell> cells)
		{
			if (point is Cell)
			{
				if (!cells.Contains((Cell)point))
				{
					if (onlyLast)
					{
						if (count == maxCount)
						{
							cells.Add((Cell)point);
						}
					}
					else
					{
						cells.Add((Cell)point);
					}
				}

				count++;
				if (count > maxCount) return;
			}

			for (int i = 0; i < point.PrevPoints.Count; i++)
			{
				AddCellAndPrev(point.PrevPoints[i], count, maxCount, onlyLast, ref cells);
			}
		}

		private void ShowMarkers(List<Cell> cells, Material material)
		{
			for (int i = 0; i < cells.Count; i++)
			{
				PathMarker marker = CreateMarker(_markers.Count + 1, cells[i], material);
				_markers.Add(marker);
			}
		}

		private void UpdateMarkers(List<Cell> cells, Material material)
		{
			List<PathMarker> markers = new List<PathMarker>(_markers);
			List<Cell> newCells = new List<Cell>(cells);

			for (int i = 0; i < markers.Count; i++)
			{
				if (newCells.Contains(markers[i].Cell))
				{
					newCells.Remove(markers[i].Cell);
				}
				else
				{
					_markers.Remove(markers[i]);
					markers[i].Hide(true);
				}
			}

			ShowMarkers(newCells, material);
		}

		private PathMarker CreateMarker(int id, Cell cell, Material material)
		{
			GameObject markerObject = Instantiate(_sourceMarker, _sourceMarker.transform.parent, false);
			markerObject.name = $"Marker{id}";
			markerObject.SetActive(true);

			PathMarker marker = markerObject.GetComponent<PathMarker>();
			marker.Init(cell, material);
			return marker;
		}

		private PathMarker GetMarker(Cell markerCell)
		{
			for (int i = 0; i < _markers.Count; i++)
			{
				if (_markers[i].Cell == markerCell)
				{
					return _markers[i];
				}
			}
			return null;
		}

		public void HideMarkers()
		{
			for (int i = 0; i < _markers.Count; i++)
			{
				_markers[i].Hide(true);
			}
			_markers.Clear();
		}
	}
}