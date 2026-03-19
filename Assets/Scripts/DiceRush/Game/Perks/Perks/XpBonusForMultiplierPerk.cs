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
				case PerkType.XpBonusForEachMultiplier:
					_xpMultiplier = 2;
					_xpBonus = 1;
					break;
			}
		}

		override public async UniTask<bool> Use(PlayerController player)
		{
			return await Apply(player);
		}

		override public async UniTask<bool> Apply(PlayerController player)
		{
			if (player.Model.XpMultiplier >= _xpMultiplier)
			{
				XpManager xpManager = ServiceLocator.Get<XpManager>();
				xpManager.AddMoveXp(player.Model, _xpBonus);
				return true;
			}
			await UniTask.Yield();
			return false;
		}
	}
}
