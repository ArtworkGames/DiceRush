using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Perks.Perks;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.DiceRush.Game.Players.Signals;
using StepanoffGames.DiceRush.Game.Xp.Signals;
using StepanoffGames.DiceRush.UI.Components.Perks;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using System.Collections.Generic;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Perks
{
	public class PerksManager : MonoBehaviour, IService
	{
		[SerializeField] private PerksPanel _panel;

		private LevelManager _levelManager;

		private void Awake()
		{
			ServiceLocator.Register(this);
		}

		private void Start()
		{
			_levelManager = ServiceLocator.Get<LevelManager>();

			SignalBus.Subscribe<XpMultiplierChangedSignal>(OnXpMultiplierChanged);
			SignalBus.Subscribe<PlayerCellPassedSignal>(OnPlayerCellPassed);
		}

		private void OnDestroy()
		{
			ServiceLocator.Unregister<PerksManager>();

			_levelManager = null;

			SignalBus.Unsubscribe<XpMultiplierChangedSignal>(OnXpMultiplierChanged);
			SignalBus.Unsubscribe<PlayerCellPassedSignal>(OnPlayerCellPassed);
		}

		public List<PerkModel> GetPerksOffer(PlayerModel player)
		{
			List<PerkType> playerPerkTypes = new List<PerkType>();
			for (int i = 0; i < player.PerksSet.Perks.Count; i++)
			{
				playerPerkTypes.Add(player.PerksSet.Perks[i].Type);
			}

			List<PerkType> perkTypes = new List<PerkType>();
			List<PerkType> rndPerkTypes = new List<PerkType>();

			// если карт в каком либо виде предложений меньше размера предложения,
			// то фиксированно добавляем перк пополнения карт
			List<CardModel> deckCards = player.Deck.GetCards(CardKind.Dice);
			List<CardModel> bagCards = player.Deck.GetCards(CardKind.Bag);
			if (player.Type == PlayerType.HI) Debug.Log($"[PerksOffer] Deck Cards = {deckCards.Count}, Bag Cards = {bagCards.Count}");
			if (deckCards.Count < player.CardsPerOffer || bagCards.Count < player.CardsPerOffer)
			{
				if (player.Type == PlayerType.HI) Debug.Log($"[PerksOffer] Add Perk = {PerkType.Take3Cards}");
				perkTypes.Add(PerkType.Take3Cards);
			}

			for (int i = 0; i < PerkModel.AllPerks.Length; i++)
			{
				PerkModel perk = PerkModel.AllPerks[i];

				// если перк уже отобран, то пропускаем
				if (perkTypes.Contains(perk.Type)) continue;
				// если перк уже есть у игрока, то пропускаем
				if (playerPerkTypes.Contains(perk.Type)) continue;

				// если перк - взять три карты, то пропускаем, т.к. этот перк обрабатывается отдельно
				if (perk.Type == PerkType.Take3Cards) continue;
				// если вид перка - воздействие на оппонентов, то пропускаем, т.к. этот вид обрабатывается отдельно
				if (perk.Kind == PerkKind.Opponents) continue;

				// если указан требуемый перк у игрока и если у игрока нет этого перка, то пропускаем
				if (perk.RequiredType != PerkType.Undefined && !playerPerkTypes.Contains(perk.RequiredType)) continue;
				// если у игрока недостаточный уровень, то пропускаем
				if (perk.StartFromLevel > player.Level) continue;

				rndPerkTypes.Add(perk.Type);
			}

			int perksCount = 3 - perkTypes.Count;
			for(int i = 0; i < perksCount; i++)
			{
				if (rndPerkTypes.Count > 0)
				{
					PerkType perkType = rndPerkTypes[Random.Range(0, rndPerkTypes.Count)];
					perkTypes.Add(perkType);
					rndPerkTypes.Remove(perkType);
				}
			}

			perksCount = 3 - perkTypes.Count;
			for (int i = 0; i < perksCount; i++)
			{
				perkTypes.Add(PerkType.Take3Cards);
			}

			List<PerkModel> perks = new List<PerkModel>();
			for (int i = 0; i < perkTypes.Count; i++)
			{
				perks.Add(PerkModel.GetPerk(perkTypes[i]).Clone());
			}

			return perks;
		}

		public async UniTask UsePerk(PlayerController player, PerkModel perkModel)
		{
			Perk perk = GetPerkByModel(perkModel);
			if (perk != null)
			{
				await perk.Use(player);
			}
		}

		public async UniTask ApplyPerk(PlayerController player, PerkModel perkModel)
		{
			Perk perk = GetPerkByModel(perkModel);
			if (perk != null)
			{
				await perk.Apply(player);
			}
		}

		private Perk GetPerkByModel(PerkModel perkModel)
		{
			Perk perk = null;
			switch (perkModel.Type)
			{
				case PerkType.FirstMultiplierX3: perk = new IncXpMultiplierPerk(perkModel); break;
				case PerkType.XpBonusForMultiplierX4: perk = new XpBonusForMultiplierPerk(perkModel); break;

				case PerkType.CardsPerOfferPlus1:
				case PerkType.CardsPerOfferPlus2:
				case PerkType.CardsPerOfferPlus3: perk = new IncCardsPerOfferPerk(perkModel); break;

				case PerkType.Take3Cards: perk = new Take3CardsPerk(perkModel); break;
			}
			return perk;
		}

		public void ShowPerks(PlayerModel player)
		{
			_panel.ShowPerks(player);
		}

		private void OnXpMultiplierChanged(XpMultiplierChangedSignal signal)
		{
			PerkModel perkModel = signal.Player.PerksSet.GetPerk(PerkType.FirstMultiplierX3);
			if (perkModel != null)
			{
				Perk perk = GetPerkByModel(perkModel);
				if (perk != null)
				{
					PlayerController playerController = _levelManager.GetPlayer(signal.Player);
					perk.Apply(playerController).Forget();
				}
			}
		}

		private void OnPlayerCellPassed(PlayerCellPassedSignal signal)
		{
			PerkModel perkModel = signal.Player.Model.PerksSet.GetPerk(PerkType.XpBonusForMultiplierX4);
			if (perkModel != null)
			{
				Perk perk = GetPerkByModel(perkModel);
				if (perk != null)
				{
					perk.Apply(signal.Player).Forget();
				}
			}
		}
	}
}
