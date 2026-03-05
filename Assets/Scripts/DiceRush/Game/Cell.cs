using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace StepanoffGames.DiceRush.Game
{
	public enum CellType
	{
		Empty,
		Start,
		Finish,
		Regular,
		SkipMove,
		ExtraMove,
		MoveToForward,
		MoveToBackward,

		Reward,
		Enemy,
		MoveForward,
		MoveBackward,
		Portal1,
		Portal2,
		Portal3,
		Portal4,
		Portal5
	}

	[ExecuteInEditMode]
	public class Cell : MapPoint
	{
		[Space]
		[SerializeField] protected CellType _type;
		[Space]
		[SerializeField] private CellDrawer _drawer;
		[SerializeField] private List<Transform> _playerPositions;
		[SerializeField] private MapPoint _moveToPoint;

		public CellType Type => _type;
		
		public List<Transform> PlayerPositions => _playerPositions;		
		public MapPoint MoveToPoint => _moveToPoint;

		public int Index => _index;
		private int _index;

		public bool IsLocked => _isLocked;
		private bool _isLocked;

		private bool isDrawerInitializing;
		private CellType oldType;

		private void Awake()
		{
			if (_drawer == null && !isDrawerInitializing)
			{
				InitCellDrawer();
			}
			else
			{
				_drawer.Show(this);
			}

			if (_playerPositions == null || _playerPositions.Count == 0)
			{
				if (_playerPositions == null) _playerPositions = new List<Transform>();

				_playerPositions.Add(CreatePlayerPosition(transform.position + new Vector3(-1f, 0f, 1f), 1));
				_playerPositions.Add(CreatePlayerPosition(transform.position + new Vector3(1f, 0f, -1f), 2));
				_playerPositions.Add(CreatePlayerPosition(transform.position + new Vector3(1f, 0f, 1f), 3));
				_playerPositions.Add(CreatePlayerPosition(transform.position + new Vector3(-1f, 0f, -1f), 4));
			}
		}

		private async void InitCellDrawer()
		{
#if UNITY_EDITOR
			isDrawerInitializing = true;
			string drawerPath = $"Game/CellDrawer.prefab";
			var handle = Addressables.LoadAssetAsync<GameObject>(drawerPath);
			await UniTask.WaitUntil(() => handle.IsDone);

			GameObject drawerObject = (GameObject)PrefabUtility.InstantiatePrefab(handle.Result, transform);
			drawerObject.name = "Drawer";
			drawerObject.transform.localScale = Vector3.one;
			drawerObject.transform.localPosition = Vector3.zero;

			_drawer = drawerObject.GetComponent<CellDrawer>();
			_drawer.Show(this);
			isDrawerInitializing = false;
#endif
		}

		private Transform CreatePlayerPosition(Vector3 pos, int id)
		{
			GameObject positionObject = new GameObject($"Position{id}");
			positionObject.transform.parent = transform;
			positionObject.transform.position = pos;

			return positionObject.transform;
		}

		public void SetIndex(int index)
		{
			_index = index;
		}

		public void SetLocked(bool locked)
		{
			_isLocked = locked;
		}

		public void SetType(CellType type)
		{
			_type = type;
			if (_drawer != null) _drawer.Show(this);
		}

#if UNITY_EDITOR
		private void Update()
		{
			if (oldType != _type)
			{
				oldType = _type;
				if (_drawer != null) _drawer.Show(this);
			}
		}
#endif

		public bool HasNearCellWithSameType(CellType type)
		{
			if (_prevPoints != null)
			{
				for (int i = 0; i < _prevPoints.Count; i++)
				{
					if (IsPrevCellWithSameType(_prevPoints[i], type)) return true;
				}
			}

			if (_nextPoints != null)
			{
				for (int i = 0; i < _nextPoints.Count; i++)
				{
					if (IsNextCellWithSameType(_nextPoints[i], type)) return true;
				}
			}

			return false;
		}

		private bool IsPrevCellWithSameType(MapPoint prevPoint, CellType type)
		{
			if (prevPoint is Cell) return ((Cell)prevPoint).Type == type;

			if (prevPoint.PrevPoints != null)
			{
				for (int i = 0; i < prevPoint.PrevPoints.Count; i++)
				{
					if (IsPrevCellWithSameType(prevPoint.PrevPoints[i], type)) return true;
				}
			}

			return false;
		}

		private bool IsNextCellWithSameType(MapPoint nextPoint, CellType type)
		{
			if (nextPoint is Cell) return ((Cell)nextPoint).Type == type;

			if (nextPoint.NextPoints != null)
			{
				for (int i = 0; i < nextPoint.NextPoints.Count; i++)
				{
					if (IsNextCellWithSameType(nextPoint.NextPoints[i], type)) return true;
				}
			}

			return false;
		}

		override protected void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(transform.position, 0.3f);

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

			if (_playerPositions != null)
			{
				Gizmos.color = Color.yellow;
				for (int i = 0; i < _playerPositions.Count; i++)
				{
					if (_playerPositions[i] != null)
					{
						Gizmos.DrawLine(transform.position, _playerPositions[i].position);
						Gizmos.DrawSphere(_playerPositions[i].position, 0.2f);
					}
				}
			}

			if (_moveToPoint != null)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawLine(transform.position, _moveToPoint.transform.position);
			}
		}
	}
}
