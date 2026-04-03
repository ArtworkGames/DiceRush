using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Game.Map;
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

		public CellType CurrentCellType => _currentCellType;
		private CellType _currentCellType;

		private LevelManager _levelManager;

		private bool animationFinished;

		private void Awake()
		{
			ServiceLocator.Register(this);

			_levelManager = ServiceLocator.Get<LevelManager>();
		}

		private void Start()
		{
			_animation.OnShowToken += OnAnimationShowToken;
			_animation.OnAnimationFinished += OnAnimationFinished;
		}

		private void OnDestroy()
		{
			ServiceLocator.Unregister<BagController>();

			_levelManager = null;
		}

		public async UniTask<CellType> Draw(PlayerController player)
		{
			List<CellType> cellTypes = new List<CellType>();
			_currentCellType = GetCellType(player, ref cellTypes, out BagDescription bagDescription);

			_panel.BagButton.DescriptionPopup.SetDescription(bagDescription);

			animationFinished = false;
			_animation.Draw();

			await UniTask.WaitUntil(() => animationFinished);
			_panel.BagButton.Show();

			return _currentCellType;
		}

		public void Confirm()
		{
			_animation.Confirm();
			_panel.BagButton.Hide();
			//_panel.HideTokens();
		}

		public CellType GetCellType(PlayerController player)
		{
			List<CellType> cellTypes = new List<CellType>();
			CellType cellType = GetCellType(player, ref cellTypes, out BagDescription bagDescription);
			return cellType;
		}

		private CellType GetCellType(PlayerController player, ref List<CellType> cellTypes, out BagDescription bagDescription)
		{
			int playerCellIndex = ((Cell)player.Avatar.CurrentPoint).Index;

			string str = $"{playerCellIndex} [";

			List<int> otherPlayerCellIndexes = new List<int>();
			for (int i = 0; i < _levelManager.Players.Count; i++)
			{
				if (player != _levelManager.Players[i])
				{
					int cellIndex = ((Cell)_levelManager.Players[i].Avatar.CurrentPoint).Index;
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

			bool hasNearPortalCell = ((Cell)player.Avatar.CurrentPoint).HasNearCellWithSameType(CellType.Portal);
			bool hasNearMoveForwardCell = ((Cell)player.Avatar.CurrentPoint).HasNearCellWithSameType(CellType.MoveForward);
			bool hasNearMoveBackwardCell = ((Cell)player.Avatar.CurrentPoint).HasNearCellWithSameType(CellType.MoveBackward);

			cellTypes = new List<CellType>();
			bagDescription = new BagDescription();

			cellTypes.Add(CellType.Reward);
			cellTypes.Add(CellType.Reward);
			cellTypes.Add(CellType.Reward);
			bagDescription.Tokens[CellType.Reward].RegularCount = 3;

			cellTypes.Add(CellType.Enemy);
			cellTypes.Add(CellType.Enemy);
			cellTypes.Add(CellType.Enemy);
			//cellTypes.Add(CellType.Enemy); // ???
			bagDescription.Tokens[CellType.Enemy].RegularCount = 3;

			cellTypes.Add(CellType.MoveForward);
			cellTypes.Add(CellType.MoveForward);
			bagDescription.Tokens[CellType.MoveForward].RegularCount = 2;
			if (!hasNearMoveForwardCell)
			{
				cellTypes.Add(CellType.MoveForward);
				cellTypes.Add(CellType.MoveForward);
				bagDescription.Tokens[CellType.MoveForward].RegularCount += 2;
			}
            else
            {
				bagDescription.Tokens[CellType.MoveForward].RemovedCount = 2;
			}
			if (isPlayerInBackOfAll)
			{
				for (int i = 0; i < backCount; i++)
				{
					cellTypes.Add(CellType.MoveForward);
				}
				bagDescription.Tokens[CellType.MoveForward].AddedCount = backCount;
			}

			if (!hasNearMoveBackwardCell)
			{
				cellTypes.Add(CellType.MoveBackward);
				cellTypes.Add(CellType.MoveBackward);
				cellTypes.Add(CellType.MoveBackward);
				cellTypes.Add(CellType.MoveBackward);
				//cellTypes.Add(CellType.MoveBackward);
				bagDescription.Tokens[CellType.MoveBackward].RegularCount = 4;
				if (isPlayerInFrontOfAll)
				{
					for (int i = 0; i < frontCount; i++)
					{
						cellTypes.Add(CellType.MoveBackward);
					}
					bagDescription.Tokens[CellType.MoveBackward].AddedCount = frontCount;
				}
			}
			else
			{
				bagDescription.Tokens[CellType.MoveBackward].RemovedCount = 4;
				if (isPlayerInFrontOfAll)
				{
					bagDescription.Tokens[CellType.MoveBackward].RemovedCount += frontCount;
				}
			}

			cellTypes.Add(CellType.Portal);
			bagDescription.Tokens[CellType.Portal].RegularCount = 1;
			if (!hasNearPortalCell)
			{
				cellTypes.Add(CellType.Portal);
				cellTypes.Add(CellType.Portal);
				bagDescription.Tokens[CellType.Portal].RegularCount += 2;
			}
			else
			{
				bagDescription.Tokens[CellType.Portal].RemovedCount = 2;
			}
			if (isPlayerInFrontOfAll)
			{
				for (int i = 0; i < frontCount; i++)
				{
					cellTypes.Add(CellType.Portal);
				}
				bagDescription.Tokens[CellType.Portal].AddedCount = frontCount;
			}
			if (isPlayerInBackOfAll)
			{
				for (int i = 0; i < backCount; i++)
				{
					cellTypes.Add(CellType.Portal);
				}
				bagDescription.Tokens[CellType.Portal].AddedCount = backCount;
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
			ShowToken(_currentCellType);
		}

		private void OnAnimationFinished()
		{
			animationFinished = true;
		}
	}
}
