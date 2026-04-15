using Cysharp.Threading.Tasks;
using DG.Tweening;
using StepanoffGames.DiceRush.Game;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using StepanoffGames.UI.Popups;
using StepanoffGames.UI.Popups.Signals;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace StepanoffGames.DiceRush.UI.Popups.DiceAndTokenPopup
{
	public class DiceAndTokenPopupParams : BasePopupParams
	{
		public DescriptionType DescriptionType;
	}

	public enum DescriptionType
	{
		Undefined,
		Dice,
		RewardToken,
		EnemyToken,
		MoveForwardToken,
		MoveBackwardToken,
		PortalToken,
	}

	public class DiceAndTokenPopup : BasePopup<DiceAndTokenPopupParams>
	{
		public static void Show(Transform uiTarget, DescriptionType descriptionType)
		{
			GameManager gameManager = ServiceLocator.Get<GameManager>();

			Vector3 worldPos = uiTarget.position;
			Vector2 scrPos = gameManager.UICamera.WorldToScreenPoint(worldPos);

			SignalBus.Publish(new OpenPopupSignal(PrefabName, scrPos, new DiceAndTokenPopupParams()
			{
				DescriptionType = descriptionType
			})
			{
				CloseOther = false,
				Autoclosing = true
			});
		}

		public static string PrefabName = "DiceAndTokenPopup";

		[Space]
		[SerializeField] private Transform _descriptionParent;

		private Tween fadeTween;
		private Tween scaleTween;

		private void OnDestroy()
		{
			fadeTween?.Kill();
			scaleTween?.Kill();
		}

		override protected void BeforeOpen()
		{
			Content.alpha = 0f;
		}

		override protected async void AfterOpen()
		{
			string descriptionName = $"{Params.DescriptionType}Description";
			string descriptionPath = $"Popups/DiceAndTokenPopup/{descriptionName}.prefab";
			await LoadDescription(descriptionPath);

			Content.alpha = 1f;
			Content.transform.localScale = Vector3.one * 0.5f;

			fadeTween = Content.DOFade(1f, 0.3f)
				.SetEase(Ease.OutCubic)
				.SetUpdate(true);
			scaleTween = Content.transform.DOScale(1f, 0.3f)
				.SetEase(Ease.OutBack)
				.SetUpdate(true);
		}

		private async UniTask LoadDescription(string descriptionPath)
		{
			var handle = Addressables.LoadAssetAsync<GameObject>(descriptionPath);
			await UniTask.WaitUntil(() => handle.IsDone);

			GameObject descriptionObject = Instantiate(handle.Result, _descriptionParent, false);
			descriptionObject.name = handle.Result.name;
		}
	}
}
