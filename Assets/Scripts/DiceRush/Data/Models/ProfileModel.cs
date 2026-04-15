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

		public void ResetDescriptionPopupsShown()
		{
			IsDiceDescriptionPopupShown = false;
			IsRewardTokenDescriptionPopupShown = false;
			IsEnemyTokenDescriptionPopupShown = false;
			IsMoveForwardTokenDescriptionPopupShown = false;
			IsMoveBackwardTokenDescriptionPopupShown = false;
			IsPortalTokenDescriptionPopupShown = false;
		}
	}
}
