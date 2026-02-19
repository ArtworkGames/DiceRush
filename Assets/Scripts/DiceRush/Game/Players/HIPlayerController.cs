using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Players
{
	public class HIPlayerController : PlayerController
	{
		public HIPlayerController(PlayerModel model, PlayerAvatar view) : base(model, view)
		{
		}

		override protected async UniTask<int> RollDice(CellType cellType)
		{
			//Level.Instance.XpManager.IncMultiplier();

			//await Game.Instance.Camera.FocusOnPlayer(_view);

			if (cellType == CellType.MoveBackward)
			{
				Level.Instance.Way.ShowMarkersInBackOfPlayer(_avatar);
			}
			else
			{
				Level.Instance.Way.ShowMarkersInFrontOfPlayer(_avatar);
			}
			await Level.Instance.Camera.FocusOnWayMarkers(_avatar);

			int diceValue = await _dice.Roll(this);
		
			Level.Instance.Way.ShowDiceValue(diceValue);

			int oldDiceValue = diceValue;
			diceValue = await Level.Instance.Deck.ConfirmDiceRoll(this, diceValue);

			if (diceValue != oldDiceValue)
			{
				_dice.ShowValue(diceValue);
				Level.Instance.Way.ShowDiceValue(diceValue);
			}
			_dice.Confirm();

			return diceValue;
		}

		//override protected async UniTask<int> ConfirmDiceRoll(int diceValue)
		//{
		//	int oldDiceValue = diceValue;
		//	diceValue = await Level.Instance.Deck.ConfirmDiceRoll(this, diceValue);

		//	if (diceValue != oldDiceValue)
		//	{
		//		Level.Instance.Dice.ShowValue(diceValue);
		//		Level.Instance.Way.ShowDiceValue(diceValue);
		//	}

		//	return diceValue;
		//}

		//override protected async UniTask AfterMoveToCell(CellType cellType)
		//{
		//	await UniTask.Yield();

		//	Level.Instance.XpManager.AddMoveXp(1);
		//}

		override protected async UniTask<int> SelectNextDirection()
		{
			//await Game.Instance.Camera.FocusOnWayMarkers(_view);
			Level.Instance.Camera.FocusOnWayMarkers(_avatar).Forget();

			return await Level.Instance.Fork.SelectNextDirectionForPoint(_avatar.CurrentPoint, _avatar);
		}

		override protected async UniTask<int> SelectPrevDirection()
		{
			//await Game.Instance.Camera.FocusOnWayMarkers(_view);
			Level.Instance.Camera.FocusOnWayMarkers(_avatar).Forget();

			return await Level.Instance.Fork.SelectPrevDirectionForPoint(_avatar.CurrentPoint, _avatar);
		}

		override protected async UniTask EndMove()
		{
			await UniTask.Yield();

			Level.Instance.Way.HideMarkers();
		}

		override protected async UniTask BeforeWaitForCellToUnlock()
		{
			await Level.Instance.Camera.FocusOnPlayer(_avatar);
		}

		override protected async UniTask<CellType> DealTile()
		{
			await Level.Instance.Camera.FocusOnPlayer(_avatar);

			//bool hasNearMoveBackwardCell = ((Cell)_avatar.CurrentPoint).HasNearCellWithSameType(CellType.MoveBackward);
			//CellType tileType = await _bag.Deal(hasNearMoveBackwardCell);

			CellType tileType = await _bag.Deal(this);

			//_view.CurrentCell.SetType(tileType);

			//Level.Instance.Bag.Confirm();

			CellType oldTileType = tileType;
			tileType = await Level.Instance.Deck.ConfirmTileTake(this, tileType);

			if (tileType != oldTileType)
			{
				_bag.ShowTile(tileType);
				//_view.CurrentCell.SetType(tileType);
			}

			_bag.Confirm();

			return tileType;
		}

		//override protected async UniTask<CellType> ConfirmTileTake(CellType tileType, bool hasNearMoveBackwardCell)
		//{
		//	CellType oldTileType = tileType;
		//	tileType = await Level.Instance.Deck.ConfirmTileTake(this, tileType, hasNearMoveBackwardCell);

		//	if (tileType != oldTileType)
		//	{
		//		Level.Instance.Bag.ShowTile(tileType);
		//		//_view.CurrentCell.SetType(tileType);
		//	}

		//	return tileType;
		//}

		override protected async UniTask OpenChest()
		{
			await _chest.Open(this);
		}

		override protected async UniTask BeforeMoveToNextPortal(Cell portalCell)
		{
			await Level.Instance.Camera.FocusOnCell(portalCell);

			//await UniTask.WaitForSeconds(0.5f);
		}
	}
}
