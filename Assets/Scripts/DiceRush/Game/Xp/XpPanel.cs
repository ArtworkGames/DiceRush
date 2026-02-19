using DG.Tweening;
using StepanoffGames.DiceRush.Data;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Xp.Signals;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StepanoffGames.DiceRush.Game.Xp
{
	public class XpPanel : MonoBehaviour
	{
		[SerializeField] private int _playerIndex;
		[Space]
		[SerializeField] private Image _xpBarFill;
		[SerializeField] private TMP_Text _xpValue;
		[SerializeField] private TMP_Text _levelValue;
		[SerializeField] private TMP_Text _moveXpValue;

		private XpManager _xpManager;
		private PlayerModel _player;

		private int destMoveXp;
		private int currMoveXp;
		private int multiplier;
		private int destTotalXp;
		private int currTotalXp;
		private int minTotalXp;
		private int maxTotalXp;
		private int level;

		private Tween moveXpValueTween;
		private Tween totalXpValueTween;

		private void Start()
		{
			DataManager dataManager = ServiceLocator.Get<DataManager>();
			_player = dataManager.Players[_playerIndex];

			_xpManager = ServiceLocator.Get<XpManager>();

			SignalBus.Subscribe<MoveXpChangedSignal>(OnMoveXpChanged);
			SignalBus.Subscribe<XpMultiplierChangedSignal>(OnXpMultiplierChanged);
			SignalBus.Subscribe<TotalXpChangedSignal>(OnTotalXpChanged);

			currMoveXp = 0;
			multiplier = 1;
			currTotalXp = 0;
			level = 1;

			UpdateLevel();
			UpdateValues();
		}

		private void OnDestroy()
		{
			moveXpValueTween?.Kill();
			totalXpValueTween?.Kill();

			_xpManager = null;

			SignalBus.Unsubscribe<MoveXpChangedSignal>(OnMoveXpChanged);
			SignalBus.Unsubscribe<XpMultiplierChangedSignal>(OnXpMultiplierChanged);
			SignalBus.Unsubscribe<TotalXpChangedSignal>(OnTotalXpChanged);
		}

		private void OnMoveXpChanged(MoveXpChangedSignal signal)
		{
			if (signal.Player != _player) return;

			destMoveXp = _player.MoveXp;
			ShowMoveXpChanging();
		}

		private void OnXpMultiplierChanged(XpMultiplierChangedSignal signal)
		{
			if (signal.Player != _player) return;

			multiplier = _player.XpMultiplier;
			if (multiplier == 0)
			{
				destMoveXp = 0;
				currMoveXp = 0;
			}
			UpdateValues();
		}

		private void OnTotalXpChanged(TotalXpChangedSignal signal)
		{
			if (signal.Player != _player) return;

			destTotalXp = _player.TotalXp;
			ShowTotalXpChanging();
		}

		private void ShowMoveXpChanging()
		{
			AnimateMoveXpValue(destMoveXp, () =>
			{
			});
		}

		private void AnimateMoveXpValue(float nextValue, Action onComplete)
		{
			moveXpValueTween?.Kill();

			float currValue = currMoveXp;

			float duration = 0.25f;
			float valueFactor = 0f;

			moveXpValueTween = DOTween.To(() => valueFactor, x => valueFactor = x, 1f, duration)
				.SetEase(Ease.Linear)
				.SetUpdate(true)
				.OnUpdate(() =>
				{
					currMoveXp = (int)Mathf.Round(Mathf.Lerp(currValue, nextValue, valueFactor));
					UpdateValues();
				})
				.OnComplete(() =>
				{
					onComplete?.Invoke();
				});
		}

		private void ShowTotalXpChanging()
		{
			if (destTotalXp < maxTotalXp)
			{
				AnimateTotalXpValue(destTotalXp, () =>
				{
					_player.IsXpAdditionCompleted = true;
				});
			}
			else
			{
				AnimateTotalXpValue(maxTotalXp, async () =>
				{
					UpdateLevel();
					UpdateValues();

					await _xpManager.LevelUp(_player);

					ShowTotalXpChanging();
				});
			}
		}

		private void AnimateTotalXpValue(float nextValue, Action onComplete)
		{
			totalXpValueTween?.Kill();

			float currValue = currTotalXp;

			float duration = 0.25f;
			float valueFactor = 0f;

			totalXpValueTween = DOTween.To(() => valueFactor, x => valueFactor = x, 1f, duration)
				.SetEase(Ease.Linear)
				.SetUpdate(true)
				.OnUpdate(() =>
				{
					currTotalXp = (int)Mathf.Round(Mathf.Lerp(currValue, nextValue, valueFactor));
					UpdateValues();
				})
				.OnComplete(() =>
				{
					onComplete?.Invoke();
				});
		}

		private void UpdateValues()
		{
			_xpBarFill.fillAmount = (float)(currTotalXp - minTotalXp) / (float)(maxTotalXp - minTotalXp);
			_xpValue.text = $"{minTotalXp} -> {currTotalXp} -> {maxTotalXp}";
			_levelValue.text = $"Level {level}";
			_moveXpValue.text = $"{currMoveXp} x {multiplier} = {(currMoveXp * multiplier)}";
		}

		private void UpdateLevel()
		{
			int minXp = 0;
			int maxXp = 0;
			int level = this.level;
			do
			{
				int levelXp = _xpManager.GetXpForLevel(level);
				if (currTotalXp < levelXp)
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

			minTotalXp = minXp;
			maxTotalXp = maxXp;
			this.level = level;
		}
	}
}
