using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Players;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Perks.Perks
{
	public class ChangeBattleStatsPerk : Perk
	{
		private int _battleRound;
		private int _extraDefense;
		private int _health;

		public ChangeBattleStatsPerk(PerkModel model) : base(model)
		{
			switch (model.Type)
			{
				case PerkType.IncreaseFirstDefenseBy1:
					_battleRound = 1;
					_extraDefense = 1;
					break;

				case PerkType.Restore1HealthAfterVictory:
					_health = 1;
					break;
			}
		}

		override public async UniTask<bool> Use(PlayerController player)
		{
			return await Apply(player);
		}

		override public async UniTask<bool> Apply(PlayerController player)
		{
			switch (Model.Type)
			{
				case PerkType.IncreaseFirstDefenseBy1:
					if (player.Model.BattleRound == _battleRound)
					{
						player.Model.ExtraDefense = _extraDefense;
						return true;
					}
					break;

				case PerkType.Restore1HealthAfterVictory:
					int oldHealth = player.Model.Health;
					player.Model.Health = Mathf.Min(player.Model.MaxHealth, player.Model.Health + _health);
					if (oldHealth < player.Model.Health)
					{
						return true;
					}
					break;
			}
			await UniTask.Yield();
			return false;
		}
	}
}
