namespace StepanoffGames.DiceRush.Gameplay
{
	public class FlipTileCard : TileCard
	{
		override protected CellType GetNewTileType(CellType tileType, bool hasNearMoveBackwardCell)
		{
			return Level.Instance.Bag.TakenTile.BackType;
		}
	}
}
