using Game.Core;
using Game.Data;

namespace Game.Service
{
    public class UpgradeService
    {
        private readonly UpgradeTableSO _upgradeTable;
        private readonly GoldService _goldService;
        private readonly EventBus _eventBus;
        private readonly UserDataService _userDataService;

        public UpgradeService(UpgradeTableSO upgradeTable, GoldService goldService, EventBus eventBus, UserDataService userDataService)
        {
            _upgradeTable = upgradeTable;
            _goldService = goldService;
            _eventBus = eventBus;
            _userDataService = userDataService;
        }

        public int GetLevel(string unitId)
        {
            return _userDataService.GetUnitLevel(unitId);
        }

        public bool IsMaxLevel(string unitId)
        {
            var currentLevel = GetLevel(unitId);
            return currentLevel >= _upgradeTable.MaxLevel;
        }

        public int GetUpgradeCost(string unitId)
        {
            if (IsMaxLevel(unitId))
            {
                return 0;
            }

            var nextLevel = GetLevel(unitId) + 1;
            var nextData = _upgradeTable.GetByLevel(nextLevel);
            if (nextData == null)
            {
                return 0;
            }

            return nextData.goldCost;
        }

        public bool TryUpgrade(string unitId)
        {
            if (IsMaxLevel(unitId))
            {
                return false;
            }

            var cost = GetUpgradeCost(unitId);
            if (!_goldService.TrySpend(cost))
            {
                return false;
            }

            var currentLevel = GetLevel(unitId);
            var newLevel = currentLevel + 1;

            _userDataService.SetUnitLevel(unitId, newLevel);
            _userDataService.Save();

            _eventBus.Publish(new UnitUpgradedEvent { UnitId = unitId });

            GameLogger.Log($"[UpgradeService] {unitId} upgraded to level {newLevel}");
            return true;
        }

        public float GetNextHpMultiplier(string unitId)
        {
            if (IsMaxLevel(unitId))
            {
                return GetHpMultiplier(unitId);
            }

            var data = _upgradeTable.GetByLevel(GetLevel(unitId) + 1);
            return data == null ? GetHpMultiplier(unitId) : data.hpMultiplier;
        }

        public float GetNextAttackMultiplier(string unitId)
        {
            if (IsMaxLevel(unitId))
            {
                return GetAttackMultiplier(unitId);
            }

            var data = _upgradeTable.GetByLevel(GetLevel(unitId) + 1);
            return data == null ? GetAttackMultiplier(unitId) : data.attackMultiplier;
        }

        public float GetHpMultiplier(string unitId)
        {
            var level = GetLevel(unitId);
            if (level <= 1)
            {
                return 1.0f;
            }

            var data = _upgradeTable.GetByLevel(level);
            if (data == null)
            {
                return 1.0f;
            }

            return data.hpMultiplier;
        }

        public float GetAttackMultiplier(string unitId)
        {
            var level = GetLevel(unitId);
            if (level <= 1)
            {
                return 1.0f;
            }

            var data = _upgradeTable.GetByLevel(level);
            if (data == null)
            {
                return 1.0f;
            }

            return data.attackMultiplier;
        }
    }
}
