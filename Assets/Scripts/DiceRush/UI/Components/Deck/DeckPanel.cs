using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace StepanoffGames.DiceRush.UI.Components.Deck
{
	public class DeckPanel : MonoBehaviour
	{
		[SerializeField] private Button _confirmButton;
		[SerializeField] private TMP_Text _cardsCountText;

		private List<DeckPanelCard> _cards;
		private DeckPanelCard _selectedCard;
		private bool _confirmSelected;

		private void Start()
		{
			_confirmButton.gameObject.SetActive(false);
			_confirmButton.onClick.AddListener(OnConfirmButtonClick);
		}

		public async UniTask<CardModel> SelectCard(List<CardModel> cardModels, int totalCardsCount)
		{
			_selectedCard = null;
			_confirmSelected = false;

			_cardsCountText.text = $"Cards: {totalCardsCount}";

			await ShowCards(cardModels);
			_confirmButton.gameObject.SetActive(true);

			await UniTask.WaitUntil(() => _selectedCard != null || _confirmSelected);

			_confirmButton.gameObject.SetActive(false);
			HideUnselectedCards();

			_cards.Clear();

			if (_selectedCard != null)
			{
				await _selectedCard.ShowSelected();
				_selectedCard.HideSelected().Forget();

				_cardsCountText.text = "";

				return _selectedCard.Model;
			}

			_cardsCountText.text = "";

			return null;
		}

		private async UniTask ShowCards(List<CardModel> cardModels)
		{
			_cards = new List<DeckPanelCard>();
			for (int i = 0; i < cardModels.Count; i++)
			{
				await AddCard(cardModels[i]);
			}

			ArrangeCards();

			for (int i = 0; i < _cards.Count; i++)
			{
				if (i > 0) await UniTask.WaitForSeconds(0.1f);
				_cards[i].Show();
			}
		}

		private async UniTask AddCard(CardModel cardModel)
		{
			string cardName = $"{cardModel.Type}Card";
			string cardPath = $"Game/Deck/{cardName}.prefab";
			var handle = Addressables.LoadAssetAsync<GameObject>(cardPath);
			await UniTask.WaitUntil(() => handle.IsDone);

			GameObject cardObject = Instantiate(handle.Result, transform, false);
			cardObject.name = cardName;
			cardObject.transform.localScale = Vector3.one * 0.75f;
			cardObject.transform.localPosition = new Vector3(0f, -350f, 0f);

			DeckPanelCard card = cardObject.GetComponent<DeckPanelCard>();
			card.Model = cardModel;
			card.OnSelect += OnCardSelect;
			_cards.Add(card);
		}

		private void ArrangeCards()
		{
			float cardsSpace = 440f;
			float x = -(_cards.Count - 1) * cardsSpace / 2f;

			for (int i = 0; i < _cards.Count; i++)
			{
				_cards[i].transform.localPosition = new Vector3(x, _cards[i].transform.localPosition.y, 0f);
				x += cardsSpace;
			}
		}

		private void OnCardSelect(DeckPanelCard card)
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

		//private void ClearCards()
		//{
		//	for (int i = 0; i < _cards.Count; i++)
		//	{
		//		_cards[i].Model = null;
		//		_cards[i].OnSelect -= OnCardSelect;

		//		Destroy(_cards[i].gameObject);
		//	}

		//	_cards.Clear();
		//}

		private void OnConfirmButtonClick()
		{
			_confirmSelected = true;
		}
	}
}
