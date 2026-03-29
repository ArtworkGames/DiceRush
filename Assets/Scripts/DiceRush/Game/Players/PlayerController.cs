using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Bag;
using StepanoffGames.DiceRush.Game.Battle;
using StepanoffGames.DiceRush.Game.Chest;
using StepanoffGames.DiceRush.Game.Deck;
using StepanoffGames.DiceRush.Game.Dice;
using StepanoffGames.DiceRush.Game.Fork;
using StepanoffGames.DiceRush.Game.Players.Signals;
using StepanoffGames.DiceRush.Game.Xp;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Players
{
	public class PlayerController
	{
		public PlayerModel Model => _model;
		protected PlayerModel _model;

		public PlayerAvatar Avatar => _avatar;
		protected PlayerAvatar _avatar;

		public PlayerController PrevPlayer => _prevPlayer;
		protected PlayerController _prevPlayer;

		public int LastDiceValue => _lastDiceValue;
		protected int _lastDiceValue;

		public CellType LastCellType => _lastCellType;
		protected CellType _lastCellType;

		protected LevelManager _levelManager;
		protected Map _map;
		protected DiceController _diceController;
		protected BagController _bagController;
		protected DeckController _deckController;
		protected ForkController _forkController;
		protected ChestController _chestController;
		protected BattleController _battleController;
		protected XpManager _xpManager;

		protected bool _isSkipNextMove;

		public PlayerController(PlayerModel model, PlayerAvatar avatar, PlayerController prevPlayer)
		{
			_model = model;
			_avatar = avatar;
			_prevPlayer = prevPlayer;

			_levelManager = ServiceLocator.Get<LevelManager>();
			_map = ServiceLocator.Get<Map>();
			_diceController = ServiceLocator.Get<DiceController>();
			_bagController = ServiceLocator.Get<BagController>();
			_deckController = ServiceLocator.Get<DeckController>();
			_forkController = ServiceLocator.Get<ForkController>();
			_chestController = ServiceLocator.Get<ChestController>();
			_battleController = ServiceLocator.Get<BattleController>();
			_xpManager = ServiceLocator.Get<XpManager>();
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
			_xpManager = null;
		}

		public void SetState(PlayerState state)
		{
			if (_model.Type == PlayerType.HI)
			{
				Debug.Log($"[PlayerController] SetState: {state} - {Time.time}");
			}

			_model.State = state;
			SignalBus.Publish(new PlayerStateChangedSignal(this));
		}

		public async UniTask Turn()
		{
			if (_model.IsFinished)
			{
				return;
			}

			if (_prevPlayer != null && _prevPlayer.Model.State != PlayerState.Finish)
			{
				SetState(PlayerState.Waiting);
				await UniTask.WaitUntil(() => _prevPlayer.Model.State == PlayerState.EndTurn || _prevPlayer.Model.State == PlayerState.Finish);

				if (_model.Type == PlayerType.HI)
					await UniTask.WaitForSeconds(0.5f);
			}

			SignalBus.Publish(new PlayerTurnStartedSignal(this));

			await MoveForward(true);

			if (_model.Type == PlayerType.HI)
				await UniTask.WaitForSeconds(0.5f);

			SetState(PlayerState.CountXp);
			await _xpManager.CountTotalXp(_model);

			if (_model.Type == PlayerType.HI)
				await UniTask.WaitForSeconds(0.5f);

			if (_model.IsFinished) SetState(PlayerState.Finish);
			else SetState(PlayerState.EndTurn);
			SignalBus.Publish(new PlayerTurnEndedSignal(this));

			if (_model.Type == PlayerType.HI)
				await UniTask.WaitForSeconds(0.5f);

			//SetState(PlayerState.EndTurn);
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

			_lastDiceValue = await RollDice(true);

			for (int i = 0; i < _lastDiceValue; i++)
			{
				do
				{
					if (_avatar.CurrentPoint.NextPoints.Count == 1)
					{
						SetState(PlayerState.MoveForward);
						await _avatar.MoveToPoint(_avatar.CurrentPoint.NextPoints[0]);
					}
					else if (_avatar.CurrentPoint.NextPoints.Count > 1)
					{
						int nextIndex = await SelectNextDirection(_lastDiceValue, i);

						SetState(PlayerState.MoveForward);
						await _avatar.MoveToPoint(_avatar.CurrentPoint.NextPoints[nextIndex]);
					}
					else
					{
						break;
					}
				}
				while (!(_avatar.CurrentPoint is Cell));

				_model.CellIndex = ((Cell)_avatar.CurrentPoint).Index;

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

			_lastDiceValue = await RollDice(false);

			for (int i = 0; i < _lastDiceValue; i++)
			{
				do
				{
					if (_avatar.CurrentPoint.PrevPoints.Count == 1)
					{
						SetState(PlayerState.MoveBackward);
						await _avatar.MoveToPoint(_avatar.CurrentPoint.PrevPoints[0]);
					}
					else if (_avatar.CurrentPoint.PrevPoints.Count > 1)
					{
						int prevIndex = await SelectPrevDirection(_lastDiceValue, i);

						SetState(PlayerState.MoveBackward);
						await _avatar.MoveToPoint(_avatar.CurrentPoint.PrevPoints[prevIndex]);
					}
					else
					{
						break;
					}
				}
				while (!(_avatar.CurrentPoint is Cell));

				_model.CellIndex = ((Cell)_avatar.CurrentPoint).Index;

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

			if (currentCell.IsUsed || currentCell.IsLocked)
			{
				await MoveToPlayerPosition();
				return;
			}

			if (currentCell.Type == CellType.Empty)
			{
				isJustDefinedCell = true;
				currentCell.SetLocked(true);

				_lastCellType = await DrawToken();
				currentCell.SetType(_lastCellType);

				currentCell.SetLocked(false);
			}

			if (currentCell.Type != CellType.Finish)
			{
				currentCell.SetUsed(true);
			}

			switch (currentCell.Type)
			{
				case CellType.Start:
				case CellType.Regular:
					await MoveToPlayerPosition();
					break;

				case CellType.Finish:
					await MoveToPlayerPosition();
					await Finish();
					break;

				case CellType.Reward:
					if (isJustDefinedCell)
					{
						await OpenChest();
					}
					await MoveToPlayerPosition();
					break;

				case CellType.Enemy:
					await Battle();
					await MoveToPlayerPosition();
					break;

				case CellType.MoveForward:
					await MoveForward();
					break;

				case CellType.MoveBackward:
					await MoveBackward();
					break;

				case CellType.Portal:
					Cell otherPortal = _map.GetOtherPortal(currentCell);

					if (otherPortal != null)
					{
						otherPortal.SetLocked(true);
						await BeforeMoveToNextPortal(otherPortal);

						otherPortal.SetUsed(true);
						otherPortal.SetLocked(false);

						_avatar.SetToCellCenterPosition(otherPortal);
						_model.CellIndex = otherPortal.Index;

						SignalBus.Publish(new PlayerPortalPassedSignal(this));
					}

					await MoveToPlayerPosition();
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

		virtual protected async UniTask MoveToPlayerPosition()
		{
			SetState(PlayerState.MoveToPosition);
			await _avatar.MoveToCurrentCellPlayerPosition();
		}

		virtual protected async UniTask Finish()
		{
			_model.IsFinished = true;
			await UniTask.Yield();
		}
	}
}
