using Cysharp.Threading.Tasks;
using DG.Tweening;
using StepanoffGames.DiceRush.Data.Models;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace StepanoffGames.DiceRush.Gameplay
{
	public class DeckCard : MonoBehaviour
	{
		public Action<DeckCard> OnSelect;

		[HideInInspector] public CardModel Model;

		private bool _canPress = true;

		private Tween moveTween;
		private Tween scaleTween;

		private void Start()
		{
			Button button = GetComponent<Button>();
			button.onClick.AddListener(OnButtonClick);
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
				.SetEase(Ease.InCubic);
		}

		public async UniTask ShowSelected()
		{
			_canPress = false;

			bool tweenCompleted = false;

			moveTween?.Kill();
			moveTween = transform.DOLocalMoveY(1080f, 1f)
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
				});

			await UniTask.WaitUntil(() => tweenCompleted);
		}

		virtual public async UniTask<int> UseForDice(int diceValue)
		{
			await UniTask.Yield();
			return diceValue;
		}

		virtual public async UniTask<CellType> UseForTile(CellType tileType, bool hasNearMoveBackwardCell)
		{
			await UniTask.Yield();
			return tileType;
		}
	}
}
