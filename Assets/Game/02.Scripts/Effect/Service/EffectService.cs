using Cysharp.Threading.Tasks;
using Game.Core;
using Game.Data;
using UnityEngine;

namespace Game.Service
{
    public class EffectService
    {
        private readonly EffectConfigSO _config;
        private readonly PoolManager _poolManager;

        public EffectService(EffectConfigSO config, PoolManager poolManager)
        {
            _config = config;
            _poolManager = poolManager;

            if (_config.HitEffectPrefab == null)
            {
                GameLogger.LogWarning("[EffectService] HitEffectPrefab is not assigned.");
            }

            if (_config.DeathEffectPrefab == null)
            {
                GameLogger.LogWarning("[EffectService] DeathEffectPrefab is not assigned.");
            }

            if (_config.BossSkillEffectPrefab == null)
            {
                GameLogger.LogWarning("[EffectService] BossSkillEffectPrefab is not assigned.");
            }

            if (_config.BossDeathEffectPrefab == null)
            {
                GameLogger.LogWarning("[EffectService] BossDeathEffectPrefab is not assigned.");
            }
        }

        public void PlayHitEffect(Vector3 position)
        {
            // Skip if prefab is null — already warned in constructor to avoid per-call spam
            if (_config.HitEffectPrefab == null)
            {
                return;
            }

            var instance = _poolManager.Get(_config.HitEffectPrefab, position, Quaternion.identity);
            ReleaseAfterDelayAsync(instance, Const.EffectLifetimeSeconds).Forget();
        }

        public void PlayDeathEffect(Vector3 position)
        {
            // Skip if prefab is null — already warned in constructor to avoid per-call spam
            if (_config.DeathEffectPrefab == null)
            {
                return;
            }

            var instance = _poolManager.Get(_config.DeathEffectPrefab, position, Quaternion.identity);
            ReleaseAfterDelayAsync(instance, Const.EffectLifetimeSeconds).Forget();
        }

        public void PlayBossSkillEffect(Vector3 position)
        {
            // Skip if prefab is null — already warned in constructor to avoid per-call spam
            if (_config.BossSkillEffectPrefab == null)
            {
                return;
            }

            var instance = _poolManager.Get(_config.BossSkillEffectPrefab, position, Quaternion.identity);
            ReleaseAfterDelayAsync(instance, Const.EffectLifetimeSeconds).Forget();
        }

        public void PlayBossDeathEffect(Vector3 position)
        {
            // Skip if prefab is null — already warned in constructor to avoid per-call spam
            if (_config.BossDeathEffectPrefab == null)
            {
                return;
            }

            var instance = _poolManager.Get(_config.BossDeathEffectPrefab, position, Quaternion.identity);
            ReleaseAfterDelayAsync(instance, Const.EffectLifetimeSeconds).Forget();
        }

        private async UniTaskVoid ReleaseAfterDelayAsync(GameObject instance, float delaySeconds)
        {
            await UniTask.Delay((int)(delaySeconds * 1000));

            if (instance != null)
            {
                _poolManager.Release(instance);
            }
        }
    }
}
