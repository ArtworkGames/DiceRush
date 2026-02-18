using Cysharp.Threading.Tasks;
using UnityEngine;

namespace StepanoffGames.UI.Popups.Animators
{
	public abstract class BasePopupAnimator : MonoBehaviour
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
			await PlayOpenAnimationAsync(immediately);
		}

		public async UniTask CloseAsync(bool immediately = false)
		{
			await PlayCloseAnimationAsync(immediately);
		}

		protected abstract UniTask PlayOpenAnimationAsync(bool immediately = false);

		protected abstract UniTask PlayCloseAnimationAsync(bool immediately = false);
	}
}
