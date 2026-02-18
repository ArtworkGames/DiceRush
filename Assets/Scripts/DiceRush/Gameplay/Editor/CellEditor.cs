using UnityEditor;
using UnityEngine;

namespace StepanoffGames.DiceRush.Gameplay
{
	[CustomEditor(typeof(Cell))]
	public class CellEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			Cell cell = (Cell)target;

			GUILayout.Space(16f);
			if (GUILayout.Button("Add next point"))
			{
				cell.AddNextPoint();
			}
			if (GUILayout.Button("Insert next point"))
			{
				cell.InsertNextPoint();
			}

			GUILayout.Space(16f);
			if (GUILayout.Button("Convert to point"))
			{
				cell.ConvertToPoint();
			}
			if (GUILayout.Button("Convert to cell"))
			{
				cell.ConvertToCell();
			}

			GUILayout.Space(16f);
			if (GUILayout.Button("Delete this point"))
			{
				cell.DeleteThisPoint();
			}
		}
	}
}
