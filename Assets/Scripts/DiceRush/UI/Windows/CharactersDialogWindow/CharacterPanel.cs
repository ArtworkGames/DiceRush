using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace StepanoffGames.DiceRush.UI.Windows.CharactersDialogWindow
{
	public class CharacterPanel : MonoBehaviour
	{
		[SerializeField] private CanvasGroup _canvasGroup;
		[SerializeField] private Transform _avatarPivot;
		[Space]
		[SerializeField] private Vector2 _hideDelta;
		[SerializeField] private Vector2 _backDelta;
		[SerializeField] private bool _initHidden;

		public bool IsShown => _isShown;
		private bool _isShown;

		public bool IsOnBack => _isOnBack;
		private bool _isOnBack;

		public string AvatarName => _avatarName;
		private string _avatarName;

		private Vector3 shownPos;
		private Vector3 hiddenPos;
		private Vector3 backPos;
		private float shownScale;
		private float hiddenScale;
		private float backScale;
		private Tween fadeTween;
		private Tween scaleTween;
		private Tween moveTween;
		private bool isDestroyed;

		private GameObject avatarObject;

		private void Start()
		{
			shownPos = transform.localPosition;
			hiddenPos = new Vector3(
				shownPos.x + _hideDelta.x,
				shownPos.y + _hideDelta.y,
				shownPos.z);
			backPos = new Vector3(
				shownPos.x + _backDelta.x,
				shownPos.y + _backDelta.y,
				shownPos.z);

			shownScale = 1f;
			hiddenScale = shownScale;
			backScale = shownScale * 0.9f;

			Debug.Log($"[CharacterPanel] {name}: shownPos = {shownPos}, hiddenPos = {hiddenPos}, backPos = {backPos}");

			_isShown = true;
			if (_initHidden)
			{
				_isShown = false;
				_canvasGroup.alpha = 0f;
				transform.localScale = Vector3.one * hiddenScale;
				transform.localPosition = hiddenPos;
			}
		}

		private void OnDestroy()
		{
			fadeTween?.Kill();
			scaleTween?.Kill();
			moveTween?.Kill();
			isDestroyed = true;
		}

		public async UniTask Show(string avatarName, bool immediately, CancellationToken ct)
		{
			_isShown = true;
			fadeTween?.Kill();
			scaleTween?.Kill();
			moveTween?.Kill();

			_avatarName = avatarName;
			string characterPath = $"Windows/CharactersDialogWindow/{_avatarName}.prefab";
			await LoadAvatar(characterPath);

			if (immediately)
			{
				_canvasGroup.alpha = 1f;
				transform.localScale = Vector3.one * shownScale;
				transform.localPosition = shownPos;
			}
			else
			{
				bool isShowCompleted = false;

				fadeTween = _canvasGroup.DOFade(1f, 0.5f)
					.SetEase(Ease.OutCubic);

				scaleTween = transform.DOScale(shownScale, 0.5f)
					.SetEase(Ease.OutCubic);

				moveTween = transform.DOLocalMove(shownPos, 0.5f)
					.SetEase(Ease.OutCubic)
					.OnComplete(() =>
					{
						isShowCompleted = true;
					});

				await UniTask.WaitUntil(() => isShowCompleted || !_isShown || isDestroyed, cancellationToken: ct);
			}
		}

		private async UniTask LoadAvatar(string path)
		{
			var handle = Addressables.LoadAssetAsync<GameObject>(path);
			await UniTask.WaitUntil(() => handle.IsDone);

			avatarObject = Instantiate(handle.Result, _avatarPivot, false);
			avatarObject.name = handle.Result.name;
		}

		public async UniTask Hide(bool immediately, CancellationToken ct)
		{
			_isShown = false;
			_isOnBack = false;
			fadeTween?.Kill();
			scaleTween?.Kill();
			moveTween?.Kill();

			_avatarName = "";

			if (immediately)
			{
				_canvasGroup.alpha = 0f;
				transform.localScale = Vector3.one * hiddenScale;
				transform.localPosition = hiddenPos;
			}
			else
			{
				bool isHideCompleted = false;

				fadeTween = _canvasGroup.DOFade(0f, 0.5f)
					.SetEase(Ease.OutCubic);

				scaleTween = transform.DOScale(hiddenScale, 0.5f)
					.SetEase(Ease.OutCubic);

				moveTween = transform.DOLocalMove(hiddenPos, 0.5f)
					.SetEase(Ease.OutCubic)
					.OnComplete(() =>
					{
						Destroy(avatarObject);
						isHideCompleted = true;
					});

				await UniTask.WaitUntil(() => isHideCompleted || _isShown || isDestroyed, cancellationToken: ct);
			}
		}

		public async UniTask MoveToBack(bool immediately, CancellationToken ct)
		{
			_isOnBack = true;
			fadeTween?.Kill();
			scaleTween?.Kill();
			moveTween?.Kill();

			transform.SetAsFirstSibling();

			if (immediately)
			{
				_canvasGroup.alpha = 0.5f;
				transform.localScale = Vector3.one * backScale;
				transform.localPosition = backPos;
			}
			else
			{
				bool isShowCompleted = false;

				fadeTween = _canvasGroup.DOFade(0.5f, 0.5f)
					.SetEase(Ease.OutCubic);

				scaleTween = transform.DOScale(backScale, 0.5f)
					.SetEase(Ease.OutCubic);

				moveTween = transform.DOLocalMove(backPos, 0.5f)
					.SetEase(Ease.OutCubic)
					.OnComplete(() =>
					{
						isShowCompleted = true;
					});

				await UniTask.WaitUntil(() => isShowCompleted || !_isOnBack || isDestroyed, cancellationToken: ct);
			}
		}

		public async UniTask MoveToFront(bool immediately, CancellationToken ct)
		{
			_isOnBack = false;
			fadeTween?.Kill();
			scaleTween?.Kill();
			moveTween?.Kill();

			if (immediately)
			{
				_canvasGroup.alpha = 1f;
				transform.localScale = Vector3.one * shownScale;
				transform.localPosition = shownPos;
			}
			else
			{
				bool isShowCompleted = false;

				fadeTween = _canvasGroup.DOFade(1f, 0.5f)
					.SetEase(Ease.OutCubic);

				scaleTween = transform.DOScale(shownScale, 0.5f)
					.SetEase(Ease.OutCubic);

				moveTween = transform.DOLocalMove(shownPos, 0.5f)
					.SetEase(Ease.OutCubic)
					.OnComplete(() =>
					{
						isShowCompleted = true;
					});

				await UniTask.WaitUntil(() => isShowCompleted || _isOnBack || isDestroyed, cancellationToken: ct);
			}
		}

	}
}
