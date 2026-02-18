using System;
using System.Collections.Generic;
using UnityEngine;

namespace StepanoffGames.DiceRush.Gameplay
{
	public class Map : MonoBehaviour
	{
		public Action OnInited;

		private Cell[] _cells;
		private Cell _startCell;

		public Cell[] Cells => _cells;
		public Cell StartCell => _startCell;

		//private List<CellType> _deck;

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

			//FillDeck();

			OnInited?.Invoke();
		}

		//public Cell GetPrevCell(Cell forCell)
		//{
		//	for (int i = 0; i < _cells.Length; i++)
		//	{
		//		if (_cells[i].NextCell == forCell)
		//		{
		//			return _cells[i];
		//		}
		//	}
		//	return null;
		//}

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

		//private void FillDeck()
		//{
		//	_deck = new List<CellType>();

		//	for (int i = 0; i < 26; i++)
		//	{
		//		_deck.Add(CellType.Regular);
		//	}
		//	for (int i = 0; i < 20; i++)
		//	{
		//		_deck.Add(CellType.MoveForward);
		//	}
		//	for (int i = 0; i < 20; i++)
		//	{
		//		_deck.Add(CellType.MoveBackward);
		//	}
		//	for (int i = 0; i < 20; i++)
		//	{
		//		_deck.Add(CellType.Portal1);
		//	}
		//	//for (int i = 0; i < 2; i++)
		//	//{
		//	//	_deck.Add(CellType.Portal2);
		//	//}
		//	//for (int i = 0; i < 2; i++)
		//	//{
		//	//	_deck.Add(CellType.Portal3);
		//	//}
		//	//for (int i = 0; i < 2; i++)
		//	//{
		//	//	_deck.Add(CellType.Portal4);
		//	//}
		//	//for (int i = 0; i < 2; i++)
		//	//{
		//	//	_deck.Add(CellType.Portal5);
		//	//}
		//}

		//public CellType GetNextTypeFromDeck()
		//{
		//	if (_deck.Count == 0) return CellType.Regular;

		//	int index = UnityEngine.Random.Range(0, _deck.Count);
		//	CellType type = _deck[index];
		//	_deck.RemoveAt(index);

		//	return type;
		//}
	}
}
