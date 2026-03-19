using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.UI.Components;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Popups.Deck.DescriptionPopup
{
	public class DeckDescriptionPopup : MonoBehaviour
	{
		[SerializeField] private GameObject _content;
		[Space]
		[SerializeField] private TMPTextLocalizer _cardsCountText;
		[SerializeField] private TMPTextLocalizer _cardsOfferText;
		[Space]
		[SerializeField] private Transform _diceLine;
		[SerializeField] private Transform _bagLine;
		[SerializeField] private Transform _battleLine;
		[Space]
		[SerializeField] private GameObject _sourceCardItem;

		private PlayerModel _player;

		private List<DeckDescriptionCardItem> _diceCards = new List<DeckDescriptionCardItem>();
		private List<DeckDescriptionCardItem> _bagCards = new List<DeckDescriptionCardItem>();
		private List<DeckDescriptionCardItem> _battleCards = new List<DeckDescriptionCardItem>();

		private void Awake()
		{
			_sourceCardItem.SetActive(false);
		}

		private void Start()
		{
			Hide();
		}

		public void Show()
		{
			_content.SetActive(true);
		}

		public void Hide()
		{
			_content.SetActive(false);
		}

		public void SetPlayer(PlayerModel player)
		{
			_player = player;
			UpdateCards();
		}

		public void UpdateCards()
		{
			if (_player == null) return;

			_cardsCountText.SetParams(_player.Deck.Cards.Count.ToString());
			_cardsOfferText.SetParams(_player.CardsPerOffer.ToString());

			List<CardModel> diceCardModels = _player.Deck.GetCards(CardKind.Dice);
			UpdateCardsLine(_diceCards, diceCardModels, _diceLine);

			List<CardModel> bagCardModels = _player.Deck.GetCards(CardKind.Bag);
			UpdateCardsLine(_bagCards, bagCardModels, _bagLine);

			List<CardModel> battleCardModels = _player.Deck.GetCards(CardKind.Battle);
			UpdateCardsLine(_battleCards, battleCardModels, _battleLine);
		}

		private void UpdateCardsLine(List<DeckDescriptionCardItem> cards, List<CardModel> cardModels, Transform cardParent)
		{
			for (int i = 0; i < cardModels.Count; i++)
			{
				while (i < cards.Count && cards[i].CardModel != cardModels[i])
				{
					DeckDescriptionCardItem item = cards[i];
					cards.RemoveAt(i);
					Destroy(item.gameObject);
				}

				if (i == cards.Count)
				{
					DeckDescriptionCardItem item = AddCard(cardModels[i], cardParent);
					cards.Add(item);
				}
			}

			int cardsCount = cards.Count;
			for (int i = cardModels.Count; i < cardsCount; i++)
			{
				DeckDescriptionCardItem item = cards[cards.Count - 1];
				cards.RemoveAt(cards.Count - 1);
				Destroy(item.gameObject);
			}
		}

		private DeckDescriptionCardItem AddCard(CardModel cardModel, Transform cardParent)
		{
			GameObject cardObject = Instantiate(_sourceCardItem, cardParent, false);
			cardObject.name = $"CardItem ({cardModel.Type})";
			cardObject.SetActive(true);

			DeckDescriptionCardItem card = cardObject.GetComponent<DeckDescriptionCardItem>();
			card.SetModel(cardModel);

			return card;
		}
	}
}
