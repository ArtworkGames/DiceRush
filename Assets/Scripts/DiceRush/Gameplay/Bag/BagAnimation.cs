using System;
using UnityEngine;

namespace StepanoffGames.DiceRush.Gameplay
{
	public class BagAnimation : MonoBehaviour
	{
		public Action OnShowTile;
		public Action OnAnimationFinished;

		[SerializeField] private Animator _animator;

		public void Take()
		{
			_animator.SetBool("Take", true);
			_animator.SetBool("Confirm", false);
		}

		public void Confirm()
		{
			_animator.SetBool("Confirm", true);
		}

		private void AnimationStarted()
		{
			_animator.SetBool("Take", false);
		}

		private void ShowTile()
		{
			OnShowTile?.Invoke();
		}

		private void AnimationFinished()
		{
			OnAnimationFinished?.Invoke();
		}
	}
}
