using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Bag;
using StepanoffGames.DiceRush.Game.Battle;
using StepanoffGames.DiceRush.Game.Chest;
using StepanoffGames.DiceRush.Game.Deck;
using StepanoffGames.DiceRush.Game.Dice;
using StepanoffGames.DiceRush.Game.Fork;
using StepanoffGames.DiceRush.Game.Players.Signals;
using StepanoffGames.Services;
using StepanoffGames.Signals;

namespace StepanoffGames.DiceRush.Game.Players
{
	public class PlayerController
	{
		public PlayerModel Model => _model;
		protected PlayerModel _model;

		public PlayerAvatar Avatar => _avatar;
		protected PlayerAvatar _avatar;

		protected LevelManager _levelManager;
		protected Map _map;
		protected DiceController _diceController;
		protected BagController _bagController;
		protected DeckController _deckController;
		protected ForkController _forkController;
		protected ChestController _chestController;
		protected BattleController _battleController;

		protected bool _isSkipNextMove;

		public PlayerController(PlayerModel model, PlayerAvatar avatar)
		{
			_model = model;
			_avatar = avatar;

			_levelManager = ServiceLocator.Get<LevelManager>();
			_map = ServiceLocator.Get<Map>();
			_diceController = ServiceLocator.Get<DiceController>();
			_bagController = ServiceLocator.Get<BagController>();
			_deckController = ServiceLocator.Get<DeckController>();
			_forkController = ServiceLocator.Get<ForkController>();
			_chestController = ServiceLocator.Get<ChestController>();
			_battleController = ServiceLocator.Get<BattleController>();
		}

		virtual public void Destroy()
		{
			_model = null;
			_avatar = null;

			_levelManager = null;
			_map = null;
			_diceController = null;
			_bagController = null;
			_deckController = null;
			_chestController = null;
			_battleController = null;
		}

		public async UniTask Turn()
		{
			await MoveForward(true);
		}

		public async UniTask MoveForward(bool isFirst = false)
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

			SignalBus.Publish(new PlayerMoveStartedSignal(this));

			//CellType cellType = ((Cell)_avatar.CurrentPoint).Type;
			//bool isMoveForward = isFirst || cellType != CellType.MoveBackward;
			int diceValue = await RollDice(true);

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
						int nextIndex = await SelectNextDirection(diceValue, i);
						await _avatar.MoveToPoint(_avatar.CurrentPoint.NextPoints[nextIndex]);
					}
					else
					{
						break;
					}
				}
				while (!(_avatar.CurrentPoint is Cell));

				SignalBus.Publish(new PlayerCellPassedSignal(this));

				if (_avatar.CurrentPoint.NextPoints.Count == 0)
				{
					break;
				}
			}

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

			SignalBus.Publish(new PlayerMoveStartedSignal(this));

			//CellType cellType = ((Cell)_avatar.CurrentPoint).Type;
			int diceValue = await RollDice(false);

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
						int prevIndex = await SelectPrevDirection(diceValue, i);
						await _avatar.MoveToPoint(_avatar.CurrentPoint.PrevPoints[prevIndex]);
					}
					else
					{
						break;
					}
				}
				while (!(_avatar.CurrentPoint is Cell));

				SignalBus.Publish(new PlayerCellPassedSignal(this));

				if (_avatar.CurrentPoint.PrevPoints.Count == 0)
				{
					break;
				}
			}

			await EndMove();

			await CheckCurrentCell();
		}

		private async UniTask CheckCurrentCell()
		{
			if (!(_avatar.CurrentPoint is Cell)) return;

			Cell currentCell = (Cell)_avatar.CurrentPoint;
			bool isJustDefinedCell = false;

			//if (currentCell.Type == CellType.Empty)
			//{
			//	if (currentCell.IsLocked)
			//	{
			//		await BeforeWaitForCellToUnlock();

			//		await UniTask.WaitWhile(() => currentCell.IsLocked);
			//	}
			//	else
			//	{
			//		isJustDefinedCell = true;
			//		currentCell.SetLocked(true);

			//		CellType tileType = await DrawToken();
			//		currentCell.SetType(tileType);

			//		currentCell.SetLocked(false);
			//	}
			//}

			//if (currentCell.IsUsed)
			//{
			//	await _avatar.MoveToCurrentCellPlayerPosition();
			//	await UniTask.WaitForSeconds(1f);
			//	return;
			//}

			if (currentCell.IsUsed || currentCell.IsLocked)
			{
				await _avatar.MoveToCurrentCellPlayerPosition();
				await UniTask.WaitForSeconds(1f);
				return;
			}

			if (currentCell.Type == CellType.Empty)
			{
				isJustDefinedCell = true;
				currentCell.SetLocked(true);

				CellType tileType = await DrawToken();
				currentCell.SetType(tileType);

				currentCell.SetLocked(false);
			}

			currentCell.SetUsed(true);

			switch (currentCell.Type)
			{
				case CellType.Start:
				case CellType.Finish:
				case CellType.Regular:
					await _avatar.MoveToCurrentCellPlayerPosition();
					await UniTask.WaitForSeconds(1f);
					break;

				case CellType.Reward:
					if (isJustDefinedCell)
					{
						await OpenChest();
					}
					await _avatar.MoveToCurrentCellPlayerPosition();
					await UniTask.WaitForSeconds(1f);
					break;

				case CellType.Enemy:
					await Battle();
					await _avatar.MoveToCurrentCellPlayerPosition();
					await UniTask.WaitForSeconds(1f);
					break;

				case CellType.MoveForward:
					await MoveForward();
					break;

				case CellType.MoveBackward:
					await MoveBackward();
					break;

				case CellType.Portal:
					//Cell otherCell = _map.GetOtherCellSameTypeClosestToFinish((Cell)_avatar.CurrentPoint);

					//Cell otherPortal = null;
					//int cellIndex = 0;
					//for (int i = 0; i < _map.Cells.Length; i++)
					//{
					//	if (_map.Cells[i].Type == CellType.Portal && !_map.Cells[i].IsUsed && _map.Cells[i] != _avatar.CurrentPoint && _map.Cells[i].Index > cellIndex)
					//	{
					//		cellIndex = _map.Cells[i].Index;
					//		otherPortal = _map.Cells[i];
					//	}
					//}

					Cell otherPortal = _map.GetOtherPortal(currentCell);

					if (otherPortal != null)
					{
						otherPortal.SetLocked(true);

						await BeforeMoveToNextPortal(otherPortal);

						otherPortal.SetUsed(true);
						otherPortal.SetLocked(false);

						_avatar.SetToCellCenterPosition(otherPortal);

						SignalBus.Publish(new PlayerPortalPassedSignal(this));
					}

					_avatar.MoveToCurrentCellPlayerPosition().Forget();
					await UniTask.WaitForSeconds(1f);
					break;
			}
		}

		virtual protected async UniTask<int> RollDice(bool isMoveForward)
		{
			await UniTask.Yield();
			return 0;
		}

		virtual protected async UniTask<int> SelectNextDirection(int diceValue, int cellsPassed)
		{
			await UniTask.Yield();
			return 0;
		}

		virtual protected async UniTask<int> SelectPrevDirection(int diceValue, int cellsPassed)
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

		virtual protected async UniTask<CellType> DrawToken()
		{
			await UniTask.Yield();
			return CellType.Empty;
		}

		virtual protected async UniTask OpenChest()
		{
			await UniTask.Yield();
		}

		virtual protected async UniTask Battle()
		{
			await UniTask.Yield();
		}

		virtual protected async UniTask BeforeMoveToNextPortal(Cell portalCell)
		{
			await UniTask.Yield();
		}
	}
}
