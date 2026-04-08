using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Game;
using StepanoffGames.DiceRush.Game.Map;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.DiceRush.Game.Players.Signals;
using StepanoffGames.DiceRush.Game.Ranking.Signals;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using System.Collections.Generic;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Ranking
{
	public class RankingPanel : MonoBehaviour
	{
		[SerializeField] private GameObject _sourceItems;

		private List<PlayerItem> _items;

		private void Awake()
		{
			_sourceItems.SetActive(false);
		}

		private async void Start()
		{
			SignalBus.Subscribe<PlayerPlaceChangedSignal>(OnPlayerPlaceChanged);
			SignalBus.Subscribe<PlayerCellPassedSignal>(OnPlayerCellPassed);
			SignalBus.Subscribe<PlayerPortalPassedSignal>(OnPlayerPortalPassed);

			//DataManager dataManager = ServiceLocator.Get<DataManager>();
			//for (int i = 0; i < _playerItems.Length; i++)
			//{
			//	_playerItems[i].Model = dataManager.Players[i];
			//}

			await UniTask.NextFrame();
			await UniTask.NextFrame();

			GameManager gameManager = ServiceLocator.Get<GameManager>();

			await UniTask.WaitUntil(() => gameManager.Players != null);

			_items = new List<PlayerItem>();
			for (int i = 0; i < gameManager.Players.Count; i++)
			{
				AddItem(i, gameManager.Players[i]);
			}
			//for (int i = gameManager.Players.Count; i < _playerItems.Length; i++)
			//{
			//	_playerItems[i].gameObject.SetActive(false);
			//}
		}

		private void OnDestroy()
		{
			SignalBus.Unsubscribe<PlayerPlaceChangedSignal>(OnPlayerPlaceChanged);
			SignalBus.Unsubscribe<PlayerCellPassedSignal>(OnPlayerCellPassed);
			SignalBus.Unsubscribe<PlayerPortalPassedSignal>(OnPlayerPortalPassed);
		}

		private void AddItem(int index, PlayerController player)
		{
			GameObject itemObject = Instantiate(_sourceItems, _sourceItems.transform.parent, false);
			itemObject.name = $"Player ({player.Model.Name})";
			itemObject.transform.localPosition = new Vector3(0f, -index * 200f);
			itemObject.SetActive(true);

			PlayerItem item = itemObject.GetComponent<PlayerItem>();
			item.SetPlayer(index, player);
			_items.Add(item);
		}

		private void OnPlayerPlaceChanged(PlayerPlaceChangedSignal signal)
		{
			for (int i = 0; i < _items.Count; i++)
			{
				if (_items[i].Player.Model == signal.Player.Model)
				{
					bool up = signal.PrevPlace > signal.Place;
					_items[i].MoveToPlace(signal.Place, up);
					_items[i].UpdatePlace();
					break;
				}
			}
		}

		private void OnPlayerCellPassed(PlayerCellPassedSignal signal)
		{
			UpdateCellForPlayer(signal.Player);
		}

		private void OnPlayerPortalPassed(PlayerPortalPassedSignal signal)
		{
			UpdateCellForPlayer(signal.Player);
		}

		private void UpdateCellForPlayer(PlayerController player)
		{
			for (int i = 0; i < _items.Count; i++)
			{
				if (_items[i].Player.Model == player.Model)
				{
					int cellIndex = ((Cell)player.Avatar.CurrentPoint).Index;
					_items[i].UpdateCell(cellIndex);
					break;
				}
			}
		}
	}
}
