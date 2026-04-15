using Cysharp.Threading.Tasks;
using StepanoffGames.Cameras.Signals;
using StepanoffGames.DiceRush.Data;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Map;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.DiceRush.Game.Xp;
using StepanoffGames.DiceRush.Tutorial;
using StepanoffGames.DiceRush.UI.Messages.Signals;
using StepanoffGames.DiceRush.UI.Windows.GamePauseWindow;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using StepanoffGames.UI.Windows;
using StepanoffGames.UI.Windows.Signals;
using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StepanoffGames.DiceRush.Game
{
	public enum GameMode
	{
		Tutorial,
		Campaign,
		LocalMultiplayer
	}

	public class GameManager : MonoBehaviour, IService
	{
		public static GameMode GameMode = GameMode.Campaign;

		[SerializeField] private List<Camera> _cameras;
		[Space]
		[SerializeField] private GameCamera _camera;
		[SerializeField] private PlayerAvatar[] _avatars;
		[Space]
		[SerializeField] private Camera _hudCamera;
		[SerializeField] private Camera _uiCamera;
		[Space]
		[SerializeField] private TMP_Text _timeLabel;

		public GameCamera Camera => _camera;
		public List<PlayerController> Players => _players;

		public int HiPlayersCount => _hiPlayersCount;
		private int _hiPlayersCount;

		public Camera HUDCamera => _hudCamera;
		public Camera UICamera => _uiCamera;

		private WindowManager _windowManager;
		private MapController _mapController;
		private DataManager _dataManager;
		private XpManager _xpManager;
		private TutorialManager _tutorialManager;

		private List<PlayerController> _players;

		private float startTime;
		private int turnsCount;

		private CancellationTokenSource cts;

		private void Awake()
		{
			ServiceLocator.Register(this);

			SignalBus.Publish(new AddCamerasSignal(_cameras));
		}

		private async void Start()
		{
			_windowManager = ServiceLocator.Get<WindowManager>();
			_mapController = ServiceLocator.Get<MapController>();
			_dataManager = ServiceLocator.Get<DataManager>();
			_xpManager = ServiceLocator.Get<XpManager>();
			_tutorialManager = ServiceLocator.Get<TutorialManager>();

			await _mapController.CreateMap();

			InitPlayers();

			startTime = Time.time;
			//Time.timeScale = 50f;

			cts?.Cancel();
			cts?.Dispose();
			cts = new CancellationTokenSource();

			if (GameMode == GameMode.Tutorial)
			{
				await _tutorialManager.RunChapter(TutorialChapterId.FirstRun, cts.Token);
			}
			else
			{
				await ShowPlayers(cts.Token);
			}

			GameLoop(cts.Token).Forget();
		}

		private void OnDestroy()
		{
			cts?.Cancel();
			cts?.Dispose();
			cts = null;

			ServiceLocator.Unregister<GameManager>();

			_windowManager = null;
			_mapController = null;
			_dataManager = null;
			_xpManager = null;

			for (int i = 0; i < _players.Count; i++)
			{
				_players[i].Destroy();
			}
		}

		private void InitPlayers()
		{
			for (int i = 0; i < _avatars.Length; i++)
			{
				_avatars[i].gameObject.SetActive(i < _dataManager.Players.Count);
			}

			_players = new List<PlayerController>();
			PlayerController prevHiPlayer = null;
			for (int i = 0; i < _dataManager.Players.Count; i++)
			{
				switch (_dataManager.Players[i].Type)
				{
					case PlayerType.HI:
						PlayerController hiPlayer = new HIPlayerController(_dataManager.Players[i], _avatars[i], prevHiPlayer);
						_players.Add(hiPlayer);
						prevHiPlayer = hiPlayer;
						_hiPlayersCount++;
						break;
					case PlayerType.AI:
						_players.Add(new AIPlayerController(_dataManager.Players[i], _avatars[i]));
						break;
				}
			}

			Cell startCell = _mapController.StartCell;
			for (int i = 0; i < _players.Count; i++)
			{
				_avatars[i].SetToPosition(_mapController.PlayerInitialPosition.position, startCell);
				_avatars[i].gameObject.SetActive(false);
				_players[i].Model.IsFinished = false;
			}
		}

		private async UniTask ShowPlayers(CancellationToken ct)
		{
			_camera.SetTo(_mapController.StartCell.transform.position);

			List<UniTask> tasks = new();
			for (int i = 0; i < _players.Count; i++)
			{
				tasks.Add(ShowPlayer(_players[i], 0.5f + i * 0.25f, ct));
			}
			await UniTask.WhenAll(tasks);

			await UniTask.WaitForSeconds(0.5f, cancellationToken: ct);
		}

		private async UniTask ShowPlayer(PlayerController player, float delay, CancellationToken ct)
		{
			await UniTask.WaitForSeconds(delay, cancellationToken: ct);

			player.Avatar.gameObject.SetActive(true);
			await player.Avatar.MoveToPoint(_mapController.StartCell, ct);
			await player.Avatar.MoveToCurrentCellPlayerPosition(ct);
		}

		private async UniTask GameLoop(CancellationToken ct)
		{
			turnsCount = 0;
			do
			{
				turnsCount++;

				_mapController.ResetUsedCells();

				SignalBus.Publish(new TurnStartedSignal());
				if (_hiPlayersCount < 2)
				{
					SignalBus.Publish(new ShowMessageSignal("Message:StartTurn", turnsCount.ToString()));
				}

				List<UniTask> tasks = new();
				for (int i = 0; i < _players.Count; i++)
				{
					tasks.Add(_players[i].Turn(ct));
				}
				await UniTask.WhenAll(tasks);

				SignalBus.Publish(new TurnEndedSignal());

				if (IsFinished()) break;
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

			for (int i = 0; i < _mapController.Cells.Length; i++)
			{
				Cell cell = _mapController.Cells[i];
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

					CellType cellType = currentCellTypes[UnityEngine.Random.Range(0, currentCellTypes.Count)];
					cell.SetType(cellType);
				}
			}
		}

		private void Update()
		{
			float time = Time.time - startTime;
			string timeStr = TimeSpan.FromSeconds(time).ToString(@"mm\:ss");
			_timeLabel.text = timeStr;

			if (_windowManager != null && _windowManager.CanUseHotkeysExternal())
			{
				if (Keyboard.current.escapeKey.wasPressedThisFrame)
				{
					SignalBus.Publish(new OpenWindowSignal(GamePauseWindow.PrefabName));
				}
			}
		}

		public PlayerController GetPlayer(PlayerModel playerModel)
		{
			for (int i = 0; i < _players.Count; i++)
			{
				if (_players[i].Model == playerModel)
				{
					return _players[i];
				}
			}
			return null;
		}

		private bool IsFinished()
		{
			int playersOnFinish = 0;

			for (int i = 0; i < _players.Count; i++)
			{
				if (_players[i].Model.IsFinished)
				{
					playersOnFinish++;
				}
			}

			//if ((_playerCount - playersOnFinish) <= 1)
			if ((_players.Count - playersOnFinish) == 0)
				return true;

			return false;
		}
	}
}
