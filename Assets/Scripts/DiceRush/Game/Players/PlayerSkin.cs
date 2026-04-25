using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Players
{
	public class PlayerSkin : MonoBehaviour
	{
		[SerializeField] private Animator _animator;

		private void Start()
		{
		}

		public void ShowIdle()
		{
			_animator.SetBool("Run", false);
		}

		public void ShowRun()
		{
			_animator.SetBool("Run", true);
		}
	}
}
