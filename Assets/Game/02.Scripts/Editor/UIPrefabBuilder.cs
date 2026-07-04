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
        private const string IconGoldPath = KitRoot + "Theme_Light/Sprites/HUD/ResourceBar_Icon_Gold.png";
        private const string IconArrowUpPath = KitRoot + "Theme_Light/Sprites/Slider_Icon/Slider_Upgrade_01_Icon_Arrow_Up.png";
        private const string CardFrameBluePath = KitRoot + "Theme_Light/Prefabs/Prefabs_Frame/CardFrame/CardFrame_02_Blue.prefab";
        private const string PetIconRoot = "Assets/Layer Lab/2D Characters-PetPack1/Sprites/ImageSequence/";

        private static readonly string[] PetNames = { "Goldfish", "Chick", "Bat", "Bomb", "Flower", "Pug", "Ghost", "Melody", "Sword", "Titan" };
        private static readonly string[] UnitIds = { "pet_goldfish", "pet_chick", "pet_bat", "pet_bomb", "pet_flower", "pet_pug", "pet_ghost", "pet_melody", "pet_sword", "pet_titan" };

        private static string[] GenerateStageIds()
        {
            var stageIds = new string[50];
            for (var i = 0; i < 50; i++)
            {
                stageIds[i] = $"stage_{(i + 1):D3}";
            }
            return stageIds;
        }

        public static void BuildAllUiPrefabs()
        {
            EnsureOutputFolder();

            var font = LoadFont();
            var coinIcon = LoadSprite(IconCoinPath);
            var lockIcon = LoadSprite(IconLockPath);
            var starIcon = LoadSprite(IconStarPath);
            var pauseIcon = LoadSprite(IconPausePath);
            var goldIcon = LoadSprite(IconGoldPath);
            var arrowIcon = LoadSprite(IconArrowUpPath);

            BuildGameHudPrefab(font, coinIcon, pauseIcon);
            BuildStageSelectScreenPrefab(font, starIcon, lockIcon, goldIcon, arrowIcon);

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
            serializedObject.FindProperty("unitIconImage").objectReferenceValue = unitIconImage;
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

            // Play_SleepMode는 폰 절전화면 데모라 배터리·LTE·시계·스테이지 표기가 포함됨 — 게임과 무관한 정보는 비활성화
            var unnecessaryNames = new[] { "Battery", "LTE", "Text_Time", "Text_Stage" };
            foreach (var child in sleepModeInstance.GetComponentsInChildren<Transform>(true))
            {
                if (unnecessaryNames.Contains(child.name))
                {
                    child.gameObject.SetActive(false);
                }
            }

            var pausedTitleObject = new GameObject("PausedTitleText");
            pausedTitleObject.transform.SetParent(pausePopupRoot.transform, false);

            var pausedTitleRect = pausedTitleObject.AddComponent<RectTransform>();
            pausedTitleRect.anchorMin = new Vector2(0.5f, 0.5f);
            pausedTitleRect.anchorMax = new Vector2(0.5f, 0.5f);
            pausedTitleRect.pivot = new Vector2(0.5f, 0.5f);
            pausedTitleRect.anchoredPosition = new Vector2(0, 200);
            pausedTitleRect.sizeDelta = new Vector2(500, 110);

            var pausedTitleText = pausedTitleObject.AddComponent<TextMeshProUGUI>();
            pausedTitleText.text = "PAUSED";
            pausedTitleText.fontSize = 80;
            pausedTitleText.alignment = TextAlignmentOptions.Center;
            pausedTitleText.font = font;

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

        private static void BuildStageSelectScreenPrefab(TMP_FontAsset font, Sprite starIcon, Sprite lockIcon, Sprite goldIcon, Sprite arrowIcon)
        {
            var rootObject = new GameObject("StageSelectScreen");
            var rootRect = rootObject.AddComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            var goldText = CreateTopGoldBar(rootRect, goldIcon);
            var tabButtons = CreateTabButtons(rootRect, font);
            var stagePanelRoot = CreateStagePanel(rootRect, font, starIcon, lockIcon);
            var deckPanel = CreateDeckPanel(rootRect, font, starIcon);
            var upgradePanel = CreateUpgradePanel(rootRect, font, starIcon, goldIcon, arrowIcon);
            var missionPanel = CreateMissionPanel(rootRect, font, goldIcon);
            var dailyRewardPopup = CreateDailyRewardPopup(rootRect, font, goldIcon, starIcon);
            var settingsPopup = CreateSettingsPopup(rootRect, font);

            var gearIcon = LoadSprite(KitRoot + "Shared/Icons/PictoIcon/128/headgear.png");
            if (gearIcon == null)
            {
                gearIcon = LoadSprite(KitRoot + "Shared/Icons/PictoIcon/128/menu_1.png");
            }
            var settingsButton = CreateSettingsButton(rootRect, gearIcon);

            var stageSelectView = stagePanelRoot.GetComponent<UI_StageSelectView>();
            var deckPanelView = deckPanel.GetComponent<UI_DeckPanelView>();
            var upgradePanelView = upgradePanel.GetComponent<UI_UpgradePanelView>();
            var missionPanelView = missionPanel.GetComponent<UI_MissionPanelView>();
            var dailyRewardPopupView = dailyRewardPopup.GetComponent<UI_DailyRewardPopupView>();
            var settingsPopupView = settingsPopup.GetComponent<UI_SettingsPopupView>();

            var outGameHomeView = rootObject.AddComponent<UI_OutGameHomeView>();
            var homeSerializer = new SerializedObject(outGameHomeView);
            homeSerializer.FindProperty("stageTabButton").objectReferenceValue = tabButtons[0];
            homeSerializer.FindProperty("deckTabButton").objectReferenceValue = tabButtons[1];
            homeSerializer.FindProperty("upgradeTabButton").objectReferenceValue = tabButtons[2];
            homeSerializer.FindProperty("missionsTabButton").objectReferenceValue = tabButtons[3];
            homeSerializer.FindProperty("settingsButton").objectReferenceValue = settingsButton;
            homeSerializer.FindProperty("stagePanelRoot").objectReferenceValue = stagePanelRoot;
            homeSerializer.FindProperty("goldText").objectReferenceValue = goldText;
            homeSerializer.ApplyModifiedProperties();

            rootObject.AddComponent<UI_StageSelectView>();
            var stageViewCopy = rootObject.GetComponent<UI_StageSelectView>();
            var stageSerializer = new SerializedObject(stageViewCopy);
            var originalButtons = stageSelectView.StageButtonList;
            stageSerializer.FindProperty("stageButtons").arraySize = originalButtons.Count;
            for (var i = 0; i < originalButtons.Count; i++)
            {
                stageSerializer.FindProperty("stageButtons").GetArrayElementAtIndex(i).objectReferenceValue = originalButtons[i];
            }
            stageSerializer.ApplyModifiedProperties();

            Object.DestroyImmediate(stageSelectView);

            rootObject.AddComponent<UI_MissionPanelView>();
            var missionViewCopy = rootObject.GetComponent<UI_MissionPanelView>();
            var missionSerializer = new SerializedObject(missionViewCopy);
            missionSerializer.FindProperty("root").objectReferenceValue = missionPanel;
            missionSerializer.FindProperty("missionRows").arraySize = missionPanelView.MissionRows.Length;
            for (var i = 0; i < missionPanelView.MissionRows.Length; i++)
            {
                missionSerializer.FindProperty("missionRows").GetArrayElementAtIndex(i).objectReferenceValue = missionPanelView.MissionRows[i];
            }
            missionSerializer.ApplyModifiedProperties();

            Object.DestroyImmediate(missionPanelView);

            rootObject.AddComponent<UI_DailyRewardPopupView>();
            var dailyRewardViewCopy = rootObject.GetComponent<UI_DailyRewardPopupView>();
            var dailyRewardSerializer = new SerializedObject(dailyRewardViewCopy);
            dailyRewardSerializer.FindProperty("root").objectReferenceValue = dailyRewardPopup;
            var originalDailyRewardSerializer = new SerializedObject(dailyRewardPopupView);
            dailyRewardSerializer.FindProperty("dayTexts").arraySize = 7;
            dailyRewardSerializer.FindProperty("goldTexts").arraySize = 7;
            dailyRewardSerializer.FindProperty("claimedMarks").arraySize = 7;
            dailyRewardSerializer.FindProperty("todayHighlights").arraySize = 7;
            for (var i = 0; i < 7; i++)
            {
                dailyRewardSerializer.FindProperty("dayTexts").GetArrayElementAtIndex(i).objectReferenceValue =
                    originalDailyRewardSerializer.FindProperty("dayTexts").GetArrayElementAtIndex(i).objectReferenceValue;
                dailyRewardSerializer.FindProperty("goldTexts").GetArrayElementAtIndex(i).objectReferenceValue =
                    originalDailyRewardSerializer.FindProperty("goldTexts").GetArrayElementAtIndex(i).objectReferenceValue;
                dailyRewardSerializer.FindProperty("claimedMarks").GetArrayElementAtIndex(i).objectReferenceValue =
                    originalDailyRewardSerializer.FindProperty("claimedMarks").GetArrayElementAtIndex(i).objectReferenceValue;
                dailyRewardSerializer.FindProperty("todayHighlights").GetArrayElementAtIndex(i).objectReferenceValue =
                    originalDailyRewardSerializer.FindProperty("todayHighlights").GetArrayElementAtIndex(i).objectReferenceValue;
            }
            dailyRewardSerializer.FindProperty("claimButton").objectReferenceValue = originalDailyRewardSerializer.FindProperty("claimButton").objectReferenceValue;
            dailyRewardSerializer.FindProperty("closeButton").objectReferenceValue = originalDailyRewardSerializer.FindProperty("closeButton").objectReferenceValue;
            dailyRewardSerializer.ApplyModifiedProperties();

            Object.DestroyImmediate(dailyRewardPopupView);

            rootObject.AddComponent<UI_SettingsPopupView>();
            var settingsViewCopy = rootObject.GetComponent<UI_SettingsPopupView>();
            var settingsSerializer = new SerializedObject(settingsViewCopy);
            var originalSettingsSerializer = new SerializedObject(settingsPopupView);
            settingsSerializer.FindProperty("root").objectReferenceValue = settingsPopup;
            settingsSerializer.FindProperty("vibrationToggle").objectReferenceValue = originalSettingsSerializer.FindProperty("vibrationToggle").objectReferenceValue;
            settingsSerializer.FindProperty("resetButton").objectReferenceValue = originalSettingsSerializer.FindProperty("resetButton").objectReferenceValue;
            settingsSerializer.FindProperty("resetConfirmRoot").objectReferenceValue = originalSettingsSerializer.FindProperty("resetConfirmRoot").objectReferenceValue;
            settingsSerializer.FindProperty("resetConfirmButton").objectReferenceValue = originalSettingsSerializer.FindProperty("resetConfirmButton").objectReferenceValue;
            settingsSerializer.FindProperty("resetCancelButton").objectReferenceValue = originalSettingsSerializer.FindProperty("resetCancelButton").objectReferenceValue;
            settingsSerializer.FindProperty("closeButton").objectReferenceValue = originalSettingsSerializer.FindProperty("closeButton").objectReferenceValue;
            settingsSerializer.FindProperty("versionText").objectReferenceValue = originalSettingsSerializer.FindProperty("versionText").objectReferenceValue;
            settingsSerializer.ApplyModifiedProperties();

            Object.DestroyImmediate(settingsPopupView);

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
            var stageIds = GenerateStageIds();
            var buttons = new UI_StageButtonView[stageIds.Length];

            var scrollViewObject = new GameObject("StageScrollView");
            scrollViewObject.transform.SetParent(parent, false);
            var scrollViewRect = scrollViewObject.AddComponent<RectTransform>();
            scrollViewRect.anchorMin = new Vector2(0.5f, 0.5f);
            scrollViewRect.anchorMax = new Vector2(0.5f, 0.5f);
            scrollViewRect.pivot = new Vector2(0.5f, 0.5f);
            scrollViewRect.anchoredPosition = Vector2.zero;
            scrollViewRect.sizeDelta = new Vector2(1600, 700);

            var scrollRect = scrollViewObject.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;

            var viewportObject = new GameObject("Viewport");
            viewportObject.transform.SetParent(scrollViewObject.transform, false);
            var viewportRect = viewportObject.AddComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;
            var mask = viewportObject.AddComponent<Mask>();
            mask.showMaskGraphic = false;
            var maskImage = viewportObject.AddComponent<Image>();
            maskImage.color = new Color(1, 1, 1, 0.01f);

            var contentObject = new GameObject("Content");
            contentObject.transform.SetParent(viewportObject.transform, false);
            var contentRect = contentObject.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0.5f, 1);
            contentRect.anchorMax = new Vector2(0.5f, 1);
            contentRect.pivot = new Vector2(0.5f, 1);

            scrollRect.viewport = viewportRect;
            scrollRect.content = contentRect;

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

            const int columns = 5;
            const float buttonWidth = 300f;
            const float buttonHeight = 110f;
            const float spacingX = 320f;
            const float spacingY = 130f;

            var rows = (stageIds.Length + columns - 1) / columns;
            var contentHeight = rows * spacingY + 50f;
            contentRect.sizeDelta = new Vector2(1600, contentHeight);

            for (var i = 0; i < stageIds.Length; i++)
            {
                var col = i % columns;
                var row = i / columns;
                var x = (col - (columns - 1) * 0.5f) * spacingX;
                var y = -50f - row * spacingY;
                var stageName = $"STAGE {i + 1}";

                buttons[i] = CreateStageButton(contentRect, new Vector2(x, y), new Vector2(buttonWidth, buttonHeight), stageIds[i], stageName, stageButtonPrefabPath, font, starIcon, lockIcon);
            }

            return buttons;
        }

        private static UI_StageButtonView CreateStageButton(RectTransform parent, Vector2 position, Vector2 size, string stageId, string stageName, string buttonPrefabPath, TMP_FontAsset font, Sprite starIcon, Sprite lockIcon)
        {
            var buttonInstance = InstantiateKitPrefab(buttonPrefabPath, parent);

            if (buttonInstance == null)
            {
                GameLogger.LogError($"[UIPrefabBuilder] Failed to create button for {stageId}.");
                return null;
            }

            buttonInstance.name = $"StageButton_{stageId}";
            var buttonRect = buttonInstance.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.5f, 1);
            buttonRect.anchorMax = new Vector2(0.5f, 1);
            buttonRect.pivot = new Vector2(0.5f, 1);
            buttonRect.anchoredPosition = position;
            buttonRect.sizeDelta = size;

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

        private static TMP_Text CreateTopGoldBar(RectTransform parent, Sprite goldIcon)
        {
            var resourceBarInstance = InstantiateKitPrefab(ResourceBarGroupPath, parent);

            if (resourceBarInstance == null)
            {
                GameLogger.LogWarning("[UIPrefabBuilder] ResourceBar_Group not found for gold bar.");
                return null;
            }

            resourceBarInstance.name = "TopGoldBar";
            var resourceBarRect = resourceBarInstance.GetComponent<RectTransform>();
            resourceBarRect.anchorMin = new Vector2(0, 1);
            resourceBarRect.anchorMax = new Vector2(0, 1);
            resourceBarRect.pivot = new Vector2(0, 1);
            resourceBarRect.anchoredPosition = new Vector2(40, -40);

            var firstSlot = resourceBarInstance.transform.GetChild(0);
            var iconTransform = firstSlot.Find("Icon");

            if (iconTransform != null && goldIcon != null)
            {
                var iconImage = iconTransform.GetComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.sprite = goldIcon;
                }
            }

            var textTMP = firstSlot.GetComponentInChildren<TMP_Text>();
            if (textTMP != null)
            {
                textTMP.text = "0";
            }

            for (var i = 1; i < resourceBarInstance.transform.childCount; i++)
            {
                resourceBarInstance.transform.GetChild(i).gameObject.SetActive(false);
            }

            return textTMP;
        }

        private static Button[] CreateTabButtons(RectTransform parent, TMP_FontAsset font)
        {
            var buttons = new Button[4];
            var labels = new[] { "STAGES", "DECK", "UPGRADE", "MISSIONS" };
            var xPositions = new[] { -330f, -110f, 110f, 330f };

            for (var i = 0; i < 4; i++)
            {
                var buttonInstance = InstantiateKitPrefab(ButtonBluePath, parent);
                if (buttonInstance == null)
                {
                    GameLogger.LogError($"[UIPrefabBuilder] Failed to create tab button {i}");
                    continue;
                }

                buttonInstance.name = $"TabButton_{labels[i]}";
                var buttonRect = buttonInstance.GetComponent<RectTransform>();
                buttonRect.anchorMin = new Vector2(0.5f, 1);
                buttonRect.anchorMax = new Vector2(0.5f, 1);
                buttonRect.pivot = new Vector2(0.5f, 1);
                buttonRect.anchoredPosition = new Vector2(xPositions[i], -140);
                buttonRect.sizeDelta = new Vector2(200, 70);

                var button = EnsureButtonComponent(buttonInstance);
                buttons[i] = button;

                var labelTMP = buttonInstance.GetComponentInChildren<TMP_Text>();
                if (labelTMP != null)
                {
                    labelTMP.text = labels[i];
                    labelTMP.fontSize = 28;
                    labelTMP.alignment = TextAlignmentOptions.Center;
                    labelTMP.font = font;
                }
            }

            return buttons;
        }

        private static GameObject CreateStagePanel(RectTransform parent, TMP_FontAsset font, Sprite starIcon, Sprite lockIcon)
        {
            var panelObject = new GameObject("StagePanel");
            panelObject.transform.SetParent(parent, false);

            var panelRect = panelObject.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            CreateStageTitle(panelRect, font);
            var stageButtons = CreateStageButtons(panelRect, font, starIcon, lockIcon);

            var stageSelectView = panelObject.AddComponent<UI_StageSelectView>();
            var serializedObject = new SerializedObject(stageSelectView);
            serializedObject.FindProperty("stageButtons").arraySize = stageButtons.Length;
            for (var i = 0; i < stageButtons.Length; i++)
            {
                serializedObject.FindProperty("stageButtons").GetArrayElementAtIndex(i).objectReferenceValue = stageButtons[i];
            }
            serializedObject.ApplyModifiedProperties();

            return panelObject;
        }

        private static GameObject CreateDeckPanel(RectTransform parent, TMP_FontAsset font, Sprite starIcon)
        {
            var panelObject = new GameObject("DeckPanel");
            panelObject.transform.SetParent(parent, false);

            var panelRect = panelObject.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            var cards = new UI_UnitCardView[15];
            const int columns = 5;
            const int rows = 3;
            const float cardWidth = 170f;
            const float cardHeight = 210f;
            const float spacingX = 190f;
            const float spacingY = 230f;
            const float startY = 200f;

            for (var i = 0; i < 15; i++)
            {
                var col = i % columns;
                var row = i / columns;
                var x = (col - (columns - 1) * 0.5f) * spacingX;
                var y = startY - row * spacingY;

                cards[i] = CreateUnitCard(panelRect, new Vector2(x, y), new Vector2(cardWidth, cardHeight), font, starIcon, $"DeckCard_{i}");
            }

            var deckCountTextObject = new GameObject("DeckCountText");
            deckCountTextObject.transform.SetParent(panelObject.transform, false);
            var deckCountRect = deckCountTextObject.AddComponent<RectTransform>();
            deckCountRect.anchorMin = new Vector2(0.5f, 0);
            deckCountRect.anchorMax = new Vector2(0.5f, 0);
            deckCountRect.pivot = new Vector2(0.5f, 0);
            deckCountRect.anchoredPosition = new Vector2(0, 120);
            deckCountRect.sizeDelta = new Vector2(200, 50);

            var deckCountText = deckCountTextObject.AddComponent<TextMeshProUGUI>();
            deckCountText.text = "10/10";
            deckCountText.fontSize = 36;
            deckCountText.alignment = TextAlignmentOptions.Center;
            deckCountText.font = font;

            var confirmButton = CreatePausePopupButton(panelObject.transform, "ConfirmButton", new Vector2(0, 50), "CONFIRM", ButtonGreenPath, font);

            var deckPanelView = panelObject.AddComponent<UI_DeckPanelView>();
            var serializedObject = new SerializedObject(deckPanelView);
            serializedObject.FindProperty("root").objectReferenceValue = panelObject;
            serializedObject.FindProperty("unitCards").arraySize = cards.Length;
            for (var i = 0; i < cards.Length; i++)
            {
                serializedObject.FindProperty("unitCards").GetArrayElementAtIndex(i).objectReferenceValue = cards[i];
            }
            serializedObject.FindProperty("deckCountText").objectReferenceValue = deckCountText;
            serializedObject.FindProperty("confirmButton").objectReferenceValue = confirmButton;
            serializedObject.ApplyModifiedProperties();

            panelObject.SetActive(false);

            return panelObject;
        }

        private static GameObject CreateUpgradePanel(RectTransform parent, TMP_FontAsset font, Sprite starIcon, Sprite goldIcon, Sprite arrowIcon)
        {
            var panelObject = new GameObject("UpgradePanel");
            panelObject.transform.SetParent(parent, false);

            var panelRect = panelObject.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            var cards = new UI_UnitCardView[15];
            const int columns = 5;
            const int rows = 3;
            const float cardWidth = 140f;
            const float cardHeight = 170f;
            const float spacingX = 155f;
            const float spacingY = 190f;
            const float startX = -320f;
            const float startY = 200f;

            for (var i = 0; i < 15; i++)
            {
                var col = i % columns;
                var row = i / columns;
                var x = startX + col * spacingX;
                var y = startY - row * spacingY;

                cards[i] = CreateUnitCard(panelRect, new Vector2(x, y), new Vector2(cardWidth, cardHeight), font, starIcon, $"UpgradeCard_{i}");
            }

            var detailPanel = CreateUpgradeDetailPanel(panelRect, font, goldIcon, arrowIcon);

            var detailNameText = detailPanel.transform.Find("DetailNameText")?.GetComponent<TMP_Text>();
            var levelText = detailPanel.transform.Find("LevelText")?.GetComponent<TMP_Text>();
            var hpText = detailPanel.transform.Find("HPText")?.GetComponent<TMP_Text>();
            var attackText = detailPanel.transform.Find("AttackText")?.GetComponent<TMP_Text>();
            var costText = detailPanel.transform.Find("CostText")?.GetComponent<TMP_Text>();
            var upgradeButton = detailPanel.transform.Find("UpgradeButton")?.GetComponent<Button>();
            var maxLevelMark = detailPanel.transform.Find("MaxLevelMark")?.gameObject;

            var upgradePanelView = panelObject.AddComponent<UI_UpgradePanelView>();
            var serializedObject = new SerializedObject(upgradePanelView);
            serializedObject.FindProperty("root").objectReferenceValue = panelObject;
            serializedObject.FindProperty("unitCards").arraySize = cards.Length;
            for (var i = 0; i < cards.Length; i++)
            {
                serializedObject.FindProperty("unitCards").GetArrayElementAtIndex(i).objectReferenceValue = cards[i];
            }
            serializedObject.FindProperty("detailNameText").objectReferenceValue = detailNameText;
            serializedObject.FindProperty("levelText").objectReferenceValue = levelText;
            serializedObject.FindProperty("hpText").objectReferenceValue = hpText;
            serializedObject.FindProperty("attackText").objectReferenceValue = attackText;
            serializedObject.FindProperty("costText").objectReferenceValue = costText;
            serializedObject.FindProperty("upgradeButton").objectReferenceValue = upgradeButton;
            serializedObject.FindProperty("maxLevelMark").objectReferenceValue = maxLevelMark;
            serializedObject.ApplyModifiedProperties();

            panelObject.SetActive(false);

            return panelObject;
        }

        private static UI_UnitCardView CreateUnitCard(RectTransform parent, Vector2 position, Vector2 size, TMP_FontAsset font, Sprite starIcon, string cardName)
        {
            var cardInstance = InstantiateKitPrefab(CardFrameBluePath, parent);

            if (cardInstance == null)
            {
                GameLogger.LogError($"[UIPrefabBuilder] Failed to create unit card: {cardName}");
                return null;
            }

            cardInstance.name = cardName;
            var cardRect = cardInstance.GetComponent<RectTransform>();
            cardRect.anchorMin = new Vector2(0.5f, 0.5f);
            cardRect.anchorMax = new Vector2(0.5f, 0.5f);
            cardRect.pivot = new Vector2(0.5f, 0.5f);
            cardRect.anchoredPosition = position;
            cardRect.sizeDelta = size;

            var button = EnsureButtonComponent(cardInstance);

            var iconObject = new GameObject("UnitIcon");
            iconObject.transform.SetParent(cardInstance.transform, false);
            var iconRect = iconObject.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.5f, 1);
            iconRect.anchorMax = new Vector2(0.5f, 1);
            iconRect.pivot = new Vector2(0.5f, 1);
            iconRect.anchoredPosition = new Vector2(0, -10);
            iconRect.sizeDelta = new Vector2(size.x * 0.6f, size.x * 0.6f);

            var iconImage = iconObject.AddComponent<Image>();
            iconImage.raycastTarget = false;

            var nameTextObject = new GameObject("NameText");
            nameTextObject.transform.SetParent(cardInstance.transform, false);
            var nameTextRect = nameTextObject.AddComponent<RectTransform>();
            nameTextRect.anchorMin = new Vector2(0, 0);
            nameTextRect.anchorMax = new Vector2(1, 0);
            nameTextRect.pivot = new Vector2(0.5f, 0);
            nameTextRect.anchoredPosition = new Vector2(0, 35);
            nameTextRect.sizeDelta = new Vector2(-10, 30);

            var nameText = nameTextObject.AddComponent<TextMeshProUGUI>();
            nameText.text = "Unit";
            nameText.fontSize = 20;
            nameText.alignment = TextAlignmentOptions.Center;
            nameText.font = font;

            var levelTextObject = new GameObject("LevelText");
            levelTextObject.transform.SetParent(cardInstance.transform, false);
            var levelTextRect = levelTextObject.AddComponent<RectTransform>();
            levelTextRect.anchorMin = new Vector2(0, 0);
            levelTextRect.anchorMax = new Vector2(1, 0);
            levelTextRect.pivot = new Vector2(0.5f, 0);
            levelTextRect.anchoredPosition = new Vector2(0, 5);
            levelTextRect.sizeDelta = new Vector2(-10, 25);

            var levelText = levelTextObject.AddComponent<TextMeshProUGUI>();
            levelText.text = "Lv.1";
            levelText.fontSize = 18;
            levelText.alignment = TextAlignmentOptions.Center;
            levelText.font = font;

            var selectedMarkObject = new GameObject("SelectedMark");
            selectedMarkObject.transform.SetParent(cardInstance.transform, false);
            var selectedMarkRect = selectedMarkObject.AddComponent<RectTransform>();
            selectedMarkRect.anchorMin = new Vector2(1, 1);
            selectedMarkRect.anchorMax = new Vector2(1, 1);
            selectedMarkRect.pivot = new Vector2(1, 1);
            selectedMarkRect.anchoredPosition = new Vector2(-5, -5);
            selectedMarkRect.sizeDelta = new Vector2(40, 40);

            var selectedMarkImage = selectedMarkObject.AddComponent<Image>();
            selectedMarkImage.sprite = starIcon;
            selectedMarkImage.raycastTarget = false;
            selectedMarkObject.SetActive(false);

            var dimOverlayObject = new GameObject("DimOverlay");
            dimOverlayObject.transform.SetParent(cardInstance.transform, false);
            var dimOverlayRect = dimOverlayObject.AddComponent<RectTransform>();
            dimOverlayRect.anchorMin = Vector2.zero;
            dimOverlayRect.anchorMax = Vector2.one;
            dimOverlayRect.offsetMin = Vector2.zero;
            dimOverlayRect.offsetMax = Vector2.zero;

            var dimOverlayImage = dimOverlayObject.AddComponent<Image>();
            dimOverlayImage.color = new Color(0, 0, 0, 0.5f);
            dimOverlayImage.raycastTarget = false;
            dimOverlayObject.SetActive(false);

            var cardView = cardInstance.AddComponent<UI_UnitCardView>();
            var serializedObject = new SerializedObject(cardView);
            serializedObject.FindProperty("button").objectReferenceValue = button;
            serializedObject.FindProperty("iconImage").objectReferenceValue = iconImage;
            serializedObject.FindProperty("nameText").objectReferenceValue = nameText;
            serializedObject.FindProperty("levelText").objectReferenceValue = levelText;
            serializedObject.FindProperty("selectedMark").objectReferenceValue = selectedMarkObject;
            serializedObject.FindProperty("dimOverlay").objectReferenceValue = dimOverlayObject;
            serializedObject.FindProperty("unitId").stringValue = "";
            serializedObject.ApplyModifiedProperties();

            return cardView;
        }

        private static GameObject CreateUpgradeDetailPanel(RectTransform parent, TMP_FontAsset font, Sprite goldIcon, Sprite arrowIcon)
        {
            var detailPanel = new GameObject("DetailPanel");
            detailPanel.transform.SetParent(parent, false);

            var detailRect = detailPanel.AddComponent<RectTransform>();
            detailRect.anchorMin = new Vector2(1, 0.5f);
            detailRect.anchorMax = new Vector2(1, 0.5f);
            detailRect.pivot = new Vector2(1, 0.5f);
            detailRect.anchoredPosition = new Vector2(-50, 0);
            detailRect.sizeDelta = new Vector2(400, 600);

            var detailNameTextObject = new GameObject("DetailNameText");
            detailNameTextObject.transform.SetParent(detailPanel.transform, false);
            var detailNameTextRect = detailNameTextObject.AddComponent<RectTransform>();
            detailNameTextRect.anchorMin = new Vector2(0.5f, 1);
            detailNameTextRect.anchorMax = new Vector2(0.5f, 1);
            detailNameTextRect.pivot = new Vector2(0.5f, 1);
            detailNameTextRect.anchoredPosition = new Vector2(0, -20);
            detailNameTextRect.sizeDelta = new Vector2(380, 50);

            var detailNameText = detailNameTextObject.AddComponent<TextMeshProUGUI>();
            detailNameText.text = "Unit Name";
            detailNameText.fontSize = 36;
            detailNameText.alignment = TextAlignmentOptions.Center;
            detailNameText.font = font;

            var levelTextObject = new GameObject("LevelText");
            levelTextObject.transform.SetParent(detailPanel.transform, false);
            var levelTextRect = levelTextObject.AddComponent<RectTransform>();
            levelTextRect.anchorMin = new Vector2(0.5f, 1);
            levelTextRect.anchorMax = new Vector2(0.5f, 1);
            levelTextRect.pivot = new Vector2(0.5f, 1);
            levelTextRect.anchoredPosition = new Vector2(0, -80);
            levelTextRect.sizeDelta = new Vector2(380, 40);

            var levelText = levelTextObject.AddComponent<TextMeshProUGUI>();
            levelText.text = "Lv.1";
            levelText.fontSize = 28;
            levelText.alignment = TextAlignmentOptions.Center;
            levelText.font = font;

            var hpTextObject = new GameObject("HPText");
            hpTextObject.transform.SetParent(detailPanel.transform, false);
            var hpTextRect = hpTextObject.AddComponent<RectTransform>();
            hpTextRect.anchorMin = new Vector2(0.5f, 0.5f);
            hpTextRect.anchorMax = new Vector2(0.5f, 0.5f);
            hpTextRect.pivot = new Vector2(0.5f, 0.5f);
            hpTextRect.anchoredPosition = new Vector2(0, 80);
            hpTextRect.sizeDelta = new Vector2(380, 40);

            var hpText = hpTextObject.AddComponent<TextMeshProUGUI>();
            hpText.text = "HP: 100 -> 120";
            hpText.fontSize = 24;
            hpText.alignment = TextAlignmentOptions.Center;
            hpText.font = font;

            var attackTextObject = new GameObject("AttackText");
            attackTextObject.transform.SetParent(detailPanel.transform, false);
            var attackTextRect = attackTextObject.AddComponent<RectTransform>();
            attackTextRect.anchorMin = new Vector2(0.5f, 0.5f);
            attackTextRect.anchorMax = new Vector2(0.5f, 0.5f);
            attackTextRect.pivot = new Vector2(0.5f, 0.5f);
            attackTextRect.anchoredPosition = new Vector2(0, 30);
            attackTextRect.sizeDelta = new Vector2(380, 40);

            var attackText = attackTextObject.AddComponent<TextMeshProUGUI>();
            attackText.text = "ATK: 10 -> 12";
            attackText.fontSize = 24;
            attackText.alignment = TextAlignmentOptions.Center;
            attackText.font = font;

            var costTextObject = new GameObject("CostText");
            costTextObject.transform.SetParent(detailPanel.transform, false);
            var costTextRect = costTextObject.AddComponent<RectTransform>();
            costTextRect.anchorMin = new Vector2(0.5f, 0);
            costTextRect.anchorMax = new Vector2(0.5f, 0);
            costTextRect.pivot = new Vector2(0.5f, 0);
            costTextRect.anchoredPosition = new Vector2(40, 160);
            costTextRect.sizeDelta = new Vector2(100, 40);

            var costText = costTextObject.AddComponent<TextMeshProUGUI>();
            costText.text = "100";
            costText.fontSize = 28;
            costText.alignment = TextAlignmentOptions.Left;
            costText.font = font;

            var goldIconObject = new GameObject("GoldIcon");
            goldIconObject.transform.SetParent(detailPanel.transform, false);
            var goldIconRect = goldIconObject.AddComponent<RectTransform>();
            goldIconRect.anchorMin = new Vector2(0.5f, 0);
            goldIconRect.anchorMax = new Vector2(0.5f, 0);
            goldIconRect.pivot = new Vector2(0.5f, 0);
            goldIconRect.anchoredPosition = new Vector2(-20, 165);
            goldIconRect.sizeDelta = new Vector2(30, 30);

            var goldIconImage = goldIconObject.AddComponent<Image>();
            goldIconImage.sprite = goldIcon;
            goldIconImage.raycastTarget = false;

            var upgradeButton = CreatePausePopupButton(detailPanel.transform, "UpgradeButton", new Vector2(0, 100), "UPGRADE", ButtonGreenPath, font);

            var maxLevelMarkObject = new GameObject("MaxLevelMark");
            maxLevelMarkObject.transform.SetParent(detailPanel.transform, false);
            var maxLevelMarkRect = maxLevelMarkObject.AddComponent<RectTransform>();
            maxLevelMarkRect.anchorMin = new Vector2(0.5f, 0);
            maxLevelMarkRect.anchorMax = new Vector2(0.5f, 0);
            maxLevelMarkRect.pivot = new Vector2(0.5f, 0);
            maxLevelMarkRect.anchoredPosition = new Vector2(0, 100);
            maxLevelMarkRect.sizeDelta = new Vector2(300, 60);

            var maxLevelMarkText = maxLevelMarkObject.AddComponent<TextMeshProUGUI>();
            maxLevelMarkText.text = "MAX LEVEL";
            maxLevelMarkText.fontSize = 32;
            maxLevelMarkText.alignment = TextAlignmentOptions.Center;
            maxLevelMarkText.font = font;
            maxLevelMarkText.color = new Color(1f, 0.8f, 0.2f);
            maxLevelMarkObject.SetActive(false);

            return detailPanel;
        }

        private static GameObject CreateMissionPanel(RectTransform parent, TMP_FontAsset font, Sprite goldIcon)
        {
            var panelObject = new GameObject("MissionPanel");
            panelObject.transform.SetParent(parent, false);

            var panelRect = panelObject.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            var rows = new UI_MissionRowView[10];
            const float rowHeight = 80f;
            const float rowWidth = 900f;
            const float spacingY = 90f;
            const float startY = 300f;

            for (var i = 0; i < 10; i++)
            {
                var y = startY - i * spacingY;
                rows[i] = CreateMissionRow(panelRect, new Vector2(0, y), new Vector2(rowWidth, rowHeight), font, goldIcon, i);
            }

            var missionPanelView = panelObject.AddComponent<UI_MissionPanelView>();
            var serializedObject = new SerializedObject(missionPanelView);
            serializedObject.FindProperty("root").objectReferenceValue = panelObject;
            serializedObject.FindProperty("missionRows").arraySize = rows.Length;
            for (var i = 0; i < rows.Length; i++)
            {
                serializedObject.FindProperty("missionRows").GetArrayElementAtIndex(i).objectReferenceValue = rows[i];
            }
            serializedObject.ApplyModifiedProperties();

            panelObject.SetActive(false);

            return panelObject;
        }

        private static UI_MissionRowView CreateMissionRow(RectTransform parent, Vector2 position, Vector2 size, TMP_FontAsset font, Sprite goldIcon, int index)
        {
            var rowObject = new GameObject($"MissionRow_{index}");
            rowObject.transform.SetParent(parent, false);

            var rowRect = rowObject.AddComponent<RectTransform>();
            rowRect.anchorMin = new Vector2(0.5f, 0.5f);
            rowRect.anchorMax = new Vector2(0.5f, 0.5f);
            rowRect.pivot = new Vector2(0.5f, 0.5f);
            rowRect.anchoredPosition = position;
            rowRect.sizeDelta = size;

            var frameInstance = InstantiateKitPrefab(CardFrameBluePath, rowObject.transform);
            if (frameInstance != null)
            {
                var frameRect = frameInstance.GetComponent<RectTransform>();
                frameRect.anchorMin = Vector2.zero;
                frameRect.anchorMax = Vector2.one;
                frameRect.offsetMin = Vector2.zero;
                frameRect.offsetMax = Vector2.zero;
            }

            var descriptionTextObject = new GameObject("DescriptionText");
            descriptionTextObject.transform.SetParent(rowObject.transform, false);
            var descriptionTextRect = descriptionTextObject.AddComponent<RectTransform>();
            descriptionTextRect.anchorMin = new Vector2(0, 0.5f);
            descriptionTextRect.anchorMax = new Vector2(0, 0.5f);
            descriptionTextRect.pivot = new Vector2(0, 0.5f);
            descriptionTextRect.anchoredPosition = new Vector2(20, 0);
            descriptionTextRect.sizeDelta = new Vector2(400, 60);

            var descriptionText = descriptionTextObject.AddComponent<TextMeshProUGUI>();
            descriptionText.text = "Mission Description";
            descriptionText.fontSize = 24;
            descriptionText.alignment = TextAlignmentOptions.Left;
            descriptionText.font = font;

            var progressTextObject = new GameObject("ProgressText");
            progressTextObject.transform.SetParent(rowObject.transform, false);
            var progressTextRect = progressTextObject.AddComponent<RectTransform>();
            progressTextRect.anchorMin = new Vector2(0.5f, 0.5f);
            progressTextRect.anchorMax = new Vector2(0.5f, 0.5f);
            progressTextRect.pivot = new Vector2(0.5f, 0.5f);
            progressTextRect.anchoredPosition = new Vector2(100, 0);
            progressTextRect.sizeDelta = new Vector2(100, 40);

            var progressText = progressTextObject.AddComponent<TextMeshProUGUI>();
            progressText.text = "0/10";
            progressText.fontSize = 22;
            progressText.alignment = TextAlignmentOptions.Center;
            progressText.font = font;

            var claimButtonInstance = InstantiateKitPrefab(ButtonGreenPath, rowObject.transform);
            if (claimButtonInstance != null)
            {
                claimButtonInstance.name = "ClaimButton";
                var claimButtonRect = claimButtonInstance.GetComponent<RectTransform>();
                claimButtonRect.anchorMin = new Vector2(1, 0.5f);
                claimButtonRect.anchorMax = new Vector2(1, 0.5f);
                claimButtonRect.pivot = new Vector2(1, 0.5f);
                claimButtonRect.anchoredPosition = new Vector2(-20, 0);
                claimButtonRect.sizeDelta = new Vector2(120, 60);

                var claimButton = EnsureButtonComponent(claimButtonInstance);
                var claimButtonText = claimButtonInstance.GetComponentInChildren<TMP_Text>();
                if (claimButtonText != null)
                {
                    claimButtonText.text = "CLAIM";
                    claimButtonText.fontSize = 22;
                    claimButtonText.alignment = TextAlignmentOptions.Center;
                    claimButtonText.font = font;
                }
            }

            var claimedMarkObject = new GameObject("ClaimedMark");
            claimedMarkObject.transform.SetParent(rowObject.transform, false);
            var claimedMarkRect = claimedMarkObject.AddComponent<RectTransform>();
            claimedMarkRect.anchorMin = new Vector2(1, 0.5f);
            claimedMarkRect.anchorMax = new Vector2(1, 0.5f);
            claimedMarkRect.pivot = new Vector2(1, 0.5f);
            claimedMarkRect.anchoredPosition = new Vector2(-60, 0);
            claimedMarkRect.sizeDelta = new Vector2(100, 40);

            var claimedMarkText = claimedMarkObject.AddComponent<TextMeshProUGUI>();
            claimedMarkText.text = "CLAIMED";
            claimedMarkText.fontSize = 22;
            claimedMarkText.alignment = TextAlignmentOptions.Center;
            claimedMarkText.font = font;
            claimedMarkText.color = new Color(0.3f, 0.8f, 0.3f);
            claimedMarkObject.SetActive(false);

            var rowView = rowObject.AddComponent<UI_MissionRowView>();
            var serializedObject = new SerializedObject(rowView);
            serializedObject.FindProperty("descriptionText").objectReferenceValue = descriptionText;
            serializedObject.FindProperty("progressText").objectReferenceValue = progressText;
            serializedObject.FindProperty("claimButton").objectReferenceValue = claimButtonInstance?.GetComponent<Button>();
            serializedObject.FindProperty("claimedMark").objectReferenceValue = claimedMarkObject;
            serializedObject.ApplyModifiedProperties();

            return rowView;
        }

        private static GameObject CreateDailyRewardPopup(RectTransform parent, TMP_FontAsset font, Sprite goldIcon, Sprite starIcon)
        {
            var popupRoot = new GameObject("DailyRewardPopupRoot");
            popupRoot.transform.SetParent(parent, false);

            var rootRect = popupRoot.AddComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            var dimImage = popupRoot.AddComponent<Image>();
            dimImage.color = new Color(0, 0, 0, 0.7f);
            dimImage.raycastTarget = true;

            var popupInstance = InstantiateKitPrefab(PopupPath, popupRoot.transform);
            if (popupInstance != null)
            {
                popupInstance.name = "PopupBox";
                var popupRect = popupInstance.GetComponent<RectTransform>();
                popupRect.anchorMin = new Vector2(0.5f, 0.5f);
                popupRect.anchorMax = new Vector2(0.5f, 0.5f);
                popupRect.pivot = new Vector2(0.5f, 0.5f);
                popupRect.anchoredPosition = Vector2.zero;
                popupRect.sizeDelta = new Vector2(900, 750);
            }

            var titleTextObject = new GameObject("TitleText");
            titleTextObject.transform.SetParent(popupRoot.transform, false);
            var titleTextRect = titleTextObject.AddComponent<RectTransform>();
            titleTextRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleTextRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleTextRect.pivot = new Vector2(0.5f, 0.5f);
            titleTextRect.anchoredPosition = new Vector2(0, 300);
            titleTextRect.sizeDelta = new Vector2(700, 80);

            var titleText = titleTextObject.AddComponent<TextMeshProUGUI>();
            titleText.text = "DAILY REWARD";
            titleText.fontSize = 56;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.font = font;

            var dayTexts = new TMP_Text[7];
            var goldTexts = new TMP_Text[7];
            var claimedMarks = new GameObject[7];
            var todayHighlights = new GameObject[7];

            const float slotWidth = 250f;
            const float slotHeight = 150f;
            const float spacingX = 270f;
            const float spacingY = 170f;

            for (var i = 0; i < 7; i++)
            {
                var col = i % 3;
                var row = i / 3;
                var x = (col - 1f) * spacingX;
                var y = 150f - row * spacingY;

                var slotObject = new GameObject($"DaySlot_{i + 1}");
                slotObject.transform.SetParent(popupRoot.transform, false);
                var slotRect = slotObject.AddComponent<RectTransform>();
                slotRect.anchorMin = new Vector2(0.5f, 0.5f);
                slotRect.anchorMax = new Vector2(0.5f, 0.5f);
                slotRect.pivot = new Vector2(0.5f, 0.5f);
                slotRect.anchoredPosition = new Vector2(x, y);
                slotRect.sizeDelta = new Vector2(slotWidth, slotHeight);

                var cardInstance = InstantiateKitPrefab(CardFrameBluePath, slotObject.transform);
                if (cardInstance != null)
                {
                    var cardRect = cardInstance.GetComponent<RectTransform>();
                    cardRect.anchorMin = Vector2.zero;
                    cardRect.anchorMax = Vector2.one;
                    cardRect.offsetMin = Vector2.zero;
                    cardRect.offsetMax = Vector2.zero;
                }

                var dayTextObject = new GameObject("DayText");
                dayTextObject.transform.SetParent(slotObject.transform, false);
                var dayTextRect = dayTextObject.AddComponent<RectTransform>();
                dayTextRect.anchorMin = new Vector2(0.5f, 1);
                dayTextRect.anchorMax = new Vector2(0.5f, 1);
                dayTextRect.pivot = new Vector2(0.5f, 1);
                dayTextRect.anchoredPosition = new Vector2(0, -10);
                dayTextRect.sizeDelta = new Vector2(230, 40);

                dayTexts[i] = dayTextObject.AddComponent<TextMeshProUGUI>();
                dayTexts[i].text = $"Day {i + 1}";
                dayTexts[i].fontSize = 28;
                dayTexts[i].alignment = TextAlignmentOptions.Center;
                dayTexts[i].font = font;

                var goldTextObject = new GameObject("GoldText");
                goldTextObject.transform.SetParent(slotObject.transform, false);
                var goldTextRect = goldTextObject.AddComponent<RectTransform>();
                goldTextRect.anchorMin = new Vector2(0.5f, 0.5f);
                goldTextRect.anchorMax = new Vector2(0.5f, 0.5f);
                goldTextRect.pivot = new Vector2(0.5f, 0.5f);
                goldTextRect.anchoredPosition = new Vector2(0, -10);
                goldTextRect.sizeDelta = new Vector2(200, 50);

                goldTexts[i] = goldTextObject.AddComponent<TextMeshProUGUI>();
                goldTexts[i].text = "0";
                goldTexts[i].fontSize = 36;
                goldTexts[i].alignment = TextAlignmentOptions.Center;
                goldTexts[i].font = font;

                var claimedMarkObject = new GameObject("ClaimedMark");
                claimedMarkObject.transform.SetParent(slotObject.transform, false);
                var claimedMarkRect = claimedMarkObject.AddComponent<RectTransform>();
                claimedMarkRect.anchorMin = new Vector2(1, 1);
                claimedMarkRect.anchorMax = new Vector2(1, 1);
                claimedMarkRect.pivot = new Vector2(1, 1);
                claimedMarkRect.anchoredPosition = new Vector2(-10, -10);
                claimedMarkRect.sizeDelta = new Vector2(40, 40);

                var claimedMarkImage = claimedMarkObject.AddComponent<Image>();
                claimedMarkImage.sprite = starIcon;
                claimedMarkImage.raycastTarget = false;
                claimedMarks[i] = claimedMarkObject;
                claimedMarkObject.SetActive(false);

                var highlightObject = new GameObject("TodayHighlight");
                highlightObject.transform.SetParent(slotObject.transform, false);
                var highlightRect = highlightObject.AddComponent<RectTransform>();
                highlightRect.anchorMin = Vector2.zero;
                highlightRect.anchorMax = Vector2.one;
                highlightRect.offsetMin = Vector2.zero;
                highlightRect.offsetMax = Vector2.zero;

                var highlightImage = highlightObject.AddComponent<Image>();
                highlightImage.color = new Color(1f, 1f, 0.3f, 0.3f);
                highlightImage.raycastTarget = false;
                todayHighlights[i] = highlightObject;
                highlightObject.SetActive(false);
            }

            var claimButton = CreatePausePopupButton(popupRoot.transform, "ClaimButton", new Vector2(-100, -270), "CLAIM", ButtonGreenPath, font);
            var closeButton = CreatePausePopupButton(popupRoot.transform, "CloseButton", new Vector2(100, -270), "CLOSE", ButtonBluePath, font);

            var popupView = popupRoot.AddComponent<UI_DailyRewardPopupView>();
            var serializedObject = new SerializedObject(popupView);
            serializedObject.FindProperty("root").objectReferenceValue = popupRoot;
            serializedObject.FindProperty("dayTexts").arraySize = 7;
            serializedObject.FindProperty("goldTexts").arraySize = 7;
            serializedObject.FindProperty("claimedMarks").arraySize = 7;
            serializedObject.FindProperty("todayHighlights").arraySize = 7;
            for (var i = 0; i < 7; i++)
            {
                serializedObject.FindProperty("dayTexts").GetArrayElementAtIndex(i).objectReferenceValue = dayTexts[i];
                serializedObject.FindProperty("goldTexts").GetArrayElementAtIndex(i).objectReferenceValue = goldTexts[i];
                serializedObject.FindProperty("claimedMarks").GetArrayElementAtIndex(i).objectReferenceValue = claimedMarks[i];
                serializedObject.FindProperty("todayHighlights").GetArrayElementAtIndex(i).objectReferenceValue = todayHighlights[i];
            }
            serializedObject.FindProperty("claimButton").objectReferenceValue = claimButton;
            serializedObject.FindProperty("closeButton").objectReferenceValue = closeButton;
            serializedObject.ApplyModifiedProperties();

            popupRoot.SetActive(false);

            return popupRoot;
        }

        private static GameObject CreateSettingsPopup(RectTransform parent, TMP_FontAsset font)
        {
            var popupRoot = new GameObject("SettingsPopupRoot");
            popupRoot.transform.SetParent(parent, false);

            var rootRect = popupRoot.AddComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            var dimImage = popupRoot.AddComponent<Image>();
            dimImage.color = new Color(0, 0, 0, 0.7f);
            dimImage.raycastTarget = true;

            var popupInstance = InstantiateKitPrefab(PopupPath, popupRoot.transform);
            if (popupInstance != null)
            {
                popupInstance.name = "PopupBox";
                var popupRect = popupInstance.GetComponent<RectTransform>();
                popupRect.anchorMin = new Vector2(0.5f, 0.5f);
                popupRect.anchorMax = new Vector2(0.5f, 0.5f);
                popupRect.pivot = new Vector2(0.5f, 0.5f);
                popupRect.anchoredPosition = Vector2.zero;
                popupRect.sizeDelta = new Vector2(700, 600);
            }

            var titleTextObject = new GameObject("TitleText");
            titleTextObject.transform.SetParent(popupRoot.transform, false);
            var titleTextRect = titleTextObject.AddComponent<RectTransform>();
            titleTextRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleTextRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleTextRect.pivot = new Vector2(0.5f, 0.5f);
            titleTextRect.anchoredPosition = new Vector2(0, 220);
            titleTextRect.sizeDelta = new Vector2(600, 80);

            var titleText = titleTextObject.AddComponent<TextMeshProUGUI>();
            titleText.text = "SETTINGS";
            titleText.fontSize = 56;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.font = font;

            var togglePath = KitRoot + "Theme_Light/Prefabs/Prefabs_Control/Toggle_Check_02.prefab";
            var toggleInstance = InstantiateKitPrefab(togglePath, popupRoot.transform);
            Toggle vibrationToggle = null;

            if (toggleInstance != null)
            {
                toggleInstance.name = "VibrationToggle";
                var toggleRect = toggleInstance.GetComponent<RectTransform>();
                toggleRect.anchorMin = new Vector2(0.5f, 0.5f);
                toggleRect.anchorMax = new Vector2(0.5f, 0.5f);
                toggleRect.pivot = new Vector2(0.5f, 0.5f);
                toggleRect.anchoredPosition = new Vector2(-50, 80);
                toggleRect.sizeDelta = new Vector2(80, 80);

                vibrationToggle = toggleInstance.GetComponent<Toggle>();
                if (vibrationToggle == null)
                {
                    vibrationToggle = toggleInstance.AddComponent<Toggle>();
                    var checkmark = toggleInstance.transform.Find("Checkmark");
                    if (checkmark != null)
                    {
                        vibrationToggle.graphic = checkmark.GetComponent<Image>();
                    }
                }

                var vibrationLabelObject = new GameObject("VibrationLabel");
                vibrationLabelObject.transform.SetParent(popupRoot.transform, false);
                var vibrationLabelRect = vibrationLabelObject.AddComponent<RectTransform>();
                vibrationLabelRect.anchorMin = new Vector2(0.5f, 0.5f);
                vibrationLabelRect.anchorMax = new Vector2(0.5f, 0.5f);
                vibrationLabelRect.pivot = new Vector2(0, 0.5f);
                vibrationLabelRect.anchoredPosition = new Vector2(50, 80);
                vibrationLabelRect.sizeDelta = new Vector2(200, 50);

                var vibrationLabelText = vibrationLabelObject.AddComponent<TextMeshProUGUI>();
                vibrationLabelText.text = "Vibration";
                vibrationLabelText.fontSize = 32;
                vibrationLabelText.alignment = TextAlignmentOptions.Left;
                vibrationLabelText.font = font;
            }

            var resetButton = CreatePausePopupButton(popupRoot.transform, "ResetButton", new Vector2(0, -20), "RESET PROGRESS", ButtonBluePath, font);
            if (resetButton != null)
            {
                var resetButtonRect = resetButton.GetComponent<RectTransform>();
                resetButtonRect.sizeDelta = new Vector2(350, 80);
            }

            var resetConfirmRoot = new GameObject("ResetConfirmRoot");
            resetConfirmRoot.transform.SetParent(popupRoot.transform, false);
            var resetConfirmRect = resetConfirmRoot.AddComponent<RectTransform>();
            resetConfirmRect.anchorMin = new Vector2(0.5f, 0.5f);
            resetConfirmRect.anchorMax = new Vector2(0.5f, 0.5f);
            resetConfirmRect.pivot = new Vector2(0.5f, 0.5f);
            resetConfirmRect.anchoredPosition = new Vector2(0, -130);
            resetConfirmRect.sizeDelta = new Vector2(500, 100);

            var confirmTextObject = new GameObject("ConfirmText");
            confirmTextObject.transform.SetParent(resetConfirmRoot.transform, false);
            var confirmTextRect = confirmTextObject.AddComponent<RectTransform>();
            confirmTextRect.anchorMin = new Vector2(0.5f, 1);
            confirmTextRect.anchorMax = new Vector2(0.5f, 1);
            confirmTextRect.pivot = new Vector2(0.5f, 1);
            confirmTextRect.anchoredPosition = new Vector2(0, 0);
            confirmTextRect.sizeDelta = new Vector2(480, 40);

            var confirmText = confirmTextObject.AddComponent<TextMeshProUGUI>();
            confirmText.text = "Are you sure?";
            confirmText.fontSize = 28;
            confirmText.alignment = TextAlignmentOptions.Center;
            confirmText.font = font;
            confirmText.color = new Color(1f, 0.3f, 0.3f);

            var resetConfirmButton = CreatePausePopupButton(resetConfirmRoot.transform, "ResetConfirmButton", new Vector2(-80, -50), "YES", ButtonGreenPath, font);
            var resetCancelButton = CreatePausePopupButton(resetConfirmRoot.transform, "ResetCancelButton", new Vector2(80, -50), "NO", ButtonBluePath, font);

            if (resetConfirmButton != null)
            {
                var confirmBtnRect = resetConfirmButton.GetComponent<RectTransform>();
                confirmBtnRect.anchoredPosition = new Vector2(-80, -50);
                confirmBtnRect.sizeDelta = new Vector2(130, 70);
            }

            if (resetCancelButton != null)
            {
                var cancelBtnRect = resetCancelButton.GetComponent<RectTransform>();
                cancelBtnRect.anchoredPosition = new Vector2(80, -50);
                cancelBtnRect.sizeDelta = new Vector2(130, 70);
            }

            resetConfirmRoot.SetActive(false);

            var versionTextObject = new GameObject("VersionText");
            versionTextObject.transform.SetParent(popupRoot.transform, false);
            var versionTextRect = versionTextObject.AddComponent<RectTransform>();
            versionTextRect.anchorMin = new Vector2(0.5f, 0);
            versionTextRect.anchorMax = new Vector2(0.5f, 0);
            versionTextRect.pivot = new Vector2(0.5f, 0);
            versionTextRect.anchoredPosition = new Vector2(0, 80);
            versionTextRect.sizeDelta = new Vector2(300, 40);

            var versionText = versionTextObject.AddComponent<TextMeshProUGUI>();
            versionText.text = "v1.0.0";
            versionText.fontSize = 24;
            versionText.alignment = TextAlignmentOptions.Center;
            versionText.font = font;
            versionText.color = new Color(0.7f, 0.7f, 0.7f);

            var closeButton = CreatePausePopupButton(popupRoot.transform, "CloseButton", new Vector2(0, 20), "CLOSE", ButtonBluePath, font);

            var settingsView = popupRoot.AddComponent<UI_SettingsPopupView>();
            var serializedObject = new SerializedObject(settingsView);
            serializedObject.FindProperty("root").objectReferenceValue = popupRoot;
            serializedObject.FindProperty("vibrationToggle").objectReferenceValue = vibrationToggle;
            serializedObject.FindProperty("resetButton").objectReferenceValue = resetButton;
            serializedObject.FindProperty("resetConfirmRoot").objectReferenceValue = resetConfirmRoot;
            serializedObject.FindProperty("resetConfirmButton").objectReferenceValue = resetConfirmButton;
            serializedObject.FindProperty("resetCancelButton").objectReferenceValue = resetCancelButton;
            serializedObject.FindProperty("closeButton").objectReferenceValue = closeButton;
            serializedObject.FindProperty("versionText").objectReferenceValue = versionText;
            serializedObject.ApplyModifiedProperties();

            popupRoot.SetActive(false);

            return popupRoot;
        }

        private static Button CreateSettingsButton(RectTransform parent, Sprite gearIcon)
        {
            var buttonInstance = InstantiateKitPrefab(ButtonCirclePath, parent);

            if (buttonInstance == null)
            {
                GameLogger.LogError("[UIPrefabBuilder] Failed to create SettingsButton.");
                return null;
            }

            buttonInstance.name = "SettingsButton";
            var buttonRect = buttonInstance.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(1, 1);
            buttonRect.anchorMax = new Vector2(1, 1);
            buttonRect.pivot = new Vector2(1, 1);
            buttonRect.anchoredPosition = new Vector2(-40, -40);
            buttonRect.sizeDelta = new Vector2(90, 90);

            var button = EnsureButtonComponent(buttonInstance);

            if (gearIcon != null)
            {
                var iconObject = new GameObject("GearIcon");
                iconObject.transform.SetParent(buttonInstance.transform, false);

                var iconRect = iconObject.AddComponent<RectTransform>();
                iconRect.anchorMin = new Vector2(0.5f, 0.5f);
                iconRect.anchorMax = new Vector2(0.5f, 0.5f);
                iconRect.pivot = new Vector2(0.5f, 0.5f);
                iconRect.anchoredPosition = Vector2.zero;
                iconRect.sizeDelta = new Vector2(50, 50);

                var iconImage = iconObject.AddComponent<Image>();
                iconImage.sprite = gearIcon;
                iconImage.raycastTarget = false;
            }

            return button;
        }
    }
}
