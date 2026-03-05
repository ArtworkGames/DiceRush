using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.DiceRush.UI.Components.Bag;
using StepanoffGames.Services;
using System.Collections.Generic;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Bag
{
	public class BagController : MonoBehaviour, IService
	{
		[SerializeField] private BagAnimation _animation;
		[SerializeField] private BagPanel _panel;

		private LevelManager _level;

		private CellType _cellType;
		private bool _animationFinished;

		private void Awake()
		{
			ServiceLocator.Register(this);

			_level = ServiceLocator.Get<LevelManager>();
		}

		private void Start()
		{
			_animation.OnShowToken += OnAnimationShowToken;
			_animation.OnAnimationFinished += OnAnimationFinished;
		}

		private void OnDestroy()
		{
			ServiceLocator.Unregister<BagController>();

			_level = null;
		}

		public async UniTask<CellType> Draw(PlayerController player)
		{
			List<CellType> cellTypes = new List<CellType>();
			_cellType = GetCellType(player, ref cellTypes);

			_panel.ShowTokens(cellTypes);

			_animationFinished = false;
			_animation.Draw();

			await UniTask.WaitUntil(() => _animationFinished);

			return _cellType;
		}

		public void Confirm()
		{
			_animation.Confirm();
			_panel.HideTokens();
		}

		public CellType GetCellType(PlayerController player)
		{
			List<CellType> cellTypes = new List<CellType>();
			_cellType = GetCellType(player, ref cellTypes);
			return _cellType;
		}

		private CellType GetCellType(PlayerController player, ref List<CellType> cellTypes)
		{
			int playerCellIndex = ((Cell)player.Avatar.CurrentPoint).Index;

			string str = $"{playerCellIndex} [";

			List<int> otherPlayerCellIndexes = new List<int>();
			for (int i = 0; i < _level.Players.Count; i++)
			{
				if (player != _level.Players[i])
				{
					int cellIndex = ((Cell)_level.Players[i].Avatar.CurrentPoint).Index;
					otherPlayerCellIndexes.Add(cellIndex);

					str += $"{cellIndex}, ";
				}
			}
			
			str += "]";

			bool isPlayerInFrontOfAll = true;
			bool isPlayerInBackOfAll = true;
			int frontDistance = 1000;
			int backDistance = 1000;
			for (int i = 0; i < otherPlayerCellIndexes.Count; i++)
			{
				if (otherPlayerCellIndexes[i] >= playerCellIndex) isPlayerInFrontOfAll = false;
				if (otherPlayerCellIndexes[i] <= playerCellIndex) isPlayerInBackOfAll = false;
				frontDistance = Mathf.Min(frontDistance, playerCellIndex - otherPlayerCellIndexes[i]);
				backDistance = Mathf.Min(backDistance, otherPlayerCellIndexes[i] - playerCellIndex);
			}
			int frontCount = frontDistance / 7;
			int backCount = backDistance / 7;

			if (player.Model.Type == Data.Models.PlayerType.HI)
			{
				Debug.Log(
					$"{str} | isPlayerInFrontOfAll = {isPlayerInFrontOfAll}, frontDistance = {frontDistance}, frontCount = {frontCount}, " +
					$"isPlayerInBackOfAll = {isPlayerInBackOfAll}, backDistance = {backDistance}, backCount = {backCount}"
				);
			}

			bool hasNearPortalCell = ((Cell)player.Avatar.CurrentPoint).HasNearCellWithSameType(CellType.Portal1);
			bool hasNearMoveForwardCell = ((Cell)player.Avatar.CurrentPoint).HasNearCellWithSameType(CellType.MoveForward);
			bool hasNearMoveBackwardCell = ((Cell)player.Avatar.CurrentPoint).HasNearCellWithSameType(CellType.MoveBackward);

			cellTypes = new List<CellType>();

			cellTypes.Add(CellType.Reward);
			cellTypes.Add(CellType.Reward);
			cellTypes.Add(CellType.Reward);

			cellTypes.Add(CellType.Enemy);
			cellTypes.Add(CellType.Enemy);
			cellTypes.Add(CellType.Enemy);

			cellTypes.Add(CellType.MoveForward);
			cellTypes.Add(CellType.MoveForward);

			if (!hasNearMoveForwardCell)
			{
				cellTypes.Add(CellType.MoveForward);
				cellTypes.Add(CellType.MoveForward);
			}
			if (isPlayerInBackOfAll)
			{
				for (int i = 0; i < backCount; i++)
				{
					cellTypes.Add(CellType.MoveForward);
				}
			}

			if (!hasNearMoveBackwardCell)
			{
				cellTypes.Add(CellType.MoveBackward);
				cellTypes.Add(CellType.MoveBackward);
				cellTypes.Add(CellType.MoveBackward);
				cellTypes.Add(CellType.MoveBackward);
				//cellTypes.Add(CellType.MoveBackward);

				if (isPlayerInFrontOfAll)
				{
					for (int i = 0; i < frontCount; i++)
					{
						cellTypes.Add(CellType.MoveBackward);
					}
				}
			}

			cellTypes.Add(CellType.Portal1);
			if (!hasNearPortalCell)
			{
				cellTypes.Add(CellType.Portal1);
				cellTypes.Add(CellType.Portal1);
			}
			if (isPlayerInFrontOfAll)
			{
				for (int i = 0; i < frontCount; i++)
				{
					cellTypes.Add(CellType.Portal1);
				}
			}
			if (isPlayerInBackOfAll)
			{
				for (int i = 0; i < backCount; i++)
				{
					cellTypes.Add(CellType.Portal1);
				}
			}

			int index = Random.Range(0, cellTypes.Count);
			CellType cellType = cellTypes[index];

			return cellType;
		}

		public void ShowToken(CellType cellType)
		{
			_animation.SetCellType(cellType);
		}

		private void OnAnimationShowToken()
		{
			ShowToken(_cellType);
		}

		private void OnAnimationFinished()
		{
			_animationFinished = true;
		}
	}
}
