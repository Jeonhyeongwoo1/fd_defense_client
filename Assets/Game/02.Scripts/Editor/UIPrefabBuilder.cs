using Game.Core;
using Game.View;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Editor
{
    public static class UIPrefabBuilder
    {
        private const string OutputRoot = "Assets/Game/03.Resources/UI/";
        private const string KitRoot = "Assets/Layer Lab/GUI Pro-MinimalGame/";
        private const string PopupPath = KitRoot + "Theme_Light/Prefabs/Prefabs_Popup/Popup_Box_01_Basic.prefab";
        private const string ButtonGreenPath = KitRoot + "Theme_Light/Prefabs/Prefabs_Button/Button_01_Green.prefab";
        private const string ButtonBluePath = KitRoot + "Theme_Light/Prefabs/Prefabs_Button/Button_01_Blue.prefab";
        private const string ButtonCirclePath = KitRoot + "Theme_Light/Prefabs/Prefabs_Button/Button_Circle_01.prefab";
        private const string SliderBluePath = KitRoot + "Theme_Light/Prefabs/Prefabs_Slider/Slider_01_Blue.prefab";
        private const string SliderRedPath = KitRoot + "Theme_Light/Prefabs/Prefabs_Slider/Slider_01_Red.prefab";
        private const string ItemFramePath = KitRoot + "Theme_Light/Prefabs/Prefabs_Frame/ItemFrame/ItemFrame_01.prefab";
        private const string FontPath = KitRoot + "Shared/Font/LTAvocado-Bold SDF.asset";
        private const string IconCoinPath = KitRoot + "Shared/Icons/PictoIcon/128/coin_2.png";
        private const string IconLockPath = KitRoot + "Shared/Icons/PictoIcon/128/lock.png";
        private const string IconStarPath = KitRoot + "Shared/Icons/PictoIcon/128/star_1.png";
        private const string PetIconRoot = "Assets/Layer Lab/2D Characters-PetPack1/Sprites/ImageSequence/";

        private static readonly string[] PetNames = { "Chick", "Pug", "Bat", "Ghost", "Titan" };
        private static readonly string[] UnitIds = { "pet_chick", "pet_pug", "pet_bat", "pet_ghost", "pet_titan" };
        private static readonly string[] StageIds = { "stage_001", "stage_002", "stage_003" };

        public static void BuildAllUiPrefabs()
        {
            EnsureOutputFolder();

            var font = LoadFont();
            var coinIcon = LoadSprite(IconCoinPath);
            var lockIcon = LoadSprite(IconLockPath);
            var starIcon = LoadSprite(IconStarPath);

            BuildGameHudPrefab(font, coinIcon);
            BuildStageSelectScreenPrefab(font, starIcon, lockIcon);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            GameLogger.Log("[UIPrefabBuilder] All UI prefabs built successfully.");
        }

        private static void EnsureOutputFolder()
        {
            if (!AssetDatabase.IsValidFolder(OutputRoot))
            {
                var parentFolder = "Assets/Game/03.Resources";
                if (!AssetDatabase.IsValidFolder(parentFolder))
                {
                    AssetDatabase.CreateFolder("Assets/Game", "03.Resources");
                }

                AssetDatabase.CreateFolder(parentFolder, "UI");
            }
        }

        private static TMP_FontAsset LoadFont()
        {
            var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);

            if (font == null)
            {
                var fontGuids = AssetDatabase.FindAssets("t:TMP_FontAsset", new[] { KitRoot });

                if (fontGuids.Length > 0)
                {
                    var path = AssetDatabase.GUIDToAssetPath(fontGuids[0]);
                    font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
                    GameLogger.LogWarning($"[UIPrefabBuilder] Font not found at {FontPath}, using fallback: {path}");
                }
            }

            if (font == null)
            {
                GameLogger.LogError("[UIPrefabBuilder] TMP font not found.");
            }

            return font;
        }

        private static Sprite LoadSprite(string path)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

            if (sprite == null)
            {
                GameLogger.LogWarning($"[UIPrefabBuilder] Sprite not found: {path}");
            }

            return sprite;
        }

        private static GameObject InstantiateKitPrefab(string path, Transform parent)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab == null)
            {
                GameLogger.LogError($"[UIPrefabBuilder] Kit prefab not found: {path}");
                return null;
            }

            var instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

            if (instance != null)
            {
                instance.transform.SetParent(parent, false);
            }

            return instance;
        }

        private static GameObject FindTitlePrefab(string keyword)
        {
            var titleGuids = AssetDatabase.FindAssets("t:Prefab", new[] { KitRoot + "Theme_Light/Prefabs/Prefabs_Title" });

            foreach (var guid in titleGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);

                if (path.Contains(keyword))
                {
                    return AssetDatabase.LoadAssetAtPath<GameObject>(path);
                }
            }

            GameLogger.LogWarning($"[UIPrefabBuilder] Title prefab not found with keyword: {keyword}");
            return null;
        }

        private static void BuildGameHudPrefab(TMP_FontAsset font, Sprite coinIcon)
        {
            var rootObject = new GameObject("GameHud");
            var rootRect = rootObject.AddComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            var moneyText = CreateMoneyFrame(rootRect, font, coinIcon);
            var waveText = CreateWaveTitle(rootRect, font);
            var allyBaseHpFillImage = CreateHpBar(rootRect, "AllyHpBar", new Vector2(60, -200), new Vector2(0, 1), SliderBluePath);
            var enemyBaseHpFillImage = CreateHpBar(rootRect, "EnemyHpBar", new Vector2(-60, -200), new Vector2(1, 1), SliderRedPath);
            var spawnButtons = CreateUnitButtons(rootRect, font);

            var hudView = rootObject.AddComponent<UI_GameHudView>();
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

            CreateResultPopupInHud(rootRect, font, hudView);

            var prefabPath = OutputRoot + "GameHud.prefab";
            DeleteExistingPrefab(prefabPath);
            PrefabUtility.SaveAsPrefabAsset(rootObject, prefabPath);
            Object.DestroyImmediate(rootObject);

            GameLogger.Log($"[UIPrefabBuilder] GameHud.prefab created at {prefabPath}");
        }

        private static TMP_Text CreateMoneyFrame(RectTransform parent, TMP_FontAsset font, Sprite coinIcon)
        {
            var frameInstance = InstantiateKitPrefab(ItemFramePath, parent);

            if (frameInstance == null)
            {
                GameLogger.LogError("[UIPrefabBuilder] Failed to create MoneyFrame.");
                return null;
            }

            frameInstance.name = "MoneyFrame";
            var frameRect = frameInstance.GetComponent<RectTransform>();
            frameRect.anchorMin = new Vector2(0, 1);
            frameRect.anchorMax = new Vector2(0, 1);
            frameRect.pivot = new Vector2(0, 1);
            frameRect.anchoredPosition = new Vector2(40, -40);
            frameRect.localScale = Vector3.one * 1.2f;

            var iconImage = frameInstance.transform.Find("Icon")?.GetComponent<Image>();

            if (iconImage == null)
            {
                GameLogger.LogWarning("[UIPrefabBuilder] Icon child not found in ItemFrame.");
            }
            else if (coinIcon != null)
            {
                iconImage.sprite = coinIcon;
            }

            var moneyTextObject = new GameObject("MoneyText");
            moneyTextObject.transform.SetParent(frameInstance.transform, false);

            var moneyTextRect = moneyTextObject.AddComponent<RectTransform>();
            moneyTextRect.anchorMin = new Vector2(1, 0.5f);
            moneyTextRect.anchorMax = new Vector2(1, 0.5f);
            moneyTextRect.pivot = new Vector2(0, 0.5f);
            moneyTextRect.anchoredPosition = new Vector2(10, 0);
            moneyTextRect.sizeDelta = new Vector2(200, 60);

            var moneyText = moneyTextObject.AddComponent<TextMeshProUGUI>();
            moneyText.text = "0";
            moneyText.fontSize = 44;
            moneyText.alignment = TextAlignmentOptions.Left;
            moneyText.font = font;

            return moneyText;
        }

        private static TMP_Text CreateWaveTitle(RectTransform parent, TMP_FontAsset font)
        {
            var titlePrefab = FindTitlePrefab("Title_01_NoDeco");

            if (titlePrefab == null)
            {
                titlePrefab = FindTitlePrefab("Title_01");
            }

            GameObject titleInstance;

            if (titlePrefab != null)
            {
                titleInstance = PrefabUtility.InstantiatePrefab(titlePrefab) as GameObject;
                titleInstance.transform.SetParent(parent, false);
            }
            else
            {
                titleInstance = new GameObject("WaveTitle");
                titleInstance.transform.SetParent(parent, false);
            }

            titleInstance.name = "WaveTitle";
            var titleRect = titleInstance.GetComponent<RectTransform>();

            if (titleRect == null)
            {
                titleRect = titleInstance.AddComponent<RectTransform>();
            }

            titleRect.anchorMin = new Vector2(0.5f, 1);
            titleRect.anchorMax = new Vector2(0.5f, 1);
            titleRect.pivot = new Vector2(0.5f, 1);
            titleRect.anchoredPosition = new Vector2(0, -40);
            titleRect.sizeDelta = new Vector2(400, 90);

            var waveText = titleInstance.GetComponentInChildren<TMP_Text>();

            if (waveText == null)
            {
                waveText = titleInstance.AddComponent<TextMeshProUGUI>();
            }

            waveText.text = "Wave 1/5";
            waveText.fontSize = 48;
            waveText.alignment = TextAlignmentOptions.Center;
            waveText.font = font;

            return waveText;
        }

        private static Image CreateHpBar(RectTransform parent, string name, Vector2 position, Vector2 anchor, string sliderPath)
        {
            var sliderInstance = InstantiateKitPrefab(sliderPath, parent);

            if (sliderInstance == null)
            {
                GameLogger.LogError($"[UIPrefabBuilder] Failed to create {name}.");
                return null;
            }

            sliderInstance.name = name;
            var sliderRect = sliderInstance.GetComponent<RectTransform>();
            sliderRect.anchorMin = anchor;
            sliderRect.anchorMax = anchor;
            sliderRect.pivot = anchor;
            sliderRect.anchoredPosition = position;
            sliderRect.sizeDelta = new Vector2(400, 40);

            var fillTransform = sliderInstance.transform.Find("Fill Area/Fill");

            if (fillTransform == null)
            {
                fillTransform = sliderInstance.transform.Find("Fill");
            }

            var fillImage = fillTransform?.GetComponent<Image>();

            if (fillImage != null)
            {
                fillImage.fillAmount = 1f;
            }
            else
            {
                GameLogger.LogError($"[UIPrefabBuilder] Fill Image not found in {name}.");
            }

            var handleTransform = sliderInstance.transform.Find("Handle Slide Area");

            if (handleTransform != null)
            {
                handleTransform.gameObject.SetActive(false);
            }

            return fillImage;
        }

        private static UI_UnitSpawnButtonView[] CreateUnitButtons(RectTransform parent, TMP_FontAsset font)
        {
            var buttons = new UI_UnitSpawnButtonView[UnitIds.Length];
            var xPositions = new[] { -300f, -150f, 0f, 150f, 300f };

            for (var i = 0; i < UnitIds.Length; i++)
            {
                buttons[i] = CreateUnitButton(parent, new Vector2(xPositions[i], 90), UnitIds[i], PetNames[i], font);
            }

            return buttons;
        }

        private static UI_UnitSpawnButtonView CreateUnitButton(RectTransform parent, Vector2 position, string unitId, string petName, TMP_FontAsset font)
        {
            var buttonInstance = InstantiateKitPrefab(ButtonCirclePath, parent);

            if (buttonInstance == null)
            {
                GameLogger.LogError($"[UIPrefabBuilder] Failed to create button for {unitId}.");
                return null;
            }

            buttonInstance.name = $"SpawnButton_{unitId}";
            var buttonRect = buttonInstance.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.5f, 0);
            buttonRect.anchorMax = new Vector2(0.5f, 0);
            buttonRect.pivot = new Vector2(0.5f, 0);
            buttonRect.anchoredPosition = position;
            buttonRect.sizeDelta = new Vector2(130, 130);

            var button = buttonInstance.GetComponent<Button>();

            if (button == null)
            {
                GameLogger.LogError($"[UIPrefabBuilder] Button component not found on {ButtonCirclePath}");
                return null;
            }

            // 펫 팩은 Idle 프레임만 zero-padding(_00)을 사용한다 (Attack은 _0)
            var unitIconSprite = LoadSprite($"{PetIconRoot}{petName}/Idle/{petName}-Idle_00.png");

            var unitIconObject = new GameObject("UnitIcon");
            unitIconObject.transform.SetParent(buttonInstance.transform, false);

            var unitIconRect = unitIconObject.AddComponent<RectTransform>();
            unitIconRect.anchorMin = new Vector2(0.5f, 0.5f);
            unitIconRect.anchorMax = new Vector2(0.5f, 0.5f);
            unitIconRect.pivot = new Vector2(0.5f, 0.5f);
            unitIconRect.anchoredPosition = Vector2.zero;
            unitIconRect.sizeDelta = new Vector2(90, 90);

            var unitIconImage = unitIconObject.AddComponent<Image>();
            unitIconImage.sprite = unitIconSprite;
            unitIconImage.raycastTarget = false;

            var nameTextObject = new GameObject("NameText");
            nameTextObject.transform.SetParent(buttonInstance.transform, false);

            var nameTextRect = nameTextObject.AddComponent<RectTransform>();
            nameTextRect.anchorMin = new Vector2(0, 1);
            nameTextRect.anchorMax = new Vector2(1, 1);
            nameTextRect.pivot = new Vector2(0.5f, 1);
            nameTextRect.anchoredPosition = new Vector2(0, -5);
            nameTextRect.sizeDelta = new Vector2(-10, 25);

            var nameText = nameTextObject.AddComponent<TextMeshProUGUI>();
            nameText.text = "Unit";
            nameText.fontSize = 20;
            nameText.alignment = TextAlignmentOptions.Center;
            nameText.font = font;

            var costTextObject = new GameObject("CostText");
            costTextObject.transform.SetParent(buttonInstance.transform, false);

            var costTextRect = costTextObject.AddComponent<RectTransform>();
            costTextRect.anchorMin = new Vector2(0, 0);
            costTextRect.anchorMax = new Vector2(1, 0);
            costTextRect.pivot = new Vector2(0.5f, 0);
            costTextRect.anchoredPosition = new Vector2(0, 5);
            costTextRect.sizeDelta = new Vector2(-10, 30);

            var costText = costTextObject.AddComponent<TextMeshProUGUI>();
            costText.text = "0";
            costText.fontSize = 28;
            costText.alignment = TextAlignmentOptions.Center;
            costText.font = font;

            var cooldownFillObject = new GameObject("CooldownFill");
            cooldownFillObject.transform.SetParent(buttonInstance.transform, false);

            var cooldownRect = cooldownFillObject.AddComponent<RectTransform>();
            cooldownRect.anchorMin = Vector2.zero;
            cooldownRect.anchorMax = Vector2.one;
            cooldownRect.offsetMin = Vector2.zero;
            cooldownRect.offsetMax = Vector2.zero;

            var cooldownImage = cooldownFillObject.AddComponent<Image>();
            cooldownImage.color = new Color(0, 0, 0, 0.6f);
            cooldownImage.type = Image.Type.Filled;
            cooldownImage.fillMethod = Image.FillMethod.Radial360;
            cooldownImage.fillAmount = 0f;
            cooldownImage.raycastTarget = false;

            var buttonView = buttonInstance.AddComponent<UI_UnitSpawnButtonView>();
            var serializedObject = new SerializedObject(buttonView);
            serializedObject.FindProperty("button").objectReferenceValue = button;
            serializedObject.FindProperty("cooldownFillImage").objectReferenceValue = cooldownImage;
            serializedObject.FindProperty("costText").objectReferenceValue = costText;
            serializedObject.FindProperty("nameText").objectReferenceValue = nameText;
            serializedObject.FindProperty("unitId").stringValue = unitId;
            serializedObject.ApplyModifiedProperties();

            return buttonView;
        }

        private static void CreateResultPopupInHud(RectTransform canvasTransform, TMP_FontAsset font, UI_GameHudView hudView)
        {
            var resultPopupRoot = new GameObject("ResultPopupRoot");
            resultPopupRoot.transform.SetParent(canvasTransform, false);

            var rootRect = resultPopupRoot.AddComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            var dimImage = resultPopupRoot.AddComponent<Image>();
            dimImage.color = new Color(0, 0, 0, 0.7f);
            dimImage.raycastTarget = true;

            var popupInstance = InstantiateKitPrefab(PopupPath, resultPopupRoot.transform);

            if (popupInstance == null)
            {
                GameLogger.LogError("[UIPrefabBuilder] Failed to create ResultPopup.");
                return;
            }

            popupInstance.name = "PopupBox";
            var popupRect = popupInstance.GetComponent<RectTransform>();
            popupRect.anchorMin = new Vector2(0.5f, 0.5f);
            popupRect.anchorMax = new Vector2(0.5f, 0.5f);
            popupRect.pivot = new Vector2(0.5f, 0.5f);
            popupRect.anchoredPosition = Vector2.zero;
            popupRect.sizeDelta = new Vector2(700, 900);

            var resultTextObject = new GameObject("ResultText");
            resultTextObject.transform.SetParent(popupInstance.transform, false);

            var resultTextRect = resultTextObject.AddComponent<RectTransform>();
            resultTextRect.anchorMin = new Vector2(0.5f, 1);
            resultTextRect.anchorMax = new Vector2(0.5f, 1);
            resultTextRect.pivot = new Vector2(0.5f, 1);
            resultTextRect.anchoredPosition = new Vector2(0, -80);
            resultTextRect.sizeDelta = new Vector2(600, 120);

            var resultText = resultTextObject.AddComponent<TextMeshProUGUI>();
            resultText.text = "VICTORY";
            resultText.fontSize = 90;
            resultText.alignment = TextAlignmentOptions.Center;
            resultText.font = font;

            var retryButton = CreateResultButton(popupInstance.transform, "RetryButton", new Vector2(-130, 80), "RETRY", ButtonGreenPath, font);
            var stageSelectButton = CreateResultButton(popupInstance.transform, "StageSelectButton", new Vector2(130, 80), "STAGES", ButtonBluePath, font);

            // 팝업 필드는 UI_ResultPopupView 소속 — 활성 GO(HUD 루트)에 부착해 RegisterComponentInHierarchy가 찾을 수 있게 한다
            var resultPopupView = hudView.gameObject.AddComponent<UI_ResultPopupView>();
            var serializedObject = new SerializedObject(resultPopupView);
            serializedObject.FindProperty("root").objectReferenceValue = resultPopupRoot;
            serializedObject.FindProperty("resultText").objectReferenceValue = resultText;
            serializedObject.FindProperty("retryButton").objectReferenceValue = retryButton;
            serializedObject.FindProperty("stageSelectButton").objectReferenceValue = stageSelectButton;
            serializedObject.ApplyModifiedProperties();

            resultPopupRoot.SetActive(false);
        }

        private static Button CreateResultButton(Transform parent, string name, Vector2 position, string labelText, string buttonPrefabPath, TMP_FontAsset font)
        {
            var buttonInstance = InstantiateKitPrefab(buttonPrefabPath, parent);

            if (buttonInstance == null)
            {
                GameLogger.LogError($"[UIPrefabBuilder] Failed to create {name}.");
                return null;
            }

            buttonInstance.name = name;
            var buttonRect = buttonInstance.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.5f, 0);
            buttonRect.anchorMax = new Vector2(0.5f, 0);
            buttonRect.pivot = new Vector2(0.5f, 0);
            buttonRect.anchoredPosition = position;
            buttonRect.sizeDelta = new Vector2(240, 90);

            var button = buttonInstance.GetComponent<Button>();

            if (button == null)
            {
                GameLogger.LogError($"[UIPrefabBuilder] Button component not found on {buttonPrefabPath}");
                return null;
            }

            var labelTMP = buttonInstance.GetComponentInChildren<TMP_Text>();

            if (labelTMP != null)
            {
                labelTMP.text = labelText;
                labelTMP.fontSize = 36;
                labelTMP.alignment = TextAlignmentOptions.Center;
                labelTMP.font = font;
            }
            else
            {
                GameLogger.LogWarning($"[UIPrefabBuilder] TMP_Text not found in {name}.");
            }

            return button;
        }

        private static void BuildStageSelectScreenPrefab(TMP_FontAsset font, Sprite starIcon, Sprite lockIcon)
        {
            var rootObject = new GameObject("StageSelectScreen");
            var rootRect = rootObject.AddComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            CreateStageTitle(rootRect, font);
            var stageButtons = CreateStageButtons(rootRect, font, starIcon, lockIcon);

            var stageSelectView = rootObject.AddComponent<UI_StageSelectView>();
            var serializedObject = new SerializedObject(stageSelectView);
            serializedObject.FindProperty("stageButtons").arraySize = stageButtons.Length;
            for (var i = 0; i < stageButtons.Length; i++)
            {
                serializedObject.FindProperty("stageButtons").GetArrayElementAtIndex(i).objectReferenceValue = stageButtons[i];
            }
            serializedObject.ApplyModifiedProperties();

            var prefabPath = OutputRoot + "StageSelectScreen.prefab";
            DeleteExistingPrefab(prefabPath);
            PrefabUtility.SaveAsPrefabAsset(rootObject, prefabPath);
            Object.DestroyImmediate(rootObject);

            GameLogger.Log($"[UIPrefabBuilder] StageSelectScreen.prefab created at {prefabPath}");
        }

        private static void CreateStageTitle(RectTransform parent, TMP_FontAsset font)
        {
            var titlePrefab = FindTitlePrefab("Title_01_Deco");

            if (titlePrefab == null)
            {
                titlePrefab = FindTitlePrefab("Title_01");
            }

            GameObject titleInstance;

            if (titlePrefab != null)
            {
                titleInstance = PrefabUtility.InstantiatePrefab(titlePrefab) as GameObject;
                titleInstance.transform.SetParent(parent, false);
            }
            else
            {
                titleInstance = new GameObject("Title");
                titleInstance.transform.SetParent(parent, false);
            }

            titleInstance.name = "Title";
            var titleRect = titleInstance.GetComponent<RectTransform>();

            if (titleRect == null)
            {
                titleRect = titleInstance.AddComponent<RectTransform>();
            }

            titleRect.anchorMin = new Vector2(0.5f, 1);
            titleRect.anchorMax = new Vector2(0.5f, 1);
            titleRect.pivot = new Vector2(0.5f, 1);
            titleRect.anchoredPosition = new Vector2(0, -60);
            titleRect.sizeDelta = new Vector2(600, 120);

            var titleText = titleInstance.GetComponentInChildren<TMP_Text>();

            if (titleText == null)
            {
                titleText = titleInstance.AddComponent<TextMeshProUGUI>();
            }

            titleText.text = "FD DEFENSE";
            titleText.fontSize = 72;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.font = font;
        }

        private static UI_StageButtonView[] CreateStageButtons(RectTransform parent, TMP_FontAsset font, Sprite starIcon, Sprite lockIcon)
        {
            var buttons = new UI_StageButtonView[StageIds.Length];
            var yPositions = new[] { 120f, -80f, -280f };
            var stageNames = new[] { "STAGE 1", "STAGE 2", "STAGE 3" };

            var stageButtonPrefabPath = KitRoot + "Theme_Light/Prefabs/Prefabs_Button/Button_03_Blue.prefab";
            var stageButtonPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(stageButtonPrefabPath);

            if (stageButtonPrefab == null)
            {
                stageButtonPrefabPath = KitRoot + "Theme_Light/Prefabs/Prefabs_Button/Button_02_Blue.prefab";
                stageButtonPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(stageButtonPrefabPath);

                if (stageButtonPrefab == null)
                {
                    stageButtonPrefabPath = ButtonBluePath;
                    GameLogger.LogWarning("[UIPrefabBuilder] Button_03_Blue not found, using Button_01_Blue as fallback.");
                }
            }

            for (var i = 0; i < StageIds.Length; i++)
            {
                buttons[i] = CreateStageButton(parent, new Vector2(0, yPositions[i]), StageIds[i], stageNames[i], stageButtonPrefabPath, font, starIcon, lockIcon);
            }

            return buttons;
        }

        private static UI_StageButtonView CreateStageButton(RectTransform parent, Vector2 position, string stageId, string stageName, string buttonPrefabPath, TMP_FontAsset font, Sprite starIcon, Sprite lockIcon)
        {
            var buttonInstance = InstantiateKitPrefab(buttonPrefabPath, parent);

            if (buttonInstance == null)
            {
                GameLogger.LogError($"[UIPrefabBuilder] Failed to create button for {stageId}.");
                return null;
            }

            buttonInstance.name = $"StageButton_{stageId}";
            var buttonRect = buttonInstance.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
            buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
            buttonRect.pivot = new Vector2(0.5f, 0.5f);
            buttonRect.anchoredPosition = position;
            buttonRect.sizeDelta = new Vector2(520, 150);

            var button = buttonInstance.GetComponent<Button>();

            if (button == null)
            {
                GameLogger.LogError($"[UIPrefabBuilder] Button component not found on {buttonPrefabPath}");
                return null;
            }

            var stageNameTMP = buttonInstance.GetComponentInChildren<TMP_Text>();

            if (stageNameTMP != null)
            {
                stageNameTMP.text = stageName;
                stageNameTMP.fontSize = 48;
                stageNameTMP.alignment = TextAlignmentOptions.Center;
                stageNameTMP.font = font;
            }
            else
            {
                var stageNameTextObject = new GameObject("StageNameText");
                stageNameTextObject.transform.SetParent(buttonInstance.transform, false);

                var textRect = stageNameTextObject.AddComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.offsetMin = Vector2.zero;
                textRect.offsetMax = Vector2.zero;

                stageNameTMP = stageNameTextObject.AddComponent<TextMeshProUGUI>();
                stageNameTMP.text = stageName;
                stageNameTMP.fontSize = 48;
                stageNameTMP.alignment = TextAlignmentOptions.Center;
                stageNameTMP.font = font;
            }

            var clearedMarkObject = new GameObject("ClearedMark");
            clearedMarkObject.transform.SetParent(buttonInstance.transform, false);

            var clearedMarkRect = clearedMarkObject.AddComponent<RectTransform>();
            clearedMarkRect.anchorMin = new Vector2(1, 0.5f);
            clearedMarkRect.anchorMax = new Vector2(1, 0.5f);
            clearedMarkRect.pivot = new Vector2(1, 0.5f);
            clearedMarkRect.anchoredPosition = new Vector2(-20, 0);
            clearedMarkRect.sizeDelta = new Vector2(64, 64);

            var clearedMarkImage = clearedMarkObject.AddComponent<Image>();
            clearedMarkImage.sprite = starIcon;
            clearedMarkImage.raycastTarget = false;

            clearedMarkObject.SetActive(false);

            var lockedOverlayObject = new GameObject("LockedOverlay");
            lockedOverlayObject.transform.SetParent(buttonInstance.transform, false);

            var overlayRect = lockedOverlayObject.AddComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;

            var overlayImage = lockedOverlayObject.AddComponent<Image>();
            overlayImage.color = new Color(0, 0, 0, 0.6f);
            overlayImage.raycastTarget = true;

            var lockIconObject = new GameObject("LockIcon");
            lockIconObject.transform.SetParent(lockedOverlayObject.transform, false);

            var lockIconRect = lockIconObject.AddComponent<RectTransform>();
            lockIconRect.anchorMin = new Vector2(0.5f, 0.5f);
            lockIconRect.anchorMax = new Vector2(0.5f, 0.5f);
            lockIconRect.pivot = new Vector2(0.5f, 0.5f);
            lockIconRect.anchoredPosition = Vector2.zero;
            lockIconRect.sizeDelta = new Vector2(64, 64);

            var lockIconImage = lockIconObject.AddComponent<Image>();
            lockIconImage.sprite = lockIcon;
            lockIconImage.raycastTarget = false;

            lockedOverlayObject.SetActive(true);

            var buttonView = buttonInstance.AddComponent<UI_StageButtonView>();
            var serializedObject = new SerializedObject(buttonView);
            serializedObject.FindProperty("button").objectReferenceValue = button;
            serializedObject.FindProperty("stageNameText").objectReferenceValue = stageNameTMP;
            serializedObject.FindProperty("clearedMark").objectReferenceValue = clearedMarkObject;
            serializedObject.FindProperty("lockedOverlay").objectReferenceValue = lockedOverlayObject;
            serializedObject.FindProperty("stageId").stringValue = stageId;
            serializedObject.ApplyModifiedProperties();

            return buttonView;
        }

        private static void DeleteExistingPrefab(string path)
        {
            if (AssetDatabase.AssetPathExists(path))
            {
                AssetDatabase.DeleteAsset(path);
            }
        }
    }
}
