using System;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Windows.BattleWindow
{
	public class CharacterAnimation : MonoBehaviour
	{
		public Action OnAttack;
		public Action OnDamage;
		public Action OnDeath;

		[SerializeField] private Animator _animator;

		public void ShowAttack()
		{
			_animator.SetBool("Attack", true);
		}

		public void ShowDamage()
		{
			_animator.SetBool("Damage", true);
		}

		public void ShowDeath()
		{
			_animator.SetBool("Death", true);
		}

		private void Attack()
		{
			_animator.SetBool("Attack", false);
			OnAttack?.Invoke();
		}

		private void Damage()
		{
			_animator.SetBool("Damage", false);
			OnDamage?.Invoke();
		}

		private void Death()
		{
			OnDeath?.Invoke();
		}
	}
}
