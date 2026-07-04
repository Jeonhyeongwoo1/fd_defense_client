using Game.Data;

namespace Game.Model
{
    public class UnitModel
    {
        public string Id { get; }
        public UnitSide Side { get; }
        public int MaxHp { get; }
        public int CurrentHp { get; set; }
        public int AttackPower { get; }
        public float AttackInterval { get; }
        public float AttackRange { get; }
        public float MoveSpeed { get; }
        public float AttackTimer { get; set; }

        public bool IsAlive => CurrentHp > 0;

        public UnitModel(UnitData data, UnitSide side)
        {
            Id = data.id;
            Side = side;
            MaxHp = data.hp;
            CurrentHp = data.hp;
            AttackPower = data.attackPower;
            AttackInterval = data.attackInterval;
            AttackRange = data.attackRange;
            MoveSpeed = data.moveSpeed;
            AttackTimer = 0f;
        }

        public UnitModel(EnemyData data, UnitSide side)
        {
            Id = data.id;
            Side = side;
            MaxHp = data.hp;
            CurrentHp = data.hp;
            AttackPower = data.attackPower;
            AttackInterval = data.attackInterval;
            AttackRange = data.attackRange;
            MoveSpeed = data.moveSpeed;
            AttackTimer = 0f;
        }
    }
}
