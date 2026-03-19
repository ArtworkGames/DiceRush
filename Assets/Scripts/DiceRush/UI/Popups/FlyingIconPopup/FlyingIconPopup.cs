using Cysharp.Threading.Tasks;
using DG.Tweening;
using StepanoffGames.DiceRush.Game;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using StepanoffGames.UI.Popups.Signals;
using StepanoffGames.UI.Popups;
using UnityEngine;
using StepanoffGames.DiceRush.Math;
using System;

namespace StepanoffGames.DiceRush.UI.Popups.FlyingIconPopup
{
	public class FlyingIconPopupParams : BasePopupParams
	{
		public GameObject SourceIcon;
		public Transform Target;
		public Action OnStarted;
		public Action OnCompleted;
		public bool UpdateTarget;
	}

	public class FlyingIconPopup : BasePopup<FlyingIconPopupParams>
	{
		public static void Show(GameObject sourceIcon, Transform target, Action onStarted, Action onCompleted, bool updateTarget = false)
		{
			LevelManager levelManager = ServiceLocator.Get<LevelManager>();

			Vector3 worldPos = sourceIcon.transform.position;
			Vector2 scrPos = levelManager.UICamera.WorldToScreenPoint(worldPos);

			SignalBus.Publish(new OpenPopupSignal(PrefabName, scrPos, new FlyingIconPopupParams()
			{
				SourceIcon = sourceIcon,
				Target = target,
				OnStarted = onStarted,
				OnCompleted = onCompleted,
				UpdateTarget = updateTarget
			})
			{
				CloseOther = false,
				Autoclosing = true
			});
		}

		public static string PrefabName = "FlyingIconPopup";

		[SerializeField] private Transform _icon;

		private GameObject iconObject;
		private Bezier3 bezier;
		private Tween scaleTween;
		private Tween moveTween;
		private bool isDestroyed;

		private void OnDestroy()
		{
			isDestroyed = true;
			scaleTween?.Kill();
			moveTween?.Kill();
		}

		override protected void BeforeOpen()
		{
			iconObject = Instantiate(Params.SourceIcon, _icon, true);
			iconObject.name = Params.SourceIcon.name;
			iconObject.transform.localPosition = Vector3.zero;
			iconObject.SetActive(true);

			FlyToTarget();
		}

		private async void FlyToTarget()
		{
			if (isDestroyed) return;

			PopupManager popupManager = ServiceLocator.Get<PopupManager>();
			LevelManager levelManager = ServiceLocator.Get<LevelManager>();

			Vector3 fromPos = _icon.position;
			Vector3 toPos = Params.Target.position;
			toPos.z = _icon.position.z;
			CreateBezier(fromPos, toPos);

			Ease ease = Ease.InOutQuad;
			float speed = 50f;
			float minDuration = 0.5f;//0.3f;

			float distance = bezier.fullLength;// Vector3.Distance(fromPos, toPos);
			float duration = Mathf.Max(minDuration, distance / speed);

			bool moveCompleted = false;
			float moveFactor = 0f;

			Params.OnStarted?.Invoke();

			scaleTween = iconObject.transform.DOScale(1f, duration)
				.SetEase(Ease.OutQuad);

			moveTween = DOTween.To(() => moveFactor, x => moveFactor = x, 1f, duration)
				.SetEase(ease)
				.SetUpdate(true)
				.OnUpdate(() =>
				{
					if (Params.UpdateTarget)
					{
						Vector3 toPos = Params.Target.position;
						toPos.z = _icon.position.z;
						CreateBezier(fromPos, toPos);
					}
					Vector3 pos = bezier.GetPointByIterator(moveFactor);
					_icon.transform.position = pos;
				})
				.OnComplete(() =>
				{
					iconObject.SetActive(false);
					moveCompleted = true;
				});

			await UniTask.WaitUntil(() => moveCompleted || isDestroyed);
			if (isDestroyed) return;

			Params.OnCompleted?.Invoke();

			await UniTask.WaitForSeconds(0.2f);
			if (isDestroyed) return;

			ClosePopup();
		}

		private void CreateBezier(Vector3 fromPos, Vector3 toPos)
		{
			Vector3[] pathPoints = null;
			if (toPos.x > fromPos.x)
			{
				pathPoints = new Vector3[]
				{
					new Vector3(0f, 0f, 0f),
					new Vector3(3f, -2f, 0f),
					new Vector3(7f, -2f, 0f),
					new Vector3(10f, 0f, 0f)
				};
			}
            else
            {
				pathPoints = new Vector3[]
				{
					new Vector3(0f, 0f, 0f),
					new Vector3(3f, 2f, 0f),
					new Vector3(7f, 2f, 0f),
					new Vector3(10f, 0f, 0f)
				};
			}

			BezierUtils.TransformPoints(pathPoints, fromPos, toPos);

			bezier = new Bezier3(pathPoints);
		}
	}
}
