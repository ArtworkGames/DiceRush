using System.Linq;

namespace StepanoffGames.DiceRush.Data.Models
{
	public enum EnemyType
	{
		Undefined,

		Mushroom,
		Rat,
		Spider,
		Slime,
		Skeleton,
		Ghost,
		Goblin,
		Orc,
	}

	public class EnemyModel
	{
		public static EnemyModel[] AllEnemies = new EnemyModel[]
		{
			new EnemyModel(EnemyType.Mushroom, health: 4,  baseAttack: 0, fromPosition: 0f,   toPosition: 0.3f),
			new EnemyModel(EnemyType.Rat,      health: 4,  baseAttack: 1, fromPosition: 0.1f, toPosition: 0.4f),
			new EnemyModel(EnemyType.Spider,   health: 4,  baseAttack: 2, fromPosition: 0.2f, toPosition: 0.5f),
			new EnemyModel(EnemyType.Slime,    health: 6,  baseAttack: 0, fromPosition: 0.3f, toPosition: 0.6f),
			new EnemyModel(EnemyType.Skeleton, health: 7,  baseAttack: 1, fromPosition: 0.4f, toPosition: 0.7f),
			new EnemyModel(EnemyType.Ghost,    health: 8,  baseAttack: 2, fromPosition: 0.5f, toPosition: 0.8f),
			new EnemyModel(EnemyType.Goblin,   health: 11, baseAttack: 1, fromPosition: 0.6f, toPosition: 0.9f),
			new EnemyModel(EnemyType.Orc,      health: 12, baseAttack: 2, fromPosition: 0.7f, toPosition: 1f),
		};

		public static EnemyModel GetEnemy(EnemyType type)
		{
			return AllEnemies.First(e => e.Type == type);
		}

		public EnemyType Type;

		public int Health;
		public int BaseAttack;
		public int MaxAttack;

		public float FromPosition;
		public float ToPosition;

		public EnemyModel(EnemyType type, int health, int baseAttack, float fromPosition, float toPosition)
		{
			Type = type;

			Health = health;
			BaseAttack = baseAttack;

			FromPosition = fromPosition;
			ToPosition = toPosition;
		}

		public EnemyModel Clone()
		{
			EnemyModel enemy = new EnemyModel(Type, Health, BaseAttack, FromPosition, ToPosition);
			return enemy;
		}
	}
}
