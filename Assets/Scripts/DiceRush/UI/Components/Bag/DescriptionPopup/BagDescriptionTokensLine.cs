using StepanoffGames.DiceRush.Game.Bag;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Components.Bag.DescriptionPopup
{
	public class BagDescriptionTokensLine : MonoBehaviour
	{
		[SerializeField] private BagDescriptionTokensGroup _regularTokensGroup;
		[SerializeField] private BagDescriptionTokensGroup _removedTokensGroup;
		[SerializeField] private BagDescriptionTokensGroup _addedTokensGroup;

		public void ShowTokens(TokensSetDescription tokensSetDescription)
		{
			_regularTokensGroup.ShowTokens(tokensSetDescription.RegularCount);
			_removedTokensGroup.ShowTokens(tokensSetDescription.RemovedCount);
			_addedTokensGroup.ShowTokens(tokensSetDescription.AddedCount);
		}
	}
}
