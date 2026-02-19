using StepanoffGames.DiceRush.Game.Players;
using System.Collections.Generic;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game
{
	public class Way : MonoBehaviour
	{
		[SerializeField] private GameObject _sourceMarker;
		[SerializeField] private Material[] _playerMaterials;

		public List<WayMarker> Markers => _markers;

		private class WaySection
		{
			public List<WayMarker> Markers = new List<WayMarker>();
			public List<WaySection> NextSections = new List<WaySection>();
		}

		private List<WayMarker> _markers = new List<WayMarker>();
		private List<WaySection> _sections = new List<WaySection>();

		private void Awake()
		{
			_sourceMarker.SetActive(false);
		}

		private WayMarker AddMarker(int id, Cell cell, Material material)
		{
			GameObject markerObject = Instantiate(_sourceMarker, _sourceMarker.transform.parent, false);
			markerObject.name = $"Marker{id}";
			markerObject.SetActive(true);

			WayMarker marker = markerObject.GetComponent<WayMarker>();
			marker.Init(cell, material);

			_markers.Add(marker);
			return marker;
		}

		private WayMarker GetMarker(Cell markerCell)
		{
			for (int i = 0; i < _markers.Count; i++)
			{
				if (_markers[i].Cell == markerCell)
				{
					return _markers[i];
				}
			}
			return null;
		}

		private WaySection AddSectionInFrontOfPlayer(MapPoint startPoint, int length, Material material)
		{
			WaySection section = new WaySection();
			_sections.Add(section);

			MapPoint currPoint = startPoint;
			if (!(startPoint is Cell)) length++;

			for (int i = 0; i < length; i++)
			{
				if (currPoint is Cell)
				{
					WayMarker marker = GetMarker((Cell)currPoint);
					if (marker == null)
					{
						marker = AddMarker(_markers.Count + 1, (Cell)currPoint, material);
					}
					section.Markers.Add(marker);
				}

				do
				{
					if (currPoint.NextPoints.Count == 1)
					{
						currPoint = currPoint.NextPoints[0];
					}
					else if (currPoint.NextPoints.Count > 1)
					{
						for (int j = 0; j < currPoint.NextPoints.Count; j++)
						{
							WaySection nextSection = AddSectionInFrontOfPlayer(currPoint.NextPoints[j], length - i - 1, material);
							section.NextSections.Add(nextSection);
						}
						break;
					}
					else
					{
						break;
					}
				}
				while (!(currPoint is Cell));
			}
			return section;
		}

		private WaySection AddSectionInBackOfPlayer(MapPoint startPoint, int length, Material material)
		{
			WaySection section = new WaySection();
			_sections.Add(section);

			MapPoint currPoint = startPoint;
			if (!(startPoint is Cell)) length++;

			for (int i = 0; i < length; i++)
			{
				if (currPoint is Cell)
				{
					WayMarker marker = GetMarker((Cell)currPoint);
					if (marker == null)
					{
						marker = AddMarker(_markers.Count + 1, (Cell)currPoint, material);
					}
					section.Markers.Add(marker);
				}

				do
				{
					if (currPoint.PrevPoints.Count == 1)
					{
						currPoint = currPoint.PrevPoints[0];
					}
					else if (currPoint.PrevPoints.Count > 1)
					{
						for (int j = 0; j < currPoint.PrevPoints.Count; j++)
						{
							WaySection nextSection = AddSectionInBackOfPlayer(currPoint.PrevPoints[j], length - i - 1, material);
							section.NextSections.Add(nextSection);
						}
						break;
					}
					else
					{
						break;
					}
				}
				while (!(currPoint is Cell));
			}
			return section;
		}

		public void ShowMarkersInFrontOfPlayer(PlayerAvatar player)
		{
			//Debug.Log("ShowMarkersInFrontOfPlayer");

			WaySection section = new WaySection();
			_sections.Add(section);

			for (int i = 0; i < player.CurrentPoint.NextPoints.Count; i++)
			{
				WaySection nextSection = AddSectionInFrontOfPlayer(player.CurrentPoint.NextPoints[i], 6, _playerMaterials[player.Id - 1]);
				section.NextSections.Add(nextSection);
			}
		}

		public void ShowMarkersInBackOfPlayer(PlayerAvatar player)
		{
			//Debug.Log("ShowMarkersInBackOfPlayer");

			WaySection section = new WaySection();
			_sections.Add(section);

			for (int i = 0; i < player.CurrentPoint.PrevPoints.Count; i++)
			{
				WaySection nextSection = AddSectionInBackOfPlayer(player.CurrentPoint.PrevPoints[i], 6, _playerMaterials[player.Id - 1]);
				section.NextSections.Add(nextSection);
			}
		}

		public void ShowDiceValue(int diceValue)
		{
			LeaveSelectedMarkerInSection(_sections[0], 0, diceValue - 1);
		}

		private void LeaveSelectedMarkerInSection(WaySection section, int startIndex, int leaveIndex)
		{
			for (int i = 0; i < section.Markers.Count; i++)
			{
				//if (section.Markers[i] == null || section.Markers[i].Cell == null) continue;

				if ((startIndex + i == leaveIndex) ||
					(startIndex + i < leaveIndex && section.Markers[i].Cell.Type == CellType.Start) ||
					(startIndex + i < leaveIndex && section.Markers[i].Cell.Type == CellType.Finish))
				{
					section.Markers[i].Select();
				}
				else
				{
					section.Markers[i].Hide();
					//section.Markers[i] = null;
				}
			}

			for (int i = 0; i < section.NextSections.Count; i++)
			{
				LeaveSelectedMarkerInSection(section.NextSections[i], startIndex + section.Markers.Count, leaveIndex);
			}
		}

		public void HideMarkers()
		{
			for (int i = 0; i < _markers.Count; i++)
			{
				//if (_markers[i] != null)
				//{
					_markers[i].Hide(true);
				//}
			}
			_markers.Clear();

			for (int i = 0; i < _sections.Count; i++)
			{
				_sections[i].Markers.Clear();
				_sections[i].NextSections.Clear();
			}
			_sections.Clear();
		}
	}
}