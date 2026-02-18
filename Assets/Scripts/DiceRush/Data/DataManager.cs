using StepanoffGames.DiceRush.Data.Models;
using System.Collections.Generic;

namespace StepanoffGames.DiceRush.Data
{
	public class DataManager
	{
		public List<PlayerModel> Players => _players;

		private List<PlayerModel> _players;

		public DataManager()
		{
			_players = new List<PlayerModel>();

			AddPlayer(new PlayerModel());
			AddPlayer(new PlayerModel());
			AddPlayer(new PlayerModel());
			AddPlayer(new PlayerModel());
		}

		public void AddPlayer(PlayerModel player)
		{
			_players.Add(player);
		}
	}
}
