using System;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Components.Dice
{
	public class DiceAnimation : MonoBehaviour
	{
		public Action OnShowValue;
		public Action OnAnimationFinished;

		[SerializeField] private Animator _animator;
		[SerializeField] private GameObject[] _numbers;

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

		public void SetValue(int value)
		{
			for (int i = 0; i < _numbers.Length; i++)
			{
				_numbers[i].SetActive((i + 1) == value);
			}
		}

		private void AnimationFinished()
		{
			OnAnimationFinished?.Invoke();
		}
	}
}
