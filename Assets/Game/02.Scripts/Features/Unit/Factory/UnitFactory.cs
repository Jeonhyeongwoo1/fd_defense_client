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

        public UnitFactory(PoolManager poolManager, UnitRegistry unitRegistry)
        {
            _poolManager = poolManager;
            _unitRegistry = unitRegistry;
        }

        public UnitEntry SpawnBoss(BossData data, Vector3 position)
        {
            var instance = _poolManager.Get(data.prefab, position, Quaternion.identity);

            var view = instance.GetComponent<UnitView>();
            if (view == null)
            {
                view = instance.AddComponent<UnitView>();
            }

            var model = new UnitModel(data, UnitSide.Enemy);

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

            var model = new UnitModel(data, UnitSide.Ally);

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

            var model = new UnitModel(data, UnitSide.Enemy);

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
