using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.Services;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Dice
{
	public class DiceController : MonoBehaviour, IService
	{
		[SerializeField] private DiceAnimation _animation;

		private int _value;
		private bool _animationFinished;

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

		public async UniTask<int> Roll(PlayerController player)
		{
			_value = GetValue(player);

			_animationFinished = false;
			_animation.Roll();

			await UniTask.WaitUntil(() => _animationFinished);

			return _value;
		}

		public void Confirm()
		{
			_animation.Confirm();
		}

		public int GetValue(PlayerController player)
		{
			return Random.Range(1, 7);
		}

		public void ShowValue(int value)
		{
			//_value = value;
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
	}
}
