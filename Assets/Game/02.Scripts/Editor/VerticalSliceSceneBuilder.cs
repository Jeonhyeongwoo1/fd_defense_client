using System.Linq;
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
    public static class VerticalSliceSceneBuilder
    {
        private const string UnitTablePath = "Assets/Game/03.Resources/Data/UnitTable.asset";
        private const string EnemyTablePath = "Assets/Game/03.Resources/Data/EnemyTable.asset";
        private const string BossTablePath = "Assets/Game/03.Resources/Data/BossTable.asset";
        private const string StageTablePath = "Assets/Game/03.Resources/Data/StageTable.asset";
        private const string WaveTablePath = "Assets/Game/03.Resources/Data/WaveTable.asset";
        private const string EffectConfigPath = "Assets/Game/03.Resources/Data/EffectConfig.asset";
        private const string ScenePath = "Assets/Game/01.Scene/GameScene.unity";

        public static void BuildGameScene()
        {
            EnsureDataTables();

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateMainCamera();
            CreateEnvironment();
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
            camera.orthographicSize = 5f;
            camera.backgroundColor = new Color(0.53f, 0.81f, 0.92f);
            camera.clearFlags = CameraClearFlags.SolidColor;
            cameraObject.transform.position = new Vector3(0f, 1f, -10f);
        }

        private static void CreateEnvironment()
        {
            var fieldPrefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Layer Lab/2D Minimal-Environment/Environment 1/Prefabs" });
            GameObject fieldPrefab = null;

            foreach (var guid in fieldPrefabGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains("Field"))
                {
                    fieldPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    break;
                }
            }

            if (fieldPrefab == null)
            {
                GameLogger.LogWarning("[VerticalSliceSceneBuilder] Field prefab not found. Skipping environment decoration.");
                return;
            }

            var environmentParent = new GameObject("Environment");

            for (var x = -8f; x <= 8f; x += 2f)
            {
                var fieldInstance = PrefabUtility.InstantiatePrefab(fieldPrefab) as GameObject;
                if (fieldInstance != null)
                {
                    fieldInstance.transform.position = new Vector3(x, Const.GroundY - 0.8f, 0);
                    fieldInstance.transform.SetParent(environmentParent.transform);
                }
            }
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

            var serializedObject = new SerializedObject(lifetimeScope);
            serializedObject.FindProperty("unitTable").objectReferenceValue = unitTable;
            serializedObject.FindProperty("enemyTable").objectReferenceValue = enemyTable;
            serializedObject.FindProperty("bossTable").objectReferenceValue = bossTable;
            serializedObject.FindProperty("stageTable").objectReferenceValue = stageTable;
            serializedObject.FindProperty("waveTable").objectReferenceValue = waveTable;
            serializedObject.FindProperty("effectConfig").objectReferenceValue = effectConfig;
            serializedObject.ApplyModifiedProperties();

            if (unitTable == null || enemyTable == null || bossTable == null || stageTable == null || waveTable == null || effectConfig == null)
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

            var tmpFont = FindTMPFont();

            var moneyText = CreateMoneyText(canvasObject.transform, tmpFont);
            var waveText = CreateWaveText(canvasObject.transform, tmpFont);
            var allyBaseHpFillImage = CreateBaseHpBar(canvasObject.transform, "AllyBaseHpBar", new Vector2(60, -140), new Vector2(0, 1), new Color(0.3f, 0.6f, 1f));
            var enemyBaseHpFillImage = CreateBaseHpBar(canvasObject.transform, "EnemyBaseHpBar", new Vector2(-60, -140), new Vector2(1, 1), new Color(1f, 0.3f, 0.3f));
            var spawnButtons = CreateSpawnButtons(canvasObject.transform, tmpFont);

            var hudView = canvasObject.AddComponent<UI_GameHudView>();
            var serializedObject = new SerializedObject(hudView);
            serializedObject.FindProperty("moneyText").objectReferenceValue = moneyText;
            serializedObject.FindProperty("waveText").objectReferenceValue = waveText;
            serializedObject.FindProperty("allyBaseHpFillImage").objectReferenceValue = allyBaseHpFillImage;
            serializedObject.FindProperty("enemyBaseHpFillImage").objectReferenceValue = enemyBaseHpFillImage;
            serializedObject.FindProperty("spawnButtons").arraySize = spawnButtons.Length;
            for (var i = 0; i < spawnButtons.Length; i++)
            {
                serializedObject.FindProperty("spawnButtons").GetArrayElementAtIndex(i).objectReferenceValue = spawnButtons[i];
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

            GameLogger.LogWarning("[VerticalSliceSceneBuilder] TMP font not found. Using default font.");
            return null;
        }

        private static TMP_Text CreateMoneyText(Transform parent, TMP_FontAsset font)
        {
            var moneyTextObject = new GameObject("MoneyText");
            moneyTextObject.transform.SetParent(parent, false);

            var rectTransform = moneyTextObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1);
            rectTransform.anchoredPosition = new Vector2(60, -60);
            rectTransform.sizeDelta = new Vector2(300, 100);

            var tmpText = moneyTextObject.AddComponent<TextMeshProUGUI>();
            tmpText.text = "0";
            tmpText.fontSize = 56;
            tmpText.font = font;

            return tmpText;
        }

        private static TMP_Text CreateWaveText(Transform parent, TMP_FontAsset font)
        {
            var waveTextObject = new GameObject("WaveText");
            waveTextObject.transform.SetParent(parent, false);

            var rectTransform = waveTextObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 1);
            rectTransform.anchorMax = new Vector2(0.5f, 1);
            rectTransform.pivot = new Vector2(0.5f, 1);
            rectTransform.anchoredPosition = new Vector2(0, -60);
            rectTransform.sizeDelta = new Vector2(400, 100);

            var tmpText = waveTextObject.AddComponent<TextMeshProUGUI>();
            tmpText.text = "Wave 1/5";
            tmpText.fontSize = 48;
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.font = font;

            return tmpText;
        }

        private static Image CreateBaseHpBar(Transform parent, string name, Vector2 position, Vector2 anchor, Color fillColor)
        {
            var barObject = new GameObject(name);
            barObject.transform.SetParent(parent, false);

            var barRectTransform = barObject.AddComponent<RectTransform>();
            barRectTransform.anchorMin = anchor;
            barRectTransform.anchorMax = anchor;
            barRectTransform.pivot = anchor;
            barRectTransform.anchoredPosition = position;
            barRectTransform.sizeDelta = new Vector2(400, 36);

            var barImage = barObject.AddComponent<Image>();
            barImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

            var fillObject = new GameObject("Fill");
            fillObject.transform.SetParent(barObject.transform, false);

            var fillRectTransform = fillObject.AddComponent<RectTransform>();
            fillRectTransform.anchorMin = Vector2.zero;
            fillRectTransform.anchorMax = Vector2.one;
            fillRectTransform.offsetMin = Vector2.zero;
            fillRectTransform.offsetMax = Vector2.zero;

            var fillImage = fillObject.AddComponent<Image>();
            fillImage.color = fillColor;
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillAmount = 1f;

            return fillImage;
        }

        private static UI_UnitSpawnButtonView[] CreateSpawnButtons(Transform parent, TMP_FontAsset font)
        {
            var buttons = new UI_UnitSpawnButtonView[5];
            var unitIds = new[] { "pet_chick", "pet_pug", "pet_bat", "pet_ghost", "pet_titan" };
            var xPositions = new[] { -420f, -210f, 0f, 210f, 420f };

            for (var i = 0; i < 5; i++)
            {
                buttons[i] = CreateSpawnButton(parent, new Vector2(xPositions[i], 110), unitIds[i], font);
            }

            return buttons;
        }

        private static UI_UnitSpawnButtonView CreateSpawnButton(Transform parent, Vector2 position, string unitId, TMP_FontAsset font)
        {
            var buttonObject = new GameObject($"SpawnButton_{unitId}");
            buttonObject.transform.SetParent(parent, false);

            var rectTransform = buttonObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0);
            rectTransform.anchorMax = new Vector2(0.5f, 0);
            rectTransform.pivot = new Vector2(0.5f, 0);
            rectTransform.anchoredPosition = position;
            rectTransform.sizeDelta = new Vector2(200, 130);

            var image = buttonObject.AddComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0.7f);

            var button = buttonObject.AddComponent<Button>();

            var nameTextObject = new GameObject("NameText");
            nameTextObject.transform.SetParent(buttonObject.transform, false);

            var nameRectTransform = nameTextObject.AddComponent<RectTransform>();
            nameRectTransform.anchorMin = new Vector2(0, 0.7f);
            nameRectTransform.anchorMax = new Vector2(1, 1);
            nameRectTransform.offsetMin = new Vector2(10, 0);
            nameRectTransform.offsetMax = new Vector2(-10, -10);

            var nameText = nameTextObject.AddComponent<TextMeshProUGUI>();
            nameText.text = "Unit";
            nameText.fontSize = 28;
            nameText.alignment = TextAlignmentOptions.Center;
            nameText.font = font;

            var costTextObject = new GameObject("CostText");
            costTextObject.transform.SetParent(buttonObject.transform, false);

            var costRectTransform = costTextObject.AddComponent<RectTransform>();
            costRectTransform.anchorMin = new Vector2(0, 0);
            costRectTransform.anchorMax = new Vector2(1, 0.3f);
            costRectTransform.offsetMin = new Vector2(10, 10);
            costRectTransform.offsetMax = new Vector2(-10, 0);

            var costText = costTextObject.AddComponent<TextMeshProUGUI>();
            costText.text = "0";
            costText.fontSize = 32;
            costText.alignment = TextAlignmentOptions.Center;
            costText.font = font;

            var cooldownFillObject = new GameObject("CooldownFill");
            cooldownFillObject.transform.SetParent(buttonObject.transform, false);

            var cooldownRectTransform = cooldownFillObject.AddComponent<RectTransform>();
            cooldownRectTransform.anchorMin = Vector2.zero;
            cooldownRectTransform.anchorMax = Vector2.one;
            cooldownRectTransform.offsetMin = Vector2.zero;
            cooldownRectTransform.offsetMax = Vector2.zero;

            var cooldownImage = cooldownFillObject.AddComponent<Image>();
            cooldownImage.color = new Color(0, 0, 0, 0.5f);
            cooldownImage.type = Image.Type.Filled;
            cooldownImage.fillMethod = Image.FillMethod.Vertical;
            cooldownImage.fillAmount = 0f;
            cooldownImage.raycastTarget = false;

            var buttonView = buttonObject.AddComponent<UI_UnitSpawnButtonView>();
            var serializedObject = new SerializedObject(buttonView);
            serializedObject.FindProperty("button").objectReferenceValue = button;
            serializedObject.FindProperty("cooldownFillImage").objectReferenceValue = cooldownImage;
            serializedObject.FindProperty("costText").objectReferenceValue = costText;
            serializedObject.FindProperty("nameText").objectReferenceValue = nameText;
            serializedObject.FindProperty("unitId").stringValue = unitId;
            serializedObject.ApplyModifiedProperties();

            return buttonView;
        }

        private static void CreateEventSystem()
        {
            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
        }
    }
}
