using System;
using UnityEngine;

namespace StepanoffGames.DiceRush.Gameplay
{
	public class DiceAnimation : MonoBehaviour
	{
		public Action OnShowValue;
		public Action OnAnimationFinished;

		[SerializeField] private Animator _animator;

		public void Roll()
		{
			_animator.SetBool("Roll", true);
			_animator.SetBool("Confirm", false);
		}

		public void Confirm()
		{
			_animator.SetBool("Confirm", true);
		}

		private void AnimationStarted()
		{
			_animator.SetBool("Roll", false);
		}

		private void ShowValue()
		{
			OnShowValue?.Invoke();
		}

		private void AnimationFinished()
		{
			OnAnimationFinished?.Invoke();
		}
	}
}
