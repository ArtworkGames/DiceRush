using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Map;
using StepanoffGames.DiceRush.Game.Path;
using StepanoffGames.Services;
using System.Threading;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Players
{
	public class HIPlayerController : PlayerController
	{
		protected PathController _pathController;

		public HIPlayerController(PlayerModel model, PlayerAvatar avatar, PlayerController prevPlayer) : base(model, avatar, prevPlayer)
		{
			_pathController = ServiceLocator.Get<PathController>();
		}

		public override void Destroy()
		{
			base.Destroy();

			_pathController = null;
		}

		override protected async UniTask<int> RollDice(bool isMoveForward, CancellationToken ct)
		{
			//SetState(PlayerState.Waiting);
			SetState(PlayerState.RollDice);

			if (!isMoveForward)
			{
				_pathController.ShowMarkersInBackOfPlayer(_avatar);
			}
			else
			{
				_pathController.ShowMarkersInFrontOfPlayer(_avatar);
			}
			//await _levelManager.Camera.FocusOnPathMarkers(_avatar, ct);
			_gameManager.Camera.FocusOnPathMarkers(_avatar, ct).Forget();

			_lastDiceValue = await _diceController.Roll(this, ct);

			if (!isMoveForward)
			{
				_pathController.ShowDiceValueInBackOfPlayer(_avatar, _lastDiceValue);
			}
			else
			{
				_pathController.ShowDiceValueInFrontOfPlayer(_avatar, _lastDiceValue);
			}
			//_levelManager.Camera.FocusOnPathMarkers(_avatar, ct).Forget();

			SetState(PlayerState.ConfirmDice);

			int oldDiceValue = _lastDiceValue;
			_lastDiceValue = await _deckController.ConfirmDiceRoll(this, _lastDiceValue, ct);

			if (_lastDiceValue != oldDiceValue)
			{
				//_dice.ShowValue(diceValue);

				if (!isMoveForward)
				{
					_pathController.ShowDiceValueInBackOfPlayer(_avatar, _lastDiceValue);
				}
				else
				{
					_pathController.ShowDiceValueInFrontOfPlayer(_avatar, _lastDiceValue);
				}
				//_levelManager.Camera.FocusOnPathMarkers(_avatar, ct).Forget();
			}
			_diceController.Confirm();

			return _lastDiceValue;
		}

		override protected async UniTask<int> SelectNextDirection(int diceValue, int cellsPassed, CancellationToken ct)
		{
			SetState(PlayerState.SelectDirection);
			Debug.Log($"[HIPlayerController] SelectNextDirection: diceValue = {diceValue}, cellsPassed = {cellsPassed}");

			//await Game.Instance.Camera.FocusOnWayMarkers(_view, ct);
			_gameManager.Camera.FocusOnPathMarkers(_avatar, ct).Forget();

			int directionIndex = await _forkController.SelectNextDirection(_avatar.CurrentPoint, _avatar, ct);
			
			_pathController.ShowDiceValueInFrontOfPlayer(_avatar, diceValue - cellsPassed, directionIndex);

			return directionIndex;
		}

		override protected async UniTask<int> SelectPrevDirection(int diceValue, int cellsPassed, CancellationToken ct)
		{
			SetState(PlayerState.SelectDirection);
			Debug.Log($"[HIPlayerController] SelectPrevDirection: diceValue = {diceValue}, cellsPassed = {cellsPassed}");

			//await Game.Instance.Camera.FocusOnWayMarkers(_view, ct);
			_gameManager.Camera.FocusOnPathMarkers(_avatar, ct).Forget();

			int directionIndex = await _forkController.SelectPrevDirection(_avatar.CurrentPoint, _avatar, ct);
			
			_pathController.ShowDiceValueInBackOfPlayer(_avatar, diceValue - cellsPassed, directionIndex);

			return directionIndex;
		}

		override protected async UniTask EndMove(CancellationToken ct)
		{
			await UniTask.Yield(ct);
			_pathController.HideMarkers();
		}

		override protected async UniTask BeforeWaitForCellToUnlock(CancellationToken ct)
		{
			await _gameManager.Camera.FocusOnPlayer(_avatar, ct);
		}

		override protected async UniTask<CellType> DrawToken(CancellationToken ct)
		{
			//SetState(PlayerState.Waiting);
			SetState(PlayerState.DrawToken);

			//await _levelManager.Camera.FocusOnPlayer(_avatar, ct);
			_gameManager.Camera.FocusOnPlayer(_avatar, ct).Forget();

			_lastCellType = await _bagController.Draw(this, ct);

			SetState(PlayerState.ConfirmToken);
			
			CellType oldCellType = _lastCellType;
			_lastCellType = await _deckController.ConfirmTokenDraw(this, _lastCellType, ct);

			if (_lastCellType != oldCellType)
			{
				_bagController.ShowToken(_lastCellType);
			}

			_bagController.Confirm();

			return _lastCellType;
		}

		override protected async UniTask OpenChest(CancellationToken ct)
		{
			SetState(PlayerState.OpenChest);
			await _chestController.Open(this, ct);
		}

		override protected async UniTask Battle(CancellationToken ct)
		{
			SetState(PlayerState.Battle);
			await _battleController.Fight(this, ct);
		}

		override protected async UniTask BeforeMoveToNextPortal(Cell portalCell, CancellationToken ct)
		{
			SetState(PlayerState.MoveToPortal);
			await _gameManager.Camera.FocusOnCell(portalCell, ct);

			//await UniTask.WaitForSeconds(0.5f);
		}
	}
}
