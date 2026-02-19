using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.Services;
using System.Collections.Generic;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Bag
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

	public class BagController : MonoBehaviour, IService
	{
		[SerializeField] private BagAnimation _animation;
		//[SerializeField] private BagTile _handTile;
		//[SerializeField] private BagTile _tile;

		public CellTile TakenTile => _takenTile;

		private List<CellTile> _tilesSet;

		private CellTile _takenTile;
		private bool _animationFinished;

		private void Awake()
		{
			ServiceLocator.Register(this);
		}

		private void Start()
		{
			_animation.OnShowTile += OnShowTile;
			_animation.OnAnimationFinished += OnAnimationFinished;
		}

		private void OnDestroy()
		{
			ServiceLocator.Unregister<BagController>();
		}

		//public async UniTask<CellType> Deal(bool hasNearMoveBackwardCell)
		public async UniTask<CellType> Deal(PlayerController player)
		{
			_takenTile = GetTile(player);

			_animationFinished = false;
			_animation.Take();

			await UniTask.WaitUntil(() => _animationFinished);

			return _takenTile.FrontType;
		}

		public void Confirm()
		{
			_animation.Confirm();
		}

		//public CellTile GetTile(bool hasNearMoveBackwardCell)
		public CellTile GetTile(PlayerController player)
		{
			bool hasNearPortalCell = ((Cell)player.Avatar.CurrentPoint).HasNearCellWithSameType(CellType.Portal1);
			bool hasNearMoveForwardCell = ((Cell)player.Avatar.CurrentPoint).HasNearCellWithSameType(CellType.MoveForward);
			bool hasNearMoveBackwardCell = ((Cell)player.Avatar.CurrentPoint).HasNearCellWithSameType(CellType.MoveBackward);

			List<CellTile> tiles = new List<CellTile>();

			//tiles.Add(new CellTile(CellType.Reward, CellType.Enemy));
			tiles.Add(new CellTile(CellType.Reward, CellType.Enemy));
			tiles.Add(new CellTile(CellType.Reward, CellType.Enemy));

			tiles.Add(new CellTile(CellType.Enemy, CellType.Reward));
			tiles.Add(new CellTile(CellType.Enemy, CellType.Reward));
			tiles.Add(new CellTile(CellType.Enemy, CellType.Reward));

			tiles.Add(new CellTile(CellType.MoveForward, CellType.Reward));
			tiles.Add(new CellTile(CellType.MoveForward, CellType.Reward));
			if (!hasNearMoveForwardCell)
			{
				tiles.Add(new CellTile(CellType.MoveForward, CellType.Reward));
				tiles.Add(new CellTile(CellType.MoveForward, CellType.Reward));
			}

			if (!hasNearMoveBackwardCell)
			{
				tiles.Add(new CellTile(CellType.MoveBackward, CellType.Enemy));
				tiles.Add(new CellTile(CellType.MoveBackward, CellType.Enemy));
				tiles.Add(new CellTile(CellType.MoveBackward, CellType.Enemy));
				tiles.Add(new CellTile(CellType.MoveBackward, CellType.Enemy));
				tiles.Add(new CellTile(CellType.MoveBackward, CellType.Enemy));
				//tiles.Add(new CellTile(CellType.MoveBackward, CellType.Enemy));
			}

			tiles.Add(new CellTile(CellType.Portal1, CellType.Enemy));
			//tiles.Add(new CellTile(CellType.Portal1, CellType.Enemy));
			if (!hasNearPortalCell)
			{
				tiles.Add(new CellTile(CellType.Portal1, CellType.Enemy));
				tiles.Add(new CellTile(CellType.Portal1, CellType.Enemy));
			}

			int index = Random.Range(0, tiles.Count);
			CellTile tile = tiles[index];

			return tile;
		}

		public void ShowTile(CellType type)
		{
			_animation.SetTile(type);
			//_handTile.UpdateView(type);
			//_tile.UpdateView(type);
		}

		private void OnShowTile()
		{
			//_animation.SetTile(_takenTile.FrontType);
			ShowTile(_takenTile.FrontType);
		}

		private void OnAnimationFinished()
		{
			_animationFinished = true;
		}
	}
}
