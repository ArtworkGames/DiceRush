using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Players;
using System.Threading;

namespace StepanoffGames.DiceRush.Game.Perks.Perks
{
	public class Perk
	{
		public PerkModel Model => _model;
		private PerkModel _model;

		public Perk(PerkModel model)
		{
			_model = model;
		}

		virtual public async UniTask<bool> Use(PlayerController player, CancellationToken ct)
		{
			await UniTask.Yield(ct);
			return false;
		}

		virtual public async UniTask<bool> Apply(PlayerController player, CancellationToken ct)
		{
			await UniTask.Yield(ct);
			return false;
		}
	}
}
