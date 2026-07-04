using Game.Core;
using Game.Data;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public static class EffectConfigBuilder
    {
        private const string EffectConfigPath = "Assets/Game/03.Resources/Data/EffectConfig.asset";
        private const string HitEffectPrefabPath = "Assets/Essential 2D Particle FX/URP Compatible/Dust URP/Red Dust URP.prefab";
        private const string DeathEffectPrefabPath = "Assets/Essential 2D Particle FX/URP Compatible/Explosion URP 1/Yellow Explosion URP 1.prefab";
        private const string BossSkillEffectPrefabPath = "Assets/Essential 2D Particle FX/URP Compatible/Explosion URP 2/Purple Explosion URP 2.prefab";
        private const string BossDeathEffectPrefabPath = "Assets/Essential 2D Particle FX/URP Compatible/Death Particle URP/Purple Death Particle URP.prefab";

        public static void CreateEffectConfig()
        {
            var config = AssetDatabase.LoadAssetAtPath<EffectConfigSO>(EffectConfigPath);
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<EffectConfigSO>();
                AssetDatabase.CreateAsset(config, EffectConfigPath);
            }

            var hitPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(HitEffectPrefabPath);
            var deathPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(DeathEffectPrefabPath);
            var bossSkillPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(BossSkillEffectPrefabPath);
            var bossDeathPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(BossDeathEffectPrefabPath);

            if (hitPrefab == null)
            {
                GameLogger.LogError($"[EffectConfigBuilder] Failed to load hit effect prefab: {HitEffectPrefabPath}");
            }

            if (deathPrefab == null)
            {
                GameLogger.LogError($"[EffectConfigBuilder] Failed to load death effect prefab: {DeathEffectPrefabPath}");
            }

            if (bossSkillPrefab == null)
            {
                GameLogger.LogError($"[EffectConfigBuilder] Failed to load boss skill effect prefab: {BossSkillEffectPrefabPath}");
            }

            if (bossDeathPrefab == null)
            {
                GameLogger.LogError($"[EffectConfigBuilder] Failed to load boss death effect prefab: {BossDeathEffectPrefabPath}");
            }

            var serializedObject = new SerializedObject(config);
            serializedObject.FindProperty("hitEffectPrefab").objectReferenceValue = hitPrefab;
            serializedObject.FindProperty("deathEffectPrefab").objectReferenceValue = deathPrefab;
            serializedObject.FindProperty("bossSkillEffectPrefab").objectReferenceValue = bossSkillPrefab;
            serializedObject.FindProperty("bossDeathEffectPrefab").objectReferenceValue = bossDeathPrefab;
            serializedObject.ApplyModifiedProperties();

            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            GameLogger.Log("[EffectConfigBuilder] EffectConfig created/updated successfully.");
        }
    }
}
