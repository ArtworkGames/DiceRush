using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data;
using StepanoffGames.DiceRush.Game;
using StepanoffGames.DiceRush.Game.Bag;
using StepanoffGames.DiceRush.Game.Deck;
using StepanoffGames.DiceRush.Game.Dice;
using StepanoffGames.DiceRush.Game.Map;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.DiceRush.Game.Ranking;
using StepanoffGames.DiceRush.Game.Xp;
using StepanoffGames.DiceRush.UI.Windows.CharactersDialogWindow;
using StepanoffGames.Localization;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using StepanoffGames.UI.Windows;
using StepanoffGames.UI.Windows.Signals;
using System.Collections.Generic;
using System.Threading;

namespace StepanoffGames.DiceRush.Tutorial.Chapters
{
	public class FirstRunChapter : TutorialChapter
	{
		private LocalizationManager _localizationManager;
		private WindowManager _windowManager;
		private DataManager _dataManager;
		private GameManager _gameManager;
		private MapController _mapController;
		private DiceController _diceController;
		private BagController _bagController;
		private DeckController _deckController;
		private XpManager _xpManager;
		private RankingManager _rankingManager;

		private PlayerController _hiPlayer;
		private List<PlayerController> _aiPlayers;

		private int hiPlayerDiceValue1;
		private int hiPlayerDiceValue2;

		override public async UniTask Run(CancellationToken ct)
		{
			Init();

			await ShowCamera(ct);

			await ShowHIPlayer(ct);

			await ShowFirstDialog(ct);

			await ShowAIPlayers(ct);

			await ShowSecondDialog(ct);

			await MakeFirstTurn(ct);

			await ShowThirdDialog(ct);

			await MakeSecondTurn(ct);

			await ShowFourthDialog(ct);

			await MakeThirdTurn(ct);

			Clear();
		}

		private void Init()
		{
			_localizationManager = ServiceLocator.Get<LocalizationManager>();
			_windowManager = ServiceLocator.Get<WindowManager>();
			_dataManager = ServiceLocator.Get<DataManager>();
			_gameManager = ServiceLocator.Get<GameManager>();
			_mapController = ServiceLocator.Get<MapController>();
			_diceController = ServiceLocator.Get<DiceController>();
			_bagController = ServiceLocator.Get<BagController>();
			_deckController = ServiceLocator.Get<DeckController>();
			_xpManager = ServiceLocator.Get<XpManager>();
			_rankingManager = ServiceLocator.Get<RankingManager>();

			_dataManager.Profile.ResetDescriptionPopupsShown();

			_aiPlayers = new List<PlayerController>();
			for (int i = 0; i < _gameManager.Players.Count; i++)
			{
				if (_gameManager.Players[i].Model.Type == Data.Models.PlayerType.HI)
				{
					_hiPlayer = _gameManager.Players[i];
				}
				else
				{
					_aiPlayers.Add(_gameManager.Players[i]);
				}

				_gameManager.Players[i].Model.Deck.Cards.Clear();
			}

			_deckController.SetShowEmptyMessages(false);
			_deckController.Panel.DeckButton.SetAlwaysHidden(true);
			_xpManager.SetActive(false);
			_xpManager.Panel.SetAlwaysHidden(true);
			_rankingManager.Panel.Hide(true, CancellationToken.None).Forget();
		}

		private void Clear()
		{
			_deckController.SetShowEmptyMessages(true);
			_xpManager.SetActive(true);
			_xpManager.Panel.SetAlwaysHidden(false);
			_rankingManager.Panel.Show(false, CancellationToken.None).Forget();

			_localizationManager = null;
			_windowManager = null;
			_dataManager = null;
			_gameManager = null;
			_mapController = null;
			_diceController = null;
			_bagController = null;
			_deckController = null;
			_xpManager = null;
			_rankingManager = null;

			_hiPlayer = null;
			_aiPlayers.Clear();
		}

		private async UniTask ShowCamera(CancellationToken ct)
		{
			_gameManager.Camera.SetTo(_mapController.GetCell(3).transform.position);
			await _gameManager.Camera.FocusOnCell(_mapController.StartCell, ct);
		}

		private async UniTask ShowHIPlayer(CancellationToken ct)
		{
			_hiPlayer.Avatar.gameObject.SetActive(true);
			await _hiPlayer.Avatar.MoveToPoint(_mapController.StartCell, ct);
		}

		private async UniTask ShowFirstDialog(CancellationToken ct)
		{
			_windowManager.SetCanCloseWindowByEsc(false);
			bool isDialogWindowClosed = false;

			SignalBus.Publish(new OpenWindowSignal(CharactersDialogWindow.PrefabName, new CharactersDialogWindowParams()
			{
				Phrases = new CharacterPhrase[] {
					new CharacterPhrase()
					{
						Side = CharacterSide.Left,
						AvatarName = $"{_hiPlayer.Avatar.Color}Player",
						Name = _hiPlayer.Model.Name,
						PhraseKey = "Tutorial:Dialog1:Player1Phrase1"
					}
				},
				OnClose = () =>
				{
					isDialogWindowClosed = true;
				}
			}));

			await UniTask.WaitUntil(() => isDialogWindowClosed);
			_windowManager.SetCanCloseWindowByEsc(true);
		}

		private async UniTask ShowAIPlayers(CancellationToken ct)
		{
			List<UniTask> tasks = new();
			tasks.Add(MoveHIPlayerToCellPlayerPosition(ct));

			for (int i = 0; i < _aiPlayers.Count; i++)
			{
				tasks.Add(ShowAIPlayer(_aiPlayers[i], i * 0.25f, ct));
			}
			await UniTask.WhenAll(tasks);
		}

		private async UniTask MoveHIPlayerToCellPlayerPosition(CancellationToken ct)
		{
			await UniTask.WaitForSeconds(0.5f, cancellationToken: ct);
			await _hiPlayer.Avatar.MoveToCurrentCellPlayerPosition(ct);
		}

		private async UniTask ShowAIPlayer(PlayerController player, float delay, CancellationToken ct)
		{
			await UniTask.WaitForSeconds(delay, cancellationToken: ct);

			player.Avatar.gameObject.SetActive(true);
			await player.Avatar.MoveToPoint(_mapController.StartCell, ct);
			await player.Avatar.MoveToCurrentCellPlayerPosition(ct);
		}

		private async UniTask ShowSecondDialog(CancellationToken ct)
		{
			_windowManager.SetCanCloseWindowByEsc(false);
			bool isDialogWindowClosed = false;

			SignalBus.Publish(new OpenWindowSignal(CharactersDialogWindow.PrefabName, new CharactersDialogWindowParams()
			{
				Phrases = new CharacterPhrase[] {
					new CharacterPhrase()
					{
						Side = CharacterSide.Left,
						AvatarName = $"{_aiPlayers[0].Avatar.Color}Player",
						Name = _aiPlayers[0].Model.Name,
						PhraseKey = "Tutorial:Dialog2:Player2Phrase1"
					},
					new CharacterPhrase()
					{
						Side = CharacterSide.Left,
						AvatarName = $"{_hiPlayer.Avatar.Color}Player",
						Name = _hiPlayer.Model.Name,
						PhraseKey = "Tutorial:Dialog2:Player1Phrase1"
					},
					new CharacterPhrase()
					{
						Side = CharacterSide.Right,
						AvatarName = $"SkeletonKing",
						Name = _localizationManager.GetString("SkeletonKing:Name"),
						PhraseKey = "Tutorial:Dialog2:SkeletonKingPhrase1",
						ButtonType = CharacterPhraseButtonType.Play
					}
				},
				OnClose = () =>
				{
					isDialogWindowClosed = true;
				}
			}));

			await UniTask.WaitUntil(() => isDialogWindowClosed);
			_windowManager.SetCanCloseWindowByEsc(true);
		}

		private async UniTask MakeFirstTurn(CancellationToken ct)
		{
			_mapController.ResetUsedCells();

			SignalBus.Publish(new TurnStartedSignal());

			PrepareDiceAndBagForHIFirstTurn();
			_deckController.SetDiceConfirmButtonAlwaysShown(true);
			_deckController.SetTokenConfirmButtonAlwaysShown(true);

			await _hiPlayer.Turn(ct);

			PrepareDiceAndBagForAIFirstTurn();

			List<UniTask> tasks = new();
			for (int i = 0; i < _aiPlayers.Count; i++)
			{
				tasks.Add(_aiPlayers[i].Turn(ct));
			}
			await UniTask.WhenAll(tasks);

			_diceController.ClearPredefinedValues();
			_bagController.ResetAcceptedCellTypes();
			_bagController.ClearPredefinedCellTypes();
			_deckController.SetDiceConfirmButtonAlwaysShown(false);
			_deckController.SetTokenConfirmButtonAlwaysShown(false);

			SignalBus.Publish(new TurnEndedSignal());
		}

		private void PrepareDiceAndBagForHIFirstTurn()
		{
			hiPlayerDiceValue1 = UnityEngine.Random.Range(4, 7);
			hiPlayerDiceValue2 = UnityEngine.Random.Range(hiPlayerDiceValue1 - 2, hiPlayerDiceValue1);
			_diceController.AddPredefinedValue(hiPlayerDiceValue1);
			_diceController.AddPredefinedValue(hiPlayerDiceValue2);

			//hiPlayerDiceValue1 = 6;
			//hiPlayerDiceValue2 = 4;

			_bagController.ClearAcceptedCellTypes();
			_bagController.AddAcceptedCellType(CellType.Enemy);
			_bagController.AddAcceptedCellType(CellType.MoveBackward);

			_bagController.AddPredefinedCellType(CellType.MoveBackward);
			_bagController.AddPredefinedCellType(CellType.Enemy);
		}

		private void PrepareDiceAndBagForAIFirstTurn()
		{
			Randomizer randomizer = new Randomizer(UnityEngine.Mathf.Min(4, hiPlayerDiceValue1 - 1), 3);
			List<int> values = new List<int>();
			for (int i = 0; i < 3; i++)
			{
				//int value = UnityEngine.Random.Range(1, hiPlayerPredefinedValue1);
				int value = randomizer.GetNextIndex() + 1;
				values.Add(value);
				_diceController.AddPredefinedValue(value);
			}
			for (int i = 1; i < 3; i++)
			{
				int value = UnityEngine.Random.Range(UnityEngine.Mathf.Max(1, values[i] - 2), values[i]);
				_diceController.AddPredefinedValue(value);
			}

			_bagController.AddPredefinedCellType(CellType.Enemy);
			for (int i = 0; i < 2; i++)
			{
				_bagController.AddPredefinedCellType(CellType.MoveBackward);
			}
			for (int i = 0; i < 2; i++)
			{
				_bagController.AddPredefinedCellType(CellType.Enemy);
			}

			//_diceController.AddPredefinedValue(5);
			//_diceController.AddPredefinedValue(1);
			//_diceController.AddPredefinedValue(3);
			//_diceController.AddPredefinedValue(2);
			//_diceController.AddPredefinedValue(3);

			//_bagController.AddPredefinedCellType(CellType.Enemy);
			//_bagController.AddPredefinedCellType(CellType.MoveBackward);
			//_bagController.AddPredefinedCellType(CellType.MoveBackward);
		}

		private async UniTask ShowThirdDialog(CancellationToken ct)
		{
			_windowManager.SetCanCloseWindowByEsc(false);
			bool isDialogWindowClosed = false;

			SignalBus.Publish(new OpenWindowSignal(CharactersDialogWindow.PrefabName, new CharactersDialogWindowParams()
			{
				Phrases = new CharacterPhrase[] {
					new CharacterPhrase()
					{
						Side = CharacterSide.Right,
						AvatarName = $"SkeletonKing",
						Name = _localizationManager.GetString("SkeletonKing:Name"),
						PhraseKey = "Tutorial:Dialog3:SkeletonKingPhrase1"
					},
					new CharacterPhrase()
					{
						Side = CharacterSide.Left,
						AvatarName = $"{_hiPlayer.Avatar.Color}Player",
						Name = _hiPlayer.Model.Name,
						PhraseKey = "Tutorial:Dialog3:Player1Phrase1"
					},
					new CharacterPhrase()
					{
						ClearFlag = CharacterClearFlag.HideAll,
						Side = CharacterSide.Left,
						AvatarName = $"LadyMorra",
						Name = _localizationManager.GetString("LadyMorra:Name"),
						PhraseKey = "Tutorial:Dialog3:LadyMorraPhrase1"
					},
					new CharacterPhrase()
					{
						Side = CharacterSide.Right,
						AvatarName = $"SkeletonKing",
						Name = _localizationManager.GetString("SkeletonKing:Name"),
						PhraseKey = "Tutorial:Dialog3:SkeletonKingPhrase2"
					},
					new CharacterPhrase()
					{
						Side = CharacterSide.Left,
						AvatarName = $"LadyMorra",
						Name = _localizationManager.GetString("LadyMorra:Name"),
						PhraseKey = "Tutorial:Dialog3:LadyMorraPhrase2"
					},
					new CharacterPhrase()
					{
						Side = CharacterSide.Right,
						AvatarName = $"SkeletonKing",
						Name = _localizationManager.GetString("SkeletonKing:Name"),
						PhraseKey = "Tutorial:Dialog3:SkeletonKingPhrase3",
						ButtonType = CharacterPhraseButtonType.Play
					}
				},
				OnClose = () =>
				{
					isDialogWindowClosed = true;
				}
			}));

			await UniTask.WaitUntil(() => isDialogWindowClosed);
			_windowManager.SetCanCloseWindowByEsc(true);
		}

		private async UniTask MakeSecondTurn(CancellationToken ct)
		{
			_mapController.ResetUsedCells();

			SignalBus.Publish(new TurnStartedSignal());

			PrepareDiceAndBagForSecondTurn();
			_deckController.SetTokenConfirmButtonAlwaysShown(true);

			List<UniTask> tasks = new();
			tasks.Add(_hiPlayer.Turn(ct));
			for (int i = 0; i < _aiPlayers.Count; i++)
			{
				tasks.Add(_aiPlayers[i].Turn(ct));
			}
			await UniTask.WhenAll(tasks);

			_diceController.ClearPredefinedValues();
			_bagController.ResetAcceptedCellTypes();
			_bagController.ClearPredefinedCellTypes();
			_deckController.SetTokenConfirmButtonAlwaysShown(false);

			SignalBus.Publish(new TurnEndedSignal());
		}

		private void PrepareDiceAndBagForSecondTurn()
		{
			int hiPlayerDiceValue = UnityEngine.Random.Range(hiPlayerDiceValue2 + 1, 7);
			_diceController.AddPredefinedValue(hiPlayerDiceValue);

			int nextHIPlayerCellIndex = ((Cell)_hiPlayer.Avatar.CurrentPoint).Index + hiPlayerDiceValue;
			for (int i = 0; i < _aiPlayers.Count; i++)
			{
				int aiPlayerCellIndex = ((Cell)_aiPlayers[i].Avatar.CurrentPoint).Index;
				int aiPlayerDiceValue = UnityEngine.Random.Range(1, UnityEngine.Mathf.Min(7, nextHIPlayerCellIndex - aiPlayerCellIndex + 1));
				_diceController.AddPredefinedValue(aiPlayerDiceValue);
			}

			_bagController.ClearAcceptedCellTypes();
			_bagController.AddAcceptedCellType(CellType.Enemy);
			_bagController.AddAcceptedCellType(CellType.MoveForward);
			_bagController.AddAcceptedCellType(CellType.MoveBackward);
			_bagController.AddAcceptedCellType(CellType.Portal);

			_bagController.AddPredefinedCellType(CellType.MoveForward);
			_bagController.AddPredefinedCellType(CellType.Portal);
			_bagController.AddPredefinedCellType(CellType.Portal);
			_bagController.AddPredefinedCellType(CellType.Portal);
			_bagController.AddPredefinedCellType(CellType.Portal);
		}

		private async UniTask ShowFourthDialog(CancellationToken ct)
		{
			_windowManager.SetCanCloseWindowByEsc(false);
			bool isDialogWindowClosed = false;

			SignalBus.Publish(new OpenWindowSignal(CharactersDialogWindow.PrefabName, new CharactersDialogWindowParams()
			{
				Phrases = new CharacterPhrase[] {
					new CharacterPhrase()
					{
						Side = CharacterSide.Left,
						AvatarName = $"LadyMorra",
						Name = _localizationManager.GetString("LadyMorra:Name"),
						PhraseKey = "Tutorial:Dialog4:LadyMorraPhrase1"
					},
					new CharacterPhrase()
					{
						Side = CharacterSide.Right,
						AvatarName = $"SkeletonKing",
						Name = _localizationManager.GetString("SkeletonKing:Name"),
						PhraseKey = "Tutorial:Dialog4:SkeletonKingPhrase1"
					},
					new CharacterPhrase()
					{
						Side = CharacterSide.Left,
						AvatarName = $"LadyMorra",
						Name = _localizationManager.GetString("LadyMorra:Name"),
						PhraseKey = "Tutorial:Dialog4:LadyMorraPhrase2"
					},
					new CharacterPhrase()
					{
						Side = CharacterSide.Right,
						AvatarName = $"SkeletonKing",
						Name = _localizationManager.GetString("SkeletonKing:Name"),
						PhraseKey = "Tutorial:Dialog4:SkeletonKingPhrase2"
					},
					new CharacterPhrase()
					{
						Side = CharacterSide.Left,
						AvatarName = $"LadyMorra",
						Name = _localizationManager.GetString("LadyMorra:Name"),
						PhraseKey = "Tutorial:Dialog4:LadyMorraPhrase3"
					},
					new CharacterPhrase()
					{
						Side = CharacterSide.Right,
						AvatarName = $"SkeletonKing",
						Name = _localizationManager.GetString("SkeletonKing:Name"),
						PhraseKey = "Tutorial:Dialog4:SkeletonKingPhrase3"
					},
					new CharacterPhrase()
					{
						ClearFlag = CharacterClearFlag.HideAll,
						Side = CharacterSide.Left,
						AvatarName = $"{_hiPlayer.Avatar.Color}Player",
						Name = _hiPlayer.Model.Name,
						PhraseKey = "Tutorial:Dialog4:Player1Phrase1",
						ButtonType = CharacterPhraseButtonType.Play
					}
				},
				OnClose = () =>
				{
					isDialogWindowClosed = true;
				}
			}));

			await UniTask.WaitUntil(() => isDialogWindowClosed);
			_windowManager.SetCanCloseWindowByEsc(true);
		}

		private async UniTask MakeThirdTurn(CancellationToken ct)
		{
			_mapController.ResetUsedCells();

			SignalBus.Publish(new TurnStartedSignal());

			PrepareDiceAndBagForThirdTurn();
			_deckController.SetTokenConfirmButtonAlwaysShown(true);
			_deckController.Panel.DeckButton.SetAlwaysHidden(false);

			List<UniTask> tasks = new();
			tasks.Add(_hiPlayer.Turn(ct));
			for (int i = 0; i < _aiPlayers.Count; i++)
			{
				tasks.Add(_aiPlayers[i].Turn(ct));
			}
			await UniTask.WhenAll(tasks);

			_diceController.ClearPredefinedValues();
			_bagController.ResetAcceptedCellTypes();
			_bagController.ClearPredefinedCellTypes();
			_deckController.SetTokenConfirmButtonAlwaysShown(false);

			SignalBus.Publish(new TurnEndedSignal());
		}

		private void PrepareDiceAndBagForThirdTurn()
		{
			_bagController.ResetAcceptedCellTypes();

			_bagController.AddPredefinedCellType(CellType.Reward);
			_bagController.AddPredefinedCellType(CellType.Reward);
			_bagController.AddPredefinedCellType(CellType.Reward);
			_bagController.AddPredefinedCellType(CellType.Reward);
		}
	}
}
