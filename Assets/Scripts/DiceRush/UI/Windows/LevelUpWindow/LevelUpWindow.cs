using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.UI.Windows;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StepanoffGames.DiceRush.UI.Windows.ConfirmWindow
{
	public class LevelUpWindowParams : BaseWindowParams
	{
		public List<PerkModel> Perks;
		public Action<PerkModel> OnSelect;
	}

	public class LevelUpWindow : BaseWindow<LevelUpWindowParams>
	{
		public static string PrefabName = "LevelUpWindow";

		[Space]
		[SerializeField] private Button _okButton;
		[SerializeField] private GameObject _sourceItem;

		private List<PerkItem> _items;
		private PerkItem _selectedItem;

		private void Awake()
		{
			_sourceItem.SetActive(false);
		}

		override protected void BeforeOpen()
		{
			_items = new List<PerkItem>();
			for (int i = 0; i < Params.Perks.Count; i++)
			{
				AddPerk(Params.Perks[i]);
			}
			_okButton.interactable = false;
		}

		override protected void AfterOpen()
		{
			_okButton.onClick.AddListener(OnOkButtonClick);
		}

		override protected void BeforeClose()
		{
			_okButton.onClick.RemoveAllListeners();
		}

		override protected void AfterClose()
		{
			Params.OnSelect?.Invoke(_selectedItem.Model);
		}

		private void AddPerk(PerkModel perkModel)
		{
			GameObject itemObject = Instantiate(_sourceItem, _sourceItem.transform.parent, false);
			itemObject.name = $"PerkItem ({perkModel.Type})";
			itemObject.SetActive(true);

			CanvasGroup cardCanvasGroup = itemObject.AddComponent<CanvasGroup>();
			cardCanvasGroup.alpha = 0.5f;

			PerkItem item = itemObject.GetComponent<PerkItem>();
			item.SetModel(perkModel);
			item.OnSelect += OnItemSelect;
			_items.Add(item);
		}

		private void OnItemSelect(PerkItem item)
		{
			_selectedItem = item;
			for (int i = 0; i < _items.Count; i++)
			{
				CanvasGroup itemCanvasGroup = _items[i].GetComponent<CanvasGroup>();
				itemCanvasGroup.alpha = _items[i] == item ? 1f : 0.5f;
			}
			_okButton.interactable = true;
		}

		private void OnOkButtonClick()
		{
			CloseWindow();
		}
	}
}
