using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.DiceRush.Game.Players.Signals;
using StepanoffGames.DiceRush.Game.Ranking.Signals;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using System.Collections.Generic;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Ranking
{
	public class RankingManager : MonoBehaviour, IService
	{
		private LevelManager _levelManager;

		private void Awake()
		{
			ServiceLocator.Register(this);
		}

		private void Start()
		{
			_levelManager = ServiceLocator.Get<LevelManager>();

			SignalBus.Subscribe<PlayerCellPassedSignal>(OnPlayerCellPassed);
			SignalBus.Subscribe<PlayerPortalPassedSignal>(OnPlayerPortalPassed);
		}

		private void OnDestroy()
		{
			ServiceLocator.Unregister<RankingManager>();

			_levelManager = null;

			SignalBus.Unsubscribe<PlayerCellPassedSignal>(OnPlayerCellPassed);
			SignalBus.Unsubscribe<PlayerPortalPassedSignal>(OnPlayerPortalPassed);
		}

		private void OnPlayerCellPassed(PlayerCellPassedSignal signal)
		{
			UpdateRanking();
		}

		private void OnPlayerPortalPassed(PlayerPortalPassedSignal signal)
		{
			UpdateRanking();
		}

		private void UpdateRanking()
		{
			List<PlayerController> players = new List<PlayerController>();

			for (int i = 0; i < _levelManager.Players.Count; i++)
			{
				PlayerController player = _levelManager.Players[i];
				players.Add(player);

				if (player.Avatar.CurrentPoint is Cell)
				{
					player.Model.CellIndex = ((Cell)player.Avatar.CurrentPoint).Index;
				}
			}

			players.Sort((a, b) => b.Model.CellIndex.CompareTo(a.Model.CellIndex));

			for (int i = 0; i < players.Count; i++)
			{
				players[i].Model.PrevPlace = players[i].Model.Place;
			}

			/*int place = 1;
			players[0].Model.Place = place;
			int cellIndex = players[0].Model.CellIndex;

			for (int i = 1; i < players.Count; i++)
			{
				if (players[i].Model.CellIndex < cellIndex)
				{
					place++;
					players[i].Model.Place = place;
					cellIndex = players[i].Model.CellIndex;
				}
				else
				{
					players[i].Model.Place = place;
				}

				if (players[i].Model.PrevPlace == 0)
					players[i].Model.PrevPlace = players[i].Model.Place;
			}*/
			
			for (int i = 0; i < players.Count; i++)
			{
				players[i].Model.Place = i + 1;
			}

			for (int i = 0; i < players.Count; i++)
			{
				if (players[i].Model.PrevPlace != players[i].Model.Place)
				{
					SignalBus.Publish(new PlayerPlaceChangedSignal(players[i], players[i].Model.PrevPlace, players[i].Model.Place));
				}
			}
		}
	}
}
