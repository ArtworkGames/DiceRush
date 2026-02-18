using Cysharp.Threading.Tasks;
using UnityEngine;

namespace StepanoffGames.UI.Windows.Animators
{
	public abstract class BaseWindowAnimator : MonoBehaviour
	{
		public virtual void Reset()
		{
		}

		protected virtual void OnEnable()
		{
			Reset();
		}

		protected virtual void OnDisable()
		{
			Reset();
		}

		public async UniTask OpenAsync(bool immediately = false)
		{
			await ShowOpenAnimationAsync(immediately);
		}

		public async UniTask CloseAsync(bool immediately = false)
		{
			await ShowCloseAnimationAsync(immediately);
		}

		protected abstract UniTask ShowOpenAnimationAsync(bool immediately = false);

		protected abstract UniTask ShowCloseAnimationAsync(bool immediately = false);
	}
}