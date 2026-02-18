using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace StepanoffGames.DiceRush.Gameplay
{
	public class Fork : MonoBehaviour
	{
		[SerializeField] private GameObject _sourceArrow;
		[SerializeField] private Material[] _playerMaterials;

		private List<ForkArrow> _arrows = new List<ForkArrow>();

		private int selectedId;

		private void Awake()
		{
			_sourceArrow.SetActive(false);
		}

		private void AddArrow(int id, Vector3 arrowPosition, Vector3 cellCenter, Material material)
		{
			GameObject arrowObject = Instantiate(_sourceArrow, _sourceArrow.transform.parent, false);
			arrowObject.name = $"Arrow{id}";
			arrowObject.SetActive(true);

			ForkArrow arrow = arrowObject.GetComponent<ForkArrow>();
			arrow.OnSelect += OnArrowSelect;
			arrow.Init(id, arrowPosition, cellCenter, material);

			_arrows.Add(arrow);
		}

		private void ClearArrows()
		{
			for (int i = 0; i < _arrows.Count; i++)
			{
				_arrows[i].OnSelect -= OnArrowSelect;
				Destroy(_arrows[i].gameObject);
			}
			_arrows.Clear();
		}

		public async UniTask<int> SelectNextDirectionForPoint(MapPoint point, Avatar player)
		{
			transform.position = point.transform.position;

			for (int i = 0; i < point.NextPoints.Count; i++)
			{
				AddArrow(i + 1,
					point.NextPoints[i].transform.position,
					point.transform.position,
					_playerMaterials[player.Id - 1]);
			}

			selectedId = -1;
			await UniTask.WaitWhile(() => selectedId == -1);

			ClearArrows();

			return selectedId - 1;
		}

		public async UniTask<int> SelectPrevDirectionForPoint(MapPoint point, Avatar player)
		{
			transform.position = point.transform.position;

			for (int i = 0; i < point.PrevPoints.Count; i++)
			{
				AddArrow(i + 1,
					point.PrevPoints[i].transform.position,
					point.transform.position,
					_playerMaterials[player.Id - 1]);
			}

			selectedId = -1;
			await UniTask.WaitWhile(() => selectedId == -1);

			ClearArrows();

			return selectedId - 1;
		}

		private void OnArrowSelect(ForkArrow arrow)
		{
			selectedId = arrow.Id;
		}
	}
}
