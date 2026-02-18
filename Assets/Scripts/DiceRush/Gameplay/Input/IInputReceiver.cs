using UnityEngine;

namespace StepanoffGames.DiceRush.Gameplay
{
	public interface IInputReceiver
	{
		bool CanPress();
		void OnPress();
		void OnRelease();
		void OnClick();
	}
}
