using StepanoffGames.DiceRush.Game;
using StepanoffGames.DiceRush.Game.Bag;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Components.Bag.DescriptionPopup
{
	public class BagDescriptionPopup : MonoBehaviour
	{
		[SerializeField] private GameObject _content;
		[Space]
		[SerializeField] private BagDescriptionTokensLine _rewardTokensLine;
		[SerializeField] private BagDescriptionTokensLine _enemyTokensLine;
		[SerializeField] private BagDescriptionTokensLine _moveForwardTokensLine;
		[SerializeField] private BagDescriptionTokensLine _moveBackwardTokensLine;
		[SerializeField] private BagDescriptionTokensLine _portalTokensLine;

		private void Start()
		{
			Hide();
		}

		public void SetDescription(BagDescription bagDescription)
		{
			_rewardTokensLine.ShowTokens(bagDescription.Tokens[CellType.Reward]);
			_enemyTokensLine.ShowTokens(bagDescription.Tokens[CellType.Enemy]);
			_moveForwardTokensLine.ShowTokens(bagDescription.Tokens[CellType.MoveForward]);
			_moveBackwardTokensLine.ShowTokens(bagDescription.Tokens[CellType.MoveBackward]);
			_portalTokensLine.ShowTokens(bagDescription.Tokens[CellType.Portal]);
		}

		public void Show()
		{
			_content.SetActive(true);
		}

		public void Hide()
		{
			_content.SetActive(false);
		}
	}
}
