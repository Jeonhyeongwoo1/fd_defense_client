using Game.App;
using Game.Core;
using Game.Data;
using Game.View;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Game.Editor
{
    public static class OutGameSceneBuilder
    {
        private const string StageTablePath = "Assets/Game/03.Resources/Data/StageTable.asset";
        private const string ScenePath = "Assets/Game/01.Scene/OutGameScene.unity";

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

            var tmpFont = FindTMPFont();

            CreateTitleText(canvasObject.transform, tmpFont);
            var stageButtons = CreateStageButtons(canvasObject.transform, tmpFont);

            var stageSelectView = canvasObject.AddComponent<UI_StageSelectView>();
            var serializedObject = new SerializedObject(stageSelectView);
            serializedObject.FindProperty("stageButtons").arraySize = stageButtons.Length;
            for (var i = 0; i < stageButtons.Length; i++)
            {
                serializedObject.FindProperty("stageButtons").GetArrayElementAtIndex(i).objectReferenceValue = stageButtons[i];
            }
            serializedObject.ApplyModifiedProperties();
        }

        private static TMP_FontAsset FindTMPFont()
        {
            var fontGuids = AssetDatabase.FindAssets("t:TMP_FontAsset", new[] { "Assets/Layer Lab/GUI Pro-MinimalGame" });

            if (fontGuids.Length > 0)
            {
                var path = AssetDatabase.GUIDToAssetPath(fontGuids[0]);
                return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
            }

            GameLogger.LogWarning("[OutGameSceneBuilder] TMP font not found. Using default font.");
            return null;
        }

        private static void CreateTitleText(Transform parent, TMP_FontAsset font)
        {
            var titleTextObject = new GameObject("TitleText");
            titleTextObject.transform.SetParent(parent, false);

            var rectTransform = titleTextObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 1);
            rectTransform.anchorMax = new Vector2(0.5f, 1);
            rectTransform.pivot = new Vector2(0.5f, 1);
            rectTransform.anchoredPosition = new Vector2(0, -150);
            rectTransform.sizeDelta = new Vector2(800, 150);

            var tmpText = titleTextObject.AddComponent<TextMeshProUGUI>();
            tmpText.text = "FD DEFENSE";
            tmpText.fontSize = 96;
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.font = font;
        }

        private static UI_StageButtonView[] CreateStageButtons(Transform parent, TMP_FontAsset font)
        {
            var buttons = new UI_StageButtonView[3];
            var stageIds = new[] { "stage_001", "stage_002", "stage_003" };
            var stageNames = new[] { "STAGE 1", "STAGE 2", "STAGE 3" };
            var yPositions = new[] { 100f, -80f, -260f };

            for (var i = 0; i < 3; i++)
            {
                buttons[i] = CreateStageButton(parent, new Vector2(0, yPositions[i]), stageIds[i], stageNames[i], font);
            }

            return buttons;
        }

        private static UI_StageButtonView CreateStageButton(Transform parent, Vector2 position, string stageId, string stageName, TMP_FontAsset font)
        {
            var buttonObject = new GameObject($"StageButton_{stageId}");
            buttonObject.transform.SetParent(parent, false);

            var rectTransform = buttonObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = position;
            rectTransform.sizeDelta = new Vector2(500, 140);

            var image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.3f, 0.4f, 0.6f, 1f);

            var button = buttonObject.AddComponent<Button>();

            var stageNameTextObject = new GameObject("StageNameText");
            stageNameTextObject.transform.SetParent(buttonObject.transform, false);

            var nameRectTransform = stageNameTextObject.AddComponent<RectTransform>();
            nameRectTransform.anchorMin = new Vector2(0, 0);
            nameRectTransform.anchorMax = new Vector2(0.7f, 1);
            nameRectTransform.offsetMin = new Vector2(30, 0);
            nameRectTransform.offsetMax = new Vector2(0, 0);

            var nameText = stageNameTextObject.AddComponent<TextMeshProUGUI>();
            nameText.text = stageName;
            nameText.fontSize = 48;
            nameText.alignment = TextAlignmentOptions.Left;
            nameText.font = font;

            var clearedMarkObject = new GameObject("ClearedMark");
            clearedMarkObject.transform.SetParent(buttonObject.transform, false);

            var clearedMarkRectTransform = clearedMarkObject.AddComponent<RectTransform>();
            clearedMarkRectTransform.anchorMin = new Vector2(1, 0.5f);
            clearedMarkRectTransform.anchorMax = new Vector2(1, 0.5f);
            clearedMarkRectTransform.pivot = new Vector2(1, 0.5f);
            clearedMarkRectTransform.anchoredPosition = new Vector2(-30, 0);
            clearedMarkRectTransform.sizeDelta = new Vector2(80, 80);

            var clearedMarkText = clearedMarkObject.AddComponent<TextMeshProUGUI>();
            clearedMarkText.text = "★";
            clearedMarkText.fontSize = 60;
            clearedMarkText.alignment = TextAlignmentOptions.Center;
            clearedMarkText.color = Color.yellow;
            clearedMarkText.font = font;

            clearedMarkObject.SetActive(false);

            var lockedOverlayObject = new GameObject("LockedOverlay");
            lockedOverlayObject.transform.SetParent(buttonObject.transform, false);

            var lockedOverlayRectTransform = lockedOverlayObject.AddComponent<RectTransform>();
            lockedOverlayRectTransform.anchorMin = Vector2.zero;
            lockedOverlayRectTransform.anchorMax = Vector2.one;
            lockedOverlayRectTransform.offsetMin = Vector2.zero;
            lockedOverlayRectTransform.offsetMax = Vector2.zero;

            var lockedOverlayImage = lockedOverlayObject.AddComponent<Image>();
            lockedOverlayImage.color = new Color(0, 0, 0, 0.6f);
            lockedOverlayImage.raycastTarget = false;

            var lockedTextObject = new GameObject("LockedText");
            lockedTextObject.transform.SetParent(lockedOverlayObject.transform, false);

            var lockedTextRectTransform = lockedTextObject.AddComponent<RectTransform>();
            lockedTextRectTransform.anchorMin = Vector2.zero;
            lockedTextRectTransform.anchorMax = Vector2.one;
            lockedTextRectTransform.offsetMin = Vector2.zero;
            lockedTextRectTransform.offsetMax = Vector2.zero;

            var lockedText = lockedTextObject.AddComponent<TextMeshProUGUI>();
            lockedText.text = "LOCKED";
            lockedText.fontSize = 56;
            lockedText.alignment = TextAlignmentOptions.Center;
            lockedText.color = new Color(0.8f, 0.8f, 0.8f, 1f);
            lockedText.font = font;

            lockedOverlayObject.SetActive(true);

            var buttonView = buttonObject.AddComponent<UI_StageButtonView>();
            var serializedObject = new SerializedObject(buttonView);
            serializedObject.FindProperty("button").objectReferenceValue = button;
            serializedObject.FindProperty("stageNameText").objectReferenceValue = nameText;
            serializedObject.FindProperty("clearedMark").objectReferenceValue = clearedMarkObject;
            serializedObject.FindProperty("lockedOverlay").objectReferenceValue = lockedOverlayObject;
            serializedObject.FindProperty("stageId").stringValue = stageId;
            serializedObject.ApplyModifiedProperties();

            return buttonView;
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
