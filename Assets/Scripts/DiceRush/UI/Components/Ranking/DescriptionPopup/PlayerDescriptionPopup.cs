using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Deck.Signals;
using StepanoffGames.DiceRush.Game.Perks.Perks;
using StepanoffGames.DiceRush.Game.Players;
using StepanoffGames.DiceRush.Game.Xp.Signals;
using StepanoffGames.DiceRush.UI.Components.Perks;
using StepanoffGames.Signals;
using StepanoffGames.UI.Components;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace StepanoffGames.DiceRush.UI.Components.Ranking.DescriptionPopup
{
	public class PlayerDescriptionPopup : MonoBehaviour
	{
		[SerializeField] private GameObject _content;
		[Space]
		[SerializeField] private TMPTextLocalizer _placeValue;
		[Space]
		[SerializeField] private TMPTextLocalizer _levelValue;
		[SerializeField] private TMPTextLocalizer _moveValue;
		[Space]
		[SerializeField] private TMPTextLocalizer _deckValue;
		[Space]
		[SerializeField] private GameObject _perksGroup;
		[SerializeField] private GameObject _sourcePerkItem;
		[Space]
		[SerializeField] private TMP_Text _healthValue;
		[SerializeField] private TMP_Text _defenseValue;
		[SerializeField] private TMP_Text _attackValue;

		private PlayerController _player;

		private Dictionary<PerkType, PerkIconItem> _perks = new Dictionary<PerkType, PerkIconItem>();

		private void Awake()
		{
			_sourcePerkItem.SetActive(false);
			_perksGroup.SetActive(false);
		}

		private void Start()
		{
			Hide();

			SignalBus.Subscribe<MoveXpChangedSignal>(OnMoveXpChanged);
			SignalBus.Subscribe<XpMultiplierChangedSignal>(OnXpMultiplierChanged);
			SignalBus.Subscribe<TotalXpChangedSignal>(OnTotalXpChanged);

			SignalBus.Subscribe<PlayerCardsPerOfferChangedSignal>(OnPlayerCardsPerOfferChanged);
			SignalBus.Subscribe<PlayerCardsChangedSignal>(OnPlayerCardsChanged);
		}

		private void OnDestroy()
		{
			SignalBus.Unsubscribe<MoveXpChangedSignal>(OnMoveXpChanged);
			SignalBus.Unsubscribe<XpMultiplierChangedSignal>(OnXpMultiplierChanged);
			SignalBus.Unsubscribe<TotalXpChangedSignal>(OnTotalXpChanged);

			SignalBus.Unsubscribe<PlayerCardsPerOfferChangedSignal>(OnPlayerCardsPerOfferChanged);
			SignalBus.Unsubscribe<PlayerCardsChangedSignal>(OnPlayerCardsChanged);
		}

		public void Show()
		{
			UpdateValues();
			_content.SetActive(true);
		}

		public void Hide()
		{
			_content.SetActive(false);
		}

		public void SetPlayer(PlayerController player)
		{
			_player = player;
			UpdateValues();
		}

		private void UpdateValues()
		{
			UpdateLevelValues();
			UpdateMoveValues();
			UpdateDeck();
			UpdatePerks();
			UpdateBattleValues();
		}

		public void UpdatePlaceValues(int place, int cell)
		{
			_placeValue.SetParams(
				place.ToString(),
				cell.ToString()
			);
		}

		private void UpdateLevelValues()
		{
			if (_player == null) return;

			int level = Mathf.Max(_player.Model.Level, 1);

			_levelValue.SetParams(
				level.ToString(),
				_player.Model.TotalXp.ToString()
			);
		}

		private void UpdateMoveValues()
		{
			if (_player == null) return;

			int xp = _player.Model.MoveXp * _player.Model.XpMultiplier;

			_moveValue.SetParams(
				_player.Model.MoveXp.ToString(),
				_player.Model.XpMultiplier.ToString(),
				xp.ToString()
			);
		}

		private void UpdateDeck()
		{
			List<CardModel> diceCardModels = _player.Model.Deck.GetCards(CardKind.Dice);
			List<CardModel> bagCardModels = _player.Model.Deck.GetCards(CardKind.Bag);
			List<CardModel> battleCardModels = _player.Model.Deck.GetCards(CardKind.Battle);

			_deckValue.SetParams(
				_player.Model.Deck.Cards.Count.ToString(),
				diceCardModels.Count.ToString(),
				bagCardModels.Count.ToString(),
				battleCardModels.Count.ToString(),
				_player.Model.CardsPerOffer.ToString()
			);
		}

		private void UpdatePerks()
		{
			for (int i = 0; i < _player.Model.PerksSet.Perks.Count; i++)
			{
				if (_player.Model.PerksSet.Perks[i].Usage == PerkUsage.Multiple &&
					!_perks.ContainsKey(_player.Model.PerksSet.Perks[i].Type))
				{
					AddPerk(_player.Model.PerksSet.Perks[i].Type);
				}
			}

			_perksGroup.SetActive(_perks.Count > 0);
		}

		private PerkIconItem AddPerk(PerkType perkType)
		{
			GameObject perkObject = Instantiate(_sourcePerkItem, _sourcePerkItem.transform.parent, false);
			perkObject.name = $"PerkItem ({perkType})";
			perkObject.SetActive(true);

			PerkIconItem perk = perkObject.GetComponent<PerkIconItem>();
			perk.Init(perkType);
			_perks.Add(perkType, perk);

			return perk;
		}

		private void UpdateBattleValues()
		{
			if (_player == null) return;

			_healthValue.text = $"{_player.Model.Health.ToString()}/{_player.Model.MaxHealth.ToString()}";

			int defense = _player.Model.BaseDefense;
			int defenseDelta = _player.Model.Defense + _player.Model.ExtraDefense - _player.Model.BaseDefense;
			string defenseDeltaStr = "";
			if (defenseDelta != 0)
			{
				if (defenseDelta > 0) defenseDeltaStr = $"<color=#090>+{defenseDelta}</color>";
				else defenseDeltaStr = $"<color=#f00>-{defenseDelta}</color>";
			}

			_defenseValue.text = defense.ToString() + defenseDeltaStr;

			int attack = _player.Model.BaseAttack;
			int attackDelta = _player.Model.Attack + _player.Model.ExtraAttack - _player.Model.BaseAttack;
			string attackDeltaStr = "";
			if (attackDelta != 0)
			{
				if (attackDelta > 0) attackDeltaStr = $"<color=#090>+{attackDelta}</color>";
				else attackDeltaStr = $"<color=#f00>-{attackDelta}</color>";
			}

			_attackValue.text = attack.ToString() + attackDeltaStr;
		}

		private void OnMoveXpChanged(MoveXpChangedSignal signal)
		{
			if (_player != null && signal.Player == _player.Model)
			{
				UpdateMoveValues();
			}
		}

		private void OnXpMultiplierChanged(XpMultiplierChangedSignal signal)
		{
			if (_player != null && signal.Player == _player.Model)
			{
				UpdateMoveValues();
			}
		}

		private void OnTotalXpChanged(TotalXpChangedSignal signal)
		{
			if (_player != null && signal.Player == _player.Model)
			{
				UpdateLevelValues();
			}
		}

		private void OnPlayerCardsPerOfferChanged(PlayerCardsPerOfferChangedSignal signal)
		{
			if (_player != null && signal.Player == _player.Model)
			{
				UpdateDeck();
			}
		}

		private void OnPlayerCardsChanged(PlayerCardsChangedSignal signal)
		{
			if (_player != null && signal.Player == _player.Model)
			{
				UpdateDeck();
			}
		}
	}
}
