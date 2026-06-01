using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.Initialization;
using StepanoffGames.Services;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using StepanoffGames.DiceRush.Data.Schemes;
using SRF;

namespace StepanoffGames.DiceRush.Data
{
	public class DataManager : BaseInitializable, IService
	{
		public List<PlayerModel> Players => _players;
		private List<PlayerModel> _players;

		public ProfileModel Profile => _profile;
		private ProfileModel _profile;

		public DataManager()
		{
			ServiceLocator.Register(this);
		}

		override public async UniTask InitializeAsync()
		{
			LoadProfile();

			_players = new List<PlayerModel>();

			//AddPlayer(new PlayerModel("Player 1", PlayerColor.Red, PlayerType.HI));
			//AddPlayer(new PlayerModel("Player 2", PlayerColor.Blue, PlayerType.AI, AIBrainType.Medium));
			//AddPlayer(new PlayerModel("Player 3", PlayerColor.Green, PlayerType.AI, AIBrainType.Medium));
			//AddPlayer(new PlayerModel("Player 4", PlayerColor.Yellow, PlayerType.AI, AIBrainType.Medium));

			await UniTask.Yield();
		}

		//public void AddPlayer(PlayerModel player)
		//{
		//	_players.Add(player);
		//}

		private void LoadProfile()
		{
			if (PlayerPrefs.HasKey("Profile"))
			{
				string profileJson = PlayerPrefs.GetString("Profile");
				if (!string.IsNullOrEmpty(profileJson))
				{
					ProfileScheme profileScheme = JsonUtility.FromJson<ProfileScheme>(profileJson);
					_profile = new ProfileModel(profileScheme);
				}
			}
			if (_profile == null)
			{
				_profile = new ProfileModel();
			}
		}

		public void SaveProfile()
		{
			ProfileScheme profileScheme = _profile.GetScheme();
			string profileJson = JsonUtility.ToJson(profileScheme);

			PlayerPrefs.SetString("Profile", profileJson);
			PlayerPrefs.Save();
		}

		public void SetPlayers(List<PlayerModel> players)
		{
			_players = new List<PlayerModel>(players);
		}
	}
}
