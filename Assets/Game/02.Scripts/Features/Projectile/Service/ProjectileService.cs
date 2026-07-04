using System;
using System.Collections.Generic;
using Game.Core;
using Game.Model;
using Game.Service;
using Game.View;
using UnityEngine;
using VContainer.Unity;

namespace Game.Service
{
    public class ProjectileService : ITickable
    {
        public Action<UnitEntry> OnTargetKilled;

        private readonly PoolManager _poolManager;
        private readonly UnitRegistry _unitRegistry;
        private readonly BaseService _baseService;
        private readonly EffectService _effectService;
        private readonly List<ProjectileEntry> _activeProjectileList = new();
        private readonly Dictionary<string, GameObject> _templateDict = new();

        private class ProjectileEntry
        {
            public ProjectileView View;
            public UnitEntry TargetEntry;
            public float TargetDestroyedX;
            public int Damage;
            public float Speed;
            public float DirectionSign;
        }

        public ProjectileService(
            PoolManager poolManager,
            UnitRegistry unitRegistry,
            BaseService baseService,
            EffectService effectService)
        {
            _poolManager = poolManager;
            _unitRegistry = unitRegistry;
            _baseService = baseService;
            _effectService = effectService;
        }

        public void Fire(UnitEntry shooter, UnitEntry target, int damage, float speed, Sprite sprite)
        {
            var template = GetOrCreateTemplate(sprite);
            var instance = _poolManager.Get(template, shooter.View.transform.position, Quaternion.identity);
            var view = instance.GetComponent<ProjectileView>();

            var isFacingRight = shooter.Model.Side == UnitSide.Ally;
            view.Initialize(sprite, isFacingRight);

            var entry = new ProjectileEntry
            {
                View = view,
                TargetEntry = target,
                TargetDestroyedX = target.View.transform.position.x,
                Damage = damage,
                Speed = speed,
                DirectionSign = isFacingRight ? 1f : -1f
            };

            _activeProjectileList.Add(entry);
        }

        public void Tick()
        {
            var deltaTime = Time.deltaTime;

            for (var i = _activeProjectileList.Count - 1; i >= 0; i--)
            {
                var entry = _activeProjectileList[i];
                var position = entry.View.transform.position;
                position.x += entry.DirectionSign * entry.Speed * deltaTime;
                entry.View.transform.position = position;

                var shouldRelease = false;

                if (entry.TargetEntry.Model.IsAlive)
                {
                    var targetX = entry.TargetEntry.View.transform.position.x;
                    var hasPassed = entry.DirectionSign > 0 ? position.x >= targetX : position.x <= targetX;

                    if (hasPassed)
                    {
                        entry.TargetEntry.Model.CurrentHp -= entry.Damage;
                        _effectService.PlayHitEffect(entry.TargetEntry.View.transform.position);

                        if (!entry.TargetEntry.Model.IsAlive)
                        {
                            OnTargetKilled?.Invoke(entry.TargetEntry);
                        }

                        shouldRelease = true;
                    }
                }
                else
                {
                    var hasPassed = entry.DirectionSign > 0 ? position.x >= entry.TargetDestroyedX : position.x <= entry.TargetDestroyedX;

                    if (hasPassed)
                    {
                        shouldRelease = true;
                    }
                }

                if (Mathf.Abs(position.x) > Const.ProjectileDespawnX)
                {
                    shouldRelease = true;
                }

                if (shouldRelease)
                {
                    _poolManager.Release(entry.View.gameObject);
                    _activeProjectileList.RemoveAt(i);
                }
            }
        }

        public void ReleaseAll()
        {
            for (var i = _activeProjectileList.Count - 1; i >= 0; i--)
            {
                _poolManager.Release(_activeProjectileList[i].View.gameObject);
            }

            _activeProjectileList.Clear();
        }

        private GameObject GetOrCreateTemplate(Sprite sprite)
        {
            var key = sprite.name;

            if (!_templateDict.TryGetValue(key, out var template))
            {
                template = new GameObject($"Projectile_{key}");
                template.SetActive(false);

                var spriteRenderer = template.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = sprite;

                template.AddComponent<ProjectileView>();

                _templateDict[key] = template;
            }

            return template;
        }
    }
}
