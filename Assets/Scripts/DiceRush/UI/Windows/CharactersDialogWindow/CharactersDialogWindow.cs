using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.UI.Components;
using StepanoffGames.UI.Windows;
using StepanoffGames.UI.Windows.Animators;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StepanoffGames.DiceRush.UI.Windows.CharactersDialogWindow
{
	public class CharactersDialogWindowParams : BaseWindowParams
	{
		public CharacterPhrase[] Phrases;
		public Action OnClose;
	}

	public enum CharacterClearFlag
	{
		None,
		HideAll,
		HideLeft,
		HideRight
	}

	public enum CharacterSide
	{
		Left,
		Right
	}

	public enum CharacterPhraseButtonType
	{
		Next,
		Play
	}

	public class CharacterPhrase
	{
		public CharacterClearFlag ClearFlag = CharacterClearFlag.None;
		public CharacterSide Side;
		public string AvatarName;
		public string Name;
		public string PhraseKey;
		public string[] PhraseParams;
		public CharacterPhraseButtonType ButtonType = CharacterPhraseButtonType.Next;
	}

	public class CharactersDialogWindow : BaseWindow<CharactersDialogWindowParams>
	{
		public static string PrefabName = "CharactersDialogWindow";

		[Space]
		[SerializeField] private BaseWindowAnimator _fadeAnimator;
		[Space]
		[SerializeField] private CharacterPanel _firstLeftCharacterPanel;
		[SerializeField] private CharacterPanel _secondLeftCharacterPanel;
		[SerializeField] private CharacterPanel _rightCharacterPanel;
		[Space]
		[SerializeField] private PhrasePanel _phrasePanel;
		[SerializeField] private TweenButton _nextButton;
		[SerializeField] private TweenButton _playButton;

		private bool waitForNextButtonClick = false;
		private bool nextButtonClicked = false;

		private CancellationTokenSource cts;

		private void OnDestroy()
		{
			cts?.Cancel();
			cts?.Dispose();
			cts = null;
		}

		protected override async void AfterOpen()
		{
			_fadeAnimator.OpenAsync().Forget();

			_nextButton.OnClick += OnNextButtonClick;
			_playButton.OnClick += OnNextButtonClick;

			cts?.Cancel();
			cts?.Dispose();
			cts = new CancellationTokenSource();

			await UniTask.NextFrame(cts.Token);

			for (int i = 0; i < Params.Phrases.Length; i++)
			{
				await ShowPhrase(Params.Phrases[i], cts.Token);
			}

			await _phrasePanel.Hide(false, cts.Token);
			List<UniTask> tasks = new();
			if (_firstLeftCharacterPanel.IsShown) tasks.Add(_firstLeftCharacterPanel.Hide(false, cts.Token));
			if (_secondLeftCharacterPanel.IsShown) tasks.Add(_secondLeftCharacterPanel.Hide(false, cts.Token));
			if (_rightCharacterPanel.IsShown) tasks.Add(_rightCharacterPanel.Hide(false, cts.Token));
			await UniTask.WhenAll(tasks);

			await _fadeAnimator.CloseAsync();

			CloseWindow();
		}

		protected override void BeforeClose()
		{
			_nextButton.OnClick -= OnNextButtonClick;
			_playButton.OnClick -= OnNextButtonClick;
		}

		protected override void AfterClose()
		{
			Params.OnClose?.Invoke();
		}

		private async UniTask ShowPhrase(CharacterPhrase phrase, CancellationToken ct)
		{
			if (_phrasePanel.IsShown) await _phrasePanel.Hide(false, ct);

			if (phrase.ClearFlag == CharacterClearFlag.HideAll)
			{
				List<UniTask> tasks = new();
				if (_firstLeftCharacterPanel.IsShown) tasks.Add(_firstLeftCharacterPanel.Hide(false, ct));
				if (_secondLeftCharacterPanel.IsShown) tasks.Add(_secondLeftCharacterPanel.Hide(false, ct));
				if (_rightCharacterPanel.IsShown) tasks.Add(_rightCharacterPanel.Hide(false, ct));
				await UniTask.WhenAll(tasks);
			}
			else if (phrase.ClearFlag == CharacterClearFlag.HideLeft)
			{
				List<UniTask> tasks = new();
				if (_firstLeftCharacterPanel.IsShown) tasks.Add(_firstLeftCharacterPanel.Hide(false, ct));
				if (_secondLeftCharacterPanel.IsShown) tasks.Add(_secondLeftCharacterPanel.Hide(false, ct));
				await UniTask.WhenAll(tasks);
			}
			else if (phrase.ClearFlag == CharacterClearFlag.HideRight)
			{
				if (_rightCharacterPanel.IsShown) await _rightCharacterPanel.Hide(false, ct);
			}

			if (phrase.Side == CharacterSide.Left)
			{
				if (!_firstLeftCharacterPanel.IsShown)
				{
					await _firstLeftCharacterPanel.Show(phrase.AvatarName, false, ct);
				}
				else
				{
					if (_firstLeftCharacterPanel.AvatarName == phrase.AvatarName)
					{
						if (_firstLeftCharacterPanel.IsOnBack)
						{
							List<UniTask> tasks = new();
							tasks.Add(_firstLeftCharacterPanel.MoveToFront(false, ct));
							tasks.Add(_secondLeftCharacterPanel.MoveToBack(false, ct));
							await UniTask.WhenAll(tasks);
						}
					}
					else
					{
						if (!_secondLeftCharacterPanel.IsShown)
						{
							List<UniTask> tasks = new();
							tasks.Add(_firstLeftCharacterPanel.MoveToBack(false, ct));
							tasks.Add(_secondLeftCharacterPanel.Show(phrase.AvatarName, false, ct));
							await UniTask.WhenAll(tasks);
						}
						else if (_secondLeftCharacterPanel.AvatarName == phrase.AvatarName)
						{
							if (_secondLeftCharacterPanel.IsOnBack)
							{
								List<UniTask> tasks = new();
								tasks.Add(_firstLeftCharacterPanel.MoveToBack(false, ct));
								tasks.Add(_secondLeftCharacterPanel.MoveToFront(false, ct));
								await UniTask.WhenAll(tasks);
							}
						}
						else
						{
							if (_firstLeftCharacterPanel.IsOnBack)
							{
								List<UniTask> tasks = new();
								tasks.Add(_firstLeftCharacterPanel.Hide(false, ct));
								tasks.Add(_secondLeftCharacterPanel.MoveToBack(false, ct));
								await UniTask.WhenAll(tasks);
								await _firstLeftCharacterPanel.Show(phrase.AvatarName, false, ct);
							}
							else
							{
								List<UniTask> tasks = new();
								tasks.Add(_firstLeftCharacterPanel.MoveToBack(false, ct));
								tasks.Add(_secondLeftCharacterPanel.Hide(false, ct));
								await UniTask.WhenAll(tasks);
								await _secondLeftCharacterPanel.Show(phrase.AvatarName, false, ct);
							}
						}
					}
				}
			}
			else
			{
				if (!_rightCharacterPanel.IsShown)
				{
					await _rightCharacterPanel.Show(phrase.AvatarName, false, ct);
				}
				else
				{
					if (_rightCharacterPanel.AvatarName != phrase.AvatarName)
					{
						await _rightCharacterPanel.Hide(false, ct);
						await _rightCharacterPanel.Show(phrase.AvatarName, false, ct);
					}
				}
			}

			await _phrasePanel.Show(phrase, false, ct);

			waitForNextButtonClick = true;
			nextButtonClicked = false;
			await UniTask.WaitUntil(() => nextButtonClicked, cancellationToken: ct);
			waitForNextButtonClick = false;
			nextButtonClicked = false;
		}

		private void Update()
		{
			if (waitForNextButtonClick)
			{
				if (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame)
				{
					waitForNextButtonClick = false;
					OnNextButtonClick();
				}
			}
		}

		private void OnNextButtonClick()
		{
			nextButtonClicked = true;
		}
	}
}
