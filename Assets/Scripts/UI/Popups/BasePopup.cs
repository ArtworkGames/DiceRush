using Cysharp.Threading.Tasks;
using StepanoffGames.Signals;
using StepanoffGames.UI.Popups.Animators;
using StepanoffGames.UI.Popups.Signals;
using UnityEngine;

namespace StepanoffGames.UI.Popups
{
	public abstract class BasePopup : MonoBehaviour
	{
		[SerializeField] private CanvasGroup _content;
		[SerializeField] private BasePopupAnimator[] _animators;

		public CanvasGroup Content => _content;

		public abstract void SetParams(BasePopupParams @params);

		protected abstract void ClearParams();

		public async UniTask OpenAsync(bool immediately = false)
		{
			_content.interactable = false;
			BeforeOpen();
			gameObject.SetActive(true);

			await PLayOpenAnimationAsync(immediately);

			AfterOpen();
			_content.interactable = true;
		}

		public async UniTask CloseAsync(bool immediately = false)
		{
			_content.interactable = false;
			BeforeClose();

			await PlayCloseAnimationAsync(immediately);

			gameObject.SetActive(false);
			AfterClose();
			ClearParams();
		}

		private async UniTask PLayOpenAnimationAsync(bool immediately = false)
		{
			if (_animators != null && _animators.Length > 0)
			{
				await UniTask.WhenAll(_animators.Select(x => x.OpenAsync(immediately)));
			}
		}

		private async UniTask PlayCloseAnimationAsync(bool immediately = false)
		{
			if (_animators != null && _animators.Length > 0)
			{
				await UniTask.WhenAll(_animators.Select(x => x.CloseAsync(immediately)));

				for (int i = 0; i < _animators.Length; i++)
				{
					_animators[i].Reset();
				}
			}
		}

		public virtual void ClosePopup(bool immediately = false)
		{
			SignalBus.Publish(new ClosePopupSignal(this, immediately));
		}

		protected virtual void BeforeOpen() { }

		protected virtual void AfterOpen() { }

		protected virtual void BeforeClose() { }

		protected virtual void AfterClose() { }
	}

	public abstract class BasePopup<T> : BasePopup where T : BasePopupParams, new()
	{
		public T Params { get; private set; }

		public override void SetParams(BasePopupParams @params)
		{
			if (@params is T)
			{
				Params = (T)@params;
			}
			else
			{
				Params = new T();
				Params.CopyBaseParams(@params);
			}
		}

		protected override void ClearParams()
		{
			Params = null;
		}
	}
}
