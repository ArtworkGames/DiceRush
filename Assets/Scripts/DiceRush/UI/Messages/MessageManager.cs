using StepanoffGames.DiceRush.UI.Messages.Signals;
using StepanoffGames.Localization;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using System.Collections.Generic;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Messages
{
	public class MessageManager : MonoBehaviour
	{
		[SerializeField] private GameObject _sourceRow;

		private List<MessageRow> _rows;

		private LocalizationManager _localizationManager;

		private void Awake()
		{
			_sourceRow.SetActive(false);
			_rows = new List<MessageRow>();
		}

		private void Start()
		{
			_localizationManager = ServiceLocator.Get<LocalizationManager>();

			SignalBus.Subscribe<ShowMessageSignal>(OnShowMessage);
		}

		private void OnDestroy()
		{
			_localizationManager = null;

			SignalBus.Unsubscribe<ShowMessageSignal>(OnShowMessage);
		}

		private void OnShowMessage(ShowMessageSignal signal)
		{
			string text = _localizationManager.GetString(signal.TextKey, signal.Params);
			Show(text);
		}

		public void Show(string text)
		{
			for (int i = 0; i < _rows.Count; i++)
			{
				_rows[i].MoveUp();
			}

			GameObject rowObject = Instantiate(_sourceRow, _sourceRow.transform.parent, false);
			rowObject.SetActive(true);

			MessageRow row = rowObject.GetComponent<MessageRow>();
			row.Show(text);
			row.OnHide += OnRowHide;
			_rows.Add(row);
		}

		private void OnRowHide(MessageRow row)
		{
			row.OnHide -= OnRowHide;
			_rows.Remove(row);
		}
	}
}
