using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Path;
using StepanoffGames.Services;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Players
{
	public class HIPlayerController : PlayerController
	{
		protected PathController _path;

		public HIPlayerController(PlayerModel model, PlayerAvatar avatar) : base(model, avatar)
		{
			_path = ServiceLocator.Get<PathController>();
		}

		public override void Destroy()
		{
			base.Destroy();

			_path = null;
		}

		override protected async UniTask<int> RollDice(CellType cellType)
		{
			if (cellType == CellType.MoveBackward)
			{
				_path.ShowMarkersInBackOfPlayer(_avatar);
			}
			else
			{
				_path.ShowMarkersInFrontOfPlayer(_avatar);
			}
			await _level.Camera.FocusOnPathMarkers(_avatar);

			int diceValue = await _dice.Roll(this);

			if (cellType == CellType.MoveBackward)
			{
				_path.ShowDiceValueInBackOfPlayer(_avatar, diceValue);
			}
			else
			{
				_path.ShowDiceValueInFrontOfPlayer(_avatar, diceValue);
			}

			int oldDiceValue = diceValue;
			diceValue = await _deck.ConfirmDiceRoll(this, diceValue);

			if (diceValue != oldDiceValue)
			{
				//_dice.ShowValue(diceValue);

				if (cellType == CellType.MoveBackward)
				{
					_path.ShowDiceValueInBackOfPlayer(_avatar, diceValue);
				}
				else
				{
					_path.ShowDiceValueInFrontOfPlayer(_avatar, diceValue);
				}
			}
			_dice.Confirm();

			return diceValue;
		}

		override protected async UniTask<int> SelectNextDirection(int diceValue, int cellsPassed)
		{
			//await Game.Instance.Camera.FocusOnWayMarkers(_view);
			_level.Camera.FocusOnPathMarkers(_avatar).Forget();

			int directionIndex = await _fork.SelectNextDirection(_avatar.CurrentPoint, _avatar);

			_path.ShowDiceValueInFrontOfPlayer(_avatar, diceValue - cellsPassed, directionIndex);

			return directionIndex;
		}

		override protected async UniTask<int> SelectPrevDirection(int diceValue, int cellsPassed)
		{
			//await Game.Instance.Camera.FocusOnWayMarkers(_view);
			_level.Camera.FocusOnPathMarkers(_avatar).Forget();

			int directionIndex = await _fork.SelectPrevDirection(_avatar.CurrentPoint, _avatar);

			_path.ShowDiceValueInBackOfPlayer(_avatar, diceValue - cellsPassed, directionIndex);

			return directionIndex;
		}

		override protected async UniTask EndMove()
		{
			await UniTask.Yield();

			_path.HideMarkers();
		}

		override protected async UniTask BeforeWaitForCellToUnlock()
		{
			await _level.Camera.FocusOnPlayer(_avatar);
		}

		override protected async UniTask<CellType> DrawToken()
		{
			await _level.Camera.FocusOnPlayer(_avatar);

			CellType cellType = await _bag.Draw(this);

			CellType oldCellType = cellType;
			cellType = await _deck.ConfirmTokenDraw(this, cellType);

			if (cellType != oldCellType)
			{
				_bag.ShowToken(cellType);
			}

			_bag.Confirm();

			return cellType;
		}

		override protected async UniTask OpenChest()
		{
			await _chest.Open(this);
		}

		override protected async UniTask Battle()
		{
			await _battle.Fight(this);
		}

		override protected async UniTask BeforeMoveToNextPortal(Cell portalCell)
		{
			await _level.Camera.FocusOnCell(portalCell);

			//await UniTask.WaitForSeconds(0.5f);
		}
	}
}
