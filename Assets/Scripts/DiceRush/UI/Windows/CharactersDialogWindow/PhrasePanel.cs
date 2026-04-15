using Cysharp.Threading.Tasks;
using DG.Tweening;
using StepanoffGames.UI.Components;
using System.Threading;
using TMPro;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Windows.CharactersDialogWindow
{
	public class PhrasePanel : MonoBehaviour
	{
		[SerializeField] private CanvasGroup _canvasGroup;
		[SerializeField] private bool _initHidden;
		[Space]
		[SerializeField] private GameObject _leftNamePanel;
		[SerializeField] private TMP_Text _leftNameText;
		[Space]
		[SerializeField] private GameObject _rightNamePanel;
		[SerializeField] private TMP_Text _rightNameText;
		[Space]
		[SerializeField] private TMPTextLocalizer _phraseTextLocalizer;
		[Space]
		[SerializeField] private GameObject _nextButtonPanel;
		[SerializeField] private GameObject _playButtonPanel;

		public bool IsShown => _isShown;
		private bool _isShown;

		private Tween fadeTween;
		private bool isDestroyed;

		private void Start()
		{
			_isShown = true;
			if (_initHidden)
			{
				_isShown = false;
				_canvasGroup.interactable = false;
				_canvasGroup.blocksRaycasts = false;
				_canvasGroup.alpha = 0f;
			}
		}

		private void OnDestroy()
		{
			fadeTween?.Kill();
			isDestroyed = true;
		}

		public async UniTask Show(CharacterPhrase phrase, bool immediately, CancellationToken ct)
		{
			_isShown = true;
			fadeTween?.Kill();

			if (phrase.Side == CharacterSide.Left)
			{
				_leftNamePanel.SetActive(true);
				_rightNamePanel.SetActive(false);
				_leftNameText.text = phrase.Name;
			}
			else
			{
				_leftNamePanel.SetActive(false);
				_rightNamePanel.SetActive(true);
				_rightNameText.text = phrase.Name;
			}
			_phraseTextLocalizer.Localize(phrase.PhraseKey, phrase.PhraseParams);
			_nextButtonPanel.SetActive(phrase.ButtonType == CharacterPhraseButtonType.Next);
			_playButtonPanel.SetActive(phrase.ButtonType == CharacterPhraseButtonType.Play);

			if (immediately)
			{
				_canvasGroup.interactable = true;
				_canvasGroup.blocksRaycasts = true;
				_canvasGroup.alpha = 1f;
			}
			else
			{
				bool isShowCompleted = false;
				fadeTween = _canvasGroup.DOFade(1f, 0.3f)
					.SetEase(Ease.OutCubic)
					.OnComplete(() =>
					{
						isShowCompleted = true;

						_canvasGroup.interactable = true;
						_canvasGroup.blocksRaycasts = true;
					});
				await UniTask.WaitUntil(() => isShowCompleted || !_isShown || isDestroyed, cancellationToken: ct);
			}
		}

		public async UniTask Hide(bool immediately, CancellationToken ct)
		{
			_isShown = false;
			fadeTween?.Kill();

			_canvasGroup.interactable = false;
			_canvasGroup.blocksRaycasts = false;

			if (immediately)
			{
				_canvasGroup.alpha = 0f;
			}
			else
			{
				bool isHideCompleted = false;
				fadeTween = _canvasGroup.DOFade(0f, 0.3f)
					.SetEase(Ease.OutCubic)
					.OnComplete(() =>
					{
						isHideCompleted = true;
					});
				await UniTask.WaitUntil(() => isHideCompleted || _isShown || isDestroyed, cancellationToken: ct);
			}
		}
	}
}
