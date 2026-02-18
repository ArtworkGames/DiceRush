using Cysharp.Threading.Tasks;
using UnityEngine;

namespace StepanoffGames.DiceRush.Gameplay
{
	public class Dice : MonoBehaviour
	{
		[SerializeField] private DiceAnimation _animation;
		[SerializeField] private GameObject[] _numbers;

		private int _value;
		private bool _animationFinished;

		private void Start()
		{
			_animation.OnShowValue += OnShowValue;
			_animation.OnAnimationFinished += OnAnimationFinished;
		}

		public async UniTask<int> Roll()
		{
			_value = GetValue();

			_animationFinished = false;
			_animation.Roll();

			await UniTask.WaitUntil(() => _animationFinished);

			return _value;
		}

		public void Confirm()
		{
			_animation.Confirm();
		}

		public int GetValue()
		{
			return Random.Range(1, 7);
		}

		public void ShowValue(int value)
		{
			for (int i = 0; i < _numbers.Length; i++)
			{
				_numbers[i].SetActive((i + 1) == value);
			}
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
