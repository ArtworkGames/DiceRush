using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.UI.Windows;

namespace StepanoffGames.DiceRush.UI.Windows.MergeCardsWindow
{
	public class MergeCardsWindowParams : BaseWindowParams
	{
		public PlayerController Player;
	}

	public class MergeCardsWindow : BaseWindow<MergeCardsWindowParams>
	{
		public static string PrefabName = "MergeCardsWindow";
	}
}
