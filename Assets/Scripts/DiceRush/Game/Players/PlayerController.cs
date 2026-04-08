using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Bag;
using StepanoffGames.DiceRush.Game.Battle;
using StepanoffGames.DiceRush.Game.Chest;
using StepanoffGames.DiceRush.Game.Deck;
using StepanoffGames.DiceRush.Game.Dice;
using StepanoffGames.DiceRush.Game.Fork;
using StepanoffGames.DiceRush.Game.Map;
using StepanoffGames.DiceRush.Game.Players.Signals;
using StepanoffGames.DiceRush.Game.Xp;
using StepanoffGames.DiceRush.UI.Messages.Signals;
using StepanoffGames.Localization;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using System.Threading;
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

		protected LocalizationManager _localizationManager;
		protected GameManager _gameManager;
		protected MapController _mapController;
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

			_localizationManager = ServiceLocator.Get<LocalizationManager>();
			_gameManager = ServiceLocator.Get<GameManager>();
			_mapController = ServiceLocator.Get<MapController>();
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

			_localizationManager = null;
			_gameManager = null;
			_mapController = null;
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

		public async UniTask Turn(CancellationToken ct)
		{
			if (_model.IsFinished)
			{
				return;
			}

			if (_prevPlayer != null && _prevPlayer.Model.State != PlayerState.Finish)
			{
				SetState(PlayerState.Waiting);
				await UniTask.WaitUntil(() => _prevPlayer.Model.State == PlayerState.EndTurn || _prevPlayer.Model.State == PlayerState.Finish, cancellationToken: ct);

				if (_model.Type == PlayerType.HI)
					await UniTask.WaitForSeconds(0.5f, cancellationToken: ct);
			}

			SignalBus.Publish(new PlayerTurnStartedSignal(this));
			if (_model.Type == PlayerType.HI && _gameManager.HiPlayersCount > 1)
			{
				SignalBus.Publish(new ShowMessageSignal(_localizationManager.GetString("Message:PlayerStartTurn", _model.Name, _model.Color.ToString())));
			}

			await MoveForward(true, ct);

			if (_model.Type == PlayerType.HI)
				await UniTask.WaitForSeconds(0.5f, cancellationToken: ct);

			SetState(PlayerState.CountXp);
			await _xpManager.CountTotalXp(_model, ct);

			if (_model.Type == PlayerType.HI)
				await UniTask.WaitForSeconds(0.5f, cancellationToken: ct);

			if (_model.IsFinished) SetState(PlayerState.Finish);
			else SetState(PlayerState.EndTurn);
			SignalBus.Publish(new PlayerTurnEndedSignal(this));

			//if (_model.Type == PlayerType.HI)
				await UniTask.WaitForSeconds(0.5f, cancellationToken: ct);

			//SetState(PlayerState.EndTurn);
		}

		public async UniTask MoveForward(bool isFirst, CancellationToken ct)
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

			_lastDiceValue = await RollDice(true, ct);

			for (int i = 0; i < _lastDiceValue; i++)
			{
				do
				{
					if (_avatar.CurrentPoint.NextPoints.Count == 1)
					{
						SetState(PlayerState.MoveForward);
						await _avatar.MoveToPoint(_avatar.CurrentPoint.NextPoints[0], ct);
					}
					else if (_avatar.CurrentPoint.NextPoints.Count > 1)
					{
						int nextIndex = await SelectNextDirection(_lastDiceValue, i, ct);

						SetState(PlayerState.MoveForward);
						await _avatar.MoveToPoint(_avatar.CurrentPoint.NextPoints[nextIndex], ct);
					}
					else
					{
						break;
					}
				}
				while (!(_avatar.CurrentPoint is Cell));

				_model.CellIndex = ((Cell)_avatar.CurrentPoint).Index;
				_model.CellIndexTime = Time.time;

				SignalBus.Publish(new PlayerCellPassedSignal(this));

				if (_avatar.CurrentPoint.NextPoints.Count == 0)
				{
					break;
				}
			}

			await EndMove(ct);
			await CheckCurrentCell(ct);
		}

		public async UniTask MoveBackward(CancellationToken ct)
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

			_lastDiceValue = await RollDice(false, ct);

			for (int i = 0; i < _lastDiceValue; i++)
			{
				do
				{
					if (_avatar.CurrentPoint.PrevPoints.Count == 1)
					{
						SetState(PlayerState.MoveBackward);
						await _avatar.MoveToPoint(_avatar.CurrentPoint.PrevPoints[0], ct);
					}
					else if (_avatar.CurrentPoint.PrevPoints.Count > 1)
					{
						int prevIndex = await SelectPrevDirection(_lastDiceValue, i, ct);

						SetState(PlayerState.MoveBackward);
						await _avatar.MoveToPoint(_avatar.CurrentPoint.PrevPoints[prevIndex], ct);
					}
					else
					{
						break;
					}
				}
				while (!(_avatar.CurrentPoint is Cell));

				_model.CellIndex = ((Cell)_avatar.CurrentPoint).Index;
				_model.CellIndexTime = Time.time;

				SignalBus.Publish(new PlayerCellPassedSignal(this));

				if (_avatar.CurrentPoint.PrevPoints.Count == 0)
				{
					break;
				}
			}

			await EndMove(ct);
			await CheckCurrentCell(ct);
		}

		private async UniTask CheckCurrentCell(CancellationToken ct)
		{
			if (!(_avatar.CurrentPoint is Cell)) return;

			Cell currentCell = (Cell)_avatar.CurrentPoint;
			bool isJustDefinedCell = false;

			if (currentCell.IsUsed || currentCell.IsLocked)
			{
				await MoveToPlayerPosition(ct);
				return;
			}

			if (currentCell.Type == CellType.Empty)
			{
				isJustDefinedCell = true;
				currentCell.SetLocked(true);

				_lastCellType = await DrawToken(ct);
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
					await MoveToPlayerPosition(ct);
					break;

				case CellType.Finish:
					await MoveToPlayerPosition(ct);
					await Finish(ct);
					break;

				case CellType.Reward:
					if (isJustDefinedCell)
					{
						await OpenChest(ct);
					}
					await MoveToPlayerPosition(ct);
					break;

				case CellType.Enemy:
					await Battle(ct);
					await MoveToPlayerPosition(ct);
					break;

				case CellType.MoveForward:
					await MoveForward(false, ct);
					break;

				case CellType.MoveBackward:
					await MoveBackward(ct);
					break;

				case CellType.Portal:
					Cell otherPortal = _mapController.GetOtherPortal(currentCell);

					if (otherPortal != null)
					{
						otherPortal.SetLocked(true);
						await BeforeMoveToNextPortal(otherPortal, ct);

						otherPortal.SetUsed(true);
						otherPortal.SetLocked(false);

						_avatar.SetToCellCenterPosition(otherPortal);
						_model.CellIndex = otherPortal.Index;

						SignalBus.Publish(new PlayerPortalPassedSignal(this));
					}

					await MoveToPlayerPosition(ct);
					break;
			}
		}

		virtual protected async UniTask<int> RollDice(bool isMoveForward, CancellationToken ct)
		{
			await UniTask.Yield(ct);
			return 0;
		}

		virtual protected async UniTask<int> SelectNextDirection(int diceValue, int cellsPassed, CancellationToken ct)
		{
			await UniTask.Yield(ct);
			return 0;
		}

		virtual protected async UniTask<int> SelectPrevDirection(int diceValue, int cellsPassed, CancellationToken ct)
		{
			await UniTask.Yield(ct);
			return 0;
		}

		virtual protected async UniTask EndMove(CancellationToken ct)
		{
			await UniTask.Yield(ct);
		}

		virtual protected async UniTask BeforeWaitForCellToUnlock(CancellationToken ct)
		{
			await UniTask.Yield(ct);
		}

		virtual protected async UniTask<CellType> DrawToken(CancellationToken ct)
		{
			await UniTask.Yield(ct);
			return CellType.Empty;
		}

		virtual protected async UniTask OpenChest(CancellationToken ct)
		{
			await UniTask.Yield(ct);
		}

		virtual protected async UniTask Battle(CancellationToken ct)
		{
			await UniTask.Yield(ct);
		}

		virtual protected async UniTask BeforeMoveToNextPortal(Cell portalCell, CancellationToken ct)
		{
			await UniTask.Yield(ct);
		}

		virtual protected async UniTask MoveToPlayerPosition(CancellationToken ct)
		{
			SetState(PlayerState.MoveToPosition);
			await _avatar.MoveToCurrentCellPlayerPosition(ct);
		}

		virtual protected async UniTask Finish(CancellationToken ct)
		{
			_model.IsFinished = true;
			await UniTask.Yield(ct);
		}
	}
}
