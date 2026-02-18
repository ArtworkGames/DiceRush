namespace StepanoffGames.DiceRush.Gameplay
{
	public class RedealTileCard : TileCard
	{
		override protected CellType GetNewTileType(CellType tileType, bool hasNearMoveBackwardCell)
		{
			Level.Instance.Bag.ReturnTile();
			return Level.Instance.Bag.GetTile(hasNearMoveBackwardCell).FrontType;
		}
	}
}
