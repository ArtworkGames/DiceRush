using System;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Map
{
	[CreateAssetMenu(menuName = "Dice Rush/Configs/Map Sections Config", fileName = "MapSectionsConfig", order = 0)]
	public class MapSectionsConfig : ScriptableObject
	{
		public enum MapSectionSide
		{
			Top,
			Right,
			Bottom,
			Left
		}

		[Serializable]
		public class MapSectionData
		{
			public string Name;
			[Space]
			public MapSectionSide EnterSide;
			public MapSectionSide ExitSide;
			[Space]
			public int Length;
		}

		[SerializeField] private MapSectionData[] _sectionsData;

		public MapSectionData[] SectionsData => _sectionsData;
	}
}
