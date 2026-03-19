using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Path;
using StepanoffGames.Services;

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

		override protected async UniTask<int> RollDice(bool isMoveForward)
		{
			if (!isMoveForward)
			{
				_path.ShowMarkersInBackOfPlayer(_avatar);
			}
			else
			{
				_path.ShowMarkersInFrontOfPlayer(_avatar);
			}
			await _levelManager.Camera.FocusOnPathMarkers(_avatar);

			int diceValue = await _diceController.Roll(this);

			if (!isMoveForward)
			{
				_path.ShowDiceValueInBackOfPlayer(_avatar, diceValue);
			}
			else
			{
				_path.ShowDiceValueInFrontOfPlayer(_avatar, diceValue);
			}

			int oldDiceValue = diceValue;
			diceValue = await _deckController.ConfirmDiceRoll(this, diceValue);

			if (diceValue != oldDiceValue)
			{
				//_dice.ShowValue(diceValue);

				if (!isMoveForward)
				{
					_path.ShowDiceValueInBackOfPlayer(_avatar, diceValue);
				}
				else
				{
					_path.ShowDiceValueInFrontOfPlayer(_avatar, diceValue);
				}
			}
			_diceController.Confirm();

			return diceValue;
		}

		override protected async UniTask<int> SelectNextDirection(int diceValue, int cellsPassed)
		{
			//await Game.Instance.Camera.FocusOnWayMarkers(_view);
			_levelManager.Camera.FocusOnPathMarkers(_avatar).Forget();

			int directionIndex = await _forkController.SelectNextDirection(_avatar.CurrentPoint, _avatar);

			_path.ShowDiceValueInFrontOfPlayer(_avatar, diceValue - cellsPassed, directionIndex);

			return directionIndex;
		}

		override protected async UniTask<int> SelectPrevDirection(int diceValue, int cellsPassed)
		{
			//await Game.Instance.Camera.FocusOnWayMarkers(_view);
			_levelManager.Camera.FocusOnPathMarkers(_avatar).Forget();

			int directionIndex = await _forkController.SelectPrevDirection(_avatar.CurrentPoint, _avatar);

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
			await _levelManager.Camera.FocusOnPlayer(_avatar);
		}

		override protected async UniTask<CellType> DrawToken()
		{
			await _levelManager.Camera.FocusOnPlayer(_avatar);

			CellType cellType = await _bagController.Draw(this);

			CellType oldCellType = cellType;
			cellType = await _deckController.ConfirmTokenDraw(this, cellType);

			if (cellType != oldCellType)
			{
				_bagController.ShowToken(cellType);
			}

			_bagController.Confirm();

			return cellType;
		}

		override protected async UniTask OpenChest()
		{
			await _chestController.Open(this);
		}

		override protected async UniTask Battle()
		{
			await _battleController.Fight(this);
		}

		override protected async UniTask BeforeMoveToNextPortal(Cell portalCell)
		{
			await _levelManager.Camera.FocusOnCell(portalCell);

			//await UniTask.WaitForSeconds(0.5f);
		}
	}
}
