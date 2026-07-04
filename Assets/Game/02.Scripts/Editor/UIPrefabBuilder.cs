using System.Linq;
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
        private const string ResourceBarGroupPath = KitRoot + "Theme_Light/Prefabs/Prefabs_HUD/ResourceBar_Group.prefab";
        private const string ResultWinPath = KitRoot + "Theme_Light/Prefabs/Prefabs~DemoScenes/Play_Result_Win_Detail.prefab";
        private const string ResultLosePath = KitRoot + "Theme_Light/Prefabs/Prefabs~DemoScenes/Play_Result_Lose.prefab";
        private const string SleepModePath = KitRoot + "Theme_Light/Prefabs/Prefabs~DemoScenes/Play_SleepMode.prefab";
        private const string FontPath = KitRoot + "Shared/Font/LTAvocado-Bold SDF.asset";
        private const string IconCoinPath = KitRoot + "Shared/Icons/PictoIcon/128/coin_2.png";
        private const string IconLockPath = KitRoot + "Shared/Icons/PictoIcon/128/lock.png";
        private const string IconStarPath = KitRoot + "Shared/Icons/PictoIcon/128/star_1.png";
        private const string IconPausePath = KitRoot + "Shared/Icons/PictoIcon/128/pause_round.png";
        private const string PetIconRoot = "Assets/Layer Lab/2D Characters-PetPack1/Sprites/ImageSequence/";

        private static readonly string[] PetNames = { "Goldfish", "Chick", "Bat", "Bomb", "Flower", "Pug", "Ghost", "Melody", "Sword", "Titan" };
        private static readonly string[] UnitIds = { "pet_goldfish", "pet_chick", "pet_bat", "pet_bomb", "pet_flower", "pet_pug", "pet_ghost", "pet_melody", "pet_sword", "pet_titan" };
        private static readonly string[] StageIds = { "stage_001", "stage_002", "stage_003" };

        public static void BuildAllUiPrefabs()
        {
            EnsureOutputFolder();

            var font = LoadFont();
            var coinIcon = LoadSprite(IconCoinPath);
            var lockIcon = LoadSprite(IconLockPath);
            var starIcon = LoadSprite(IconStarPath);
            var pauseIcon = LoadSprite(IconPausePath);

            BuildGameHudPrefab(font, coinIcon, pauseIcon);
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


        private static Button EnsureButtonComponent(GameObject buttonInstance)
        {
            var button = buttonInstance.GetComponent<Button>();

            if (button == null)
            {
                // GUI Pro 킷 버튼 프리팹은 순수 이미지 프레임이라 Button 컴포넌트를 사용처에서 부착해야 한다
                button = buttonInstance.AddComponent<Button>();
                button.targetGraphic = buttonInstance.GetComponentInChildren<Image>();
            }

            return button;
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

        private static void BuildGameHudPrefab(TMP_FontAsset font, Sprite coinIcon, Sprite pauseIcon)
        {
            var rootObject = new GameObject("GameHud");
            var rootRect = rootObject.AddComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            var moneyText = CreateResourceBarGroup(rootRect, coinIcon);
            var waveText = CreateWaveTitle(rootRect, font);
            var allyBaseHpFillImage = CreateHpBar(rootRect, "AllyHpBar", new Vector2(60, -200), new Vector2(0, 1), SliderBluePath);
            var enemyBaseHpFillImage = CreateHpBar(rootRect, "EnemyHpBar", new Vector2(-60, -200), new Vector2(1, 1), SliderRedPath);
            var spawnButtons = CreateUnitButtons(rootRect, font);
            var pauseButton = CreatePauseButton(rootRect, pauseIcon);
            var bossBanner = CreateBossBanner(rootRect, font);

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
            serializedObject.FindProperty("bossBanner").objectReferenceValue = bossBanner;
            serializedObject.ApplyModifiedProperties();

            var pausePopupRoot = CreatePausePopupInHud(rootRect, font, hudView, pauseButton);
            CreateResultPopupInHud(rootRect, font, hudView);

            if (pausePopupRoot != null)
            {
                pausePopupRoot.transform.SetAsFirstSibling();
            }

            var prefabPath = OutputRoot + "GameHud.prefab";
            DeleteExistingPrefab(prefabPath);
            PrefabUtility.SaveAsPrefabAsset(rootObject, prefabPath);
            Object.DestroyImmediate(rootObject);

            GameLogger.Log($"[UIPrefabBuilder] GameHud.prefab created at {prefabPath}");
        }

        private static TMP_Text CreateResourceBarGroup(RectTransform parent, Sprite coinIcon)
        {
            var resourceBarInstance = InstantiateKitPrefab(ResourceBarGroupPath, parent);

            if (resourceBarInstance == null)
            {
                GameLogger.LogWarning("[UIPrefabBuilder] ResourceBar_Group not found, falling back to ItemFrame.");
                return CreateMoneyFrameFallback(parent, coinIcon);
            }

            resourceBarInstance.name = "ResourceBarGroup";
            var resourceBarRect = resourceBarInstance.GetComponent<RectTransform>();
            resourceBarRect.anchorMin = new Vector2(0, 1);
            resourceBarRect.anchorMax = new Vector2(0, 1);
            resourceBarRect.pivot = new Vector2(0, 1);
            resourceBarRect.anchoredPosition = new Vector2(40, -40);

            var firstSlot = resourceBarInstance.transform.GetChild(0);
            var iconTransform = firstSlot.Find("Icon");

            if (iconTransform != null && coinIcon != null)
            {
                var iconImage = iconTransform.GetComponent<Image>();

                if (iconImage != null)
                {
                    iconImage.sprite = coinIcon;
                }
            }

            var textTMP = firstSlot.GetComponentInChildren<TMP_Text>();

            if (textTMP == null)
            {
                GameLogger.LogError("[UIPrefabBuilder] Text (TMP) not found in ResourceBar_Group first slot.");
            }
            else
            {
                textTMP.text = "0";
            }

            for (var i = 1; i < resourceBarInstance.transform.childCount; i++)
            {
                resourceBarInstance.transform.GetChild(i).gameObject.SetActive(false);
            }

            return textTMP;
        }

        private static TMP_Text CreateMoneyFrameFallback(RectTransform parent, Sprite coinIcon)
        {
            var frameInstance = InstantiateKitPrefab(ItemFramePath, parent);

            if (frameInstance == null)
            {
                GameLogger.LogError("[UIPrefabBuilder] Failed to create MoneyFrame fallback.");
                return null;
            }

            frameInstance.name = "MoneyFrame";
            var frameRect = frameInstance.GetComponent<RectTransform>();
            frameRect.anchorMin = new Vector2(0, 1);
            frameRect.anchorMax = new Vector2(0, 1);
            frameRect.pivot = new Vector2(0, 1);
            frameRect.anchoredPosition = new Vector2(40, -40);
            frameRect.localScale = Vector3.one * 1.2f;

            var iconTransform = frameInstance.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name == "Icon");
            var iconImage = iconTransform?.GetComponent<Image>();

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

            var font = LoadFont();

            if (font != null)
            {
                moneyText.font = font;
            }

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
            const int columnCount = 5;
            const float spacing = 135f;
            const float rowHeight = 135f;
            const float bottomRowY = 90f;

            for (var i = 0; i < UnitIds.Length; i++)
            {
                var column = i % columnCount;
                var row = i / columnCount;
                var x = (column - (columnCount - 1) * 0.5f) * spacing;
                // 코스트 오름차순 앞 5개가 아랫줄(빠른 접근), 다음 5개가 윗줄
                var y = bottomRowY + row * rowHeight;
                buttons[i] = CreateUnitButton(parent, new Vector2(x, y), UnitIds[i], PetNames[i], font);
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

            var button = EnsureButtonComponent(buttonInstance);

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

        private static Button CreatePauseButton(RectTransform parent, Sprite pauseIcon)
        {
            var buttonInstance = InstantiateKitPrefab(ButtonCirclePath, parent);

            if (buttonInstance == null)
            {
                GameLogger.LogError("[UIPrefabBuilder] Failed to create PauseButton.");
                return null;
            }

            buttonInstance.name = "PauseButton";
            var buttonRect = buttonInstance.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(1, 1);
            buttonRect.anchorMax = new Vector2(1, 1);
            buttonRect.pivot = new Vector2(1, 1);
            buttonRect.anchoredPosition = new Vector2(-80, -80);
            buttonRect.sizeDelta = new Vector2(90, 90);

            var button = EnsureButtonComponent(buttonInstance);

            var iconObject = new GameObject("PauseIcon");
            iconObject.transform.SetParent(buttonInstance.transform, false);

            var iconRect = iconObject.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.5f, 0.5f);
            iconRect.anchorMax = new Vector2(0.5f, 0.5f);
            iconRect.pivot = new Vector2(0.5f, 0.5f);
            iconRect.anchoredPosition = Vector2.zero;
            iconRect.sizeDelta = new Vector2(50, 50);

            var iconImage = iconObject.AddComponent<Image>();
            iconImage.sprite = pauseIcon;
            iconImage.raycastTarget = false;

            return button;
        }

        private static GameObject CreateBossBanner(RectTransform parent, TMP_FontAsset font)
        {
            var titlePrefab = FindTitlePrefab("Deco");

            GameObject bannerInstance;

            if (titlePrefab != null)
            {
                bannerInstance = PrefabUtility.InstantiatePrefab(titlePrefab) as GameObject;
                bannerInstance.transform.SetParent(parent, false);
            }
            else
            {
                bannerInstance = new GameObject("BossBanner");
                bannerInstance.transform.SetParent(parent, false);
            }

            bannerInstance.name = "BossBanner";
            var bannerRect = bannerInstance.GetComponent<RectTransform>();

            if (bannerRect == null)
            {
                bannerRect = bannerInstance.AddComponent<RectTransform>();
            }

            bannerRect.anchorMin = new Vector2(0.5f, 1);
            bannerRect.anchorMax = new Vector2(0.5f, 1);
            bannerRect.pivot = new Vector2(0.5f, 1);
            bannerRect.anchoredPosition = new Vector2(0, -350);
            bannerRect.sizeDelta = new Vector2(600, 110);

            var bannerText = bannerInstance.GetComponentInChildren<TMP_Text>();

            if (bannerText == null)
            {
                bannerText = bannerInstance.AddComponent<TextMeshProUGUI>();
            }

            bannerText.text = "BOSS INCOMING";
            bannerText.fontSize = 56;
            bannerText.alignment = TextAlignmentOptions.Center;
            bannerText.font = font;
            bannerText.color = new Color(0.9f, 0.2f, 0.2f);

            bannerInstance.SetActive(false);

            return bannerInstance;
        }

        private static GameObject CreatePausePopupInHud(RectTransform canvasTransform, TMP_FontAsset font, UI_GameHudView hudView, Button pauseButton)
        {
            var pausePopupRoot = new GameObject("PausePopupRoot");
            pausePopupRoot.transform.SetParent(canvasTransform, false);

            var rootRect = pausePopupRoot.AddComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            var dimImage = pausePopupRoot.AddComponent<Image>();
            dimImage.color = new Color(0, 0, 0, 0.7f);
            dimImage.raycastTarget = true;

            var sleepModeInstance = InstantiateKitPrefab(SleepModePath, pausePopupRoot.transform);

            if (sleepModeInstance == null)
            {
                GameLogger.LogWarning("[UIPrefabBuilder] Play_SleepMode not found, using Popup_Box fallback.");
                return CreatePausePopupFallback(pausePopupRoot, font, hudView, pauseButton);
            }

            sleepModeInstance.name = "SleepModeContent";
            var sleepModeRect = sleepModeInstance.GetComponent<RectTransform>();
            sleepModeRect.anchorMin = new Vector2(0.5f, 0.5f);
            sleepModeRect.anchorMax = new Vector2(0.5f, 0.5f);
            sleepModeRect.pivot = new Vector2(0.5f, 0.5f);
            sleepModeRect.anchoredPosition = Vector2.zero;

            var resumeButton = CreatePausePopupButton(pausePopupRoot.transform, "ResumeButton", new Vector2(0, 40), "RESUME", ButtonGreenPath, font);
            var retryButton = CreatePausePopupButton(pausePopupRoot.transform, "RetryButton", new Vector2(0, -80), "RETRY", ButtonBluePath, font);
            var stageSelectButton = CreatePausePopupButton(pausePopupRoot.transform, "StageSelectButton", new Vector2(0, -200), "STAGES", ButtonBluePath, font);

            var pausePopupView = hudView.gameObject.AddComponent<UI_PausePopupView>();
            var serializedObject = new SerializedObject(pausePopupView);
            serializedObject.FindProperty("root").objectReferenceValue = pausePopupRoot;
            serializedObject.FindProperty("pauseButton").objectReferenceValue = pauseButton;
            serializedObject.FindProperty("resumeButton").objectReferenceValue = resumeButton;
            serializedObject.FindProperty("retryButton").objectReferenceValue = retryButton;
            serializedObject.FindProperty("stageSelectButton").objectReferenceValue = stageSelectButton;
            serializedObject.ApplyModifiedProperties();

            pausePopupRoot.SetActive(false);

            return pausePopupRoot;
        }

        private static GameObject CreatePausePopupFallback(GameObject pausePopupRoot, TMP_FontAsset font, UI_GameHudView hudView, Button pauseButton)
        {
            var popupInstance = InstantiateKitPrefab(PopupPath, pausePopupRoot.transform);

            if (popupInstance == null)
            {
                GameLogger.LogError("[UIPrefabBuilder] Failed to create PausePopup fallback.");
                return null;
            }

            popupInstance.name = "PopupBox";
            var popupRect = popupInstance.GetComponent<RectTransform>();
            popupRect.anchorMin = new Vector2(0.5f, 0.5f);
            popupRect.anchorMax = new Vector2(0.5f, 0.5f);
            popupRect.pivot = new Vector2(0.5f, 0.5f);
            popupRect.anchoredPosition = Vector2.zero;
            popupRect.sizeDelta = new Vector2(600, 700);

            var titleTextObject = new GameObject("TitleText");
            titleTextObject.transform.SetParent(popupInstance.transform, false);

            var titleTextRect = titleTextObject.AddComponent<RectTransform>();
            titleTextRect.anchorMin = new Vector2(0.5f, 1);
            titleTextRect.anchorMax = new Vector2(0.5f, 1);
            titleTextRect.pivot = new Vector2(0.5f, 1);
            titleTextRect.anchoredPosition = new Vector2(0, -80);
            titleTextRect.sizeDelta = new Vector2(500, 120);

            var titleText = titleTextObject.AddComponent<TextMeshProUGUI>();
            titleText.text = "PAUSED";
            titleText.fontSize = 72;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.font = font;

            var resumeButton = CreatePausePopupButton(popupInstance.transform, "ResumeButton", new Vector2(0, 40), "RESUME", ButtonGreenPath, font);
            var retryButton = CreatePausePopupButton(popupInstance.transform, "RetryButton", new Vector2(0, -80), "RETRY", ButtonBluePath, font);
            var stageSelectButton = CreatePausePopupButton(popupInstance.transform, "StageSelectButton", new Vector2(0, -200), "STAGES", ButtonBluePath, font);

            var pausePopupView = hudView.gameObject.AddComponent<UI_PausePopupView>();
            var serializedObject = new SerializedObject(pausePopupView);
            serializedObject.FindProperty("root").objectReferenceValue = pausePopupRoot;
            serializedObject.FindProperty("pauseButton").objectReferenceValue = pauseButton;
            serializedObject.FindProperty("resumeButton").objectReferenceValue = resumeButton;
            serializedObject.FindProperty("retryButton").objectReferenceValue = retryButton;
            serializedObject.FindProperty("stageSelectButton").objectReferenceValue = stageSelectButton;
            serializedObject.ApplyModifiedProperties();

            return pausePopupRoot;
        }

        private static Button CreatePausePopupButton(Transform parent, string name, Vector2 position, string labelText, string buttonPrefabPath, TMP_FontAsset font)
        {
            var buttonInstance = InstantiateKitPrefab(buttonPrefabPath, parent);

            if (buttonInstance == null)
            {
                GameLogger.LogError($"[UIPrefabBuilder] Failed to create {name}.");
                return null;
            }

            buttonInstance.name = name;
            var buttonRect = buttonInstance.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
            buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
            buttonRect.pivot = new Vector2(0.5f, 0.5f);
            buttonRect.anchoredPosition = position;
            buttonRect.sizeDelta = new Vector2(300, 90);

            var button = EnsureButtonComponent(buttonInstance);

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

            var winInstance = InstantiateKitPrefab(ResultWinPath, resultPopupRoot.transform);
            var loseInstance = InstantiateKitPrefab(ResultLosePath, resultPopupRoot.transform);

            if (winInstance == null || loseInstance == null)
            {
                GameLogger.LogWarning("[UIPrefabBuilder] Play_Result prefabs not found, using Popup_Box fallback.");
                CreateResultPopupFallback(resultPopupRoot, font, hudView);
                return;
            }

            winInstance.name = "WinContent";
            loseInstance.name = "LoseContent";

            var winRect = winInstance.GetComponent<RectTransform>();
            winRect.anchorMin = new Vector2(0.5f, 0.5f);
            winRect.anchorMax = new Vector2(0.5f, 0.5f);
            winRect.pivot = new Vector2(0.5f, 0.5f);
            winRect.anchoredPosition = Vector2.zero;

            var loseRect = loseInstance.GetComponent<RectTransform>();
            loseRect.anchorMin = new Vector2(0.5f, 0.5f);
            loseRect.anchorMax = new Vector2(0.5f, 0.5f);
            loseRect.pivot = new Vector2(0.5f, 0.5f);
            loseRect.anchoredPosition = Vector2.zero;

            winInstance.SetActive(false);
            loseInstance.SetActive(false);

            var retryButton = CreateResultButton(resultPopupRoot.transform, "RetryButton", new Vector2(-130, 80), "RETRY", ButtonGreenPath, font);
            var stageSelectButton = CreateResultButton(resultPopupRoot.transform, "StageSelectButton", new Vector2(130, 80), "STAGES", ButtonBluePath, font);

            var resultPopupView = hudView.gameObject.AddComponent<UI_ResultPopupView>();
            var serializedObject = new SerializedObject(resultPopupView);
            serializedObject.FindProperty("root").objectReferenceValue = resultPopupRoot;
            serializedObject.FindProperty("winRoot").objectReferenceValue = winInstance;
            serializedObject.FindProperty("loseRoot").objectReferenceValue = loseInstance;
            serializedObject.FindProperty("retryButton").objectReferenceValue = retryButton;
            serializedObject.FindProperty("stageSelectButton").objectReferenceValue = stageSelectButton;
            serializedObject.ApplyModifiedProperties();

            resultPopupRoot.SetActive(false);
        }

        private static void CreateResultPopupFallback(GameObject resultPopupRoot, TMP_FontAsset font, UI_GameHudView hudView)
        {
            var popupInstance = InstantiateKitPrefab(PopupPath, resultPopupRoot.transform);

            if (popupInstance == null)
            {
                GameLogger.LogError("[UIPrefabBuilder] Failed to create ResultPopup fallback.");
                return;
            }

            popupInstance.name = "PopupBox";
            var popupRect = popupInstance.GetComponent<RectTransform>();
            popupRect.anchorMin = new Vector2(0.5f, 0.5f);
            popupRect.anchorMax = new Vector2(0.5f, 0.5f);
            popupRect.pivot = new Vector2(0.5f, 0.5f);
            popupRect.anchoredPosition = Vector2.zero;
            popupRect.sizeDelta = new Vector2(700, 900);

            var winRoot = new GameObject("WinContent");
            winRoot.transform.SetParent(popupInstance.transform, false);
            var winRootRect = winRoot.AddComponent<RectTransform>();
            winRootRect.anchorMin = Vector2.zero;
            winRootRect.anchorMax = Vector2.one;
            winRootRect.offsetMin = Vector2.zero;
            winRootRect.offsetMax = Vector2.zero;

            var winTextObject = new GameObject("WinText");
            winTextObject.transform.SetParent(winRoot.transform, false);
            var winTextRect = winTextObject.AddComponent<RectTransform>();
            winTextRect.anchorMin = new Vector2(0.5f, 1);
            winTextRect.anchorMax = new Vector2(0.5f, 1);
            winTextRect.pivot = new Vector2(0.5f, 1);
            winTextRect.anchoredPosition = new Vector2(0, -80);
            winTextRect.sizeDelta = new Vector2(600, 120);
            var winText = winTextObject.AddComponent<TextMeshProUGUI>();
            winText.text = "VICTORY";
            winText.fontSize = 90;
            winText.alignment = TextAlignmentOptions.Center;
            winText.font = font;

            var loseRoot = new GameObject("LoseContent");
            loseRoot.transform.SetParent(popupInstance.transform, false);
            var loseRootRect = loseRoot.AddComponent<RectTransform>();
            loseRootRect.anchorMin = Vector2.zero;
            loseRootRect.anchorMax = Vector2.one;
            loseRootRect.offsetMin = Vector2.zero;
            loseRootRect.offsetMax = Vector2.zero;

            var loseTextObject = new GameObject("LoseText");
            loseTextObject.transform.SetParent(loseRoot.transform, false);
            var loseTextRect = loseTextObject.AddComponent<RectTransform>();
            loseTextRect.anchorMin = new Vector2(0.5f, 1);
            loseTextRect.anchorMax = new Vector2(0.5f, 1);
            loseTextRect.pivot = new Vector2(0.5f, 1);
            loseTextRect.anchoredPosition = new Vector2(0, -80);
            loseTextRect.sizeDelta = new Vector2(600, 120);
            var loseText = loseTextObject.AddComponent<TextMeshProUGUI>();
            loseText.text = "DEFEAT";
            loseText.fontSize = 90;
            loseText.alignment = TextAlignmentOptions.Center;
            loseText.font = font;

            winRoot.SetActive(false);
            loseRoot.SetActive(false);

            var retryButton = CreateResultButton(popupInstance.transform, "RetryButton", new Vector2(-130, 80), "RETRY", ButtonGreenPath, font);
            var stageSelectButton = CreateResultButton(popupInstance.transform, "StageSelectButton", new Vector2(130, 80), "STAGES", ButtonBluePath, font);

            var resultPopupView = hudView.gameObject.AddComponent<UI_ResultPopupView>();
            var serializedObject = new SerializedObject(resultPopupView);
            serializedObject.FindProperty("root").objectReferenceValue = resultPopupRoot;
            serializedObject.FindProperty("winRoot").objectReferenceValue = winRoot;
            serializedObject.FindProperty("loseRoot").objectReferenceValue = loseRoot;
            serializedObject.FindProperty("retryButton").objectReferenceValue = retryButton;
            serializedObject.FindProperty("stageSelectButton").objectReferenceValue = stageSelectButton;
            serializedObject.ApplyModifiedProperties();
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

            var button = EnsureButtonComponent(buttonInstance);

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

            var button = EnsureButtonComponent(buttonInstance);

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
