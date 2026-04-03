using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Bag;
using StepanoffGames.DiceRush.Game.Map;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.Services;

namespace StepanoffGames.DiceRush.Game.Deck.Cards
{
	public class ReplaceTokenWithEnemyCard : Card
	{
		public ReplaceTokenWithEnemyCard(CardModel model) : base(model)
		{
		}

		override public async UniTask<CellType> UseForToken(PlayerController player, CellType cellType)
		{
			BagController bag = ServiceLocator.Get<BagController>();

			cellType = CellType.Enemy;

			bag.ShowToken(cellType);
			//bag.Confirm();

			await UniTask.NextFrame();

			return cellType;
		}

		override public CellType ApplyForToken(PlayerController player, CellType cellType)
		{
			BagController bag = ServiceLocator.Get<BagController>();
			cellType = CellType.Enemy;

			return cellType;
		}
	}
}
