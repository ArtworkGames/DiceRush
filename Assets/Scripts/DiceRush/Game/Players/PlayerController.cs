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

		protected LevelManager _level;
		protected Map _map;
		protected DiceController _dice;
		protected BagController _bag;
		protected DeckController _deck;
		protected ForkController _fork;
		protected ChestController _chest;
		protected BattleController _battle;

		protected bool _isSkipNextMove;

		public PlayerController(PlayerModel model, PlayerAvatar avatar)
		{
			_model = model;
			_avatar = avatar;

			_level = ServiceLocator.Get<LevelManager>();
			_map = ServiceLocator.Get<Map>();
			_dice = ServiceLocator.Get<DiceController>();
			_bag = ServiceLocator.Get<BagController>();
			_deck = ServiceLocator.Get<DeckController>();
			_fork = ServiceLocator.Get<ForkController>();
			_chest = ServiceLocator.Get<ChestController>();
			_battle = ServiceLocator.Get<BattleController>();
		}

		virtual public void Destroy()
		{
			_level = null;
			_map = null;
			_dice = null;
			_bag = null;
			_deck = null;
			_chest = null;
			_battle = null;
		}

		public async UniTask Turn()
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

			SignalBus.Publish(new PlayerMoveStartedSignal(this));

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
			bool isJustDefinedCell = false;
			if (_avatar.CurrentPoint is Cell && ((Cell)_avatar.CurrentPoint).Type == CellType.Empty)
			{
				if (((Cell)_avatar.CurrentPoint).IsLocked)
				{
					await BeforeWaitForCellToUnlock();

					await UniTask.WaitWhile(() => ((Cell)_avatar.CurrentPoint).IsLocked);

				}
				else
				{
					isJustDefinedCell = true;
					((Cell)_avatar.CurrentPoint).SetLocked(true);

					CellType tileType = await DrawToken();
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

					//case CellType.MoveToForward:
					//case CellType.MoveToBackward:
					//	if (((Cell)_avatar.CurrentPoint).MoveToPoint != null)
					//	{
					//		do
					//		{
					//			await _avatar.MoveToPoint(((Cell)_avatar.CurrentPoint).MoveToPoint);
					//		}
					//		while (!(_avatar.CurrentPoint is Cell));

					//		await CheckCurrentCell();
					//	}
					//	break;

					//case CellType.SkipMove:
					//	_isSkipNextMove = true;
					//	await _avatar.MoveToCurrentCellPlayerPosition();
					//	await UniTask.WaitForSeconds(1f);
					//	break;

					//case CellType.ExtraMove:
					//	await MoveForward();
					//	break;

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
						Cell otherCell = _map.GetOtherCellSameTypeClosestToFinish((Cell)_avatar.CurrentPoint);
						if (otherCell != null)
						{
							//await Level.Instance.Camera.FocusOnCell(otherCell);

							//await UniTask.WaitForSeconds(0.5f);

							await BeforeMoveToNextPortal(otherCell);

							_avatar.SetToCellCenterPosition(otherCell);

							SignalBus.Publish(new PlayerPortalPassedSignal(this));

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
