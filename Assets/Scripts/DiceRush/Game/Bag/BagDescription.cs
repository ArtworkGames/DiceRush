using StepanoffGames.DiceRush.Game.Map;
using System.Collections.Generic;

namespace StepanoffGames.DiceRush.Game.Bag
{
	public class BagDescription
	{
		public Dictionary<CellType, TokensSetDescription> Tokens;

		public BagDescription()
		{
			Tokens = new Dictionary<CellType, TokensSetDescription>();
			Tokens.Add(CellType.Reward, new TokensSetDescription());
			Tokens.Add(CellType.Enemy, new TokensSetDescription());
			Tokens.Add(CellType.MoveForward, new TokensSetDescription());
			Tokens.Add(CellType.MoveBackward, new TokensSetDescription());
			Tokens.Add(CellType.Portal, new TokensSetDescription());
		}
	}

	public class TokensSetDescription
	{
		public int RegularCount = 0;
		public int RemovedCount = 0;
		public int AddedCount = 0;
	}
}
