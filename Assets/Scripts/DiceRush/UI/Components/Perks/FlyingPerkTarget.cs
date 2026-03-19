using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Perks;
using StepanoffGames.Services;
using System;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Components.Perks
{
	public class FlyingPerkTarget : MonoBehaviour
	{
		[SerializeField] private string _perkType;

		public PerkType PerkType
		{
			get
			{
				if (Enum.TryParse(_perkType, out PerkType type))
				{
					return type;
				}
				return PerkType.Undefined;
			}
		}

		private PerksManager _perksManager;

		//private void Awake()
		//{
		//	_perksManager = ServiceLocator.Get<PerksManager>();
		//}

		private void OnDestroy()
		{
			if (_perksManager != null)
			{
				_perksManager.UnregisterFlyingPerkTarget(this);
				_perksManager = null;
			}
		}

		private async void OnEnable()
		{
			await UniTask.NextFrame();

			if (_perksManager == null)
			{
				_perksManager = ServiceLocator.Get<PerksManager>();
			}
			_perksManager.RegisterFlyingPerkTarget(this);
		}

		private void OnDisable()
		{
			if (_perksManager != null)
			{
				_perksManager.UnregisterFlyingPerkTarget(this);
			}
		}
	}
}
