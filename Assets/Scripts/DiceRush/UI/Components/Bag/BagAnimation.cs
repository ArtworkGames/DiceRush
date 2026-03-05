using StepanoffGames.DiceRush.Game;
using System;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Components.Bag
{
	public class BagAnimation : MonoBehaviour
	{
		public Action OnShowToken;
		public Action OnAnimationFinished;

		[SerializeField] private Animator _animator;
		[SerializeField] private BagToken _handToken;
		[SerializeField] private BagToken _token;

		public void Draw()
		{
			_animator.SetBool("Draw", true);
			_animator.SetBool("Confirm", false);
		}

		public void Confirm()
		{
			_animator.SetBool("Confirm", true);
		}

		private void AnimationStarted()
		{
			_animator.SetBool("Draw", false);
		}

		private void ShowToken()
		{
			OnShowToken?.Invoke();
		}

		public void SetCellType(CellType type)
		{
			_handToken.UpdateView(type);
			_token.UpdateView(type);
		}

		private void AnimationFinished()
		{
			OnAnimationFinished?.Invoke();
		}
	}
}
