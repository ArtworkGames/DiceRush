using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Chest;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.Services;

namespace StepanoffGames.DiceRush.Game.Perks.Perks
{
	public class Take3CardsPerk : Perk
	{
		public Take3CardsPerk(PerkModel model) : base(model)
		{
		}

		override public async UniTask Use(PlayerController player)
		{
			ChestController chestController = ServiceLocator.Get<ChestController>();
			await chestController.Open(player);
		}

		override public async UniTask Apply(PlayerController player)
		{
			ChestController chestController = ServiceLocator.Get<ChestController>();
			chestController.AddCards(player);
			await UniTask.Yield();
		}
	}
}
