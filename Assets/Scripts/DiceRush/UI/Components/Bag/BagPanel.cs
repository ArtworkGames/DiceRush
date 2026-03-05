using StepanoffGames.DiceRush.Game;
using System.Collections.Generic;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Components.Bag
{
	public class BagPanel : MonoBehaviour
	{
		[SerializeField] private GameObject _sourceToken;

		private List<BagToken> _tokens;

		private void Awake()
		{
			_sourceToken.SetActive(false);			
		}

		public void ShowTokens(List<CellType> cellTypes)
		{
			_tokens = new List<BagToken>();
			for (int i = 0; i < cellTypes.Count; i++)
			{
				AddToken(i, cellTypes[i]);
			}
		}

		private void AddToken(int index, CellType cellType)
		{
			GameObject tokenObject = Instantiate(_sourceToken, _sourceToken.transform.parent, false);
			tokenObject.name = "Token" + index;
			tokenObject.SetActive(true);

			BagToken token = tokenObject.GetComponent<BagToken>();
			token.UpdateView(cellType);
			_tokens.Add(token);
		}

		public void HideTokens()
		{
			for (int i = 0; i < _tokens.Count; i++)
			{
				Destroy(_tokens[i].gameObject);
			}
			_tokens.Clear();
		}
	}
}
