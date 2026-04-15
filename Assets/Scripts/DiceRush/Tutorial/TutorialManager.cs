using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Tutorial.Chapters;
using StepanoffGames.Initialization;
using StepanoffGames.Services;
using System.Threading;

namespace StepanoffGames.DiceRush.Tutorial
{
	public enum TutorialChapterId
	{
		Undefined,
		FirstRun
	}

	public class TutorialManager : BaseInitializable, IService
	{
		public bool IsTutorialRunning => _isTutorialRunning;
		private bool _isTutorialRunning;

		public TutorialChapterId CurrentChapterId => _currentChapterId;
		private TutorialChapterId _currentChapterId = TutorialChapterId.Undefined;

		public TutorialManager()
		{
			ServiceLocator.Register(this);
		}

		override public async UniTask InitializeAsync()
		{
			await UniTask.Yield();
		}

		public async UniTask RunChapter(TutorialChapterId chapterId, CancellationToken ct)
		{
			_currentChapterId = chapterId;
			TutorialChapter chapter = null;

			switch (_currentChapterId)
			{
				case TutorialChapterId.FirstRun:
					chapter = new FirstRunChapter();
					break;
			}

			if (chapter != null)
			{
				await chapter.Run(ct);
			}
		}
	}
}

