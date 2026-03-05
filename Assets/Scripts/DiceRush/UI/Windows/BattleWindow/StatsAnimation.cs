using System;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Windows.BattleWindow
{
	public class StatsAnimation : MonoBehaviour
	{
		public Action OnEnemyAttack;
		public Action OnPlayerAttack;

		[SerializeField] private Animator _animator;

		public void ShowEnemyAttack()
		{
			_animator.SetBool("EnemyAttack", true);
		}

		public void ShowPlayerAttack()
		{
			_animator.SetBool("PlayerAttack", true);
		}

		private void EnemyAttack()
		{
			_animator.SetBool("EnemyAttack", false);
			OnEnemyAttack?.Invoke();
		}

		private void PlayerAttack()
		{
			_animator.SetBool("PlayerAttack", false);
			OnPlayerAttack?.Invoke();
		}
	}
}
