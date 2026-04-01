using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Components.Deck
{
	public class ConfirmButton : TweenButton
	{
		public Action OnConfirm;

		[Space]
		[SerializeField] private HideablePanel _hideablePanel;

		override public void OnClick()
		{
			OnConfirm?.Invoke();
		}

		public async UniTask Show()
		{
			await _hideablePanel.Show();
		}

		public async UniTask Hide()
		{
			await _hideablePanel.Hide();
		}
	}
}
