using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Path;
using StepanoffGames.Services;

namespace StepanoffGames.DiceRush.Game.Players
{
	public class HIPlayerController : PlayerController
	{
		protected PathController _path;

		public HIPlayerController(PlayerModel model, PlayerAvatar avatar, PlayerController prevPlayer) : base(model, avatar, prevPlayer)
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
			//SetState(PlayerState.Waiting);
			SetState(PlayerState.RollDice);

			if (!isMoveForward)
			{
				_path.ShowMarkersInBackOfPlayer(_avatar);
			}
			else
			{
				_path.ShowMarkersInFrontOfPlayer(_avatar);
			}
			//await _levelManager.Camera.FocusOnPathMarkers(_avatar);
			_levelManager.Camera.FocusOnPathMarkers(_avatar).Forget();

			_lastDiceValue = await _diceController.Roll(this);

			if (!isMoveForward)
			{
				_path.ShowDiceValueInBackOfPlayer(_avatar, _lastDiceValue);
			}
			else
			{
				_path.ShowDiceValueInFrontOfPlayer(_avatar, _lastDiceValue);
			}

			SetState(PlayerState.ConfirmDice);

			int oldDiceValue = _lastDiceValue;
			_lastDiceValue = await _deckController.ConfirmDiceRoll(this, _lastDiceValue);

			if (_lastDiceValue != oldDiceValue)
			{
				//_dice.ShowValue(diceValue);

				if (!isMoveForward)
				{
					_path.ShowDiceValueInBackOfPlayer(_avatar, _lastDiceValue);
				}
				else
				{
					_path.ShowDiceValueInFrontOfPlayer(_avatar, _lastDiceValue);
				}
			}
			_diceController.Confirm();

			return _lastDiceValue;
		}

		override protected async UniTask<int> SelectNextDirection(int diceValue, int cellsPassed)
		{
			SetState(PlayerState.SelectDirection);

			//await Game.Instance.Camera.FocusOnWayMarkers(_view);
			_levelManager.Camera.FocusOnPathMarkers(_avatar).Forget();

			int directionIndex = await _forkController.SelectNextDirection(_avatar.CurrentPoint, _avatar);
			
			_path.ShowDiceValueInFrontOfPlayer(_avatar, diceValue - cellsPassed, directionIndex);

			return directionIndex;
		}

		override protected async UniTask<int> SelectPrevDirection(int diceValue, int cellsPassed)
		{
			SetState(PlayerState.SelectDirection);

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
			//SetState(PlayerState.Waiting);
			SetState(PlayerState.DrawToken);

			//await _levelManager.Camera.FocusOnPlayer(_avatar);
			_levelManager.Camera.FocusOnPlayer(_avatar).Forget();

			_lastCellType = await _bagController.Draw(this);

			SetState(PlayerState.ConfirmToken);
			
			CellType oldCellType = _lastCellType;
			_lastCellType = await _deckController.ConfirmTokenDraw(this, _lastCellType);

			if (_lastCellType != oldCellType)
			{
				_bagController.ShowToken(_lastCellType);
			}

			_bagController.Confirm();

			return _lastCellType;
		}

		override protected async UniTask OpenChest()
		{
			SetState(PlayerState.OpenChest);
			await _chestController.Open(this);
		}

		override protected async UniTask Battle()
		{
			SetState(PlayerState.Battle);
			await _battleController.Fight(this);
		}

		override protected async UniTask BeforeMoveToNextPortal(Cell portalCell)
		{
			SetState(PlayerState.MoveToPortal);
			await _levelManager.Camera.FocusOnCell(portalCell);

			//await UniTask.WaitForSeconds(0.5f);
		}
	}
}
