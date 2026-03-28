using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.UI.Components.Deck;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace StepanoffGames.DiceRush.UI.Popups.Deck.DescriptionPopup
{
	public class DeckDescriptionCardItem : MonoBehaviour
	{
		[SerializeField] private Transform _card;

		public CardModel CardModel => _cardModel;
		private CardModel _cardModel;

		private void OnDestroy()
		{
			_cardModel = null;
		}

		public void SetModel(CardModel cardModel)
		{
			_cardModel = cardModel;
			LoadCard(cardModel).Forget();
		}

		private async UniTask LoadCard(CardModel cardModel)
		{
			string cardName = $"{cardModel.Type}Card";
			string cardPath = $"Game/Deck/{cardName}.prefab";
			var handle = Addressables.LoadAssetAsync<GameObject>(cardPath);
			await UniTask.WaitUntil(() => handle.IsDone);

			GameObject cardObject = Instantiate(handle.Result, _card, false);
			cardObject.name = cardName;

			DeckPanelCard card = cardObject.GetComponent<DeckPanelCard>();
			card.Model = cardModel;

			Button button = cardObject.GetComponent<Button>();
			Destroy(button);
		}
	}
}
