using StepanoffGames.DiceRush.Game.Bag;
using StepanoffGames.DiceRush.Game.Map;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Bag.DescriptionPopup
{
	public class BagDescriptionPopup : MonoBehaviour
	{
		[SerializeField] private GameObject _content;
		[Space]
		[SerializeField] private GameObject _rewardLabel;
		[SerializeField] private GameObject _enemyLabel;
		[SerializeField] private GameObject _moveForwardLabel;
		[SerializeField] private GameObject _moveBackwardLabel;
		[SerializeField] private GameObject _portalLabel;
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
			_rewardLabel.SetActive(bagDescription.Tokens[CellType.Reward].IsAccepted);
			_enemyLabel.SetActive(bagDescription.Tokens[CellType.Enemy].IsAccepted);
			_moveForwardLabel.SetActive(bagDescription.Tokens[CellType.MoveForward].IsAccepted);
			_moveBackwardLabel.SetActive(bagDescription.Tokens[CellType.MoveBackward].IsAccepted);
			_portalLabel.SetActive(bagDescription.Tokens[CellType.Portal].IsAccepted);

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
