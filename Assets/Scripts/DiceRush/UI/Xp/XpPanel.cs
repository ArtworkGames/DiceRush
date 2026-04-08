using Cysharp.Threading.Tasks;
using DG.Tweening;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Players.Signals;
using StepanoffGames.DiceRush.Game.Xp;
using StepanoffGames.DiceRush.Game.Xp.Signals;
using StepanoffGames.DiceRush.UI.Components;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StepanoffGames.DiceRush.UI.Xp
{
	public class XpPanel : MonoBehaviour
	{
		[SerializeField] private HideablePanel _totalXpPanel;
		[SerializeField] private HideablePanel _moveXpPanel;
		[Space]
		[SerializeField] private Image _xpBarFill;
		[SerializeField] private TMP_Text _xpValue;
		[SerializeField] private TMP_Text _levelValue;
		[Space]
		[SerializeField] private Transform _xpMultiplierPanel;
		[Space]
		[SerializeField] private TMP_Text _moveXpValue;
		[SerializeField] private TMP_Text _xpMultiplierValue;
		[SerializeField] private TMP_Text _totalXpValue;
		[SerializeField] private GameObject[] _fireImages;

		private XpManager _xpManager;
		
		private PlayerModel _player;

		private int destMoveXp;
		private int currMoveXp;
		private int xpMultiplier;
		private int destTotalXp;
		private int currTotalXp;
		private int minTotalXp;
		private int maxTotalXp;
		private int level;

		private Tween moveXpValueTween;
		private Tween totalXpValueTween;

		private Vector3 xpMultiplierPosition;

		private CancellationTokenSource cts;
		private CancellationTokenSource ctsLevelUp;

		private void Start()
		{
			_xpManager = ServiceLocator.Get<XpManager>();

			SignalBus.Subscribe<PlayerTurnStartedSignal>(OnPlayerTurnStarted);
			SignalBus.Subscribe<PlayerTurnEndedSignal>(OnPlayerTurnEnded);
			SignalBus.Subscribe<MoveXpChangedSignal>(OnMoveXpChanged);
			SignalBus.Subscribe<XpMultiplierChangedSignal>(OnXpMultiplierChanged);
			SignalBus.Subscribe<TotalXpChangedSignal>(OnTotalXpChanged);

			currMoveXp = 0;
			xpMultiplier = 1;
			currTotalXp = 0;
			level = 1;

			xpMultiplierPosition = _xpMultiplierPanel.localPosition;

			UpdateLevel();
			UpdateValues();
		}

		private void OnDestroy()
		{
			cts?.Cancel();
			cts?.Dispose();
			cts = null;

			ctsLevelUp?.Cancel();
			ctsLevelUp?.Dispose();
			ctsLevelUp = null;

			moveXpValueTween?.Kill();
			totalXpValueTween?.Kill();

			_xpManager = null;
			_player = null;

			SignalBus.Unsubscribe<PlayerTurnStartedSignal>(OnPlayerTurnStarted);
			SignalBus.Unsubscribe<PlayerTurnEndedSignal>(OnPlayerTurnEnded);
			SignalBus.Unsubscribe<MoveXpChangedSignal>(OnMoveXpChanged);
			SignalBus.Unsubscribe<XpMultiplierChangedSignal>(OnXpMultiplierChanged);
			SignalBus.Unsubscribe<TotalXpChangedSignal>(OnTotalXpChanged);
		}

		private void Update()
		{
			Vector3 posOffset = Vector3.zero;
			float rotationOffset = 0f;
			float scaleOffset = 0f;

			if (xpMultiplier > 1)
			{
				float k = xpMultiplier / 10f;
				float posAmplitude = k * 10f;
				float rotAmplitude = k * 3f;
				float scaleAmplitude = k * 0.1f;

				float frequency = 10f;
				float t = Time.time * frequency;

				float posX = Mathf.PerlinNoise(t, 11.1f) * 2 - 1;
				float posY = Mathf.PerlinNoise(t, 22.2f) * 2 - 1;

				float rot = Mathf.PerlinNoise(t, 33.3f) * 2 - 1;
				float scale = Mathf.PerlinNoise(t, 44.4f) * 2 - 1;

				posOffset = new Vector3(
					posX * posAmplitude,
					posY * posAmplitude,
					0f
				);
				rotationOffset = rot * rotAmplitude;
				scaleOffset = scale * scaleAmplitude;
			}

			_xpMultiplierPanel.localPosition = xpMultiplierPosition + posOffset;
			_xpMultiplierPanel.localRotation = Quaternion.Euler(0f, 0f, rotationOffset);
			_xpMultiplierPanel.localScale = Vector3.one * (1f + scaleOffset);
		}

		private async UniTask Show(CancellationToken ct)
		{
			List<UniTask> tasks = new();
			tasks.Add(_totalXpPanel.Show(false, ct));
			tasks.Add(_moveXpPanel.Show(false, ct));
			await UniTask.WhenAll(tasks);
		}

		private async UniTask Hide(CancellationToken ct)
		{
			List<UniTask> tasks = new();
			tasks.Add(_totalXpPanel.Hide(false, ct));
			tasks.Add(_moveXpPanel.Hide(false, ct));
			await UniTask.WhenAll(tasks);
		}

		public async void SetPlayer(PlayerModel player)
		{
			if (_player == player) return;

			cts?.Cancel();
			cts?.Dispose();
			cts = new CancellationTokenSource();

			if (_totalXpPanel.IsShown)
			{
				await Hide(cts.Token);
			}

			_player = player;

			moveXpValueTween?.Kill();
			totalXpValueTween?.Kill();

			destMoveXp = _player.MoveXp;
			xpMultiplier = _player.XpMultiplier;
			destTotalXp = _player.TotalXp;
			level = _player.Level;

			currMoveXp = destMoveXp;
			currTotalXp = destTotalXp;
			minTotalXp = 0;
			maxTotalXp = 0;

			UpdateLevel();
			UpdateValues();

			await Show(cts.Token);
		}

		public async void ClearPlayer()
		{
			_player = null;
			if (_totalXpPanel.IsShown)
			{
				cts?.Cancel();
				cts?.Dispose();
				cts = new CancellationTokenSource();

				await Hide(cts.Token);
			}
		}

		private void OnPlayerTurnStarted(PlayerTurnStartedSignal signal)
		{
			if (signal.Player.Model.Type != PlayerType.HI) return;
			SetPlayer(signal.Player.Model);
		}

		private void OnPlayerTurnEnded(PlayerTurnEndedSignal signal)
		{
			if (signal.Player.Model == _player)
			{
				ClearPlayer();
			}
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

			xpMultiplier = _player.XpMultiplier;
			if (xpMultiplier == 0)
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
					_player.IsTotalXpCounted = true;
				});
			}
			else
			{
				AnimateTotalXpValue(maxTotalXp, async () =>
				{
					UpdateLevel();
					UpdateValues();

					ctsLevelUp?.Cancel();
					ctsLevelUp?.Dispose();
					ctsLevelUp = new CancellationTokenSource();

					await _xpManager.LevelUp(_player, ctsLevelUp.Token);

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

			_moveXpValue.text = $"{currMoveXp}";
			_xpMultiplierValue.text = $"<size=120>x</size>{xpMultiplier}";
			_totalXpValue.text = $"={(currMoveXp * xpMultiplier)}";

			for (int i = 0; i < _fireImages.Length; i++)
			{
				_fireImages[i].SetActive(i == xpMultiplier || (xpMultiplier > 10 && i == 10));
			}
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
