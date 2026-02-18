using UnityEngine;

namespace StepanoffGames.UI.Components
{
	[RequireComponent(typeof(TextMesh))]
	public class TextMeshLocalizer : TextLocalizer
	{
		private TextMesh _textMesh;
		public TextMesh TextMesh
		{
			get
			{
				if (_textMesh == null)
					_textMesh = GetComponent<TextMesh>();
				return _textMesh;
			}
		}

		public override void SetText(string txt)
		{
			TextMesh.text = txt;
		}

		public override string GetText()
		{
			return TextMesh.text;
		}
	}
}
