using DG.Tweening;
using StepanoffGames.DiceRush.Data.Models;
using TMPro;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Components.Ranking
{
	public class PlayerItem : MonoBehaviour
	{
		[SerializeField] private TMP_Text _placeText;
		[SerializeField] private TMP_Text _cellText;

		[HideInInspector] public PlayerModel Model;

		private Tween moveTween;

		private void OnDestroy()
		{
			Model = null;
			moveTween?.Kill();
		}

		public void UpdatePlace()
		{
			_placeText.text = Model.Place.ToString();
		}

		public void UpdateCell(int cellIndex)
		{
			_cellText.text = cellIndex.ToString();
		}

		public void SetToPlace(int place, bool up)
		{
			moveTween?.Kill();

			float y = -(place - 1) * 180f;
			transform.localPosition = new Vector3(transform.localPosition.x, y);
		}

		public void MoveToPlace(int place, bool up)
		{
			moveTween?.Kill();

			float y = -(place - 1) * 180f;
			if (up)
			{
				moveTween = transform.DOLocalMoveY(y, 0.5f)
					.SetEase(Ease.OutCubic);
			}
            else
            {
				moveTween = transform.DOLocalMoveY(y, 0.5f)
					.SetEase(Ease.OutCubic);
			}
		}
	}
}
