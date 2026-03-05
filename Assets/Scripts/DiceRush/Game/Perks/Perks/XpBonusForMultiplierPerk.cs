using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.DiceRush.Game.Xp;
using StepanoffGames.Services;

namespace StepanoffGames.DiceRush.Game.Perks.Perks
{
	public class XpBonusForMultiplierPerk : Perk
	{
		private int _xpMultiplier;
		private int _xpBonus;

		public XpBonusForMultiplierPerk(PerkModel model) : base(model)
		{
			switch (model.Type)
			{
				case PerkType.XpBonusForMultiplierX4:
					_xpMultiplier = 4;
					_xpBonus = 1;
					break;
			}
		}

		override public async UniTask Use(PlayerController player)
		{
			if (player.Model.XpMultiplier >= _xpMultiplier)
			{
				XpManager xpManager = ServiceLocator.Get<XpManager>();
				xpManager.AddMoveXp(player.Model, _xpBonus);
			}
			await UniTask.Yield();
		}

		override public async UniTask Apply(PlayerController player)
		{
			if (player.Model.XpMultiplier >= _xpMultiplier)
			{
				XpManager xpManager = ServiceLocator.Get<XpManager>();
				xpManager.AddMoveXp(player.Model, _xpBonus);
			}
			await UniTask.Yield();
		}
	}
}
