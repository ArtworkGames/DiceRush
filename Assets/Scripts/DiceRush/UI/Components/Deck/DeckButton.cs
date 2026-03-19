using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Deck.Signals;
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
		[SerializeField] private TMP_Text _cardsCountText;
		[SerializeField] private TMP_Text _cardsOfferText;
		[Space]
		[SerializeField] private DeckDescriptionPopup _descriptionPopup;

		public PlayerModel Player => _player;
		private PlayerModel _player;

		private void Awake()
		{
			mode = TweenButtonMode.Focusable;
		}

		private void Start()
		{
			SignalBus.Subscribe<PlayerCardsPerOfferChangedSignal>(OnPlayerCardsPerOfferChanged);
			SignalBus.Subscribe<PlayerCardsChangedSignal>(OnPlayerCardsChanged);
		}

		override protected void OnDestroy()
		{
			base.OnDestroy();

			SignalBus.Unsubscribe<PlayerCardsPerOfferChangedSignal>(OnPlayerCardsPerOfferChanged);
			SignalBus.Unsubscribe<PlayerCardsChangedSignal>(OnPlayerCardsChanged);
		}

		public void SetPlayer(PlayerModel player)
		{
			_player = player;
			UpdateCount();

			_descriptionPopup.SetPlayer(player);
		}

		private void OnPlayerCardsPerOfferChanged(PlayerCardsPerOfferChangedSignal signal)
		{
			if (signal.Player == _player)
			{
				UpdateCount();
			}
		}

		private void OnPlayerCardsChanged(PlayerCardsChangedSignal signal)
		{
			if (signal.Player == _player)
			{
				UpdateCount();
			}
		}

		public void UpdateCount()
		{
			_cardsCountText.text = _player.Deck.Cards.Count.ToString();
			_cardsOfferText.text = _player.CardsPerOffer.ToString();

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
