using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Players
{
	public class AIPlayerController : PlayerController
	{
		public AIPlayerController(PlayerModel model, PlayerAvatar view) : base(model, view, null)
		{
		}

		override protected async UniTask<int> RollDice(bool isMoveForward)
		{
			//SetState(PlayerState.Waiting);
			//await UniTask.WaitForSeconds(1f);

			SetState(PlayerState.RollDice);

			await UniTask.WaitForSeconds(1.25f);
			_lastDiceValue = _diceController.GetValue(this);

			SetState(PlayerState.ConfirmDice);

			await UniTask.WaitForSeconds(1f);
			_lastDiceValue = await _deckController.ApplyDiceRoll(this, _lastDiceValue);

			return _lastDiceValue;
		}

		override protected async UniTask<int> SelectNextDirection(int diceValue, int cellsPassed)
		{
			SetState(PlayerState.SelectDirection);
			
			await UniTask.WaitForSeconds(1f);
			int direction = Random.Range(0, _avatar.CurrentPoint.NextPoints.Count);

			return direction;
		}

		override protected async UniTask<int> SelectPrevDirection(int diceValue, int cellsPassed)
		{
			SetState(PlayerState.SelectDirection);
			
			await UniTask.WaitForSeconds(1f);
			int direction = Random.Range(0, _avatar.CurrentPoint.PrevPoints.Count);

			return direction;
		}

		override protected async UniTask EndMove()
		{
			await UniTask.Yield();
		}

		override protected async UniTask BeforeWaitForCellToUnlock()
		{
			await UniTask.Yield();
		}

		override protected async UniTask<CellType> DrawToken()
		{
			//SetState(PlayerState.Waiting);
			//await UniTask.WaitForSeconds(1f);

			SetState(PlayerState.DrawToken);

			await UniTask.WaitForSeconds(1.25f);
			_lastCellType = _bagController.GetCellType(this);

			SetState(PlayerState.ConfirmToken);

			await UniTask.WaitForSeconds(1f);
			_lastCellType = await _deckController.ApplyTokenDraw(this, _lastCellType);

			return _lastCellType;
		}

		override protected async UniTask OpenChest()
		{
			SetState(PlayerState.OpenChest);

			await UniTask.WaitForSeconds(0.5f);
			_chestController.AddCards(this);
		}

		override protected async UniTask Battle()
		{
			SetState(PlayerState.Battle);
			await UniTask.Yield();
		}

		override protected async UniTask BeforeMoveToNextPortal(Cell portalCell)
		{
			SetState(PlayerState.MoveToPortal);
			await UniTask.Yield();
		}
	}
}
