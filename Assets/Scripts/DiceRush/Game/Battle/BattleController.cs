using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.DiceRush.UI.Windows.BattleWindow;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using StepanoffGames.UI.Windows.Signals;
using System.Collections.Generic;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Battle
{
	public class BattleController : MonoBehaviour, IService
	{
		[SerializeField] private Transform _battleWindowParent;

		private void Awake()
		{
			ServiceLocator.Register(this);
		}

		private void Start()
		{
		}

		private void OnDestroy()
		{
			ServiceLocator.Unregister<BattleController>();
		}

		public async UniTask Fight(PlayerController player)
		{
			player.Model.Defense = player.Model.BaseDefense;
			player.Model.Attack = player.Model.BaseAttack;

			EnemyModel enemy = GetEnemyForPlayer(player);

			bool isVictory = false;
			bool battleWindowClosed = false;

			SignalBus.Publish(new OpenWindowSignal(BattleWindow.PrefabName, new BattleWindowParams()
			{
				Player = player,
				Enemy = enemy,
				OnVictory = () =>
				{
					isVictory = true;
					battleWindowClosed = true;
				},
				OnDefeat = () =>
				{
					isVictory = false;
					battleWindowClosed = true;
				}
			})
			{
				Parent = _battleWindowParent
			});

			await UniTask.WaitUntil(() => battleWindowClosed);

			player.Model.Defense = player.Model.BaseDefense;
			player.Model.Attack = player.Model.BaseAttack;
		}

		private EnemyModel GetEnemyForPlayer(PlayerController player)
		{
			float position = (float)((Cell)player.Avatar.CurrentPoint).Index / 100f;

			List<EnemyModel> enemies = new List<EnemyModel>();

			for (int i = 0; i < EnemyModel.AllEnemies.Length; i++)
			{
				if (position >= EnemyModel.AllEnemies[i].FromPosition &&
					position < EnemyModel.AllEnemies[i].ToPosition)
				{
					enemies.Add(EnemyModel.AllEnemies[i]);
				}
			}

			EnemyModel enemy = enemies[Random.Range(0, enemies.Count)].Clone();
			return enemy;
		}
	}
}
