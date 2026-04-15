using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Map;
using StepanoffGames.DiceRush.Game.Players.AI;
using System.Threading;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Players
{
	public class AIPlayerController : PlayerController
	{
		public AIPlayerBrain Brain => _brain;
		private AIPlayerBrain _brain;

		public AIPlayerController(PlayerModel model, PlayerAvatar view) : base(model, view, null)
		{
			_brain = new AIPlayerBrain(this);
		}

		override protected async UniTask<int> RollDice(bool isMoveForward, CancellationToken ct)
		{
			//SetState(PlayerState.Waiting);
			//await UniTask.WaitForSeconds(1f);

			SetState(PlayerState.RollDice);

			await UniTask.WaitForSeconds(2f, cancellationToken: ct);
			_lastDiceValue = _diceController.GetValue(this);

			SetState(PlayerState.ConfirmDice);

			await UniTask.WaitForSeconds(1f, cancellationToken: ct);
			_lastDiceValue = await _deckController.ApplyDiceRoll(this, _lastDiceValue, ct);

			return _lastDiceValue;
		}

		override protected async UniTask<int> SelectNextDirection(int diceValue, int cellsPassed, CancellationToken ct)
		{
			SetState(PlayerState.SelectDirection);
			
			await UniTask.WaitForSeconds(1f, cancellationToken: ct);
			//int direction = Random.Range(0, _avatar.CurrentPoint.NextPoints.Count);
			int direction = _brain.SelectDirection(diceValue, cellsPassed, true);

			return direction;
		}

		override protected async UniTask<int> SelectPrevDirection(int diceValue, int cellsPassed, CancellationToken ct)
		{
			SetState(PlayerState.SelectDirection);
			
			await UniTask.WaitForSeconds(1f, cancellationToken: ct);
			//int direction = Random.Range(0, _avatar.CurrentPoint.PrevPoints.Count);
			int direction = _brain.SelectDirection(diceValue, cellsPassed, false);

			return direction;
		}

		override protected async UniTask EndMove(CancellationToken ct)
		{
			await UniTask.Yield(ct);
		}

		override protected async UniTask BeforeWaitForCellToUnlock(CancellationToken ct)
		{
			await UniTask.Yield(ct);
		}

		override protected async UniTask<CellType> DrawToken(CancellationToken ct)
		{
			//SetState(PlayerState.Waiting);
			//await UniTask.WaitForSeconds(1f);

			SetState(PlayerState.DrawToken);

			await UniTask.WaitForSeconds(1.25f, cancellationToken: ct);
			_lastCellType = _bagController.GetCellType(this);

			SetState(PlayerState.ConfirmToken);

			await UniTask.WaitForSeconds(1f, cancellationToken: ct);
			_lastCellType = await _deckController.ApplyTokenDraw(this, _lastCellType, ct);

			return _lastCellType;
		}

		override protected async UniTask OpenChest(CancellationToken ct)
		{
			SetState(PlayerState.OpenChest);

			await UniTask.WaitForSeconds(0.5f, cancellationToken: ct);
			_chestController.AddCards(this);
		}

		override protected async UniTask Battle(CancellationToken ct)
		{
			SetState(PlayerState.Battle);
			await UniTask.Yield(ct);
		}

		override protected async UniTask BeforeMoveToNextPortal(Cell portalCell, CancellationToken ct)
		{
			SetState(PlayerState.MoveToPortal);
			await UniTask.Yield(ct);
		}
	}
}
