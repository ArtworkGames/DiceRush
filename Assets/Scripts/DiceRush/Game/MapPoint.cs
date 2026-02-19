using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game
{
	public class MapPoint : MonoBehaviour
	{
		[SerializeField] protected List<MapPoint> _prevPoints;
		[SerializeField] protected List<MapPoint> _nextPoints;

		public List<MapPoint> PrevPoints => _prevPoints;
		public List<MapPoint> NextPoints => _nextPoints;

		public void InitPrevPoints()
		{
			if (_prevPoints == null) _prevPoints = new List<MapPoint>();
		}

		public void InitNextPoints()
		{
			if (_nextPoints == null) _nextPoints = new List<MapPoint>();
		}

		public void AddNextPoint()
		{
			MapPoint newPoint = CreatePoint(transform.position + new Vector3(1f, 0f, 0f));

			if (newPoint.PrevPoints == null) newPoint.InitPrevPoints();
			newPoint.PrevPoints.Add(this);

			if (_nextPoints == null) InitNextPoints();
			_nextPoints.Add(newPoint);
		}

		public void InsertNextPoint()
		{
			if (_nextPoints != null)
			{
				for (int i = 0; i < _nextPoints.Count; i++)
				{
					MapPoint newPoint = CreatePoint((transform.position + _nextPoints[i].transform.position) / 2f);

					if (newPoint.PrevPoints == null) newPoint.InitPrevPoints();
					newPoint.PrevPoints.Add(this);

					if (newPoint.NextPoints == null) newPoint.InitNextPoints();
					newPoint.NextPoints.Add(_nextPoints[i]);

					int index = _nextPoints[i].PrevPoints.IndexOf(this);
					if (index >= 0) _nextPoints[i].PrevPoints[index] = newPoint;

					_nextPoints[i] = newPoint;
				}
			}
		}

		private MapPoint CreatePoint(Vector3 pos)
		{
			GameObject pointObject = new GameObject("Point");
			pointObject.transform.parent = transform.parent;
			pointObject.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
			pointObject.transform.position = pos;

			MapPoint point = pointObject.AddComponent<MapPoint>();
			return point;
		}

		private Cell CreateCell(Vector3 pos)
		{
			GameObject cellObject = new GameObject("Cell");
			cellObject.transform.parent = transform.parent;
			cellObject.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
			cellObject.transform.position = pos;

			Cell cell = cellObject.AddComponent<Cell>();
			return cell;
		}

		public void ConvertToPoint()
		{
#if UNITY_EDITOR
			if (!(this is Cell)) return;

			MapPoint point = CreatePoint(transform.position);
			ReplaceWith(point);
			Selection.activeGameObject = point.gameObject;
#endif
		}

		public void ConvertToCell()
		{
#if UNITY_EDITOR
			if (this is Cell) return;

			Cell cell = CreateCell(transform.position);
			ReplaceWith(cell);
			Selection.activeGameObject = cell.gameObject;
#endif
		}

		private void ReplaceWith(MapPoint point)
		{
			if (_prevPoints != null)
			{
				point.InitPrevPoints();
				
				for (int i = 0; i < _prevPoints.Count; i++)
				{
					int index = _prevPoints[i].NextPoints.IndexOf(this);
					if (index >= 0) _prevPoints[i].NextPoints[index] = point;

					point.PrevPoints.Add(_prevPoints[i]);
				}
				
				_prevPoints.Clear();
			}

			if (_nextPoints != null)
			{
				point.InitNextPoints();
				
				for (int i = 0; i < _nextPoints.Count; i++)
				{
					int index = _nextPoints[i].PrevPoints.IndexOf(this);
					if (index >= 0) _nextPoints[i].PrevPoints[index] = point;

					point.NextPoints.Add(_nextPoints[i]);
				}
				
				_nextPoints.Clear();
			}

			DestroyImmediate(gameObject);
		}

		public void DeleteThisPoint()
		{
			if (_nextPoints != null && _nextPoints.Count > 0 &&
				_prevPoints != null && _prevPoints.Count > 0)
			{
				int count = Mathf.Max(_prevPoints.Count, _nextPoints.Count);
				for (int i = 0; i < count; i++)
				{
					int index = Mathf.Min(_prevPoints.Count - 1, i);
					MapPoint prevPoint = index >= 0 ? _prevPoints[index] : null;

					index = Mathf.Min(_nextPoints.Count - 1, i);
					MapPoint nextPoint = index >= 0 ? _nextPoints[index] : null;

					if (prevPoint != null)
					{
						index = prevPoint.NextPoints.IndexOf(this);
						if (index >= 0) prevPoint.NextPoints[index] = nextPoint;
					}

					if (nextPoint != null)
					{
						index = nextPoint.PrevPoints.IndexOf(this);
						if (index >= 0) nextPoint.PrevPoints[index] = prevPoint;
					}
				}
			}
			else
			{
				if (_prevPoints != null)
				{
					for (int i = 0; i < _prevPoints.Count; i++)
					{
						_prevPoints[i].NextPoints.Remove(this);
					}
				}
				if (_nextPoints != null)
				{
					for (int i = 0; i < _nextPoints.Count; i++)
					{
						_nextPoints[i].PrevPoints.Remove(this);
					}
				}
			}

			if (_prevPoints != null) _prevPoints.Clear();
			if (_nextPoints != null) _nextPoints.Clear();

			DestroyImmediate(gameObject);
		}

		virtual protected void OnDrawGizmos()
		{
			Gizmos.color = UnityEngine.Color.green;
			Gizmos.DrawSphere(transform.position, 0.2f);

			if (_nextPoints != null)
			{
				for (int i = 0; i < _nextPoints.Count; i++)
				{
					if (_nextPoints[i] != null)
					{
						Gizmos.DrawLine(transform.position, _nextPoints[i].transform.position);
					}
				}
			}
		}
	}
}
