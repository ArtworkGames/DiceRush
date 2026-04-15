using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.DiceRush.UI.Dice;
using StepanoffGames.Services;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Dice
{
	public class DiceController : MonoBehaviour, IService
	{
		[SerializeField] private DiceAnimation _animation;

		private int _value;
		private bool _animationFinished;

		private List<int> _predefinedValues = new List<int>();

		private void Awake()
		{
			ServiceLocator.Register(this);
		}

		private void Start()
		{
			_animation.OnShowValue += OnShowValue;
			_animation.OnAnimationFinished += OnAnimationFinished;
		}

		private void OnDestroy()
		{
			ServiceLocator.Unregister<DiceController>();
		}

		public async UniTask<int> Roll(PlayerController player, CancellationToken ct)
		{
			_value = GetValue(player);

			_animationFinished = false;
			_animation.Roll();

			await UniTask.WaitUntil(() => _animationFinished, cancellationToken: ct);

			return _value;
		}

		public void Confirm()
		{
			_animation.Confirm();
		}

		public int GetValue(PlayerController player)
		{
			int value = 0;
			if (_predefinedValues.Count > 0)
			{
				value = _predefinedValues[0];
				_predefinedValues.RemoveAt(0);
			}
			else
			{
				value = Random.Range(1, 7);
			}
			return value;
		}

		public void ShowValue(int value)
		{
			_animation.SetValue(value);
		}

		private void OnShowValue()
		{
			ShowValue(_value);
		}

		private void OnAnimationFinished()
		{
			_animationFinished = true;
		}

		public void AddPredefinedValue(int value)
		{
			_predefinedValues.Add(value);
		}

		public void ClearPredefinedValues()
		{
			_predefinedValues.Clear();
		}
	}
}
