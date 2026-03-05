using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.DiceRush.Game.Xp;
using StepanoffGames.Services;

namespace StepanoffGames.DiceRush.Game.Perks.Perks
{
	public class IncXpMultiplierPerk : Perk
	{
		private int _xpMultiplier;

		public IncXpMultiplierPerk(PerkModel model) : base(model)
		{
			switch (model.Type)
			{
				case PerkType.FirstMultiplierX3: _xpMultiplier = 2; break;
			}
		}

		override public async UniTask Use(PlayerController player)
		{
			if (player.Model.XpMultiplier == _xpMultiplier)
			{
				XpManager xpManager = ServiceLocator.Get<XpManager>();
				xpManager.IncMultiplier(player.Model);
			}
			await UniTask.Yield();
		}

		override public async UniTask Apply(PlayerController player)
		{
			if (player.Model.XpMultiplier == _xpMultiplier)
			{
				XpManager xpManager = ServiceLocator.Get<XpManager>();
				xpManager.IncMultiplier(player.Model);
			}
			await UniTask.Yield();
		}
	}
}
