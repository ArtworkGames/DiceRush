using Cysharp.Threading.Tasks;
using DG.Tweening;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.UI.Components;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace StepanoffGames.DiceRush.UI.Components.Deck
{
	public class DeckPanelCard : MonoBehaviour
	{
		public Action<DeckPanelCard> OnSelect;

		[SerializeField] private TextLocalizer _textLocalizer;

		[HideInInspector] public CardModel Model;

		private bool _canPress = true;

		private Tween moveTween;
		private Tween scaleTween;

		private void Start()
		{
			string key = $"Card:{Model.Type}";
			_textLocalizer.Localize(key);

			Button button = GetComponent<Button>();
			button.onClick.AddListener(OnButtonClick);
		}

		private void OnDestroy()
		{
			OnSelect = null;
			Model = null;

			moveTween?.Kill();
			scaleTween?.Kill();
		}

		public bool CanPress()
		{
			return _canPress;
		}

		public void OnButtonClick()
		{
			OnSelect?.Invoke(this);
		}

		public void Show()
		{
			moveTween?.Kill();
			moveTween = transform.DOLocalMoveY(300f, 0.33f)
				.SetEase(Ease.OutBack);
		}

		public void Hide()
		{
			_canPress = false;

			moveTween?.Kill();
			moveTween = transform.DOLocalMoveY(-350f, 0.2f)
				.SetEase(Ease.InCubic)
				.OnComplete(() =>
				{
					Destroy(gameObject);
				});
		}

		public async UniTask ShowSelected()
		{
			_canPress = false;

			bool tweenCompleted = false;

			moveTween?.Kill();
			moveTween = transform.DOLocalMoveY(1080f, 0.5f)
				.SetEase(Ease.InOutCubic)
				.OnComplete(() =>
				{
					tweenCompleted = true;
				});

			await UniTask.WaitUntil(() => tweenCompleted);
		}

		public async UniTask HideSelected()
		{
			bool tweenCompleted = false;

			scaleTween?.Kill();
			scaleTween = transform.DOScale(0f, 0.5f)
				.SetEase(Ease.OutCubic)
				.OnComplete(() =>
				{
					tweenCompleted = true;
					Destroy(gameObject);
				});

			await UniTask.WaitUntil(() => tweenCompleted);
		}
	}
}
