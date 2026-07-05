using Game.App;
using Game.Core;
using Game.Data;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace Game.Editor
{
    public static class VerticalSliceSceneBuilder
    {
        private const string UnitTablePath = "Assets/Game/03.Resources/Data/UnitTable.asset";
        private const string EnemyTablePath = "Assets/Game/03.Resources/Data/EnemyTable.asset";
        private const string BossTablePath = "Assets/Game/03.Resources/Data/BossTable.asset";
        private const string StageTablePath = "Assets/Game/03.Resources/Data/StageTable.asset";
        private const string WaveTablePath = "Assets/Game/03.Resources/Data/WaveTable.asset";
        private const string EffectConfigPath = "Assets/Game/03.Resources/Data/EffectConfig.asset";
        private const string MapTablePath = "Assets/Game/03.Resources/Data/MapTable.asset";
        private const string MissionTablePath = "Assets/Game/03.Resources/Data/MissionTable.asset";
        private const string UpgradeTablePath = "Assets/Game/03.Resources/Data/UpgradeTable.asset";
        private const string ScenePath = "Assets/Game/01.Scene/GameScene.unity";
        private const string GameHudPrefabPath = "Assets/Game/03.Resources/UI/GameHud.prefab";

        public static void BuildGameScene()
        {
            EnsureDataTables();

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateMainCamera();
            CreateBases();
            CreateLifetimeScope();
            CreateCanvas();
            CreateEventSystem();

            var isSaved = EditorSceneManager.SaveScene(scene, ScenePath);

            if (!isSaved)
            {
                GameLogger.LogError($"[VerticalSliceSceneBuilder] Failed to save scene: {ScenePath}");
            }
            else
            {
                GameLogger.Log($"[VerticalSliceSceneBuilder] Scene built and saved: {ScenePath}");
            }

            BuildSettingsRegistrar.RegisterScenes();
        }

        private static void EnsureDataTables()
        {
            if (!AssetDatabase.AssetPathExists(UnitTablePath))
            {
                GameLogger.LogWarning($"[VerticalSliceSceneBuilder] UnitTable not found. Importing sheets...");
                CsvSheetImporter.ImportAllSheets();
            }

            if (!AssetDatabase.AssetPathExists(BossTablePath))
            {
                GameLogger.LogWarning($"[VerticalSliceSceneBuilder] BossTable not found. Importing sheets...");
                CsvSheetImporter.ImportAllSheets();
            }

            if (!AssetDatabase.AssetPathExists(EffectConfigPath))
            {
                GameLogger.LogWarning($"[VerticalSliceSceneBuilder] EffectConfig not found. Creating...");
                EffectConfigBuilder.CreateEffectConfig();
            }
        }

        private static void CreateMainCamera()
        {
            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";

            var camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 8f;
            camera.backgroundColor = new Color(0.53f, 0.81f, 0.92f);
            camera.clearFlags = CameraClearFlags.SolidColor;
            cameraObject.transform.position = new Vector3(0f, 1f, -10f);
        }

        private static void CreateBases()
        {
            var stoneSprite = FindSpriteByKeyword("Stone");

            CreateBase("AllyBase", new Vector3(Const.AllyBaseX, Const.GroundY + 0.7f, 0), new Color(0.5f, 0.6f, 1f), stoneSprite);
            CreateBase("EnemyBase", new Vector3(Const.EnemyBaseX, Const.GroundY + 0.7f, 0), new Color(1f, 0.5f, 0.5f), stoneSprite);
        }

        private static void CreateBase(string name, Vector3 position, Color color, Sprite sprite)
        {
            var baseObject = new GameObject(name);
            baseObject.transform.position = position;

            var spriteRenderer = baseObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.color = color;
            baseObject.transform.localScale = new Vector3(1.5f, 2f, 1f);

            if (sprite == null)
            {
                GameLogger.LogWarning($"[VerticalSliceSceneBuilder] Stone sprite not found for {name}.");
            }
        }

        private static Sprite FindSpriteByKeyword(string keyword)
        {
            var spriteGuids = AssetDatabase.FindAssets("t:Sprite", new[] { "Assets/Layer Lab/2D Minimal-Environment" });

            foreach (var guid in spriteGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains(keyword))
                {
                    return AssetDatabase.LoadAssetAtPath<Sprite>(path);
                }
            }

            return null;
        }

        private static void CreateLifetimeScope()
        {
            var lifetimeScopeObject = new GameObject("GameSceneLifetimeScope");
            var lifetimeScope = lifetimeScopeObject.AddComponent<GameSceneLifetimeScope>();

            var unitTable = AssetDatabase.LoadAssetAtPath<UnitTableSO>(UnitTablePath);
            var enemyTable = AssetDatabase.LoadAssetAtPath<EnemyTableSO>(EnemyTablePath);
            var bossTable = AssetDatabase.LoadAssetAtPath<BossTableSO>(BossTablePath);
            var stageTable = AssetDatabase.LoadAssetAtPath<StageTableSO>(StageTablePath);
            var waveTable = AssetDatabase.LoadAssetAtPath<WaveTableSO>(WaveTablePath);
            var effectConfig = AssetDatabase.LoadAssetAtPath<EffectConfigSO>(EffectConfigPath);
            var mapTable = AssetDatabase.LoadAssetAtPath<MapTableSO>(MapTablePath);
            var missionTable = AssetDatabase.LoadAssetAtPath<MissionTableSO>(MissionTablePath);
            var upgradeTable = AssetDatabase.LoadAssetAtPath<UpgradeTableSO>(UpgradeTablePath);

            var serializedObject = new SerializedObject(lifetimeScope);
            serializedObject.FindProperty("unitTable").objectReferenceValue = unitTable;
            serializedObject.FindProperty("enemyTable").objectReferenceValue = enemyTable;
            serializedObject.FindProperty("bossTable").objectReferenceValue = bossTable;
            serializedObject.FindProperty("stageTable").objectReferenceValue = stageTable;
            serializedObject.FindProperty("waveTable").objectReferenceValue = waveTable;
            serializedObject.FindProperty("effectConfig").objectReferenceValue = effectConfig;
            serializedObject.FindProperty("mapTable").objectReferenceValue = mapTable;
            serializedObject.FindProperty("missionTable").objectReferenceValue = missionTable;
            serializedObject.FindProperty("upgradeTable").objectReferenceValue = upgradeTable;
            serializedObject.ApplyModifiedProperties();

            if (unitTable == null || enemyTable == null || bossTable == null || stageTable == null || waveTable == null || effectConfig == null || mapTable == null || missionTable == null || upgradeTable == null)
            {
                GameLogger.LogError("[VerticalSliceSceneBuilder] Failed to assign one or more SO tables to LifetimeScope.");
            }
        }

        private static void CreateCanvas()
        {
            var canvasObject = new GameObject("HUDCanvas");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var canvasScaler = canvasObject.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            canvasScaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();

            if (!AssetDatabase.AssetPathExists(GameHudPrefabPath))
            {
                GameLogger.LogWarning("[VerticalSliceSceneBuilder] GameHud prefab not found. Building UI prefabs...");
                UIPrefabBuilder.BuildAllUiPrefabs();
            }

            var gameHudPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(GameHudPrefabPath);

            if (gameHudPrefab == null)
            {
                GameLogger.LogError("[VerticalSliceSceneBuilder] Failed to load GameHud prefab.");
                return;
            }

            var gameHudInstance = PrefabUtility.InstantiatePrefab(gameHudPrefab) as GameObject;

            if (gameHudInstance != null)
            {
                gameHudInstance.transform.SetParent(canvasObject.transform, false);

                var hudRect = gameHudInstance.GetComponent<RectTransform>();
                hudRect.anchorMin = Vector2.zero;
                hudRect.anchorMax = Vector2.one;
                hudRect.offsetMin = Vector2.zero;
                hudRect.offsetMax = Vector2.zero;
            }
            else
            {
                GameLogger.LogError("[VerticalSliceSceneBuilder] Failed to instantiate GameHud prefab.");
            }
        }


        private static void CreateEventSystem()
        {
            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
        }
    }
}
