using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Players
{
	public class AIPlayerController : PlayerController
	{
		public AIPlayerController(PlayerModel model, PlayerAvatar view) : base(model, view)
		{
		}

		override protected async UniTask<int> RollDice(CellType cellType)
		{
			int diceValue = _dice.GetValue(this);
			diceValue = await _deck.ApplyDiceRoll(this, diceValue);

			return diceValue;
		}

		override protected async UniTask<int> SelectNextDirection(int diceValue, int cellsPassed)
		{
			await UniTask.WaitForSeconds(1f);

			int direction = Random.Range(0, _avatar.CurrentPoint.NextPoints.Count);
			return direction;
		}

		override protected async UniTask<int> SelectPrevDirection(int diceValue, int cellsPassed)
		{
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
			CellType cellType = _bag.GetCellType(this);
			cellType = await _deck.ApplyTokenDraw(this, cellType);

			return cellType;
		}

		override protected async UniTask OpenChest()
		{
			await UniTask.Yield();

			_chest.AddCards(this);
		}

		override protected async UniTask BeforeMoveToNextPortal(Cell portalCell)
		{
			await UniTask.Yield();
		}
	}
}
