using System.IO;
using Game.Core;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    /// <summary>
    /// UI 프리팹을 1080x1920 카메라로 렌더해 PNG로 저장한다.
    /// 헤드리스 검증의 시각 공백을 메우는 도구 — 결과 이미지를 사람/AI가 직접 확인.
    /// </summary>
    public static class UiScreenshotTool
    {
        private const int Width = 1080;
        private const int Height = 1920;
        private const string OutputDir = "UiScreenshots";

        private static readonly string[] PrefabPaths =
        {
            "Assets/Game/03.Resources/UI/GameHud.prefab",
            "Assets/Game/03.Resources/UI/StageSelectScreen.prefab"
        };

        public static void CaptureAllUiPrefabs()
        {
            Directory.CreateDirectory(OutputDir);

            foreach (var path in PrefabPaths)
            {
                CapturePrefab(path, Path.GetFileNameWithoutExtension(path));
            }
        }

        /// <summary>킷 원본 템플릿을 기준 이미지로 캡처 — 재현 목표 확인용.</summary>
        public static void CaptureReferenceTemplates()
        {
            Directory.CreateDirectory(OutputDir);

            var referencePaths = new[]
            {
                "Assets/Layer Lab/GUI Pro-MinimalGame/Theme_Light/Prefabs/Prefabs~DemoScenes/Lobby_Default.prefab",
                "Assets/Layer Lab/GUI Pro-MinimalGame/Theme_Light/Prefabs/Prefabs~DemoScenes/Lobby_02.prefab"
            };

            foreach (var path in referencePaths)
            {
                CapturePrefab(path, "REF_" + Path.GetFileNameWithoutExtension(path));
            }
        }

        private static void CapturePrefab(string prefabPath, string baseName)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab == null)
            {
                GameLogger.LogWarning($"[UiScreenshotTool] Prefab not found: {prefabPath}");
                return;
            }

            var cameraObject = new GameObject("UiCaptureCamera");
            var camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.16f, 0.35f, 0.25f);
            camera.cullingMask = 1 << 5;

            var canvasObject = new GameObject("UiCaptureCanvas");
            canvasObject.layer = 5;
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            var scaler = canvasObject.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(Width, Height);
            scaler.matchWidthOrHeight = 0.5f;

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.transform.SetParent(canvasObject.transform, false);
            SetLayerRecursive(instance, 5);

            var instanceRect = instance.GetComponent<RectTransform>();
            if (instanceRect != null)
            {
                instanceRect.anchorMin = Vector2.zero;
                instanceRect.anchorMax = Vector2.one;
                instanceRect.offsetMin = Vector2.zero;
                instanceRect.offsetMax = Vector2.zero;
            }

            // 기본 상태 캡처 + 각 패널 루트를 하나씩 켜서 캡처
            Capture(camera, $"{OutputDir}/{baseName}_home.png");

            var panelNames = new[] { "StagePanel", "DeckPanel", "UpgradePanel", "MissionPanel", "ShopPanel", "DailyRewardPopupRoot", "SettingsPopupRoot" };
            foreach (var panelName in panelNames)
            {
                var panel = FindChildRecursive(instance.transform, panelName);

                if (panel == null)
                {
                    continue;
                }

                panel.gameObject.SetActive(true);
                Capture(camera, $"{OutputDir}/{baseName}_{panelName}.png");
                panel.gameObject.SetActive(false);
            }

            Object.DestroyImmediate(instance);
            Object.DestroyImmediate(canvasObject);
            Object.DestroyImmediate(cameraObject);

            GameLogger.Log($"[UiScreenshotTool] Captured {baseName} to {OutputDir}/");
        }

        private static void Capture(Camera camera, string filePath)
        {
            var renderTexture = new RenderTexture(Width, Height, 24);
            camera.targetTexture = renderTexture;
            camera.Render();

            RenderTexture.active = renderTexture;
            var texture = new Texture2D(Width, Height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, Width, Height), 0, 0);
            texture.Apply();
            RenderTexture.active = null;
            camera.targetTexture = null;

            File.WriteAllBytes(filePath, texture.EncodeToPNG());

            Object.DestroyImmediate(texture);
            Object.DestroyImmediate(renderTexture);
        }

        private static Transform FindChildRecursive(Transform root, string name)
        {
            foreach (var child in root.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == name)
                {
                    return child;
                }
            }
            return null;
        }

        private static void SetLayerRecursive(GameObject target, int layer)
        {
            target.layer = layer;
            foreach (Transform child in target.transform)
            {
                SetLayerRecursive(child.gameObject, layer);
            }
        }
    }
}
