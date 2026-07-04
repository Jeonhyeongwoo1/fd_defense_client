using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Core;
using Game.GameState;
using Game.Model;
using UnityEngine;
using VContainer.Unity;

namespace Game.Service
{
    public class UnitBattleService : ITickable
    {
        private readonly UnitRegistry _unitRegistry;
        private readonly BaseService _baseService;
        private readonly GameStateModel _gameStateModel;
        private readonly PoolManager _poolManager;
        private readonly ProjectileService _projectileService;
        private readonly EffectService _effectService;
        private readonly BossSkillService _bossSkillService;
        private readonly EventBus _eventBus;
        private readonly List<UnitEntry> _allySnapshotList = new();
        private readonly List<UnitEntry> _enemySnapshotList = new();

        public UnitBattleService(
            UnitRegistry unitRegistry,
            BaseService baseService,
            GameStateModel gameStateModel,
            PoolManager poolManager,
            ProjectileService projectileService,
            EffectService effectService,
            BossSkillService bossSkillService,
            EventBus eventBus)
        {
            _unitRegistry = unitRegistry;
            _baseService = baseService;
            _gameStateModel = gameStateModel;
            _poolManager = poolManager;
            _projectileService = projectileService;
            _effectService = effectService;
            _bossSkillService = bossSkillService;
            _eventBus = eventBus;

            _projectileService.OnTargetKilled = HandleDeath;
            _bossSkillService.OnTargetKilled = HandleDeath;
        }

        public void Tick()
        {
            if (_gameStateModel.CurrentStateType.Value != GameStateType.WavePlaying)
            {
                return;
            }

            ProcessSide(UnitSide.Ally);
            ProcessSide(UnitSide.Enemy);
        }

        private void ProcessSide(UnitSide side)
        {
            var entryList = _unitRegistry.GetEntryList(side);
            var snapshot = side == UnitSide.Ally ? _allySnapshotList : _enemySnapshotList;
            snapshot.Clear();
            snapshot.AddRange(entryList);
            var deltaTime = Time.deltaTime;

            foreach (var entry in snapshot)
            {
                if (!entry.Model.IsAlive)
                {
                    continue;
                }

                var enemySide = side == UnitSide.Ally ? UnitSide.Enemy : UnitSide.Ally;
                var enemyEntryList = _unitRegistry.GetEntryList(enemySide);

                UnitEntry closestTarget = null;
                var minDistance = float.MaxValue;

                foreach (var enemyEntry in enemyEntryList)
                {
                    if (!enemyEntry.Model.IsAlive)
                    {
                        continue;
                    }

                    var dx = Mathf.Abs(enemyEntry.View.transform.position.x - entry.View.transform.position.x);

                    if (dx <= entry.Model.AttackRange && dx < minDistance)
                    {
                        closestTarget = enemyEntry;
                        minDistance = dx;
                    }
                }

                if (closestTarget != null)
                {
                    AttackTarget(entry, closestTarget, deltaTime);
                }
                else
                {
                    var baseX = side == UnitSide.Ally ? Const.EnemyBaseX : Const.AllyBaseX;
                    var distanceToBase = Mathf.Abs(baseX - entry.View.transform.position.x);

                    if (distanceToBase <= entry.Model.AttackRange)
                    {
                        AttackBase(entry, enemySide, deltaTime);
                    }
                    else
                    {
                        Move(entry, side, deltaTime);
                    }
                }
            }
        }

        public void ReleaseAllUnits()
        {
            var allySnapshot = new List<UnitEntry>(_unitRegistry.GetEntryList(UnitSide.Ally));
            var enemySnapshot = new List<UnitEntry>(_unitRegistry.GetEntryList(UnitSide.Enemy));

            foreach (var entry in allySnapshot)
            {
                _unitRegistry.Unregister(entry);
                _poolManager.Release(entry.View.gameObject);
            }

            foreach (var entry in enemySnapshot)
            {
                _unitRegistry.Unregister(entry);
                _poolManager.Release(entry.View.gameObject);
            }

            _projectileService.ReleaseAll();
        }

        private void AttackTarget(UnitEntry attacker, UnitEntry target, float deltaTime)
        {
            attacker.Model.AttackTimer -= deltaTime;

            if (attacker.Model.AttackTimer <= 0f)
            {
                if (attacker.Model.IsRanged)
                {
                    _projectileService.Fire(attacker, target, attacker.Model.AttackPower, attacker.Model.ProjectileSpeed, attacker.Model.ProjectileSprite);
                }
                else
                {
                    target.Model.CurrentHp -= attacker.Model.AttackPower;
                    _effectService.PlayHitEffect(target.View.transform.position);
                }

                attacker.View.PlayAttack();
                attacker.Model.AttackTimer = attacker.Model.AttackInterval;

                if (!target.Model.IsAlive)
                {
                    HandleDeath(target);
                }
            }
        }

        private void AttackBase(UnitEntry attacker, UnitSide targetSide, float deltaTime)
        {
            attacker.Model.AttackTimer -= deltaTime;

            if (attacker.Model.AttackTimer <= 0f)
            {
                _baseService.ApplyDamage(targetSide, attacker.Model.AttackPower);
                attacker.View.PlayAttack();
                attacker.Model.AttackTimer = attacker.Model.AttackInterval;
            }
        }

        private void Move(UnitEntry entry, UnitSide side, float deltaTime)
        {
            var direction = side == UnitSide.Ally ? 1f : -1f;
            var position = entry.View.transform.position;
            position.x += direction * entry.Model.MoveSpeed * deltaTime;
            entry.View.transform.position = position;

            entry.View.PlayWalk();
        }

        private void HandleDeath(UnitEntry entry)
        {
            if (!_unitRegistry.Unregister(entry))
            {
                return;
            }

            if (entry.Model.Side == UnitSide.Enemy)
            {
                _eventBus.Publish(new EnemyKilledEvent { IsBoss = entry.Model.IsBoss });
            }

            entry.View.PlayDead();

            if (entry.Model.IsBoss)
            {
                _effectService.PlayBossDeathEffect(entry.View.transform.position);
            }
            else
            {
                _effectService.PlayDeathEffect(entry.View.transform.position);
            }

            ReleaseAfterDelayAsync(entry).Forget();
        }

        private async UniTaskVoid ReleaseAfterDelayAsync(UnitEntry entry)
        {
            // Capture generation before delay — prevents double-release if Result cleanup already returned this view or pool recycled it
            var generation = entry.View.SpawnGeneration;

            await UniTask.Delay((int)(Const.UnitDeathDelaySeconds * 1000));

            if (entry.View == null || entry.View.SpawnGeneration != generation)
            {
                return;
            }

            _poolManager.Release(entry.View.gameObject);
        }
    }
}
