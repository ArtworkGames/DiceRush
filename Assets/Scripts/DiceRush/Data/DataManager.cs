using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.Initialization;
using StepanoffGames.Services;
using System.Collections.Generic;

namespace StepanoffGames.DiceRush.Data
{
	public class DataManager : BaseInitializable, IService
	{
		public List<PlayerModel> Players => _players;
		private List<PlayerModel> _players;


		public DataManager()
		{
			ServiceLocator.Register(this);
		}

		override public async UniTask InitializeAsync()
		{
			_players = new List<PlayerModel>();

			AddPlayer(new PlayerModel("Player 1", PlayerType.HI));
			AddPlayer(new PlayerModel("Player 2", PlayerType.HI));
			AddPlayer(new PlayerModel("Player 3", PlayerType.AI));
			AddPlayer(new PlayerModel("Player 4", PlayerType.AI));

			await UniTask.Yield();
		}

		public void AddPlayer(PlayerModel player)
		{
			_players.Add(player);
		}
	}
}
