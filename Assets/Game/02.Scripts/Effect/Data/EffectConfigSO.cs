using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "EffectConfig", menuName = "Game/EffectConfig")]
    public class EffectConfigSO : ScriptableObject
    {
        [SerializeField] private GameObject hitEffectPrefab;
        [SerializeField] private GameObject deathEffectPrefab;
        [SerializeField] private GameObject bossSkillEffectPrefab;
        [SerializeField] private GameObject bossDeathEffectPrefab;

        public GameObject HitEffectPrefab => hitEffectPrefab;
        public GameObject DeathEffectPrefab => deathEffectPrefab;
        public GameObject BossSkillEffectPrefab => bossSkillEffectPrefab;
        public GameObject BossDeathEffectPrefab => bossDeathEffectPrefab;
    }
}
