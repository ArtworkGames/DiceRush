using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Bag;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.Services;

namespace StepanoffGames.DiceRush.Game
{
	public class RedealTileCard : TileCard
	{
		//override protected CellType GetNewTileType(PlayerController player, CellType tileType)
		//{
		//	BagController bag = ServiceLocator.Get<BagController>();
		//	return bag.GetTile(player).FrontType;
		//}

		override public async UniTask<CellType> UseForTile(PlayerController player, CellType tileType)
		{
			BagController bag = ServiceLocator.Get<BagController>();

			if (player.Model.Type == PlayerType.HI)
			{
				bag.Confirm();

				await UniTask.NextFrame();

				tileType = await bag.Deal(player);
			}
			else
			{
				tileType = bag.GetTile(player).FrontType;
			}

			return tileType;
		}
	}
}
