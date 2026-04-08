using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.UI.Deck;
using StepanoffGames.UI.Windows;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace StepanoffGames.DiceRush.UI.Windows.ConfirmWindow
{
	public class ChestWindowParams : BaseWindowParams
	{
		public CardModel[] Cards;
		public Action<CardModel> OnSelect;
	}

	public class ChestWindow : BaseWindow<ChestWindowParams>
	{
		public static string PrefabName = "ChestWindow";

		[Space]
		[SerializeField] private Button _okButton;
		[SerializeField] private Transform _cardsParent;

		private List<CardView> _cards;
		private CardView _selectedCard;

		override protected void BeforeOpen()
		{
			_cards = new List<CardView>();
			for (int i = 0; i < Params.Cards.Length; i++)
			{
				AddCard(Params.Cards[i]).Forget();
			}
			//_okButton.interactable = false;
		}

		override protected void AfterOpen()
		{
			_okButton.onClick.AddListener(OnOkButtonClick);
		}

		override protected void BeforeClose()
		{
			_okButton.onClick.RemoveAllListeners();
		}

		override protected void AfterClose()
		{
			//Params.OnSelect?.Invoke(_selectedCard.Model);
			Params.OnSelect?.Invoke(null);
		}

		private async UniTask AddCard(CardModel cardModel)
		{
			string cardName = $"{cardModel.Type}Card";
			string cardPath = $"UI/Deck/{cardName}.prefab";
			var handle = Addressables.LoadAssetAsync<GameObject>(cardPath);
			await UniTask.WaitUntil(() => handle.IsDone);

			GameObject cardObject = Instantiate(handle.Result, _cardsParent, false);
			cardObject.name = cardName;

			CardView cardView = cardObject.GetComponent<CardView>();
			cardView.SetModel(cardModel);
			_cards.Add(cardView);
		}

		private void OnCardSelect(CardView card)
		{
			_selectedCard = card;
			for (int i = 0; i < _cards.Count; i++)
			{
				CanvasGroup cardCanvasGroup = _cards[i].GetComponent<CanvasGroup>();
				cardCanvasGroup.alpha = _cards[i] == card ? 1f : 0.5f;
			}
			_okButton.interactable = true;
		}

		private void OnOkButtonClick()
		{
			CloseWindow();
		}
	}
}
