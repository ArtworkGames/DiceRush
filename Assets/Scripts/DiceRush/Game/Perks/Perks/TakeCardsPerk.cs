using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Chest;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.Services;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Perks.Perks
{
	public class TakeCardsPerk : Perk
	{
		public TakeCardsPerk(PerkModel model) : base(model)
		{
		}

		override public async UniTask<bool> Use(PlayerController player)
		{
			//ChestController chestController = ServiceLocator.Get<ChestController>();
			//await chestController.Open(player);
			//return true;
			return await Apply(player);
		}

		override public async UniTask<bool> Apply(PlayerController player)
		{
			Debug.Log($"[TakeCardsPerk] Apply");

			ChestController chestController = ServiceLocator.Get<ChestController>();
			chestController.AddCards(player);
			await UniTask.Yield();
			return true;
		}
	}
}
