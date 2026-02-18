using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StepanoffGames.DiceRush.Gameplay.Xp
{
	public class XpPanel : MonoBehaviour
	{
		[SerializeField] private Image _xpBarFill;
		[SerializeField] private TMP_Text _xpValue;
		[SerializeField] private TMP_Text _levelValue;
		[SerializeField] private TMP_Text _moveXpValue;

		private XpManager _xpManager;

		private int destDisplayingXp;
		private int currDisplayingXp;
		private int minDisplayingXp;
		private int maxDisplayingXp;
		private int displayingLevel;
		private int destDisplayingMoveXp;
		private int currDisplayingMoveXp;
		private int displayingMultiplier;

		private Tween xpValueTween;
		private Tween moveXpValueTween;

		private void Start()
		{
			//_xpManager = ServiceLocator.Get<XpManager>();
			_xpManager = Level.Instance.XpManager;
			_xpManager.OnXpChanged += OnXpChanged;
			_xpManager.OnMoveXpChanged += OnMoveXpChanged;
			_xpManager.OnMultiplierChanged += OnMultiplierChanged;

			currDisplayingXp = 0;
			displayingLevel = 1;
			currDisplayingMoveXp = 0;
			displayingMultiplier = 1;

			UpdateLevel();
			UpdateValues();
			//OnMultiplierChanged(0);

			//SignalBus.Subscribe<XpChangedSignal>(OnXpChanged);
		}

		private void OnDestroy()
		{
			xpValueTween?.Kill();
			moveXpValueTween?.Kill();

			_xpManager.OnXpChanged -= OnXpChanged;
			_xpManager.OnMoveXpChanged -= OnMoveXpChanged;
			_xpManager.OnMultiplierChanged -= OnMultiplierChanged;
			_xpManager = null;

			//SignalBus.Unsubscribe<XpChangedSignal>(OnXpChanged);
		}

		//private void OnXpChanged(XpChangedSignal signal)
		private void OnXpChanged(int value)
		{
			//Debug.Log($"XpPanel.OnXpChanged: {value}");

			destDisplayingXp = value;
			ShowXpChanging();
		}

		private void OnMoveXpChanged(int value)
		{
			//Debug.Log($"XpPanel.OnMoveXpChanged: {value}");

			destDisplayingMoveXp = value;
			ShowMoveXpChanging();
		}

		private void OnMultiplierChanged(int value)
		{
			//Debug.Log($"XpPanel.OnMultiplierChanged: {value}");

			displayingMultiplier = value;
			if (displayingMultiplier == 0)
			{
				destDisplayingMoveXp = 0;
				currDisplayingMoveXp = 0;
			}
			UpdateValues();
		}

		private void ShowXpChanging()
		{
			if (destDisplayingXp < maxDisplayingXp)
			{
				AnimateXpValue(destDisplayingXp, () =>
				{
				});
			}
			else
			{
				AnimateXpValue(maxDisplayingXp, () =>
				{
					UpdateLevel();
					UpdateValues();
					ShowXpChanging();
				});
			}
		}

		private void AnimateXpValue(float nextValue, Action onComplete)
		{
			xpValueTween?.Kill();

			float currValue = currDisplayingXp;

			float duration = 0.25f;
			float valueFactor = 0f;

			xpValueTween = DOTween.To(() => valueFactor, x => valueFactor = x, 1f, duration)
				.SetEase(Ease.Linear)
				.SetUpdate(true)
				.OnUpdate(() =>
				{
					currDisplayingXp = (int)Mathf.Round(Mathf.Lerp(currValue, nextValue, valueFactor));
					UpdateValues();
				})
				.OnComplete(() =>
				{
					onComplete?.Invoke();
				});
		}

		private void ShowMoveXpChanging()
		{
			AnimateMoveXpValue(destDisplayingMoveXp, () =>
			{
			});
		}

		private void AnimateMoveXpValue(float nextValue, Action onComplete)
		{
			moveXpValueTween?.Kill();

			float currValue = currDisplayingMoveXp;

			float duration = 0.25f;
			float valueFactor = 0f;

			moveXpValueTween = DOTween.To(() => valueFactor, x => valueFactor = x, 1f, duration)
				.SetEase(Ease.Linear)
				.SetUpdate(true)
				.OnUpdate(() =>
				{
					currDisplayingMoveXp = (int)Mathf.Round(Mathf.Lerp(currValue, nextValue, valueFactor));
					UpdateValues();
				})
				.OnComplete(() =>
				{
					onComplete?.Invoke();
				});
		}

		private void UpdateValues()
		{
			_xpBarFill.fillAmount = (float)(currDisplayingXp - minDisplayingXp) / (float)(maxDisplayingXp - minDisplayingXp);
			_xpValue.text = $"{minDisplayingXp} -> {currDisplayingXp} -> {maxDisplayingXp}";
			_levelValue.text = $"Level {displayingLevel}";
			_moveXpValue.text = $"{currDisplayingMoveXp} x {displayingMultiplier} = {(currDisplayingMoveXp * displayingMultiplier)}";
		}

		private void UpdateLevel()
		{
			int minXp = 0;
			int maxXp = 0;
			int level = displayingLevel;
			do
			{
				int levelXp = _xpManager.GetXpForLevel(level);
				if (currDisplayingXp < levelXp)
				{
					if (level > 1)
					{
						minXp = _xpManager.GetXpForLevel(level - 1);
					}
					maxXp = levelXp;
					break;
				}
				else
				{
					level++;
				}
			}
			while (true);

			minDisplayingXp = minXp;
			maxDisplayingXp = maxXp;
			displayingLevel = level;
		}
	}
}
