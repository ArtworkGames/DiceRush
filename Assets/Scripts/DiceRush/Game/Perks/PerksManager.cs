using Cysharp.Threading.Tasks;
using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Perks.Perks;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.DiceRush.Game.Xp.Signals;
using StepanoffGames.DiceRush.UI.Perks;
using StepanoffGames.DiceRush.UI.Popups.FlyingIconPopup;
using StepanoffGames.DiceRush.UI.Windows.SelectPerkWindow;
using StepanoffGames.Services;
using StepanoffGames.Signals;
using StepanoffGames.UI.Windows.Signals;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Perks
{
	public class PerksManager : MonoBehaviour, IService
	{
		[SerializeField] private PerksPanel _panel;

		public PerksPanel Panel => _panel;

		private Dictionary<PerkType, FlyingPerkTarget> _flyingPerkTargets;

		private GameManager _gameManager;

		private CancellationTokenSource cts;

		private void Awake()
		{
			ServiceLocator.Register(this);

			_flyingPerkTargets = new Dictionary<PerkType, FlyingPerkTarget>();
		}

		private void Start()
		{
			_gameManager = ServiceLocator.Get<GameManager>();

			SignalBus.Subscribe<XpMultiplierChangedSignal>(OnXpMultiplierChanged);
		}

		private void OnDestroy()
		{
			cts?.Cancel();
			cts?.Dispose();
			cts = null;

			ServiceLocator.Unregister<PerksManager>();

			_gameManager = null;

			SignalBus.Unsubscribe<XpMultiplierChangedSignal>(OnXpMultiplierChanged);
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
			List<CardModel> battleCards = player.Deck.GetCards(CardKind.Battle);

			if (player.Type == PlayerType.HI) Debug.Log($"[PerksOffer] Deck Cards = {deckCards.Count}, Bag Cards = {bagCards.Count}, Battle Cards = {battleCards.Count}");
			if (deckCards.Count < player.CardsPerOffer || bagCards.Count < player.CardsPerOffer || battleCards.Count < player.CardsPerOffer)
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

				rndPerkTypes.Add(perk.Type);
			}

			int perksCount = 3 - perkTypes.Count;
			for (int i = 0; i < perksCount; i++)
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

		//public async UniTask UsePerk(PlayerController player, PerkModel perkModel)
		//{
		//	Perk perk = GetPerkByModel(perkModel);
		//	if (perk != null)
		//	{
		//		await perk.Use(player);
		//	}
		//}

		//public async UniTask ApplyPerk(PlayerController player, PerkModel perkModel)
		//{
		//	Perk perk = GetPerkByModel(perkModel);
		//	if (perk != null)
		//	{
		//		await perk.Apply(player);
		//	}
		//}

		public async UniTask SelectPerk(PlayerModel player, CancellationToken ct)
		{
			List<PerkModel> perks = GetPerksOffer(player);
			if (perks.Count == 0) return;

			bool levelUpWindowClosed = false;
			PerkModel selectedPerk = null;

			SignalBus.Publish(new OpenWindowSignal(SelectPerkWindow.PrefabName, new SelectPerkWindowParams()
			{
				Perks = perks,
				OnSelect = (PerkModel perk) =>
				{
					selectedPerk = perk;
					levelUpWindowClosed = true;
				}
			}));

			await UniTask.WaitUntil(() => levelUpWindowClosed, cancellationToken: ct);

			if (selectedPerk.Usage != PerkUsage.OneTime)
			{
				player.PerksSet.AddPerk(selectedPerk);
			}

			if (selectedPerk.Usage != PerkUsage.Multiple)
			{
				await ApplyPerk(player, selectedPerk.Type, ct);
			}
		}

		public async UniTask AddPerk(PlayerModel player, CancellationToken ct)
		{
			List<PerkModel> perks = GetPerksOffer(player);
			if (perks.Count == 0) return;

			//PerkModel selectedPerk = perks[Random.Range(0, perks.Count)];
			PerkModel selectedPerk = perks[0];
			for (int i = 1; i < perks.Count; i++)
			{
				if (selectedPerk.Priority > perks[i].Priority)
				{
					selectedPerk = perks[i];
				}
			}

			if (selectedPerk.Usage != PerkUsage.OneTime)
			{
				player.PerksSet.AddPerk(selectedPerk);
			}

			if (selectedPerk.Usage != PerkUsage.Multiple)
			{
				await ApplyPerk(player, selectedPerk.Type, ct);
			}
		}

		//public void ShowPerks(PlayerModel player)
		//{
		//	_panel.ShowPerks(player);
		//}

		private void OnXpMultiplierChanged(XpMultiplierChangedSignal signal)
		{
			cts?.Cancel();
			cts?.Dispose();
			cts = new CancellationTokenSource();

			if (signal.Player.Type == PlayerType.HI)
			{
				UsePlayerPerk(signal.Player, PerkType.FirstMultiplierX3, cts.Token).Forget();
				UsePlayerPerk(signal.Player, PerkType.XpBonusForEachMultiplier, cts.Token).Forget();
				UsePlayerPerk(signal.Player, PerkType.OneCardForMultiplierX5, cts.Token).Forget();
			}
			else
			{
				ApplyPlayerPerk(signal.Player, PerkType.FirstMultiplierX3, cts.Token).Forget();
				ApplyPlayerPerk(signal.Player, PerkType.XpBonusForEachMultiplier, cts.Token).Forget();
				ApplyPlayerPerk(signal.Player, PerkType.OneCardForMultiplierX5, cts.Token).Forget();
			}
		}

		public async UniTask UseForBattleRoundStarted(PlayerModel player, CancellationToken ct)
		{
			await UsePlayerPerk(player, PerkType.IncreaseFirstDefenseBy1, ct);
		}

		public async UniTask UseForPlayerWonBattle(PlayerModel player, CancellationToken ct)
		{
			await UsePlayerPerk(player, PerkType.Restore1HealthAfterVictory, ct);
		}

		private async UniTask UsePlayerPerk(PlayerModel player, PerkType perkType, CancellationToken ct)
		{
			PerkModel perkModel = player.PerksSet.GetPerk(perkType);
			if (perkModel != null)
			{
				Perk perk = GetPerkByModel(perkModel);
				if (perk != null)
				{
					PlayerController playerController = _gameManager.GetPlayer(player);
					bool result = await perk.Use(playerController, ct);

					if (result)
					{
						PerkIconItem perksPanelItem = _panel.GetPerkItem(perkType);
						FlyingPerkTarget flyingPerkTarget = GetFlyingPerkTarget(perkType);

						if (perksPanelItem != null && flyingPerkTarget != null)
						{
							bool isCompleted = false;
							FlyingIconPopup.Show(perksPanelItem.IconObject, flyingPerkTarget.transform, null, () =>
							{
								isCompleted = true;
							});

							await UniTask.WaitUntil(() => isCompleted, cancellationToken: ct);
						}
					}
				}
			}
		}

		private async UniTask ApplyPlayerPerk(PlayerModel player, PerkType perkType, CancellationToken ct)
		{
			PerkModel perkModel = player.PerksSet.GetPerk(perkType);
			if (perkModel != null)
			{
				Perk perk = GetPerkByModel(perkModel);
				if (perk != null)
				{
					PlayerController playerController = _gameManager.GetPlayer(player);
					bool result = await perk.Apply(playerController, ct);
				}
			}
		}

		private async UniTask ApplyPerk(PlayerModel player, PerkType perkType, CancellationToken ct)
		{
			PerkModel perkModel = PerkModel.GetPerk(perkType);
			Perk perk = GetPerkByModel(perkModel);
			if (perk != null)
			{
				PlayerController playerController = _gameManager.GetPlayer(player);
				bool result = await perk.Apply(playerController, ct);
			}
		}

		private Perk GetPerkByModel(PerkModel perkModel)
		{
			Perk perk = null;
			switch (perkModel.Type)
			{
				case PerkType.FirstMultiplierX3: perk = new IncMultiplierPerk(perkModel); break;
				case PerkType.XpBonusForEachMultiplier: perk = new XpBonusForMultiplierPerk(perkModel); break;
				case PerkType.OneCardForMultiplierX5: perk = new CardsForMultiplierPerk(perkModel); break;

				case PerkType.CardsPerOfferPlus1:
				case PerkType.CardsPerOfferPlus2:
				case PerkType.CardsPerOfferPlus3: perk = new IncCardsPerOfferPerk(perkModel); break;

				case PerkType.IncreaseFirstDefenseBy1:
				case PerkType.Restore1HealthAfterVictory: perk = new ChangeBattleStatsPerk(perkModel); break;

				case PerkType.Take3Cards: perk = new TakeCardsPerk(perkModel); break;
			}
			return perk;
		}

		public void RegisterFlyingPerkTarget(FlyingPerkTarget flyingPerkTarget)
		{
			if (_flyingPerkTargets.ContainsKey(flyingPerkTarget.PerkType))
			{
				_flyingPerkTargets[flyingPerkTarget.PerkType] = flyingPerkTarget;
			}
			else
			{
				_flyingPerkTargets.Add(flyingPerkTarget.PerkType, flyingPerkTarget);
			}
		}

		public void UnregisterFlyingPerkTarget(FlyingPerkTarget flyingPerkTarget)
		{
			if (_flyingPerkTargets.ContainsKey(flyingPerkTarget.PerkType) &&
				_flyingPerkTargets[flyingPerkTarget.PerkType] == flyingPerkTarget)
			{
				_flyingPerkTargets.Remove(flyingPerkTarget.PerkType);
			}
		}

		public FlyingPerkTarget GetFlyingPerkTarget(PerkType perkType)
		{
			if (_flyingPerkTargets.ContainsKey(perkType))
			{
				return _flyingPerkTargets[perkType];
			}
			return null;
		}
	}
}
