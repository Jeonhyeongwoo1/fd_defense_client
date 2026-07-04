using System;
using Game.Core;
using Game.Data;
using Game.Service;
using VContainer.Unity;

namespace Game.Controller
{
    public class MissionProgressController : IStartable, IDisposable
    {
        private readonly EventBus _eventBus;
        private readonly MissionService _missionService;

        private IDisposable _enemyKilledSubscription;
        private IDisposable _stageClearedSubscription;
        private IDisposable _unitUpgradedSubscription;

        public MissionProgressController(EventBus eventBus, MissionService missionService)
        {
            _eventBus = eventBus;
            _missionService = missionService;
        }

        public void Start()
        {
            _enemyKilledSubscription = _eventBus.Subscribe<EnemyKilledEvent>(OnEnemyKilled);
            _stageClearedSubscription = _eventBus.Subscribe<StageClearedEvent>(OnStageCleared);
            _unitUpgradedSubscription = _eventBus.Subscribe<UnitUpgradedEvent>(OnUnitUpgraded);
        }

        public void Dispose()
        {
            _enemyKilledSubscription?.Dispose();
            _stageClearedSubscription?.Dispose();
            _unitUpgradedSubscription?.Dispose();
        }

        private void OnEnemyKilled(EnemyKilledEvent evt)
        {
            _missionService.AddProgress(MissionType.KillEnemies, 1);

            if (evt.IsBoss)
            {
                _missionService.AddProgress(MissionType.KillBosses, 1);
            }
        }

        private void OnStageCleared(StageClearedEvent evt)
        {
            _missionService.AddProgress(MissionType.ClearStages, 1);
        }

        private void OnUnitUpgraded(UnitUpgradedEvent evt)
        {
            _missionService.AddProgress(MissionType.UpgradeUnits, 1);
        }
    }
}
