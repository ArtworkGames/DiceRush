using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.Signals;

namespace StepanoffGames.DiceRush.Game.Ranking.Signals
{
	public class PlayerPlaceChangedSignal : BaseSignal
	{
		public PlayerController Player;
		public int PrevPlace;
		public int Place;

		public PlayerPlaceChangedSignal(PlayerController player, int prevPlace, int place)
		{
			Player = player;
			PrevPlace = prevPlace;
			Place = place;
		}
	}
}
