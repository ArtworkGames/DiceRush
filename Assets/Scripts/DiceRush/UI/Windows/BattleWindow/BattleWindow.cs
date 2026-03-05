using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Deck;
using StepanoffGames.DiceRush.Game.Dice;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.Services;
using StepanoffGames.UI.Windows;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace StepanoffGames.DiceRush.UI.Windows.BattleWindow
{
	public class BattleWindowParams : BaseWindowParams
	{
		public PlayerController Player;
		public EnemyModel Enemy;
		public Action OnVictory;
		public Action OnDefeat;
	}

	public class BattleWindow : BaseWindow<BattleWindowParams>
	{
		public static string PrefabName = "BattleWindow";

		[Serializable]
		public class PlayerStats
		{
			public TMP_Text HealthValue;
			public TMP_Text DefenseValue;
			public TMP_Text AttackValue;
		}

		[Serializable]
		public class EnemyStats
		{
			public TMP_Text HealthValue;
			public TMP_Text AttackValue;
			public GameObject[] DiceValues;
		}

		[Space]
		[SerializeField] private PlayerStats _playerStats;
		[SerializeField] private EnemyStats _enemyStats;

		[Space]
		[SerializeField] private CharacterAnimation _playerAnimation;
		[SerializeField] private Transform _playerImageContainer;
		
		[Space]
		[SerializeField] private CharacterAnimation _enemyAnimation;
		[SerializeField] private Transform _enemyImageContainer;

		[Space]
		[SerializeField] private StatsAnimation _statsAnimation;

		private bool isVictory;

		private bool isEnemyAttacked;
		private bool isEnemyDamaged;
		private bool isEnemyDied;
		private bool isPlayerAttacked;
		private bool isPlayerDamaged;
		private bool isPlayerDied;
		private bool isStatsEnemyAttacked;
		private bool isStatsPlayerAttacked;

		override protected void BeforeOpen()
		{
			_enemyAnimation.OnAttack += OnEnemyAttack;
			_enemyAnimation.OnDamage += OnEnemyDamage;
			_enemyAnimation.OnDeath += OnEnemyDeath;

			_playerAnimation.OnAttack += OnPlayerAttack;
			_playerAnimation.OnDamage += OnPlayerDamage;
			_playerAnimation.OnDeath += OnPlayerDeath;

			_statsAnimation.OnEnemyAttack += OnStatsEnemyAttack;
			_statsAnimation.OnPlayerAttack += OnStatsPlayerAttack;

			string playerImageName = $"RedPlayer";
			string playerImagePath = $"Windows/BattleWindow/Players/{playerImageName}.prefab";
			LoadImage(_playerImageContainer, playerImageName, playerImagePath).Forget();

			string enemyImageName = $"{Params.Enemy.Type}Enemy";
			string enemyImagePath = $"Windows/BattleWindow/Enemies/{enemyImageName}.prefab";
			LoadImage(_enemyImageContainer, enemyImageName, enemyImagePath).Forget();

			_playerStats.HealthValue.text = Params.Player.Model.Health.ToString();
			_playerStats.DefenseValue.text = Params.Player.Model.Defense.ToString();
			_playerStats.AttackValue.text = Params.Player.Model.Attack.ToString();

			_enemyStats.HealthValue.text = Params.Enemy.Health.ToString();
			_enemyStats.AttackValue.text = Params.Enemy.BaseAttack > 0 ? $"+{Params.Enemy.BaseAttack}" : "";

			UpdateEnemyDiceValue(0);
		}

		override protected void AfterOpen()
		{
			Battle();
		}

		override protected void BeforeClose()
		{
		}

		override protected void AfterClose()
		{
			Params.OnVictory?.Invoke();
		}

		private async UniTask LoadImage(Transform parent, string imageName, string imagePath)
		{
			var handle = Addressables.LoadAssetAsync<GameObject>(imagePath);
			await UniTask.WaitUntil(() => handle.IsDone);

			GameObject imageObject = Instantiate(handle.Result, parent, false);
			imageObject.name = imageName;
		}

		private void UpdateEnemyDiceValue(int value)
		{
			for (int i = 0; i < _enemyStats.DiceValues.Length; i++)
			{
				_enemyStats.DiceValues[i].SetActive(i == value);
			}
		}

		private void UpdatePlayerBattleValues()
		{
			_playerStats.HealthValue.text = Params.Player.Model.Health.ToString();

			int defense = Params.Player.Model.BaseDefense;
			int defenseDelta = Params.Player.Model.Defense - Params.Player.Model.BaseDefense;
			string defenseDeltaStr = "";
			if (defenseDelta != 0)
			{
				if (defenseDelta > 0) defenseDeltaStr = $"<color=#090>+{defenseDelta}</color>";
				else defenseDeltaStr = $"<color=#f00>-{defenseDelta}</color>";
			}

			_playerStats.DefenseValue.text = defense.ToString() + defenseDeltaStr;

			int attack = Params.Player.Model.BaseAttack;
			int attackDelta = Params.Player.Model.Attack - Params.Player.Model.BaseAttack;
			string attackDeltaStr = "";
			if (attackDelta != 0)
			{
				if (attackDelta > 0) attackDeltaStr = $"<color=#090>+{attackDelta}</color>";
				else attackDeltaStr = $"<color=#f00>-{attackDelta}</color>";
			}

			_playerStats.AttackValue.text = attack.ToString() + attackDeltaStr;
		}

		private void UpdatePlayerHealthValue()
		{
			_playerStats.HealthValue.text = Params.Player.Model.Health.ToString();
		}

		private void UpdateEnemyHealthValue()
		{
			_enemyStats.HealthValue.text = Params.Enemy.Health.ToString();
		}

		private async void Battle()
		{
			DeckController deckController = ServiceLocator.Get<DeckController>();
			DiceController diceController = ServiceLocator.Get<DiceController>();

			//await deckController.PrepareForBattle(Params.Player);
			//UpdatePlayerBattleValues();

			//await UniTask.WaitForSeconds(0.5f);

			do
			{
				UpdateEnemyDiceValue(0);
				await deckController.PrepareForBattle(Params.Player);
				UpdatePlayerBattleValues();

				int diceValue = await diceController.Roll(Params.Player);
				UpdateEnemyDiceValue(diceValue);
				diceController.Confirm();

				isEnemyAttacked = false;
				_enemyAnimation.ShowAttack();
				await UniTask.WaitUntil(() => isEnemyAttacked);

				isStatsEnemyAttacked = false;
				_statsAnimation.ShowEnemyAttack();
				await UniTask.WaitUntil(() => isStatsEnemyAttacked);

				int enemyAttack = Params.Enemy.BaseAttack + diceValue;
				int playerDamage = Mathf.Max(0, enemyAttack - Params.Player.Model.Defense);
				Params.Player.Model.Health = Math.Max(0, Params.Player.Model.Health - playerDamage);
				UpdatePlayerHealthValue();

				if (Params.Player.Model.Health == 0)
				{
					isPlayerDied = false;
					_playerAnimation.ShowDeath();
					await UniTask.WaitUntil(() => isPlayerDied);
					break;
				}

				isPlayerDamaged = false;
				_playerAnimation.ShowDamage();
				await UniTask.WaitUntil(() => isPlayerDamaged);

				await UniTask.WaitForSeconds(0.5f);

				isPlayerAttacked = false;
				_playerAnimation.ShowAttack();
				await UniTask.WaitUntil(() => isPlayerAttacked);

				isStatsPlayerAttacked = false;
				_statsAnimation.ShowPlayerAttack();
				await UniTask.WaitUntil(() => isStatsPlayerAttacked);

				Params.Enemy.Health = Math.Max(0, Params.Enemy.Health - Params.Player.Model.Attack);
				UpdateEnemyHealthValue();

				if (Params.Enemy.Health == 0)
				{
					isEnemyDied = false;
					_enemyAnimation.ShowDeath();
					await UniTask.WaitUntil(() => isEnemyDied);
					break;
				}

				isEnemyDamaged = false;
				_enemyAnimation.ShowDamage();
				await UniTask.WaitUntil(() => isEnemyDamaged);

				await UniTask.WaitForSeconds(0.5f);
			}
			while (true);

			await UniTask.WaitForSeconds(0.5f);

			CloseWindow();
		}

		private void OnEnemyAttack()
		{
			isEnemyAttacked = true;
		}

		private void OnEnemyDamage()
		{
			isEnemyDamaged = true;
		}

		private void OnEnemyDeath()
		{
			isEnemyDied = true;
		}

		private void OnPlayerAttack()
		{
			isPlayerAttacked = true;
		}

		private void OnPlayerDamage()
		{
			isPlayerDamaged = true;
		}

		private void OnPlayerDeath()
		{
			isPlayerDied = true;
		}

		private void OnStatsEnemyAttack()
		{
			isStatsEnemyAttacked = true;
		}

		private void OnStatsPlayerAttack()
		{
			isStatsPlayerAttacked = true;
		}
	}
}
