using System.Collections.Generic;
using Game.Core;
using Game.Data;
using UnityEngine;

namespace Game.Service
{
    public class UnitSpawnService
    {
        private readonly UnitTableSO _unitTable;
        private readonly UnitFactory _unitFactory;
        private readonly WalletService _walletService;
        private readonly Dictionary<string, float> _nextSpawnTimeDict = new();

        public UnitSpawnService(
            UnitTableSO unitTable,
            UnitFactory unitFactory,
            WalletService walletService)
        {
            _unitTable = unitTable;
            _unitFactory = unitFactory;
            _walletService = walletService;
        }

        public bool TrySpawnAlly(string unitId)
        {
            var data = _unitTable.GetById(unitId);
            if (data == null)
            {
                GameLogger.LogError($"[UnitSpawnService] Unit not found: {unitId}");
                return false;
            }

            if (_nextSpawnTimeDict.TryGetValue(unitId, out var nextSpawnTime))
            {
                if (Time.time < nextSpawnTime)
                {
                    return false;
                }
            }

            if (!_walletService.TrySpend(data.cost))
            {
                return false;
            }

            var position = new Vector3(Const.AllyBaseX + Const.UnitSpawnOffsetX, Const.GroundY, 0);
            _unitFactory.SpawnAlly(data, position);

            _nextSpawnTimeDict[unitId] = Time.time + data.cooldown;

            return true;
        }

        public float GetCooldownRemaining(string unitId)
        {
            if (!_nextSpawnTimeDict.TryGetValue(unitId, out var nextSpawnTime))
            {
                return 0f;
            }

            return Mathf.Max(0f, nextSpawnTime - Time.time);
        }
    }
}
