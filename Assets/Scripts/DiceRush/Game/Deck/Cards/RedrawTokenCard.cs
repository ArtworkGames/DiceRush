using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Bag;
using StepanoffGames.DiceRush.Game.Map;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.Services;

namespace StepanoffGames.DiceRush.Game.Deck.Cards
{
	public class RedrawTokenCard : Card
	{
		public RedrawTokenCard(CardModel model) : base(model)
		{
		}

		override public async UniTask<CellType> UseForToken(PlayerController player, CellType cellType)
		{
			BagController bag = ServiceLocator.Get<BagController>();
			bag.Confirm();
			await UniTask.NextFrame();

			player.SetState(PlayerState.DrawToken);
			cellType = await bag.Draw(player);
			player.SetState(PlayerState.ConfirmToken);

			return cellType;
		}

		override public CellType ApplyForToken(PlayerController player, CellType cellType)
		{
			BagController bag = ServiceLocator.Get<BagController>();
			cellType = bag.GetCellType(player);

			return cellType;
		}
	}
}
