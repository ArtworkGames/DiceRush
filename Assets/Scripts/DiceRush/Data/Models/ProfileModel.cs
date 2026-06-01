using StepanoffGames.DiceRush.Data.Schemes;

namespace StepanoffGames.DiceRush.Data.Models
{
	public class ProfileModel
	{
		public bool IsTutorialCompleted;

		// эти ключи не надо сохранять; они нужны только для туториала
		public bool IsDiceDescriptionPopupShown;
		public bool IsRewardTokenDescriptionPopupShown;
		public bool IsEnemyTokenDescriptionPopupShown;
		public bool IsMoveForwardTokenDescriptionPopupShown;
		public bool IsMoveBackwardTokenDescriptionPopupShown;
		public bool IsPortalTokenDescriptionPopupShown;

		public ProfileModel()
		{
		}

		public ProfileModel(ProfileScheme s)
		{
			IsTutorialCompleted = s.t == 1;
		}

		public void ResetDescriptionPopupsShown()
		{
			IsDiceDescriptionPopupShown = false;
			IsRewardTokenDescriptionPopupShown = false;
			IsEnemyTokenDescriptionPopupShown = false;
			IsMoveForwardTokenDescriptionPopupShown = false;
			IsMoveBackwardTokenDescriptionPopupShown = false;
			IsPortalTokenDescriptionPopupShown = false;
		}

		public ProfileScheme GetScheme()
		{
			ProfileScheme s = new ProfileScheme();

			s.t = IsTutorialCompleted ? 1 : 0;

			return s;
		}
	}
}
