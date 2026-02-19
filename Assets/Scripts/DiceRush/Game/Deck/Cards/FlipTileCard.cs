using StepanoffGames.DiceRush.Game.Bag;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.Services;

namespace StepanoffGames.DiceRush.Game
{
	public class FlipTileCard : TileCard
	{
		override protected CellType GetNewTileType(PlayerController player, CellType tileType)
		{
			BagController bag = ServiceLocator.Get<BagController>();
			return bag.TakenTile.BackType;
		}
	}
}
