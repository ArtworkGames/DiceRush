using System;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Bag
{
	public class BagAnimation : MonoBehaviour
	{
		public Action OnShowTile;
		public Action OnAnimationFinished;

		[SerializeField] private Animator _animator;
		[SerializeField] private BagTile _handTile;
		[SerializeField] private BagTile _tile;

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

		public void SetTile(CellType type)
		{
			_handTile.UpdateView(type);
			_tile.UpdateView(type);
		}

		private void AnimationFinished()
		{
			OnAnimationFinished?.Invoke();
		}
	}
}
