using Cysharp.Threading.Tasks;

namespace StepanoffGames.DiceRush.Gameplay
{
	public class TileCard : DeckCard
	{
		override public async UniTask<CellType> UseForTile(CellType tileType, bool hasNearMoveBackwardCell)
		{
			tileType = GetNewTileType(tileType, hasNearMoveBackwardCell);

			await UniTask.Yield();
			return tileType;
		}

		virtual protected CellType GetNewTileType(CellType tileType, bool hasNearMoveBackwardCell)
		{
			return tileType;
		}
	}
}
