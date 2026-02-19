using UnityEngine;

namespace StepanoffGames.DiceRush.Game
{
	public interface IInputReceiver
	{
		bool CanPress();
		void OnPress();
		void OnRelease();
		void OnClick();
	}
}
