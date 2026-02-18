using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using UnityEngine.UIElements;

namespace StepanoffGames.DiceRush.Gameplay
{
	public class Player
	{
		public PlayerModel Model => _model;
		public Avatar View => _avatar;

		protected PlayerModel _model;
		protected Avatar _avatar;

		protected bool _isSkipNextMove;

		public Player(PlayerModel model, Avatar view)
		{
			_model = model;
			_avatar = view;
		}

		public async UniTask Move()
		{
			await MoveForward();
		}

		public async UniTask MoveForward()
		{
			if (_isSkipNextMove)
			{
				_isSkipNextMove = false;
				return;
			}

			if (_avatar.CurrentPoint is Cell && ((Cell)_avatar.CurrentPoint).Type == CellType.Finish)
			{
				return;
			}

			/*//await Game.Instance.Camera.FocusOnPlayer(_view);

			Level.Instance.Way.ShowMarkersInFrontOfPlayer(_view);
			await Level.Instance.Camera.FocusOnWayMarkers(_view);

			int diceValue = await Level.Instance.Dice.Roll();
			Level.Instance.Way.ShowDiceValue(diceValue);

			//Level.Instance.Dice.Confirm();
			diceValue = await ConfirmDiceRoll(diceValue);
			Level.Instance.Dice.Confirm();*/

			CellType cellType = ((Cell)_avatar.CurrentPoint).Type;

			int diceValue = await RollDice(cellType);

			for (int i = 0; i < diceValue; i++)
			{
				do
				{
					if (_avatar.CurrentPoint.NextPoints.Count == 1)
					{
						await _avatar.MoveToPoint(_avatar.CurrentPoint.NextPoints[0]);
					}
					else if (_avatar.CurrentPoint.NextPoints.Count > 1)
					{
						//await Game.Instance.Camera.FocusOnWayMarkers(_view);
						//Level.Instance.Camera.FocusOnWayMarkers(_view).Forget();

						int nextIndex = await SelectNextDirection();

						await _avatar.MoveToPoint(_avatar.CurrentPoint.NextPoints[nextIndex]);
					}
					else
					{
						break;
					}
				}
				while (!(_avatar.CurrentPoint is Cell));

				await AfterMoveToCell(((Cell)_avatar.CurrentPoint).Type);

				if (_avatar.CurrentPoint.NextPoints.Count == 0)
				{
					break;
				}
			}
			//Level.Instance.Way.HideMarkers();

			await EndMove();

			await CheckCurrentCell();
		}

		public async UniTask MoveBackward()
		{
			if (_isSkipNextMove)
			{
				_isSkipNextMove = false;
				return;
			}

			if (_avatar.CurrentPoint is Cell && ((Cell)_avatar.CurrentPoint).Type == CellType.Start)
			{
				return;
			}

			/*//await Game.Instance.Camera.FocusOnPlayer(_view);

			Level.Instance.Way.ShowMarkersInBackOfPlayer(_view);
			await Level.Instance.Camera.FocusOnWayMarkers(_view);

			int diceValue = await Level.Instance.Dice.Roll();
			Level.Instance.Way.ShowDiceValue(diceValue);

			//Level.Instance.Dice.Confirm();
			diceValue = await ConfirmDiceRoll(diceValue);
			Level.Instance.Dice.Confirm();*/

			CellType cellType = ((Cell)_avatar.CurrentPoint).Type;

			int diceValue = await RollDice(cellType);

			for (int i = 0; i < diceValue; i++)
			{
				do
				{
					if (_avatar.CurrentPoint.PrevPoints.Count == 1)
					{
						await _avatar.MoveToPoint(_avatar.CurrentPoint.PrevPoints[0]);
					}
					else if (_avatar.CurrentPoint.PrevPoints.Count > 1)
					{
						//await Game.Instance.Camera.FocusOnWayMarkers(_view);
						//Level.Instance.Camera.FocusOnWayMarkers(_view).Forget();

						int prevIndex = await SelectPrevDirection();

						await _avatar.MoveToPoint(_avatar.CurrentPoint.PrevPoints[prevIndex]);
					}
					else
					{
						break;
					}
				}
				while (!(_avatar.CurrentPoint is Cell));

				await AfterMoveToCell(((Cell)_avatar.CurrentPoint).Type);

				if (_avatar.CurrentPoint.PrevPoints.Count == 0)
				{
					break;
				}
			}
			//Level.Instance.Way.HideMarkers();

			await EndMove();

			await CheckCurrentCell();
		}

		private async UniTask CheckCurrentCell()
		{
			if (_avatar.CurrentPoint is Cell && ((Cell)_avatar.CurrentPoint).Type == CellType.Empty)
			{
				if (((Cell)_avatar.CurrentPoint).IsLocked)
				{
					//await Level.Instance.Camera.FocusOnPlayer(_view);

					await BeforeWaitForCellToUnlock();

					await UniTask.WaitWhile(() => ((Cell)_avatar.CurrentPoint).IsLocked);

				}
				else
				{
					((Cell)_avatar.CurrentPoint).SetLocked(true);

					//await Level.Instance.Camera.FocusOnPlayer(_view);

					//bool hasNearMoveBackwardCell = ((Cell)_view.CurrentPoint).HasNearCellWithSameType(CellType.MoveBackward);
					//CellType tileType = await Level.Instance.Bag.TakeTile(hasNearMoveBackwardCell);
					////_view.CurrentCell.SetType(tileType);

					////Level.Instance.Bag.Confirm();
					//tileType = await ConfirmTileTake(tileType, hasNearMoveBackwardCell);
					//Level.Instance.Bag.Confirm();

					CellType tileType = await TakeTile();

					((Cell)_avatar.CurrentPoint).SetType(tileType);

					((Cell)_avatar.CurrentPoint).SetLocked(false);
				}
			}

			if (_avatar.CurrentPoint is Cell)
			{
				switch (((Cell)_avatar.CurrentPoint).Type)
				{
					case CellType.Start:
					case CellType.Finish:
					case CellType.Regular:
					case CellType.Reward:
					case CellType.Enemy:
						await _avatar.MoveToCurrentCellPlayerPosition();
						await UniTask.WaitForSeconds(1f);
						break;

					case CellType.MoveToForward:
					case CellType.MoveToBackward:
						if (((Cell)_avatar.CurrentPoint).MoveToPoint != null)
						{
							do
							{
								await _avatar.MoveToPoint(((Cell)_avatar.CurrentPoint).MoveToPoint);
							}
							while (!(_avatar.CurrentPoint is Cell));

							await CheckCurrentCell();
						}
						break;

					case CellType.SkipMove:
						_isSkipNextMove = true;
						await _avatar.MoveToCurrentCellPlayerPosition();
						await UniTask.WaitForSeconds(1f);
						break;

					case CellType.ExtraMove:
						await MoveForward();
						break;

					case CellType.MoveForward:
						await MoveForward();
						break;

					case CellType.MoveBackward:
						await MoveBackward();
						break;

					case CellType.Portal1:
					case CellType.Portal2:
					case CellType.Portal3:
					case CellType.Portal4:
					case CellType.Portal5:
						Cell otherCell = Level.Instance.Map.GetOtherCellSameTypeClosestToFinish((Cell)_avatar.CurrentPoint);
						if (otherCell != null)
						{
							//await Level.Instance.Camera.FocusOnCell(otherCell);

							//await UniTask.WaitForSeconds(0.5f);

							await BeforeMoveToNextPortal(otherCell);

							_avatar.SetToCellCenterPosition(otherCell);

							//await UniTask.WaitForSeconds(1f);
						}
						//await _view.MoveToCellPlayerPosition();
						_avatar.MoveToCurrentCellPlayerPosition().Forget();
						await UniTask.WaitForSeconds(1f);
						break;
				}
			}
		}

		virtual protected async UniTask<int> RollDice(CellType cellType)
		{
			await UniTask.Yield();
			return 0;
		}

		//virtual protected async UniTask<int> ConfirmDiceRoll(int diceValue)
		//{
		//	await UniTask.Yield();
		//	return diceValue;
		//}

		virtual protected async UniTask AfterMoveToCell(CellType cellType)
		{
			await UniTask.Yield();
		}

		virtual protected async UniTask<int> SelectNextDirection()
		{
			await UniTask.Yield();
			return 0;
		}

		virtual protected async UniTask<int> SelectPrevDirection()
		{
			await UniTask.Yield();
			return 0;
		}

		virtual protected async UniTask EndMove()
		{
			await UniTask.Yield();
		}

		virtual protected async UniTask BeforeWaitForCellToUnlock()
		{
			await UniTask.Yield();
		}

		virtual protected async UniTask<CellType> TakeTile()
		{
			await UniTask.Yield();
			return CellType.Empty;
		}

		virtual protected async UniTask BeforeMoveToNextPortal(Cell portalCell)
		{
			await UniTask.Yield();
		}

		//virtual protected async UniTask<CellType> ConfirmTileTake(CellType tileType, bool hasNearMoveBackwardCell)
		//{
		//	await UniTask.Yield();
		//	return tileType;
		//}
	}
}
