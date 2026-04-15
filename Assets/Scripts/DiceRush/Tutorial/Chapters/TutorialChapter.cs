using Cysharp.Threading.Tasks;
using System.Threading;

namespace StepanoffGames.DiceRush.Tutorial.Chapters
{
	public class TutorialChapter
	{
		public TutorialChapter()
		{
		}

		virtual public async UniTask Run(CancellationToken ct)
		{
			await UniTask.Yield(ct);
		}
	}
}
