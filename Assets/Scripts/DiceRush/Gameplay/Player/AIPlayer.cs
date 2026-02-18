using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using UnityEngine;

namespace StepanoffGames.DiceRush.Gameplay
{
	public class AIPlayer : Player
	{
		public AIPlayer(PlayerModel model, Avatar view) : base(model, view)
		{
		}

		override protected async UniTask<int> RollDice(CellType cellType)
		{
			//await UniTask.Yield();
			await UniTask.WaitForSeconds(2f);

			int diceValue = Level.Instance.Dice.GetValue();
			return diceValue;
		}

		//override protected async UniTask<int> ConfirmDiceRoll(int diceValue)
		//{
		//	await UniTask.Yield();
		//	return diceValue;
		//}

		override protected async UniTask AfterMoveToCell(CellType cellType)
		{
			await UniTask.Yield();
		}

		override protected async UniTask<int> SelectNextDirection()
		{
			//await UniTask.Yield();
			await UniTask.WaitForSeconds(1f);

			int direction = Random.Range(0, _avatar.CurrentPoint.NextPoints.Count);
			return direction;
		}

		override protected async UniTask<int> SelectPrevDirection()
		{
			//await UniTask.Yield();
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

		override protected async UniTask<CellType> TakeTile()
		{
			//await UniTask.Yield();
			await UniTask.WaitForSeconds(2f);

			bool hasNearMoveBackwardCell = ((Cell)_avatar.CurrentPoint).HasNearCellWithSameType(CellType.MoveBackward);
			CellType tileType = Level.Instance.Bag.GetTile(hasNearMoveBackwardCell).FrontType;
			return tileType;
		}

		//override protected async UniTask<CellType> ConfirmTileTake(CellType tileType, bool hasNearMoveBackwardCell)
		//{
		//	await UniTask.Yield();
		//	return tileType;
		//}

		override protected async UniTask BeforeMoveToNextPortal(Cell portalCell)
		{
			//await UniTask.Yield();
			await UniTask.WaitForSeconds(1f);
		}
	}
}
