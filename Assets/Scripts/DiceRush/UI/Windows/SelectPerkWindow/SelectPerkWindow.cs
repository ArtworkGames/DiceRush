using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.UI.Windows;
using StepanoffGames.UI.Windows.Animators;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Windows.SelectPerkWindow
{
	public class SelectPerkWindowParams : BaseWindowParams
	{
		public List<PerkModel> Perks;
		public Action<PerkModel> OnSelect;
	}

	public class SelectPerkWindow : BaseWindow<SelectPerkWindowParams>
	{
		public static string PrefabName = "SelectPerkWindow";

		[Space]
		[SerializeField] private BaseWindowAnimator _fadeAnimator;
		[SerializeField] private BaseWindowAnimator _titleAnimator;
		[Space]
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
				AddItem(i, Params.Perks[i]);
			}
		}

		protected override void AfterOpen()
		{
			_fadeAnimator.OpenAsync().Forget();
			_titleAnimator.OpenAsync().Forget();
		}

		override protected void AfterClose()
		{
			Params.OnSelect?.Invoke(_selectedItem.Model);
		}

		private void AddItem(int index, PerkModel perkModel)
		{
			GameObject itemObject = Instantiate(_sourceItem, _sourceItem.transform.parent, false);
			itemObject.name = $"PerkItem ({perkModel.Type})";
			itemObject.SetActive(true);

			PerkItem item = itemObject.GetComponent<PerkItem>();
			item.Show(index, perkModel);
			item.OnSelect += OnItemSelect;
			_items.Add(item);
		}

		private async void OnItemSelect(PerkItem item)
		{
			_selectedItem = item;

			_fadeAnimator.CloseAsync().Forget();
			_titleAnimator.CloseAsync().Forget();

			for (int i = 0; i < _items.Count; i++)
			{
				if (_items[i] != item || _items[i].Model.Usage != PerkUsage.Multiple)
				{
					_items[i].Hide();
				}
			}

			if (item.Model.Usage == PerkUsage.Multiple)
			{
				item.FlyToIconsPanel();
			}
			else
			{
				item.FlyToTarget();
			}

			await UniTask.WaitForSeconds(0.3f);

			//Params.OnSelect?.Invoke(_selectedItem.Model);

			CloseWindow();
		}
	}
}
