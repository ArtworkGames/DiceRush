using Cysharp.Threading.Tasks;
using StepanoffGames.Cameras.Signals;
using StepanoffGames.DiceRush.Data;
using StepanoffGames.DiceRush.Gameplay.Xp;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace StepanoffGames.DiceRush.Gameplay
{
	public class Level : MonoBehaviour
	{
		private static Level _instance;
		public static Level Instance => _instance;

		[SerializeField] private List<Camera> _cameras;
		[Space]
		[SerializeField] private LevelCamera _camera;
		[SerializeField] private Map _map;
		[SerializeField] private Way _way;
		[SerializeField] private Fork _fork;
		[SerializeField] private Avatar[] _avatars;
		[Space]
		[SerializeField] private Camera _hudCamera;
		[SerializeField] private Dice _dice;
		[SerializeField] private Bag _bag;
		[SerializeField] private Deck _deck;
		[Space]
		[Range(1, 4)]
		[SerializeField] private int _playersCount;
		[Space]
		[SerializeField] private XpManager _xpManager;
		//[Space]
		//[SerializeField] private TMP_Text _movesCount;
		//[SerializeField] private SpriteRenderer _playerColor;

		public LevelCamera Camera => _camera;
		public Map Map => _map;
		public Way Way => _way;
		public Fork Fork => _fork;
		public List<Player> Players => _players;

		public Camera HUDCamera => _hudCamera;
		public Dice Dice => _dice;
		public Bag Bag => _bag;
		public Deck Deck => _deck;

		public XpManager XpManager => _xpManager;

		private DataManager _dataManager;
		private List<Player> _players;

		private int movesCount;

		private void Awake()
		{
			_instance = this;

			SignalBus.Publish(new AddCamerasSignal(_cameras));

			if ((_map == null) || !_map.gameObject.activeSelf)
				_map = GetComponentInChildren<Map>();
			_map.OnInited += OnMapInited;

			for (int i = 0; i < _avatars.Length; i++)
			{
				_avatars[i].gameObject.SetActive(i < _playersCount);
			}

			_dataManager = new DataManager();
			_players = new List<Player>();
			for (int i = 0; i < _playersCount; i++)
			{
				if (i == 0)
					_players.Add(new HIPlayer(_dataManager.Players[i], _avatars[i]));
				else
					_players.Add(new AIPlayer(_dataManager.Players[i], _avatars[i]));
			}
		}

		private void OnMapInited()
		{
			_map.OnInited -= OnMapInited;

			for (int i = 0; i < _playersCount; i++)
			{
				_avatars[i].SetToCellPlayerPosition(_map.StartCell);
			}

			GameLoop();
		}

		private async void GameLoop()
		{
			// VAR 1

			//movesCount = 0;
			//do
			//{
			//	movesCount++;
			//	//_movesCount.text = "Move: " + movesCount;

			//	for (int i = 0; i < _playerCount; i++)
			//	{
			//		//_playerColor.color = _players[i].Color;

			//		await _playerControllers[i].MoveForward();

			//		await UniTask.WaitForSeconds(0.1f);
			//	}

			//	if (IsFinished()) break;
			//}
			//while (true);


			// VAR 2

			//FillMap();

			//for (int i = 0; i < _playersCount; i++)
			//{
			//	PlayerLoop(_players[i]);
			//}


			// VAR 3

			movesCount = 0;
			do
			{
				movesCount++;
				//_movesCount.text = "Move: " + movesCount;

				_xpManager.StartMove();

				List<UniTask> tasks = new();
				for (int i = 0; i < _playersCount; i++)
				{
					//_playerColor.color = _players[i].Color;

					tasks.Add(_players[i].Move());
				}
				await UniTask.WhenAll(tasks);

				_xpManager.EndMove();

				await UniTask.WaitForSeconds(0.1f);

				if (IsFinished()) break;
			}
			while (true);
		}

		private async void PlayerLoop(Player playerController)
		{
			do
			{
				await playerController.Move();

				//if (playerController) break;
			}
			while (true);
		}

		private void FillMap()
		{
			List<CellType> commonCellTypes = new List<CellType>();

			commonCellTypes.Add(CellType.Reward);
			commonCellTypes.Add(CellType.Reward);

			commonCellTypes.Add(CellType.Enemy);
			commonCellTypes.Add(CellType.Enemy);
			commonCellTypes.Add(CellType.Enemy);

			commonCellTypes.Add(CellType.MoveForward);
			commonCellTypes.Add(CellType.MoveForward);
			commonCellTypes.Add(CellType.MoveForward);

			//commonCellTypes.Add(CellType.Portal1);

			List<CellType> moveBackwardCellTypes = new List<CellType>();

			moveBackwardCellTypes.Add(CellType.MoveBackward);
			moveBackwardCellTypes.Add(CellType.MoveBackward);
			moveBackwardCellTypes.Add(CellType.MoveBackward);
			moveBackwardCellTypes.Add(CellType.MoveBackward);
			moveBackwardCellTypes.Add(CellType.MoveBackward);

			for (int i = 0; i < _map.Cells.Length; i++)
			{
				Cell cell = _map.Cells[i];
				if (cell.Type == CellType.Empty)
				{
					List<CellType> currentCellTypes = new List<CellType>();
					currentCellTypes.AddRange(commonCellTypes);

					bool hasNearMoveBackwardCell = cell.HasNearCellWithSameType(CellType.MoveBackward);
					Debug.Log($"hasNearMoveBackwardCell: {hasNearMoveBackwardCell}");
					if (!hasNearMoveBackwardCell)
					{
						currentCellTypes.AddRange(moveBackwardCellTypes);
					}

					CellType cellType = currentCellTypes[Random.Range(0, currentCellTypes.Count)];
					cell.SetType(cellType);
				}
			}
		}

		private bool IsFinished()
		{
			int playersOnFinish = 0;

			for (int i = 0; i < _playersCount; i++)
			{
				if (_avatars[i].CurrentPoint is Cell && ((Cell)_avatars[i].CurrentPoint).Type == CellType.Finish)
				{
					playersOnFinish++;
				}
			}

			//if ((_playerCount - playersOnFinish) <= 1)
			if ((_playersCount - playersOnFinish) == 0)
				return true;

			return false;
		}
	}
}
