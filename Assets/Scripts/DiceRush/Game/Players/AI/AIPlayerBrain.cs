using StepanoffGames.DiceRush.Data.Models;
using StepanoffGames.DiceRush.Game.Deck.Cards;
using StepanoffGames.DiceRush.Game.Map;
using StepanoffGames.Services;
using System.Collections.Generic;
using UnityEngine;

namespace StepanoffGames.DiceRush.Game.Players.AI
{
	public class AIPlayerBrain
	{
		private class TargetCellData
		{
			public Cell TargetCell;
			public List<Cell> PrevCells;
			public List<Cell> NextCells;
			public bool HasNearMoveForwardCells;
			public bool HasNearMoveBackwardCells;
			public bool HasNearPortalCells;
			public Card Card;
			public int Distance;
		}

		private PlayerController _player;

		private MapController _mapController;

		public AIPlayerBrain(PlayerController player)
		{
			_player = player;
			_mapController = ServiceLocator.Get<MapController>();
		}

		public CardModel SelectCardForDice(int diceValue, List<Card> cards)
		{
			if (_player.Model.AIBrainType == AIBrainType.NoBrain ||
				_player.Model.AIBrainType == AIBrainType.Easy)
			{
				return null;
			}

			bool forward = true;
			if (((Cell)_player.Avatar.CurrentPoint).Type == CellType.MoveBackward)
			{
				forward = false;
			}

			List<TargetCellData> targetCellDatas = new List<TargetCellData>();

			// находим целевые ячейки без использования карт
			FillTargetCellDatas(targetCellDatas, diceValue, forward, null);

			// находим целевые ячейки при использовании каждой карты
			// при этом, откладываем карты переброса кубика, т.к. их результат заранее неизвестен
			List<Card> rerollDiceCards = new List<Card>();
			for (int i = 0; i < cards.Count; i++)
			{
				Card card = cards[i];
				if (card.Model.Type == CardType.RerollDice)
				{
					rerollDiceCards.Add(card);
					continue;
				}

				if (_player.Model.Color == PlayerColor.Blue)
				{
					Debug.Log($"[AIPlayerBrain] SelectCardForDice: card = {card.Model.Type}");
				}

				int newDiceValue = card.ApplyForDice(_player, diceValue);
				FillTargetCellDatas(targetCellDatas, newDiceValue, forward, card);
			}

			if (_player.Model.Color == PlayerColor.Blue)
			{
				for (int i = 0; i < targetCellDatas.Count; i++)
				Debug.Log($"[AIPlayerBrain] SelectCardForDice | targetCellDatas[{i}]: targetCellIndex = {targetCellDatas[i].TargetCell.Index}, " +
					$"card = {(targetCellDatas[i].Card == null ? "null" : targetCellDatas[i].Card.Model.Type)}");
			}

			TargetCellData targetCellData = SelectTargetCell(targetCellDatas);

			if (_player.Model.Color == PlayerColor.Blue)
			{
				if (targetCellData != null)
				{
					Debug.Log($"[AIPlayerBrain] SelectCardForDice | targetCellData: targetCellIndex = {targetCellData.TargetCell.Index}, " +
						$"card = {(targetCellData.Card == null ? "null" : targetCellData.Card.Model.Type)}");
				}
                else
                {
					Debug.Log($"[AIPlayerBrain] SelectCardForDice | targetCellData: null");
				}
			}

			// TODO добавить использование карт переброса кубика

			if (targetCellData != null && targetCellData.Card != null)
			{
				return targetCellData.Card.Model;
			}
			return null;
		}

		private void FillTargetCellDatas(List<TargetCellData> targetCellDatas, int diceValue, bool forward, Card card)
		{
			List<Cell> targetCells = _mapController.GetCellsOnDistance((Cell)_player.Avatar.CurrentPoint, diceValue, forward);
			for (int i = 0; i < targetCells.Count; i++)
			{
				List<Cell> prevCells = _mapController.GetCellsOnDistance(targetCells[i], 1, false);
				List<Cell> nextCells = _mapController.GetCellsOnDistance(targetCells[i], 1, true);

				bool hasNearMoveForwardCells = false;
				bool hasNearMoveBackwardCells = false;
				bool hasNearPortalCells = false;
				for (int j = 0; j < prevCells.Count; j++)
				{
					if (prevCells[j].Type == CellType.MoveForward) hasNearMoveForwardCells = true;
					if (prevCells[j].Type == CellType.MoveBackward) hasNearMoveBackwardCells = true;
					if (prevCells[j].Type == CellType.Portal) hasNearPortalCells = true;
				}
				for (int j = 0; j < nextCells.Count; j++)
				{
					if (nextCells[j].Type == CellType.MoveForward) hasNearMoveForwardCells = true;
					if (nextCells[j].Type == CellType.MoveBackward) hasNearMoveBackwardCells = true;
					if (nextCells[j].Type == CellType.Portal) hasNearPortalCells = true;
				}

				int distance = targetCells[i].Index - ((Cell)_player.Avatar.CurrentPoint).Index;
				targetCellDatas.Add(new TargetCellData()
				{
					TargetCell = targetCells[i],
					PrevCells = prevCells,
					NextCells = nextCells,
					HasNearMoveForwardCells = hasNearMoveForwardCells,
					HasNearMoveBackwardCells = hasNearMoveBackwardCells,
					HasNearPortalCells = hasNearPortalCells,
					Card = card,
					Distance = distance
				});
			}
		}

		private TargetCellData SelectTargetCell(List<TargetCellData> targetCellDatas)
		{
			// ищем вариант с целевой ячейкой "Портал"
			TargetCellData portalCellData = null;
			for (int i = 0; i < targetCellDatas.Count; i++)
			{
				if (targetCellDatas[i].TargetCell.Type == CellType.Portal &&
					!targetCellDatas[i].TargetCell.IsUsed && !targetCellDatas[i].TargetCell.IsLocked)
				{
					if (portalCellData == null)
					{
						portalCellData = targetCellDatas[i];
					}
					else
					{
						portalCellData = GetBetterTargetCellData(portalCellData, targetCellDatas[i]);
					}
				}
			}
			// если есть вариант с целевой ячейкой "Портал",
			// и этот портал дальше от текущей позиции более чем на 6 ячеек,
			// то останавливаемся на нём
			if (portalCellData != null)
			{
				Cell otherPortal = _mapController.GetOtherPortal(portalCellData.TargetCell);
				if (otherPortal != null && ((otherPortal.Index - portalCellData.TargetCell.Index) > 6))
				{
					return portalCellData;
				}
			}

			// ищем вариант с целевой ячейкой "Ход вперед"
			TargetCellData moveForwardCellData = null;
			for (int i = 0; i < targetCellDatas.Count; i++)
			{
				if (targetCellDatas[i].TargetCell.Type == CellType.MoveForward &&
					!targetCellDatas[i].TargetCell.IsUsed && !targetCellDatas[i].TargetCell.IsLocked)
				{
					if (moveForwardCellData == null)
					{
						moveForwardCellData = targetCellDatas[i];
					}
					else
					{
						moveForwardCellData = GetBetterTargetCellData(moveForwardCellData, targetCellDatas[i]);
					}
				}
			}
			// если есть вариант с целевой ячейкой "Ход вперед", то останавливаемся на нём
			if (moveForwardCellData != null)
			{
				return moveForwardCellData;
			}

			// ищем вариант с пустой целевой ячейкой
			TargetCellData emptyCellData = null;
			for (int i = 0; i < targetCellDatas.Count; i++)
			{
				if (targetCellDatas[i].TargetCell.Type == CellType.Empty &&
					!targetCellDatas[i].TargetCell.IsUsed && !targetCellDatas[i].TargetCell.IsLocked)
				{
					if (emptyCellData == null)
					{
						emptyCellData = targetCellDatas[i];
					}
					else
					{
						// если текущая ячейка не имеет соседних ячеек "Ход вперед" и имеет соседние ячейки "Ход назад"
						if (!targetCellDatas[i].HasNearMoveForwardCells && targetCellDatas[i].HasNearMoveBackwardCells)
						{
							// если уже отобранная ячейка такая же, то берём лучшую
							if (!emptyCellData.HasNearMoveForwardCells && emptyCellData.HasNearMoveBackwardCells)
							{
								emptyCellData = GetBetterTargetCellData(emptyCellData, targetCellDatas[i]);
							}
							// если уже отобранная ячейка не такая, то берём текущую
							else
							{
								emptyCellData = targetCellDatas[i];
							}
						}
						// если текущая ячейка не имеет соседних ячеек "Ход вперед"
						else if (!targetCellDatas[i].HasNearMoveForwardCells)
						{
							// если уже отобранная ячейка такая же, то берём лучшую
							if (!emptyCellData.HasNearMoveForwardCells)
							{
								emptyCellData = GetBetterTargetCellData(emptyCellData, targetCellDatas[i]);
							}
							// если уже отобранная ячейка не такая, то берём текущую
							else
							{
								emptyCellData = targetCellDatas[i];
							}
						}
						else
						{
							emptyCellData = GetBetterTargetCellData(emptyCellData, targetCellDatas[i]);
						}
					}
				}
			}
			// если есть вариант с пустой целевой ячейкой, то останавливаемся на нём
			if (emptyCellData != null)
			{
				return emptyCellData;
			}

			return null;
		}

		private TargetCellData GetBetterTargetCellData(TargetCellData firstTargetCellData, TargetCellData secondTargetCellData)
		{
			if ((firstTargetCellData.Card != null && secondTargetCellData.Card == null) ||
				(firstTargetCellData.Card == null && secondTargetCellData.Card == null && firstTargetCellData.Distance < secondTargetCellData.Distance) ||
				(firstTargetCellData.Card != null && secondTargetCellData.Card != null && firstTargetCellData.Distance < secondTargetCellData.Distance))
			{
				return secondTargetCellData;
			}
			return firstTargetCellData;
		}

		public int SelectDirection(int diceValue, int cellsPassed, bool forward)
		{
			int direction = 0;

			if (_player.Model.AIBrainType == AIBrainType.NoBrain ||
				_player.Model.AIBrainType == AIBrainType.Easy)
			{
				if (forward)
					direction = Random.Range(0, _player.Avatar.CurrentPoint.NextPoints.Count);
				else
					direction = Random.Range(0, _player.Avatar.CurrentPoint.PrevPoints.Count);
				return direction;
			}

			List<TargetCellData> targetCellDatas = new List<TargetCellData>();
			FillTargetCellDatas(targetCellDatas, diceValue - cellsPassed, forward, null);

			TargetCellData targetCellData = SelectTargetCell(targetCellDatas);
			if (targetCellData != null)
			{
				direction = _mapController.GetDirectionToCell((Cell)_player.Avatar.CurrentPoint, targetCellData.TargetCell, forward);
				if (direction >= 0)
					return direction;
			}

			if (forward)
				direction = Random.Range(0, _player.Avatar.CurrentPoint.NextPoints.Count);
			else
				direction = Random.Range(0, _player.Avatar.CurrentPoint.PrevPoints.Count);
			return direction;
		}
	}
}
