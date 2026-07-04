using Game.Core;
using Game.Data;
using Game.Model;
using Game.View;
using UnityEngine;

namespace Game.Service
{
    public class UnitFactory
    {
        private readonly PoolManager _poolManager;
        private readonly UnitRegistry _unitRegistry;
        private readonly UpgradeService _upgradeService;
        private float _enemyStatMultiplier = 1.0f;

        public UnitFactory(PoolManager poolManager, UnitRegistry unitRegistry, UpgradeService upgradeService)
        {
            _poolManager = poolManager;
            _unitRegistry = unitRegistry;
            _upgradeService = upgradeService;
        }

        public void SetEnemyStatMultiplier(float multiplier)
        {
            _enemyStatMultiplier = multiplier;
        }

        public UnitEntry SpawnBoss(BossData data, Vector3 position)
        {
            var instance = _poolManager.Get(data.prefab, position, Quaternion.identity);

            var view = instance.GetComponent<UnitView>();
            if (view == null)
            {
                view = instance.AddComponent<UnitView>();
            }

            var scaledHp = Mathf.RoundToInt(data.hp * _enemyStatMultiplier);
            var scaledAttackPower = Mathf.RoundToInt(data.attackPower * _enemyStatMultiplier);

            // NOTE: Boss skill damage is not scaled — skill damage remains at base value per design constraint
            var scaledData = new BossData
            {
                id = data.id,
                unitName = data.unitName,
                hp = scaledHp,
                attackPower = scaledAttackPower,
                attackInterval = data.attackInterval,
                attackRange = data.attackRange,
                moveSpeed = data.moveSpeed,
                skillDamage = data.skillDamage,
                skillInterval = data.skillInterval,
                skillRange = data.skillRange,
                prefab = data.prefab
            };

            var model = new UnitModel(scaledData, UnitSide.Enemy);

            var entry = new UnitEntry
            {
                Model = model,
                View = view
            };

            view.Initialize(UnitSide.Enemy);
            _unitRegistry.Register(entry);
            view.PlayWalk();

            return entry;
        }

        public UnitEntry SpawnAlly(UnitData data, Vector3 position)
        {
            var instance = _poolManager.Get(data.prefab, position, Quaternion.identity);

            // Third-party prefab boundary — UnitView not serialized in prefab.
            // GetComponent fallback allowed here (exception to rule).
            var view = instance.GetComponent<UnitView>();
            if (view == null)
            {
                view = instance.AddComponent<UnitView>();
            }

            var hpMultiplier = _upgradeService.GetHpMultiplier(data.id);
            var attackMultiplier = _upgradeService.GetAttackMultiplier(data.id);

            var upgradedHp = Mathf.RoundToInt(data.hp * hpMultiplier);
            var upgradedAttackPower = Mathf.RoundToInt(data.attackPower * attackMultiplier);

            var upgradedData = new UnitData
            {
                id = data.id,
                unitName = data.unitName,
                hp = upgradedHp,
                attackPower = upgradedAttackPower,
                attackInterval = data.attackInterval,
                attackRange = data.attackRange,
                moveSpeed = data.moveSpeed,
                cost = data.cost,
                cooldown = data.cooldown,
                isRanged = data.isRanged,
                projectileSpeed = data.projectileSpeed,
                projectileSprite = data.projectileSprite,
                iconSprite = data.iconSprite,
                prefab = data.prefab
            };

            var model = new UnitModel(upgradedData, UnitSide.Ally);

            var entry = new UnitEntry
            {
                Model = model,
                View = view
            };

            view.Initialize(UnitSide.Ally);
            _unitRegistry.Register(entry);
            view.PlayWalk();

            return entry;
        }

        public UnitEntry SpawnEnemy(EnemyData data, Vector3 position)
        {
            var instance = _poolManager.Get(data.prefab, position, Quaternion.identity);

            // Third-party prefab boundary — UnitView not serialized in prefab.
            // GetComponent fallback allowed here (exception to rule).
            var view = instance.GetComponent<UnitView>();
            if (view == null)
            {
                view = instance.AddComponent<UnitView>();
            }

            var scaledHp = Mathf.RoundToInt(data.hp * _enemyStatMultiplier);
            var scaledAttackPower = Mathf.RoundToInt(data.attackPower * _enemyStatMultiplier);

            var scaledData = new EnemyData
            {
                id = data.id,
                unitName = data.unitName,
                hp = scaledHp,
                attackPower = scaledAttackPower,
                attackInterval = data.attackInterval,
                attackRange = data.attackRange,
                moveSpeed = data.moveSpeed,
                prefab = data.prefab
            };

            var model = new UnitModel(scaledData, UnitSide.Enemy);

            var entry = new UnitEntry
            {
                Model = model,
                View = view
            };

            view.Initialize(UnitSide.Enemy);
            _unitRegistry.Register(entry);
            view.PlayWalk();

            return entry;
        }
    }
}
