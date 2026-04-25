using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static StepanoffGames.DiceRush.Game.Map.MapSectionsConfig;

namespace StepanoffGames.DiceRush.Game.Map
{
	public class MapGenerator : MonoBehaviour
	{
		public class SelectedSectionData
		{
			public MapSectionData SectionData;
			public Vector2Int Pos;
			public int SideShift;
			public MapSectionSide ExitSide;
			public Vector2Int NextPos;
		}

		[SerializeField] private MapSectionsConfig _sectionsConfig;

		public List<MapSection> Sections => _sections;
		private List<MapSection> _sections;

		private List<SelectedSectionData> _selectedSectionsData;

		public async UniTask Generate(int targetLength)
		{
			_selectedSectionsData = new List<SelectedSectionData>();
			SelectSectionsData(targetLength);

			_sections = new List<MapSection>();
			for (int i = 0; i < _selectedSectionsData.Count; i++)
			{
				MapSection section = await LoadSection(_selectedSectionsData[i].SectionData.Name);
				_sections.Add(section);

				Vector3 angles = new Vector3(0f, 0f, -_selectedSectionsData[i].SideShift * 90f);
				section.transform.eulerAngles = angles;

				Vector3 pos = new Vector3(_selectedSectionsData[i].Pos.x * 20f, _selectedSectionsData[i].Pos.y * 20f, 0f);
				section.transform.position = pos;
			}

			for (int i = 0; i < _sections.Count; i++)
			{
				if (i == 0)
				{
					_sections[i].EnterCell.SetType(CellType.Start);
				}
				if (i < _sections.Count - 1)
				{
					_sections[i].ExitCell.NextPoints.Add(_sections[i + 1].EnterCell);
				}
				if (i > 0)
				{
					_sections[i].EnterCell.PrevPoints.Add(_sections[i - 1].ExitCell);
				}
				if (i == _sections.Count - 1)
				{
					_sections[i].ExitCell.SetType(CellType.Finish);
				}
			}
		}

		private void SelectSectionsData(int targetLength)
		{
			Vector2Int currPos = Vector2Int.zero;
			MapSectionSide currEnterSide = MapSectionSide.Left;
			int totalLength = 0;
			string lastSectionName = "";

			for (int i = 0; i < _sectionsConfig.SectionsData.Length; i++)
			{
				if (_sectionsConfig.SectionsData[i].Name == "SectionStart")
				{
					Vector2Int nextPos = GetNextSectionPos(currPos, _sectionsConfig.SectionsData[i].ExitSide);

					_selectedSectionsData.Add(new SelectedSectionData
					{
						SectionData = _sectionsConfig.SectionsData[i],
						Pos = currPos,
						SideShift = 0,
						ExitSide = _sectionsConfig.SectionsData[i].ExitSide,
						NextPos = nextPos
					});

					currPos = nextPos;
					currEnterSide = InvertSide(_sectionsConfig.SectionsData[i].ExitSide);
					totalLength += _sectionsConfig.SectionsData[i].Length;

					break;
				}
			}

			if (GameManager.GameMode == GameMode.Tutorial)
			{
				for (int i = 0; i < _sectionsConfig.SectionsData.Length; i++)
				{
					if (_sectionsConfig.SectionsData[i].Name == "Section1")
					{
						for (int j = 0; j < 3; j++)
						{
							Vector2Int nextPos = GetNextSectionPos(currPos, _sectionsConfig.SectionsData[i].ExitSide);

							_selectedSectionsData.Add(new SelectedSectionData
							{
								SectionData = _sectionsConfig.SectionsData[i],
								Pos = currPos,
								SideShift = 0,
								ExitSide = _sectionsConfig.SectionsData[i].ExitSide,
								NextPos = nextPos
							});

							currPos = nextPos;
							currEnterSide = InvertSide(_sectionsConfig.SectionsData[i].ExitSide);
							totalLength += _sectionsConfig.SectionsData[i].Length;
						}

						break;
					}
				}
			}

			do
			{
				//Debug.Log($"------------- [MapGenerator] SelectSectionsData: {_selectedSectionsData.Count + 1}");
	
				List < SelectedSectionData> currentSelectedSections = new List<SelectedSectionData>();
				if (totalLength >= (targetLength - 3))
				{
					for (int i = 0; i < _sectionsConfig.SectionsData.Length; i++)
					{
						if (_sectionsConfig.SectionsData[i].Name == "SectionFinish")
						{
							(bool canPlace, int sideShift, MapSectionSide exitSide, Vector2Int nextPos) =
								CanPlaceSection(_sectionsConfig.SectionsData[i], currPos, currEnterSide);

							currentSelectedSections.Add(new SelectedSectionData
							{
								SectionData = _sectionsConfig.SectionsData[i],
								Pos = currPos,
								SideShift = sideShift,
								ExitSide = exitSide,
								NextPos = nextPos
							});

							break;
						}
					}
				}
				else
				{
					for (int i = 0; i < _sectionsConfig.SectionsData.Length; i++)
					{
						if (_sectionsConfig.SectionsData[i].Name == "SectionStart" ||
							_sectionsConfig.SectionsData[i].Name == "SectionFinish" ||
							_sectionsConfig.SectionsData[i].Name == lastSectionName) continue;

						(bool canPlace, int sideShift, MapSectionSide exitSide, Vector2Int nextPos) =
							CanPlaceSection(_sectionsConfig.SectionsData[i], currPos, currEnterSide);

						//Debug.Log($"[MapGenerator] CanPlaceSection: {_sectionsData[i].Name}, " +
						//	$"pos: {currPos}, " +
						//	$"sideShift: {sideShift}, " +
						//	$"exitSide: {exitSide}, " +
						//	$"nextPos: {nextPos} - {canPlace.ToString().ToUpper()}");

						if (canPlace)
						{
							currentSelectedSections.Add(new SelectedSectionData
							{
								SectionData = _sectionsConfig.SectionsData[i],
								Pos = currPos,
								SideShift = sideShift,
								ExitSide = exitSide,
								NextPos = nextPos
							});
						}
					}
				}

				if (currentSelectedSections.Count > 0)
				{
					SelectedSectionData selectedSectionData = currentSelectedSections[UnityEngine.Random.Range(0, currentSelectedSections.Count)];
					_selectedSectionsData.Add(selectedSectionData);

					currPos = selectedSectionData.NextPos;
					currEnterSide = InvertSide(selectedSectionData.ExitSide);
					totalLength += selectedSectionData.SectionData.Length;
					lastSectionName = selectedSectionData.SectionData.Name;

					//Debug.Log($"[MapGenerator] SelectSectionsData: {selectedSectionData.SectionData.Name}, " +
					//	$"pos: {selectedSectionData.Pos}, " +
					//	$"sideShift: {selectedSectionData.SideShift}, " +
					//	$"exitSide: {selectedSectionData.ExitSide}, " +
					//	$"nextPos: {selectedSectionData.NextPos}");

					//if (totalLength >= targetLength)
					if (selectedSectionData.SectionData.Name == "SectionFinish")
					{
						break;
					}
				}
				else
				{
					break;
				}
			}
			while (true);
			Debug.Log($"[MapGenerator] SelectSectionsData totalLength = {totalLength}");
		}

		private (bool, int, MapSectionSide, Vector2Int) CanPlaceSection(MapSectionData section, Vector2Int pos, MapSectionSide enterSide)
		{
			int sideShift = GetSideShift(section.EnterSide, enterSide);
			MapSectionSide exitSide = ShiftSide(section.ExitSide, sideShift);
			Vector2Int nextPos = GetNextSectionPos(pos, exitSide);
			//Vector2Int zeroPos = new Vector2Int(-1, 0);

			for (int i = 0; i < _selectedSectionsData.Count; i++)
			{
				if (_selectedSectionsData[i].Pos == pos || _selectedSectionsData[i].Pos == nextPos)// || nextPos == zeroPos)
				{
					return (false, sideShift, exitSide, nextPos);
				}
			}
			return (true, sideShift, exitSide, nextPos);
		}

		private int GetSideShift(MapSectionSide sectionSide, MapSectionSide targetSide)
		{
			int shift = (int)targetSide - (int)sectionSide;
			if (shift == 3) shift = -1;
			if (shift == -3) shift = 1;
			return shift;
		}

		private MapSectionSide ShiftSide(MapSectionSide side, int shift)
		{
			int shiftedSide = (int)side + shift;
			if (shiftedSide > 3) shiftedSide -= 4;
			if (shiftedSide < 0) shiftedSide += 4;
			return (MapSectionSide)shiftedSide;
		}

		private Vector2Int GetNextSectionPos(Vector2Int sectionPos, MapSectionSide exitSide)
		{
			Vector2Int nextPos = sectionPos;
			switch (exitSide)
			{
				case MapSectionSide.Top: nextPos.y += 1; break;
				case MapSectionSide.Right: nextPos.x += 1; break;
				case MapSectionSide.Bottom: nextPos.y -= 1; break;
				case MapSectionSide.Left: nextPos.x -= 1; break;
			}
			return nextPos;
		}

		private MapSectionSide InvertSide(MapSectionSide side)
		{
			MapSectionSide invertedSide = side;
			switch (side)
			{
				case MapSectionSide.Top: invertedSide = MapSectionSide.Bottom; break;
				case MapSectionSide.Right: invertedSide = MapSectionSide.Left; break;
				case MapSectionSide.Bottom: invertedSide = MapSectionSide.Top; break;
				case MapSectionSide.Left: invertedSide = MapSectionSide.Right; break;
			}
			return invertedSide;
		}

		private async UniTask<MapSection> LoadSection(string sectionName)
		{
			string sectionPath = $"Game/Map/{sectionName}.prefab";
			var handle = Addressables.LoadAssetAsync<GameObject>(sectionPath);
			await UniTask.WaitUntil(() => handle.IsDone);

			GameObject sectionObject = Instantiate(handle.Result, transform, false);
			sectionObject.name = sectionName;

			MapSection section = sectionObject.GetComponent<MapSection>();
			return section;
		}
	}
}
