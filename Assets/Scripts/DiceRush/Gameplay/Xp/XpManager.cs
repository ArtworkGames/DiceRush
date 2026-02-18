using System;
using UnityEngine;

namespace StepanoffGames.DiceRush.Gameplay.Xp
{
	public class XpManager : MonoBehaviour
	{
		public Action<int> OnXpChanged;
		public Action<int> OnMoveXpChanged;
		public Action<int> OnMultiplierChanged;

		public int Multiplier => _multiplier;
		private int _multiplier = 0;

		public int MoveXp => _moveXp;
		private int _moveXp = 0;

		public int Xp => _xp;
		private int _xp = 0;

		public int Level => _level;
		private int _level = 0;

		private float baseXp = 20f;
		private float power = 1.5f;

		private void Awake()
		{
			//ServiceLocator.Register(this);
		}

		private void Start()
		{
			//_xp = 0;
			//_level = 1;
			//_multiplier = 0;

			//SignalBus.Subscribe<EnemyDamageAddedSignal>(OnEnemyDamageAdded);
			//SignalBus.Subscribe<GoldChangedSignal>(OnGoldChanged);
		}

		private void OnDestroy()
		{
			//ServiceLocator.Unregister<XpManager>();

			//SignalBus.Unsubscribe<EnemyDamageAddedSignal>(OnEnemyDamageAdded);
			//SignalBus.Unsubscribe<GoldChangedSignal>(OnGoldChanged);
		}

		public int GetXpForLevel(int level)
		{
			int xp = (int)Mathf.Round(baseXp * Mathf.Pow(level, power));
			return xp;
		}

		//private void OnEnemyDamageAdded(EnemyDamageAddedSignal signal)
		//{
		//	//Debug.Log($"OnEnemyDamageAdded: {signal.Damage}");

		//	AddXp(signal.Damage);
		//}

		//private void OnGoldChanged(GoldChangedSignal signal)
		//{
		//	//Debug.Log($"OnEnemyDamageAdded: {signal.OldValue} -> {signal.NewValue}");

		//	if (signal.NewValue > signal.OldValue)
		//	{
		//		AddXp(signal.NewValue - signal.OldValue);
		//	}
		//}

		public void StartMove()
		{
			_multiplier = 0;
			_moveXp = 0;

			//Debug.Log($"XpManager.StartMove: {_moveXp} x {_multiplier}");

			OnMultiplierChanged?.Invoke(_multiplier);
			OnMoveXpChanged?.Invoke(_moveXp);
		}

		public void IncMultiplier()
		{
			_multiplier += 1;
			//Debug.Log($"XpManager.IncMultiplier: {_multiplier}");
			OnMultiplierChanged?.Invoke(_multiplier);
		}

		public void AddMoveXp(int xp)
		{
			if (xp == 0f) return;

			float oldXp = _moveXp;
			_moveXp += xp;

			//Debug.Log($"XpManager.AddMoveXp: {oldXp} + {xp} = {_moveXp}");

			//SignalBus.Publish(new XpChangedSignal(oldXp, _xp));

			OnMoveXpChanged?.Invoke(_moveXp);
		}

		public void EndMove()
		{
			ApplyMoveXp();
		}

		private void ApplyMoveXp()
		{
			int xp = _moveXp * _multiplier;
			if (xp == 0f) return;

			float oldXp = _xp;
			_xp += xp;

			//Debug.Log($"XpManager.AddXp: {oldXp} + {xp} = {_xp}");

			UpdateLevel();

			//SignalBus.Publish(new XpChangedSignal(oldXp, _xp));

			OnXpChanged?.Invoke(_xp);
		}

		private void UpdateLevel()
		{
			int level = _level;
			do
			{
				float levelXp = GetXpForLevel(level);
				if (_xp < levelXp)
				{
					break;
				}
				else
				{
					level++;
				}
			}
			while (true);

			_level = level;
		}
	}
}
