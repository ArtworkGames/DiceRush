using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Deck.Signals;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.DiceRush.Game.Players.Signals;
using StepanoffGames.DiceRush.UI.Popups.Deck.DescriptionPopup;
using StepanoffGames.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace StepanoffGames.DiceRush.UI.Components.Deck
{
	public class DeckButton : TweenButton
	{
		[Space]
		[SerializeField] private HideablePanel _hideablePanel;
		[Space]
		[SerializeField] private TMP_Text _playerNameText;
		[SerializeField] private TMP_Text _cardsCountText;
		[SerializeField] private TMP_Text _cardsOfferText;
		[Space]
		[SerializeField] private DeckDescriptionPopup _descriptionPopup;

		public PlayerController Player => _player;
		private PlayerController _player;

		private void Awake()
		{
			mode = TweenButtonMode.Focusable;
		}

		private void Start()
		{
			SignalBus.Subscribe<PlayerTurnStartedSignal>(OnPlayerTurnStarted);
			SignalBus.Subscribe<PlayerTurnEndedSignal>(OnPlayerTurnEnded);
			SignalBus.Subscribe<PlayerCardsPerOfferChangedSignal>(OnPlayerCardsPerOfferChanged);
			SignalBus.Subscribe<PlayerCardsChangedSignal>(OnPlayerCardsChanged);
		}

		override protected void OnDestroy()
		{
			base.OnDestroy();

			_player = null;

			SignalBus.Unsubscribe<PlayerTurnStartedSignal>(OnPlayerTurnStarted);
			SignalBus.Unsubscribe<PlayerTurnEndedSignal>(OnPlayerTurnEnded);
			SignalBus.Unsubscribe<PlayerCardsPerOfferChangedSignal>(OnPlayerCardsPerOfferChanged);
			SignalBus.Unsubscribe<PlayerCardsChangedSignal>(OnPlayerCardsChanged);
		}

		private async UniTask Show()
		{
			await _hideablePanel.Show();
		}

		private async UniTask Hide()
		{
			_descriptionPopup.Hide();
			await _hideablePanel.Hide();
		}

		public async void SetPlayer(PlayerController player)
		{
			if (player == _player) return;

			if (_hideablePanel.IsShown)
			{
				await Hide();
			}

			_player = player;

			_playerNameText.text = _player.Model.Name;
			UpdateCount();
			_descriptionPopup.SetPlayer(_player.Model);

			await Show();
		}

		public async void ClearPlayer()
		{
			_player = null;
			if (_hideablePanel.IsShown)
			{
				await Hide();
			}
		}

		private void OnPlayerTurnStarted(PlayerTurnStartedSignal signal)
		{
			if (signal.Player.Model.Type != PlayerType.HI) return;
			SetPlayer(signal.Player);
		}

		private void OnPlayerTurnEnded(PlayerTurnEndedSignal signal)
		{
			if (signal.Player == _player)
			{
				ClearPlayer();
			}
		}

		private void OnPlayerCardsPerOfferChanged(PlayerCardsPerOfferChangedSignal signal)
		{
			if (_player != null && signal.Player == _player.Model)
			{
				UpdateCount();
			}
		}

		private void OnPlayerCardsChanged(PlayerCardsChangedSignal signal)
		{
			if (_player != null && signal.Player == _player.Model)
			{
				UpdateCount();
			}
		}

		public void UpdateCount()
		{
			_cardsCountText.text = _player.Model.Deck.Cards.Count.ToString();
			_cardsOfferText.text = _player.Model.CardsPerOffer.ToString();

			_descriptionPopup.UpdateCards();
		}

		override public void OnPointerEnter(PointerEventData eventData)
		{
			_descriptionPopup.Show();

			base.OnPointerEnter(eventData);
		}

		override public void OnPointerExit(PointerEventData eventData)
		{
			_descriptionPopup.Hide();

			base.OnPointerExit(eventData);
		}
	}
}
