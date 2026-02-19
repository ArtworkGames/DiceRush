using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Players;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace StepanoffGames.DiceRush.Game
{
	public class Deck : MonoBehaviour
	{
		[SerializeField] private Button _confirmDiceButton;
		[SerializeField] private Button _confirmTileButton;

		private List<DeckCard> _cards;
		private DeckCard _selectedCard;
		private bool _confirmSelected;

		private void Start()
		{
			_confirmDiceButton.gameObject.SetActive(false);
			_confirmDiceButton.onClick.AddListener(OnConfirmButtonClick);

			_confirmTileButton.gameObject.SetActive(false);
			_confirmTileButton.onClick.AddListener(OnConfirmButtonClick);
		}

		public async UniTask<int> ConfirmDiceRoll(PlayerController player, int diceValue)
		{
			_selectedCard = null;
			_confirmSelected = false;

			await ShowCards(player, CardKind.Dice);

			if (_cards.Count == 0)
			{
				return diceValue;
			}

			_confirmDiceButton.gameObject.SetActive(true);

			await UniTask.WaitUntil(() => _selectedCard != null || _confirmSelected);

			_confirmDiceButton.gameObject.SetActive(false);
			HideUnselectedCards();

			if (_selectedCard != null)
			{
				player.Model.Deck.RemoveCard(_selectedCard.Model);

				await _selectedCard.ShowSelected();
				diceValue = await _selectedCard.UseForDice(player, diceValue);
				await _selectedCard.HideSelected();

				//Level.Instance.Dice.ShowValue(diceValue);
				//Level.Instance.Way.ShowDiceValue(diceValue);
			}
			else
			{
				await UniTask.WaitForSeconds(0.5f);
			}

			ClearCards();

			return diceValue;
		}

		public async UniTask<CellType> ConfirmTileTake(PlayerController player, CellType tileType)
		{
			_selectedCard = null;
			_confirmSelected = false;

			await ShowCards(player, CardKind.Tile);

			if (_cards.Count == 0)
			{
				return tileType;
			}

			_confirmTileButton.gameObject.SetActive(true);

			await UniTask.WaitUntil(() => _selectedCard != null || _confirmSelected);

			_confirmTileButton.gameObject.SetActive(false);
			HideUnselectedCards();

			if (_selectedCard != null)
			{
				player.Model.Deck.RemoveCard(_selectedCard.Model);

				await _selectedCard.ShowSelected();
				tileType = await _selectedCard.UseForTile(player, tileType);
				await _selectedCard.HideSelected();

				//Level.Instance.Dice.ShowValue(diceValue);
				//Level.Instance.Way.ShowDiceValue(diceValue);
			}
			else
			{
				await UniTask.WaitForSeconds(0.5f);
			}

			ClearCards();

			return tileType;
		}

		private async UniTask ShowCards(PlayerController player, CardKind kind)
		{
			_cards = new List<DeckCard>();
			for (int i = 0; i < player.Model.Deck.Cards.Count; i++)
			{
				if (player.Model.Deck.Cards[i].Kind == kind)
				{
					await AddCard(player.Model.Deck.Cards[i]);
				}
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

			DeckCard card = cardObject.GetComponent<DeckCard>();
			card.Model = cardModel;
			card.OnSelect += OnCardSelect;
			_cards.Add(card);
		}

		private void OnCardSelect(DeckCard card)
		{
			_selectedCard = card;
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

		private void ClearCards()
		{
			for (int i = 0; i < _cards.Count; i++)
			{
				_cards[i].Model = null;
				_cards[i].OnSelect -= OnCardSelect;

				Destroy(_cards[i].gameObject);
			}

			_cards.Clear();
		}

		private void OnConfirmButtonClick()
		{
			_confirmSelected = true;
		}
	}
}
