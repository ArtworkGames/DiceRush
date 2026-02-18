using UnityEditor;
using UnityEngine;

namespace StepanoffGames.DiceRush.Gameplay
{
	[CustomEditor(typeof(MapPoint))]
	public class MapPointEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			MapPoint point = (MapPoint)target;

			GUILayout.Space(16f);
			if (GUILayout.Button("Add next point"))
			{
				point.AddNextPoint();
			}
			if (GUILayout.Button("Insert next point"))
			{
				point.InsertNextPoint();
			}

			GUILayout.Space(16f);
			if (GUILayout.Button("Convert to point"))
			{
				point.ConvertToPoint();
			}
			if (GUILayout.Button("Convert to cell"))
			{
				point.ConvertToCell();
			}

			GUILayout.Space(16f);
			if (GUILayout.Button("Delete this point"))
			{
				point.DeleteThisPoint();
			}
		}
	}
}
