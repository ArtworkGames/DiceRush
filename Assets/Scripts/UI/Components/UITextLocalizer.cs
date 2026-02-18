using UnityEngine;
using UnityEngine.UI;

namespace StepanoffGames.UI.Components
{
	[RequireComponent(typeof(Text))]
	public class UITextLocalizer : TextLocalizer
	{
		private Text _text;
		public Text Text
		{
			get
			{
				if (_text == null)
					_text = GetComponent<Text>();
				return _text;
			}
		}

		public override void SetText(string txt)
		{
			Text.text = txt;
		}

		public override string GetText()
		{
			return Text.text;
		}

	}
}
