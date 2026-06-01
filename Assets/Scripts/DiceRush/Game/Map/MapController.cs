using Cysharp.Threading.Tasks;
using StepanoffGames.Services;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Map
{
	public enum MapLength
	{
		Short,
		Medium,
		Long
	}

	public class MapController : MonoBehaviour, IService
	{
		public static MapLength MapLength = MapLength.Medium;

		public Action OnInited;

		[SerializeField] private MapGenerator _generator;

		private Cell[] _cells;
		private Cell _startCell;

		public Cell[] Cells => _cells;
		public Cell StartCell => _startCell;

		public Transform PlayerInitialPosition => _playerInitialPosition;
		private Transform _playerInitialPosition;

		private void Awake()
		{
			ServiceLocator.Register(this);
		}

		public async UniTask CreateMap()
		{
			if (GameManager.GameMode == GameMode.Tutorial)
			{
				await _generator.Generate(30);
			}
			else
			{
				switch (MapLength)
				{
					case MapLength.Short:
						await _generator.Generate(60);
						break;
					case MapLength.Medium:
						await _generator.Generate(80);
						break;
					case MapLength.Long:
						await _generator.Generate(100);
						break;
				}
			}

			_cells = GetComponentsInChildren<Cell>();

			for (int i = 0; i < _cells.Length; i++)
			{
				if (_cells[i].Type == CellType.Start)
				{
					_startCell = _cells[i];
					break;
				}
			}

			if (_startCell != null)
			{
				SetCellIndex(_startCell, 0);
			}

			PlayerInitialPosition initialPositionComponent = GetComponentInChildren<PlayerInitialPosition>();
			_playerInitialPosition = initialPositionComponent.transform;

			OnInited?.Invoke();
		}

		private void OnDestroy()
		{
			ServiceLocator.Unregister<MapController>();
		}

		private void SetCellIndex(MapPoint point, int index)
		{
			if (point is Cell)
			{
				if (((Cell)point).Index == 0)
				{
					((Cell)point).SetIndex(index);
					index++;
				}
				else
				{
					return;
				}
			}

			for (int i = 0; i < point.NextPoints.Count; i++)
			{
				SetCellIndex(point.NextPoints[i], index);
			}
		}

		public Cell GetCell(int index)
		{
			for (int i = 0; i < _cells.Length; i++)
			{
				if (_cells[i].Index == index)
				{
					return _cells[i];
				}
			}
			return null;
		}

		public void ResetUsedCells()
		{
			for (int i = 0; i < _cells.Length; i++)
			{
				_cells[i].SetUsed(false);
			}
		}

		public Cell GetOtherPortal(Cell currentPortal)
		{
			Cell otherPortal = null;
			int cellIndex = 0;
			for (int i = 0; i < _cells.Length; i++)
			{
				if (_cells[i].Type == CellType.Portal && _cells[i] != currentPortal &&
					_cells[i].Index > cellIndex &&
					!_cells[i].IsUsed && !_cells[i].IsLocked)
				{
					cellIndex = _cells[i].Index;
					otherPortal = _cells[i];
				}
			}
			return otherPortal;
		}

		//public Cell GetOtherCellSameType(Cell forCell)
		//{
		//	for (int i = 0; i < _cells.Length; i++)
		//	{
		//		if (_cells[i].Type == forCell.Type && _cells[i] != forCell)
		//		{
		//			return _cells[i];
		//		}
		//	}
		//	return null;
		//}

		//public Cell GetOtherCellSameTypeClosestToFinish(Cell forCell)
		//{
		//	Cell cell = null;
		//	for (int i = 0; i < _cells.Length; i++)
		//	{
		//		if (_cells[i].Type == forCell.Type && _cells[i] != forCell)
		//		{
		//			cell = _cells[i];
		//		}
		//	}
		//	return cell;
		//}

		public List<Cell> GetCellsOnDistance(Cell fromCell, int distance, bool forward)
		{
			List<Cell> cells = new List<Cell>();

			if (forward)
			{
				int newIndex = fromCell.Index + distance;
				cells = GetNextCells(fromCell, newIndex);
			}
			else
			{
				int newIndex = fromCell.Index - distance;
				cells = GetPrevCells(fromCell, newIndex);
			}

			return cells;
		}

		private List<Cell> GetNextCells(MapPoint point, int requiredCellIndex)
		{
			List<Cell> cells = new List<Cell>();

			if (point.NextPoints.Count == 0 && point is Cell)
			{
				cells.Add((Cell)point);
			}

			for (int i = 0; i < point.NextPoints.Count; i++)
			{
				if (point.NextPoints[i] is Cell)
				{
					Cell cell = (Cell)point.NextPoints[i];
					if (cell.Index == requiredCellIndex)
					{
						cells.Add(cell);
					}
					else
					{
						cells.AddRange(GetNextCells(point.NextPoints[i], requiredCellIndex));
					}
				}
				else
				{
					cells.AddRange(GetNextCells(point.NextPoints[i], requiredCellIndex));
				}
			}

			return cells;
		}

		private List<Cell> GetPrevCells(MapPoint point, int requiredCellIndex)
		{
			List<Cell> cells = new List<Cell>();

			if (point.PrevPoints.Count == 0 && point is Cell)
			{
				cells.Add((Cell)point);
			}

			for (int i = 0; i < point.PrevPoints.Count; i++)
			{
				if (point.PrevPoints[i] is Cell)
				{
					Cell cell = (Cell)point.PrevPoints[i];
					if (cell.Index == requiredCellIndex)
					{
						cells.Add(cell);
					}
					else
					{
						cells.AddRange(GetPrevCells(point.PrevPoints[i], requiredCellIndex));
					}
				}
				else
				{
					cells.AddRange(GetPrevCells(point.PrevPoints[i], requiredCellIndex));
				}
			}

			return cells;
		}

		public int GetDirectionToCell(Cell fromCell, Cell toCell, bool forward)
		{
			if (forward)
			{
				for (int i = 0; i < fromCell.NextPoints.Count; i++)
				{
					if (IsNextCellReached(fromCell.NextPoints[i], toCell))
					{
						return i;
					}
				}
			}
			else
			{
				for (int i = 0; i < fromCell.PrevPoints.Count; i++)
				{
					if (IsPrevCellReached(fromCell.PrevPoints[i], toCell))
					{
						return i;
					}
				}
			}
			return -1;
		}

		private bool IsNextCellReached(MapPoint fromPoint, Cell toCell)
		{
			if (fromPoint == toCell) return true;

			for (int i = 0; i < fromPoint.NextPoints.Count; i++)
			{
				if (IsNextCellReached(fromPoint.NextPoints[i], toCell)) return true;
			}
			return false;
		}

		private bool IsPrevCellReached(MapPoint fromPoint, Cell toCell)
		{
			if (fromPoint == toCell) return true;

			for (int i = 0; i < fromPoint.PrevPoints.Count; i++)
			{
				if (IsPrevCellReached(fromPoint.PrevPoints[i], toCell)) return true;
			}
			return false;
		}
	}
}
