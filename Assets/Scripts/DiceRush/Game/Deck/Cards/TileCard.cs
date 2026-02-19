using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Game.Players;

namespace StepanoffGames.DiceRush.Game
{
	public class TileCard : DeckCard
	{
		override public async UniTask<CellType> UseForTile(PlayerController player, CellType tileType)
		{
			tileType = GetNewTileType(player, tileType);

			await UniTask.Yield();
			return tileType;
		}

		virtual protected CellType GetNewTileType(PlayerController player, CellType tileType)
		{
			return tileType;
		}
	}
}
