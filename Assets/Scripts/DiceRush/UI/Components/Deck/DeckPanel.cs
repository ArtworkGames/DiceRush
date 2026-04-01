using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using System.Collections.Generic;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Components.Deck
{
	public class DeckPanel : MonoBehaviour
	{
		[SerializeField] private ConfirmButton _confirmButton;
		[SerializeField] private DeckButton _deckButton;
		[SerializeField] private GameObject _sourceCardButton;

		public DeckButton DeckButton => _deckButton;

		private List<CardButton> _cards;
		private CardButton _selectedCard;
		private bool _confirmSelected;

		private void Awake()
		{
			_sourceCardButton.SetActive(false);
		}

		private void Start()
		{
			_confirmButton.OnConfirm += OnConfirm;
		}

		public async UniTask<CardModel> SelectCard(List<CardModel> cardModels, int totalCardsCount)
		{
			_selectedCard = null;
			_confirmSelected = false;

			ShowCards(cardModels);
			await UniTask.WaitUntil(() => IsAllCardsShown());

			EnableCards();
			await _confirmButton.Show();

			await UniTask.WaitUntil(() => _selectedCard != null || _confirmSelected);

			_confirmButton.Hide().Forget();
			HideUnselectedCards();

			_cards.Clear();

			if (_selectedCard != null)
			{
				await _selectedCard.ShowSelected();
				_selectedCard.HideSelected().Forget();

				return _selectedCard.Model;
			}

			return null;
		}

		private void ShowCards(List<CardModel> cardModels)
		{
			float cardsSpace = 440f;
			float x = -(cardModels.Count - 1) * cardsSpace / 2f;

			_cards = new List<CardButton>();
			for (int i = 0; i < cardModels.Count; i++)
			{
				AddCard(cardModels[i], (cardModels.Count - i - 1) * 0.05f, i * 0.05f, new Vector3(x, -740f, 0f));
				x += cardsSpace;
			}
		}

		private void AddCard(CardModel cardModel, float showDelay, float hideDelay, Vector3 destPos)
		{
			GameObject cardObject = Instantiate(_sourceCardButton, _sourceCardButton.transform.parent, false);
			cardObject.name = $"{cardModel.Type}Card";
			cardObject.transform.localPosition = _deckButton.transform.localPosition;
			//cardObject.transform.localPosition = new Vector3(destPos.x, -1600f, 0f);
			cardObject.SetActive(true);

			CardButton card = cardObject.GetComponent<CardButton>();
			card.OnSelect += OnCardSelect;
			_cards.Add(card);

			card.Show(cardModel, showDelay, hideDelay, destPos);
		}

		private bool IsAllCardsShown()
		{
			for (int i = 0; i < _cards.Count; i++)
			{
				if (!_cards[i].IsShown)
				{
					return false;
				}
			}
			return true;
		}

		private void EnableCards()
		{
			for (int i = 0; i < _cards.Count; i++)
			{
				_cards[i].EnableButton();
			}
		}

		private void OnCardSelect(CardButton card)
		{
			_selectedCard = card;
		}

		private void HideUnselectedCards()
		{
			for (int i = 0; i < _cards.Count; i++)
			{
				if (_cards[i] != _selectedCard)
				{
					_cards[i].Hide();
				}
			}
		}

		private void OnConfirm()
		{
			_confirmSelected = true;
		}
	}
}
