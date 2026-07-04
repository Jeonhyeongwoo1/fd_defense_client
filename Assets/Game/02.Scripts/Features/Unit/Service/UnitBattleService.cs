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
        private readonly GameFlowService _gameFlowService;
        private readonly PoolManager _poolManager;
        private readonly List<UnitEntry> _allySnapshotList = new();
        private readonly List<UnitEntry> _enemySnapshotList = new();

        public UnitBattleService(
            UnitRegistry unitRegistry,
            BaseService baseService,
            GameFlowService gameFlowService,
            PoolManager poolManager)
        {
            _unitRegistry = unitRegistry;
            _baseService = baseService;
            _gameFlowService = gameFlowService;
            _poolManager = poolManager;
        }

        public void Tick()
        {
            if (_gameFlowService.CurrentStateType.Value != GameStateType.WavePlaying)
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

        private void AttackTarget(UnitEntry attacker, UnitEntry target, float deltaTime)
        {
            attacker.Model.AttackTimer -= deltaTime;

            if (attacker.Model.AttackTimer <= 0f)
            {
                target.Model.CurrentHp -= attacker.Model.AttackPower;
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
            _unitRegistry.Unregister(entry);
            entry.View.PlayDead();
            ReleaseAfterDelayAsync(entry).Forget();
        }

        private async UniTaskVoid ReleaseAfterDelayAsync(UnitEntry entry)
        {
            await UniTask.Delay((int)(Const.UnitDeathDelaySeconds * 1000));

            if (entry.View != null)
            {
                _poolManager.Release(entry.View.gameObject);
            }
        }
    }
}
