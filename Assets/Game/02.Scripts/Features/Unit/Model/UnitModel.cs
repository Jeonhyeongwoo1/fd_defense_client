using Game.Data;
using UnityEngine;

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
        public bool IsRanged { get; }
        public float ProjectileSpeed { get; }
        public Sprite ProjectileSprite { get; }
        public bool IsBoss { get; }
        public int SkillDamage { get; }
        public float SkillInterval { get; }
        public float SkillRange { get; }
        public float SkillTimer { get; set; }

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
            IsRanged = data.isRanged;
            ProjectileSpeed = data.projectileSpeed;
            ProjectileSprite = data.projectileSprite;
            IsBoss = false;
            SkillDamage = 0;
            SkillInterval = 0f;
            SkillRange = 0f;
            SkillTimer = 0f;
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
            IsRanged = false;
            ProjectileSpeed = 0f;
            ProjectileSprite = null;
            IsBoss = false;
            SkillDamage = 0;
            SkillInterval = 0f;
            SkillRange = 0f;
            SkillTimer = 0f;
        }

        public UnitModel(BossData data, UnitSide side)
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
            IsRanged = false;
            ProjectileSpeed = 0f;
            ProjectileSprite = null;
            IsBoss = true;
            SkillDamage = data.skillDamage;
            SkillInterval = data.skillInterval;
            SkillRange = data.skillRange;
            SkillTimer = 0f;
        }
    }
}
