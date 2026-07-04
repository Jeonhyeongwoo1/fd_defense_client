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
    public static class OutGameSceneBuilder
    {
        private const string StageTablePath = "Assets/Game/03.Resources/Data/StageTable.asset";
        private const string ScenePath = "Assets/Game/01.Scene/OutGameScene.unity";
        private const string StageSelectScreenPrefabPath = "Assets/Game/03.Resources/UI/StageSelectScreen.prefab";

        public static void BuildOutGameScene()
        {
            EnsureDataTables();

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateMainCamera();
            CreateCanvas();
            CreateEventSystem();
            CreateLifetimeScope();

            var isSaved = EditorSceneManager.SaveScene(scene, ScenePath);

            if (!isSaved)
            {
                GameLogger.LogError($"[OutGameSceneBuilder] Failed to save scene: {ScenePath}");
            }
            else
            {
                GameLogger.Log($"[OutGameSceneBuilder] Scene built and saved: {ScenePath}");
            }

            BuildSettingsRegistrar.RegisterScenes();
        }

        private static void EnsureDataTables()
        {
            if (!AssetDatabase.AssetPathExists(StageTablePath))
            {
                GameLogger.LogWarning($"[OutGameSceneBuilder] StageTable not found. Importing sheets...");
                CsvSheetImporter.ImportAllSheets();
            }
        }

        private static void CreateMainCamera()
        {
            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";

            var camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 5f;
            camera.backgroundColor = new Color(0.15f, 0.2f, 0.35f);
            camera.clearFlags = CameraClearFlags.SolidColor;
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);
        }

        private static void CreateCanvas()
        {
            var canvasObject = new GameObject("Canvas");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var canvasScaler = canvasObject.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            canvasScaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();

            if (!AssetDatabase.AssetPathExists(StageSelectScreenPrefabPath))
            {
                GameLogger.LogWarning("[OutGameSceneBuilder] StageSelectScreen prefab not found. Building UI prefabs...");
                UIPrefabBuilder.BuildAllUiPrefabs();
            }

            var stageSelectScreenPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(StageSelectScreenPrefabPath);

            if (stageSelectScreenPrefab == null)
            {
                GameLogger.LogError("[OutGameSceneBuilder] Failed to load StageSelectScreen prefab.");
                return;
            }

            var stageSelectScreenInstance = PrefabUtility.InstantiatePrefab(stageSelectScreenPrefab) as GameObject;

            if (stageSelectScreenInstance != null)
            {
                stageSelectScreenInstance.transform.SetParent(canvasObject.transform, false);

                var screenRect = stageSelectScreenInstance.GetComponent<RectTransform>();
                screenRect.anchorMin = Vector2.zero;
                screenRect.anchorMax = Vector2.one;
                screenRect.offsetMin = Vector2.zero;
                screenRect.offsetMax = Vector2.zero;
            }
            else
            {
                GameLogger.LogError("[OutGameSceneBuilder] Failed to instantiate StageSelectScreen prefab.");
            }
        }


        private static void CreateEventSystem()
        {
            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
        }

        private static void CreateLifetimeScope()
        {
            var lifetimeScopeObject = new GameObject("OutGameLifetimeScope");
            var lifetimeScope = lifetimeScopeObject.AddComponent<OutGameLifetimeScope>();

            var stageTable = AssetDatabase.LoadAssetAtPath<StageTableSO>(StageTablePath);

            var serializedObject = new SerializedObject(lifetimeScope);
            serializedObject.FindProperty("stageTable").objectReferenceValue = stageTable;
            serializedObject.ApplyModifiedProperties();

            if (stageTable == null)
            {
                GameLogger.LogError("[OutGameSceneBuilder] Failed to assign StageTable to LifetimeScope.");
            }
        }
    }
}
