using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace StepanoffGames.DiceRush.Gameplay
{
	public class CellTile
	{
		public CellType FrontType;
		public CellType BackType;

		public CellTile(CellType frontType, CellType backType)
		{
			FrontType = frontType;
			BackType = backType;
		}
	}

	public class Bag : MonoBehaviour
	{
		[SerializeField] private BagAnimation _animation;
		[SerializeField] private BagTile _handTile;
		[SerializeField] private BagTile _tile;

		public CellTile TakenTile => _takenTile;

		private List<CellTile> _tilesSet;

		private CellTile _takenTile;
		private bool _animationFinished;

		private void Start()
		{
			_animation.OnShowTile += OnShowTile;
			_animation.OnAnimationFinished += OnAnimationFinished;

			FillBag();
		}

		private void FillBag()
		{
			_tilesSet = new List<CellTile>();

			for (int i = 0; i < 5; i++)
			{
				_tilesSet.Add(new CellTile(CellType.Reward, CellType.Enemy));
			}
			for (int i = 0; i < 5; i++)
			{
				_tilesSet.Add(new CellTile(CellType.Enemy, CellType.Reward));
			}
			for (int i = 0; i < 10; i++)
			{
				_tilesSet.Add(new CellTile(CellType.MoveForward, CellType.Reward));
			}
			for (int i = 0; i < 10; i++)
			{
				_tilesSet.Add(new CellTile(CellType.MoveBackward, CellType.Enemy));
			}
			for (int i = 0; i < 10; i++)
			{
				_tilesSet.Add(new CellTile(CellType.Portal1, CellType.Enemy));
			}
			//for (int i = 0; i < 2; i++)
			//{
			//	_deck.Add(CellType.Portal2);
			//}
			//for (int i = 0; i < 2; i++)
			//{
			//	_deck.Add(CellType.Portal3);
			//}
			//for (int i = 0; i < 2; i++)
			//{
			//	_deck.Add(CellType.Portal4);
			//}
			//for (int i = 0; i < 2; i++)
			//{
			//	_deck.Add(CellType.Portal5);
			//}
		}

		public async UniTask<CellType> TakeTile(bool hasNearMoveBackwardCell)
		{
			_takenTile = GetTile(hasNearMoveBackwardCell);

			_animationFinished = false;
			_animation.Take();

			await UniTask.WaitUntil(() => _animationFinished);

			return _takenTile.FrontType;
		}

		public void Confirm()
		{
			_animation.Confirm();
		}

		public CellTile GetTile_Old(bool hasNearMoveBackwardCell)
		{
			if (_tilesSet.Count == 0) return new CellTile(CellType.Reward, CellType.Reward);

			List<CellTile> selectedTilesSet = new List<CellTile>();
			for (int i = 0; i < _tilesSet.Count; i++)
			{
				if (hasNearMoveBackwardCell)
				{
					if (_tilesSet[i].FrontType != CellType.MoveBackward)
					{
						selectedTilesSet.Add(_tilesSet[i]);
					}
				}
				else
				{
					selectedTilesSet.Add(_tilesSet[i]);
				}
			}

			if (selectedTilesSet.Count == 0) return new CellTile(CellType.Reward, CellType.Reward);

			int index = Random.Range(0, selectedTilesSet.Count);
			CellTile tile = selectedTilesSet[index];
			_tilesSet.Remove(tile);

			return tile;
		}

		public CellTile GetTile(bool hasNearMoveBackwardCell)
		{
			List<CellTile> commonTiles = new List<CellTile>();

			commonTiles.Add(new CellTile(CellType.Reward, CellType.Enemy));
			commonTiles.Add(new CellTile(CellType.Reward, CellType.Enemy));

			commonTiles.Add(new CellTile(CellType.Enemy, CellType.Reward));
			commonTiles.Add(new CellTile(CellType.Enemy, CellType.Reward));
			commonTiles.Add(new CellTile(CellType.Enemy, CellType.Reward));

			commonTiles.Add(new CellTile(CellType.MoveForward, CellType.Reward));
			commonTiles.Add(new CellTile(CellType.MoveForward, CellType.Reward));
			commonTiles.Add(new CellTile(CellType.MoveForward, CellType.Reward));

			commonTiles.Add(new CellTile(CellType.Portal1, CellType.Enemy));
			commonTiles.Add(new CellTile(CellType.Portal1, CellType.Enemy));

			List<CellTile> moveBackwardTiles = new List<CellTile>();

			moveBackwardTiles.Add(new CellTile(CellType.MoveBackward, CellType.Enemy));
			moveBackwardTiles.Add(new CellTile(CellType.MoveBackward, CellType.Enemy));
			moveBackwardTiles.Add(new CellTile(CellType.MoveBackward, CellType.Enemy));
			moveBackwardTiles.Add(new CellTile(CellType.MoveBackward, CellType.Enemy));
			moveBackwardTiles.Add(new CellTile(CellType.MoveBackward, CellType.Enemy));

			List<CellTile> currentTiles = new List<CellTile>();
			currentTiles.AddRange(commonTiles);

			if (!hasNearMoveBackwardCell)
			{
				currentTiles.AddRange(moveBackwardTiles);
			}

			int index = Random.Range(0, currentTiles.Count);
			CellTile tile = currentTiles[index];

			return tile;
		}

		public void ReturnTile()
		{
			if (_takenTile == null) return;
			_tilesSet.Add(_takenTile);
		}

		public void ShowTile(CellType type)
		{
			_handTile.UpdateView(type);
			_tile.UpdateView(type);
		}

		private void OnShowTile()
		{
			ShowTile(_takenTile.FrontType);
		}

		private void OnAnimationFinished()
		{
			_animationFinished = true;
		}
	}
}
