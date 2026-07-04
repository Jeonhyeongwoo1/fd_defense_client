using System;
using System.Collections.Generic;
using Game.GameState;
using Game.Model;
using Game.Service;
using UnityEngine;
using VContainer.Unity;

namespace Game.Service
{
    public class BossSkillService : ITickable
    {
        public Action<UnitEntry> OnTargetKilled;

        private readonly UnitRegistry _unitRegistry;
        private readonly GameStateModel _gameStateModel;
        private readonly EffectService _effectService;

        private UnitEntry _cachedBossEntry;
        private readonly List<UnitEntry> _skillTargetSnapshotList = new();

        public BossSkillService(
            UnitRegistry unitRegistry,
            GameStateModel gameStateModel,
            EffectService effectService)
        {
            _unitRegistry = unitRegistry;
            _gameStateModel = gameStateModel;
            _effectService = effectService;
        }

        public void Tick()
        {
            if (_gameStateModel.CurrentStateType.Value != GameStateType.WavePlaying)
            {
                return;
            }

            var bossEntry = GetBossEntry();
            if (bossEntry == null)
            {
                return;
            }

            bossEntry.Model.SkillTimer += Time.deltaTime;

            if (bossEntry.Model.SkillTimer >= bossEntry.Model.SkillInterval)
            {
                ExecuteBossSkill(bossEntry);
                bossEntry.Model.SkillTimer = 0f;
            }
        }

        private UnitEntry GetBossEntry()
        {
            if (_cachedBossEntry != null && _cachedBossEntry.Model.IsAlive && _cachedBossEntry.Model.IsBoss)
            {
                return _cachedBossEntry;
            }

            _cachedBossEntry = null;

            var enemyEntryList = _unitRegistry.GetEntryList(UnitSide.Enemy);
            foreach (var entry in enemyEntryList)
            {
                if (entry.Model.IsBoss && entry.Model.IsAlive)
                {
                    _cachedBossEntry = entry;
                    return entry;
                }
            }

            return null;
        }

        private void ExecuteBossSkill(UnitEntry bossEntry)
        {
            var bossX = bossEntry.View.transform.position.x;
            var allyEntryList = _unitRegistry.GetEntryList(UnitSide.Ally);

            _skillTargetSnapshotList.Clear();
            _skillTargetSnapshotList.AddRange(allyEntryList);

            foreach (var allyEntry in _skillTargetSnapshotList)
            {
                if (!allyEntry.Model.IsAlive)
                {
                    continue;
                }

                var allyX = allyEntry.View.transform.position.x;
                var distance = Mathf.Abs(allyX - bossX);

                if (distance <= bossEntry.Model.SkillRange)
                {
                    allyEntry.Model.CurrentHp -= bossEntry.Model.SkillDamage;

                    if (!allyEntry.Model.IsAlive)
                    {
                        OnTargetKilled?.Invoke(allyEntry);
                    }
                }
            }

            _effectService.PlayBossSkillEffect(bossEntry.View.transform.position);
            bossEntry.View.PlaySkill();
        }
    }
}
