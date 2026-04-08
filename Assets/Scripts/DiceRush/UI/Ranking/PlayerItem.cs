using Cysharp.Threading.Tasks;
using DG.Tweening;
using StepanoffGames.DiceRush.Game;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.DiceRush.UI.Components;
using StepanoffGames.DiceRush.UI.Ranking.DescriptionPopup;
using StepanoffGames.Services;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StepanoffGames.DiceRush.UI.Ranking
{
	public class PlayerItem : TweenButton
	{
		[Space]
		[SerializeField] private Image _back;
		[SerializeField] private Color[] _playerColors;
		[Space]
		[SerializeField] private TMP_Text _placeText;
		[SerializeField] private TMP_Text _cellText;
		[SerializeField] private PlayerStateIcon _playerStateIcon;
		[Space]
		[SerializeField] private PlayerDescriptionPopup _descriptionPopup;

		public PlayerController Player => _player;
		private PlayerController _player;

		private int cellIndex;
		private Tween moveTween;

		private GameManager _gameManager;

		private void Awake()
		{
			//mode = TweenButtonMode.Focusable;

			_gameManager = ServiceLocator.Get<GameManager>();
		}

		override protected void OnDestroy()
		{
			base.OnDestroy();

			_player = null;
			moveTween?.Kill();

			_gameManager = null;
		}

		public void SetPlayer(int index, PlayerController player)
		{
			_back.color = _playerColors[index];

			_player = player;
			_playerStateIcon.SetPlayerAvatar(player.Avatar);
			_descriptionPopup.SetPlayer(player);
		}

		public void UpdatePlace()
		{
			_placeText.text = _player.Model.Place.ToString();

			_descriptionPopup.UpdatePlaceValues(_player.Model.Place, cellIndex);
		}

		public void UpdateCell(int cellIndex)
		{
			this.cellIndex = cellIndex;
			_cellText.text = cellIndex.ToString();

			_descriptionPopup.UpdatePlaceValues(_player.Model.Place, cellIndex);
		}

		public void SetToPlace(int place, bool up)
		{
			moveTween?.Kill();

			float y = -(place - 1) * 200f;
			transform.localPosition = new Vector3(transform.localPosition.x, y);
		}

		public void MoveToPlace(int place, bool up)
		{
			moveTween?.Kill();

			float y = -(place - 1) * 200f;
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

		override public void OnPointerEnter(PointerEventData eventData)
		{
			_descriptionPopup.Show();

			base.OnPointerEnter(eventData);
		}

		override public void OnPointerExit(PointerEventData eventData)
		{
			_descriptionPopup.Hide();

			base.OnPointerExit(eventData);
		}

		public override void DoClick()
		{
			_gameManager.Camera.FocusOnPlayer(_player.Avatar, CancellationToken.None).Forget();
		}
	}
}
