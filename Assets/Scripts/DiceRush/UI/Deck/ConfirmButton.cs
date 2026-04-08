using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.UI.Components;
using System;
using System.Threading;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Deck
{
	public class ConfirmButton : TweenButton
	{
		public Action OnConfirm;

		[Space]
		[SerializeField] private HideablePanel _hideablePanel;

		override public void DoClick()
		{
			OnConfirm?.Invoke();
		}

		public async UniTask Show(CancellationToken ct)
		{
			await _hideablePanel.Show(false, ct);
		}

		public async UniTask Hide(CancellationToken ct)
		{
			await _hideablePanel.Hide(false, ct);
		}
	}
}
