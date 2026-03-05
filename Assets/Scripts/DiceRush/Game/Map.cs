using StepanoffGames.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

namespace StepanoffGames.DiceRush.Game
{
	public class Map : MonoBehaviour, IService
	{
		public Action OnInited;

		private Cell[] _cells;
		private Cell _startCell;

		public Cell[] Cells => _cells;
		public Cell StartCell => _startCell;

		private void Awake()
		{
			ServiceLocator.Register(this);
		}

		private void Start()
		{
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

			OnInited?.Invoke();
		}

		private void OnDestroy()
		{
			ServiceLocator.Unregister<Map>();
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

		public Cell GetOtherCellSameType(Cell forCell)
		{
			for (int i = 0; i < _cells.Length; i++)
			{
				if (_cells[i].Type == forCell.Type && _cells[i] != forCell)
				{
					return _cells[i];
				}
			}
			return null;
		}

		public Cell GetOtherCellSameTypeClosestToFinish(Cell forCell)
		{
			Cell cell = null;
			for (int i = 0; i < _cells.Length; i++)
			{
				if (_cells[i].Type == forCell.Type && _cells[i] != forCell)
				{
					cell = _cells[i];
				}
			}
			return cell;
		}
	}
}
