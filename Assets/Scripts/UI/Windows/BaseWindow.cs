using Cysharp.Threading.Tasks;
using StepanoffGames.Signals;
using StepanoffGames.UI.Windows.Animators;
using StepanoffGames.UI.Windows.Signals;
using UnityEngine;

namespace StepanoffGames.UI.Windows
{
	public abstract class BaseWindow : MonoBehaviour
	{
		[SerializeField] private CanvasGroup _touchMask;
		[SerializeField] private CanvasGroup _content;
		[SerializeField] private BaseWindowAnimator[] _animators;

		public CanvasGroup Content => _content;

		public abstract void SetParams(BaseWindowParams @params);

		protected abstract void ClearParams();

		public async UniTask OpenAsync(bool immediately = false)
		{
			if (_touchMask != null) _touchMask.interactable = true;
			_content.interactable = false;
			BeforeOpen();
			gameObject.SetActive(true);

			await ShowOpenAnimationAsync(immediately);

			AfterOpen();
			_content.interactable = true;
		}

		public async UniTask CloseAsync(bool immediately = false)
		{
			_content.interactable = false;
			BeforeClose();

			await ShowCloseAnimationAsync(immediately);

			gameObject.SetActive(false);
			AfterClose();
			ClearParams();
		}

		public async UniTask ShowAsync(bool immediately = false)
		{
			gameObject.SetActive(true);
			_content.interactable = false;

			await ShowOpenAnimationAsync(immediately);

			_content.interactable = true;
		}

		public async UniTask HideAsync(bool immediately = false)
		{
			_content.interactable = false;

			await ShowCloseAnimationAsync(immediately);

			gameObject.SetActive(false);
		}

		private async UniTask ShowOpenAnimationAsync(bool immediately = false)
		{
			if (_animators != null && _animators.Length > 0)
			{
				await UniTask.WhenAll(_animators.Select(x => x.OpenAsync(immediately)));
			}
		}

		private async UniTask ShowCloseAnimationAsync(bool immediately = false)
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

		public virtual void CloseWindow(bool immediately = false)
		{
			SignalBus.Publish(new CloseWindowSignal(gameObject.name, immediately));
		}

		protected virtual void BeforeOpen() { }

		protected virtual void AfterOpen() { }

		protected virtual void BeforeClose() { }

		protected virtual void AfterClose() { }
	}

	public abstract class BaseWindow<T> : BaseWindow where T : BaseWindowParams, new()
	{
		public T Params { get; private set; }

		public override void SetParams(BaseWindowParams @params)
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
