using TMPro;
using UnityEngine;

namespace StepanoffGames.UI.Components
{
	[RequireComponent(typeof(TMP_Text))]
	public class TMPTextLocalizer : TextLocalizer
	{
		private TMP_Text _tmpText;
		public TMP_Text TmpText
		{
			get
			{
				if (_tmpText == null)
					_tmpText = GetComponent<TMP_Text>();
				return _tmpText;
			}
		}

		public override void SetText(string txt)
		{
			TmpText.text = txt;
		}

		public override string GetText()
		{
			return TmpText.text;
		}
	}
}
