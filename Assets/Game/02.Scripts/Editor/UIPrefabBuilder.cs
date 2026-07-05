using System.Collections.Generic;
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
        private const string BossWarningPath = KitRoot + "Theme_Light/Prefabs/Prefabs~DemoScenes/Play_Warning_Boss.prefab";
        private const string TugOfWarBarPath = KitRoot + "Theme_Light/Prefabs/Prefabs_HUD/TugOfWarBar.prefab";
        private const string FontPath = KitRoot + "Shared/Font/LTAvocado-Bold SDF.asset";
        private const string IconCoinPath = KitRoot + "Shared/Icons/PictoIcon/128/coin_2.png";
        private const string IconLockPath = KitRoot + "Shared/Icons/PictoIcon/128/lock.png";
        private const string IconStarPath = KitRoot + "Shared/Icons/PictoIcon/128/star_1.png";
        private const string IconPausePath = KitRoot + "Shared/Icons/PictoIcon/128/pause_round.png";
        private const string IconGoldPath = KitRoot + "Theme_Light/Sprites/HUD/ResourceBar_Icon_Gold.png";
        private const string IconArrowUpPath = KitRoot + "Theme_Light/Sprites/Slider_Icon/Slider_Upgrade_01_Icon_Arrow_Up.png";
        private const string CardFrameBluePath = KitRoot + "Theme_Light/Prefabs/Prefabs_Frame/CardFrame/CardFrame_02_Blue.prefab";
        private const string PetIconRoot = "Assets/Layer Lab/2D Characters-PetPack1/Sprites/ImageSequence/";
        private const string ButtonOrangePath = KitRoot + "Theme_Light/Prefabs/Prefabs_Button/Button_03_Orange.prefab";
        private const string IconBattlePath = KitRoot + "Shared/Icons/PictoIcon/128/battle.png";
        private const string IconCardPath = KitRoot + "Shared/Icons/PictoIcon/128/card.png";
        private const string IconHammerPath = KitRoot + "Shared/Icons/PictoIcon/128/hammer_1.png";
        private const string IconCheckPath = KitRoot + "Shared/Icons/PictoIcon/128/check_round.png";
        private const string IconShopPath = KitRoot + "Shared/Icons/PictoIcon/128/shop.png";
        private const string IconGiftPath = KitRoot + "Shared/Icons/PictoIcon/128/gift.png";
        private const string AlertDotRedPath = KitRoot + "Theme_Light/Prefabs/Prefabs_HUD/Alert_Dot_01_Red.prefab";
        private const string ProfileFrameBluePath = KitRoot + "Theme_Light/Prefabs/Prefabs_Frame/ProfileFrame/ProfileFrame_01_l_Blue.prefab";

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
            var battleIcon = LoadSprite(IconBattlePath);
            var cardIcon = LoadSprite(IconCardPath);
            var hammerIcon = LoadSprite(IconHammerPath);
            var checkIcon = LoadSprite(IconCheckPath);
            var shopIcon = LoadSprite(IconShopPath);
            var giftIcon = LoadSprite(IconGiftPath);

            BuildGameHudPrefab(font, coinIcon, pauseIcon);
            BuildStageSelectScreenPrefab(font, starIcon, lockIcon, goldIcon, arrowIcon, battleIcon, cardIcon, hammerIcon, checkIcon, shopIcon, giftIcon);

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

        /// <summary>버튼의 "Icon" 자식(재귀)에 스프라이트를 적용 — 첫 Image가 이펙트·배경인 킷 버튼 대응.</summary>
        private static void SwapIconSprite(Transform button, Sprite sprite)
        {
            if (sprite == null)
            {
                return;
            }

            var iconTransform = button.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name == "Icon");
            var iconImage = iconTransform != null ? iconTransform.GetComponent<Image>() : button.GetComponentInChildren<Image>(true);

            if (iconImage == null)
            {
                GameLogger.LogWarning($"[UIPrefabBuilder] Icon image not found under {button.name}.");
                return;
            }

            iconImage.sprite = sprite;
        }

        private static bool HasAncestorNamed(Transform target, Transform stopAt, string nameFragment)
        {
            var current = target;
            while (current != null && current != stopAt)
            {
                if (current.name.Contains(nameFragment))
                {
                    return true;
                }
                current = current.parent;
            }
            return false;
        }

        private static void DisableDemoDescendants(GameObject root, string[] nameFragments)
        {
            var contractNames = new HashSet<string>
            {
                "UnitIcon", "NameText", "LevelText", "SelectedMark", "DimOverlay",
                "StageNameText", "ClearedMark", "LockedOverlay", "CloseButton",
                "ClaimButton", "DescriptionText", "ProgressText", "ClaimedMark",
                "BestStageText", "UnitCountText", "DeckTabButton", "UpgradeTabButton",
                "MissionsTabButton", "ShopTabButton", "PlayButton", "SettingsButton",
                "DailyRewardButton", "DailyRewardBadge", "ConfirmButton"
            };

            var allDescendants = root.GetComponentsInChildren<Transform>(true);
            foreach (var t in allDescendants)
            {
                if (t == root.transform)
                {
                    continue;
                }

                if (contractNames.Contains(t.name))
                {
                    continue;
                }

                foreach (var fragment in nameFragments)
                {
                    if (t.name.Contains(fragment))
                    {
                        t.gameObject.SetActive(false);
                        break;
                    }
                }
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

        private static void AddPanelBackgroundFrame(GameObject panelObject)
        {
            const string framePath = KitRoot + "Theme_Light/Prefabs/Prefabs_Popup/Popup_Box_02_DecoLine_Basic.prefab";
            var frameInstance = InstantiateKitPrefab(framePath, panelObject.transform);

            if (frameInstance == null)
            {
                return;
            }

            frameInstance.name = "BackgroundFrame";
            frameInstance.transform.SetSiblingIndex(0);

            var frameRect = frameInstance.GetComponent<RectTransform>();
            frameRect.anchorMin = Vector2.zero;
            frameRect.anchorMax = Vector2.one;
            frameRect.offsetMin = Vector2.zero;
            frameRect.offsetMax = Vector2.zero;

            foreach (Transform child in frameInstance.transform)
            {
                if (child.name.Contains("Text") || child.name.Contains("Button"))
                {
                    child.gameObject.SetActive(false);
                }
            }
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
            var (allyBaseHpFillImage, enemyBaseHpFillImage) = CreateTugOfWarHpBar(rootRect);
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

        private static (Image allyFill, Image enemyFill) CreateTugOfWarHpBar(RectTransform parent)
        {
            var tugOfWarInstance = InstantiateKitPrefab(TugOfWarBarPath, parent);

            if (tugOfWarInstance == null)
            {
                GameLogger.LogWarning("[UIPrefabBuilder] TugOfWarBar not found, using fallback.");
                return CreateTugOfWarHpBarFallback(parent);
            }

            tugOfWarInstance.name = "TugOfWarHpBar";
            var barRect = tugOfWarInstance.GetComponent<RectTransform>();
            barRect.anchorMin = new Vector2(0.5f, 1);
            barRect.anchorMax = new Vector2(0.5f, 1);
            barRect.pivot = new Vector2(0.5f, 1);
            barRect.anchoredPosition = new Vector2(0, -180);
            barRect.sizeDelta = new Vector2(700, 40);

            var allTexts = tugOfWarInstance.GetComponentsInChildren<TMP_Text>(true);
            foreach (var txt in allTexts)
            {
                txt.gameObject.SetActive(false);
            }

            var barLeft = tugOfWarInstance.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name.Contains("Bar_Left") || t.name.Contains("BarLeft") || t.name.Contains("Left"));
            var barRight = tugOfWarInstance.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name.Contains("Bar_Right") || t.name.Contains("BarRight") || t.name.Contains("Right"));

            Image allyFillImage = null;
            Image enemyFillImage = null;

            if (barLeft != null)
            {
                allyFillImage = barLeft.GetComponentInChildren<Image>(true);
                if (allyFillImage != null)
                {
                    allyFillImage.type = Image.Type.Filled;
                    allyFillImage.fillMethod = Image.FillMethod.Horizontal;
                    allyFillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
                    allyFillImage.fillAmount = 1f;
                }
            }

            if (barRight != null)
            {
                enemyFillImage = barRight.GetComponentInChildren<Image>(true);
                if (enemyFillImage != null)
                {
                    enemyFillImage.type = Image.Type.Filled;
                    enemyFillImage.fillMethod = Image.FillMethod.Horizontal;
                    enemyFillImage.fillOrigin = (int)Image.OriginHorizontal.Right;
                    enemyFillImage.fillAmount = 1f;
                }
            }

            if (allyFillImage == null || enemyFillImage == null)
            {
                GameLogger.LogWarning("[UIPrefabBuilder] TugOfWarBar fill images not found correctly, using fallback.");
                Object.DestroyImmediate(tugOfWarInstance);
                return CreateTugOfWarHpBarFallback(parent);
            }

            return (allyFillImage, enemyFillImage);
        }

        private static (Image allyFill, Image enemyFill) CreateTugOfWarHpBarFallback(RectTransform parent)
        {
            var allyFillImage = CreateHpBar(parent, "AllyHpBar", new Vector2(60, -200), new Vector2(0, 1), SliderBluePath);
            var enemyFillImage = CreateHpBar(parent, "EnemyHpBar", new Vector2(-60, -200), new Vector2(1, 1), SliderRedPath);
            return (allyFillImage, enemyFillImage);
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

            // 킷 슬라이더의 데모 수치 텍스트("9999/9999")는 미배선 상태이므로 비활성화 (UiAuditor)
            var demoValueText = sliderInstance.GetComponentsInChildren<TMP_Text>(true).FirstOrDefault();
            if (demoValueText != null)
            {
                demoValueText.gameObject.SetActive(false);
            }

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
            const float rowHeight = 140f;
            const float bottomRowY = 90f;

            for (var i = 0; i < UnitIds.Length; i++)
            {
                var column = i % columnCount;
                var row = i / columnCount;
                var x = (column - (columnCount - 1) * 0.5f) * spacing;
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

            var darkSquareBgs = buttonInstance.GetComponentsInChildren<Image>(true)
                .Where(img => img.gameObject != buttonInstance && img.color == new Color(0, 0, 0, 1) || (img.color.r < 0.2f && img.color.g < 0.2f && img.color.b < 0.2f && img.color.a > 0.8f))
                .ToArray();
            foreach (var bgImg in darkSquareBgs)
            {
                bgImg.gameObject.SetActive(false);
            }

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
            var bannerInstance = InstantiateKitPrefab(BossWarningPath, parent);

            if (bannerInstance == null)
            {
                GameLogger.LogWarning("[UIPrefabBuilder] Play_Warning_Boss not found, using fallback.");
                return CreateBossBannerFallback(parent, font);
            }

            bannerInstance.name = "BossBanner";
            var bannerRect = bannerInstance.GetComponent<RectTransform>();
            bannerRect.anchorMin = new Vector2(0.5f, 0.5f);
            bannerRect.anchorMax = new Vector2(0.5f, 0.5f);
            bannerRect.pivot = new Vector2(0.5f, 0.5f);
            bannerRect.anchoredPosition = new Vector2(0, -250);
            bannerRect.localScale = Vector3.one * 2.0f;

            var allTexts = bannerInstance.GetComponentsInChildren<TMP_Text>(true);
            TMP_Text bannerText = null;

            foreach (var txt in allTexts)
            {
                if (txt.name.Contains("Text") && txt.text.Contains("BOSS"))
                {
                    bannerText = txt;
                    bannerText.text = "BOSS INCOMING!";
                    bannerText.font = font;
                }
                else
                {
                    txt.gameObject.SetActive(false);
                }
            }

            if (bannerText == null)
            {
                GameLogger.LogWarning("[UIPrefabBuilder] BOSS text not found in Play_Warning_Boss template.");
            }

            var panelDimmed = bannerInstance.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name.Contains("PanelDimmed"))?.gameObject;
            if (panelDimmed != null)
            {
                var dimImage = panelDimmed.GetComponent<Image>();
                if (dimImage != null && dimImage.color.a > 0.5f)
                {
                    panelDimmed.SetActive(false);
                }
            }

            bannerInstance.SetActive(false);

            return bannerInstance;
        }

        private static GameObject CreateBossBannerFallback(RectTransform parent, TMP_FontAsset font)
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
            bannerRect.anchoredPosition = new Vector2(0, -250);
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

        private static void BuildStageSelectScreenPrefab(TMP_FontAsset font, Sprite starIcon, Sprite lockIcon, Sprite goldIcon, Sprite arrowIcon, Sprite battleIcon, Sprite cardIcon, Sprite hammerIcon, Sprite checkIcon, Sprite shopIcon, Sprite giftIcon)
        {
            var rootObject = new GameObject("StageSelectScreen");
            var rootRect = rootObject.AddComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            Button playButton;
            Button[] tabButtons;
            Button dailyRewardButton;
            Button settingsButton;
            TMP_Text goldText;
            GameObject dailyRewardBadge;
            GameObject missionsBadge;
            GameObject statusPanel;
            TMP_Text bestStageText;
            TMP_Text unitCountText;

            const string lobbyDefaultPath = "Assets/Layer Lab/GUI Pro-MinimalGame/Theme_Light/Prefabs/Prefabs~DemoScenes/Lobby_Default.prefab";
            var lobbyDefaultTemplate = AssetDatabase.LoadAssetAtPath<GameObject>(lobbyDefaultPath);

            if (lobbyDefaultTemplate != null && TryBuildHomeFromLobbyDefault(rootRect, lobbyDefaultTemplate, font, battleIcon, cardIcon, hammerIcon, checkIcon, shopIcon, giftIcon, goldIcon, out playButton, out tabButtons, out dailyRewardButton, out settingsButton, out goldText, out dailyRewardBadge, out missionsBadge, out statusPanel, out bestStageText, out unitCountText))
            {
                GameLogger.Log("[UIPrefabBuilder] Home UI built from Lobby_Default template.");
            }
            else
            {
                GameLogger.LogWarning("[UIPrefabBuilder] Lobby_Default template failed, falling back to legacy creation.");
                goldText = CreateTopGoldBar(rootRect, goldIcon);
                statusPanel = CreatePlayerStatusPanel(rootRect, font);
                bestStageText = statusPanel.GetComponentsInChildren<Transform>(true)
                    .FirstOrDefault(t => t.name == "BestStageText")?.GetComponent<TMP_Text>();
                unitCountText = statusPanel.GetComponentsInChildren<Transform>(true)
                    .FirstOrDefault(t => t.name == "UnitCountText")?.GetComponent<TMP_Text>();

                playButton = CreatePlayButton(rootRect, font, battleIcon);
                tabButtons = CreateBottomDockButtons(rootRect, font, cardIcon, hammerIcon, checkIcon, shopIcon);
                dailyRewardButton = CreateDailyRewardButton(rootRect, giftIcon);

                var gearIcon = LoadSprite(KitRoot + "Shared/Icons/PictoIcon/128/headgear.png");
                if (gearIcon == null)
                {
                    gearIcon = LoadSprite(KitRoot + "Shared/Icons/PictoIcon/128/menu_1.png");
                }
                settingsButton = CreateSettingsButton(rootRect, gearIcon);

                dailyRewardBadge = CreateAlertBadge(dailyRewardButton.transform, new Vector2(30, 30));
                missionsBadge = CreateAlertBadge(tabButtons[2].transform, new Vector2(30, 30));
            }

            var stagePanelRoot = CreateStagePanel(rootRect, font, starIcon, lockIcon);
            var deckPanel = CreateDeckPanel(rootRect, font, starIcon);
            var upgradePanel = CreateUpgradePanel(rootRect, font, starIcon, goldIcon, arrowIcon);
            var missionPanel = CreateMissionPanel(rootRect, font, goldIcon);
            var shopPanel = CreateShopPanel(rootRect, font, goldIcon);
            var dailyRewardPopup = CreateDailyRewardPopup(rootRect, font, goldIcon, starIcon);
            var settingsPopup = CreateSettingsPopup(rootRect, font);

            stagePanelRoot.SetActive(false);
            deckPanel.SetActive(false);
            upgradePanel.SetActive(false);
            missionPanel.SetActive(false);
            shopPanel.SetActive(false);

            var stageSelectView = stagePanelRoot.GetComponent<UI_StageSelectView>();
            var deckPanelView = deckPanel.GetComponent<UI_DeckPanelView>();
            var upgradePanelView = upgradePanel.GetComponent<UI_UpgradePanelView>();
            var missionPanelView = missionPanel.GetComponent<UI_MissionPanelView>();
            var shopPanelView = shopPanel.GetComponent<UI_ShopPanelView>();
            var dailyRewardPopupView = dailyRewardPopup.GetComponent<UI_DailyRewardPopupView>();
            var settingsPopupView = settingsPopup.GetComponent<UI_SettingsPopupView>();

            var outGameHomeView = rootObject.AddComponent<UI_OutGameHomeView>();
            var homeSerializer = new SerializedObject(outGameHomeView);
            homeSerializer.FindProperty("stageTabButton").objectReferenceValue = playButton;
            homeSerializer.FindProperty("deckTabButton").objectReferenceValue = tabButtons[0];
            homeSerializer.FindProperty("upgradeTabButton").objectReferenceValue = tabButtons[1];
            homeSerializer.FindProperty("missionsTabButton").objectReferenceValue = tabButtons[2];
            homeSerializer.FindProperty("shopTabButton").objectReferenceValue = tabButtons[3];
            homeSerializer.FindProperty("settingsButton").objectReferenceValue = settingsButton;
            homeSerializer.FindProperty("playButton").objectReferenceValue = playButton;
            homeSerializer.FindProperty("dailyRewardButton").objectReferenceValue = dailyRewardButton;
            homeSerializer.FindProperty("stagePanelRoot").objectReferenceValue = stagePanelRoot;
            homeSerializer.FindProperty("goldText").objectReferenceValue = goldText;
            homeSerializer.FindProperty("bestStageText").objectReferenceValue = bestStageText;
            homeSerializer.FindProperty("unitCountText").objectReferenceValue = unitCountText;
            homeSerializer.FindProperty("dailyRewardBadge").objectReferenceValue = dailyRewardBadge;
            homeSerializer.FindProperty("missionsBadge").objectReferenceValue = missionsBadge;
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
            stageSerializer.FindProperty("closeButton").objectReferenceValue = stageSelectView.CloseButton;
            stageSerializer.FindProperty("root").objectReferenceValue = stagePanelRoot;
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
            missionSerializer.FindProperty("closeButton").objectReferenceValue = missionPanelView.CloseButton;
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

            rootObject.AddComponent<UI_ShopPanelView>();
            var shopViewCopy = rootObject.GetComponent<UI_ShopPanelView>();
            var shopSerializer = new SerializedObject(shopViewCopy);
            shopSerializer.FindProperty("root").objectReferenceValue = shopPanel;
            var originalShopSerializer = new SerializedObject(shopPanelView);
            shopSerializer.FindProperty("shopItems").arraySize = shopPanelView.ShopItems.Length;
            for (var i = 0; i < shopPanelView.ShopItems.Length; i++)
            {
                shopSerializer.FindProperty("shopItems").GetArrayElementAtIndex(i).objectReferenceValue = shopPanelView.ShopItems[i];
            }
            shopSerializer.FindProperty("closeButton").objectReferenceValue = shopPanelView.CloseButton;
            shopSerializer.ApplyModifiedProperties();

            Object.DestroyImmediate(shopPanelView);

            var prefabPath = OutputRoot + "StageSelectScreen.prefab";
            DeleteExistingPrefab(prefabPath);
            PrefabUtility.SaveAsPrefabAsset(rootObject, prefabPath);
            Object.DestroyImmediate(rootObject);

            GameLogger.Log($"[UIPrefabBuilder] StageSelectScreen.prefab created at {prefabPath}");
        }

        private static bool TryBuildHomeFromLobbyDefault(RectTransform parent, GameObject lobbyDefaultTemplate, TMP_FontAsset font, Sprite battleIcon, Sprite cardIcon, Sprite hammerIcon, Sprite checkIcon, Sprite shopIcon, Sprite giftIcon, Sprite goldIcon, out Button playButton, out Button[] tabButtons, out Button dailyRewardButton, out Button settingsButton, out TMP_Text goldText, out GameObject dailyRewardBadge, out GameObject missionsBadge, out GameObject statusPanel, out TMP_Text bestStageText, out TMP_Text unitCountText)
        {
            playButton = null;
            tabButtons = new Button[4];
            dailyRewardButton = null;
            settingsButton = null;
            goldText = null;
            dailyRewardBadge = null;
            missionsBadge = null;
            statusPanel = null;
            bestStageText = null;
            unitCountText = null;

            try
            {
                var lobbyInstance = PrefabUtility.InstantiatePrefab(lobbyDefaultTemplate) as GameObject;
                if (lobbyInstance == null)
                {
                    return false;
                }

                lobbyInstance.transform.SetParent(parent, false);
                lobbyInstance.name = "LobbyDefaultHome";
                var lobbyRect = lobbyInstance.GetComponent<RectTransform>();
                lobbyRect.anchorMin = Vector2.zero;
                lobbyRect.anchorMax = Vector2.one;
                lobbyRect.offsetMin = Vector2.zero;
                lobbyRect.offsetMax = Vector2.zero;

                var allChildren = lobbyInstance.GetComponentsInChildren<Transform>(true);

                // 1. goldText: 상단 첫 번째 재화바(골드) TMP 텍스트 재활용
                Transform goldBar = null;
                // 재화 바 그룹에서 보석(Gem) 바는 끄고 골드 바에 goldText 배선 — 스프라이트명으로 판별
                var resourceGroup = allChildren.FirstOrDefault(t => t.name.Contains("ResourceBar_Group")) ?? goldBar;
                if (resourceGroup != null)
                {
                    foreach (Transform bar in resourceGroup)
                    {
                        var hasGemSprite = bar.GetComponentsInChildren<Image>(true)
                            .Any(img => img.sprite != null && img.sprite.name.Contains("Gem"));

                        if (hasGemSprite)
                        {
                            bar.gameObject.SetActive(false);
                        }
                        else if (goldText == null)
                        {
                            goldText = bar.GetComponentInChildren<TMP_Text>(true);
                            if (goldText != null && font != null)
                            {
                                goldText.font = font;
                            }
                        }
                    }
                }

                // 2. bestStageText + unitCountText: 프로필 카드의 텍스트 재활용
                Transform profileCard = allChildren.FirstOrDefault(t => t.name.Contains("UserInfo") || t.name.Contains("Profile") || t.name.Contains("Player"));
                if (profileCard != null)
                {
                    // 이름/부제 텍스트 우선 (슬라이더 게이지 "25/55"·레벨 "3" 텍스트 제외)
                    var profileTexts = profileCard.GetComponentsInChildren<TMP_Text>(true)
                        .Where(t => !HasAncestorNamed(t.transform, profileCard, "Slider"))
                        .OrderByDescending(t => t.gameObject.name.Contains("UserName"))
                        .ToArray();
                    if (profileTexts.Length >= 2)
                    {
                        bestStageText = profileTexts[0];
                        bestStageText.text = "BEST STAGE -";
                        bestStageText.color = new Color(0.35f, 0.27f, 0.2f);
                        if (font != null)
                        {
                            bestStageText.font = font;
                        }

                        unitCountText = profileTexts[1];
                        unitCountText.text = "UNITS 10/15";
                        if (font != null)
                        {
                            unitCountText.font = font;
                        }
                    }

                    statusPanel = profileCard.gameObject;
                }

                // 3. settingsButton: 우상단 햄버거 버튼
                var hambergerMenu = allChildren.FirstOrDefault(t => t.name.Contains("Hamberger") || t.name.Contains("Menu") || t.name.Contains("Setting"));
                if (hambergerMenu != null)
                {
                    hambergerMenu.name = "SettingsButton";
                    settingsButton = EnsureButtonComponent(hambergerMenu.gameObject);
                }

                // 4. dailyRewardButton: 좌측 상단 타이머 아이콘 버튼
                Transform timerButton = null;
                foreach (var child in allChildren)
                {
                    if (child.name.Contains("Ticket") || child.name.Contains("Timer") || child.name.Contains("Time"))
                    {
                        var img = child.GetComponent<Image>() != null ? child.GetComponent<Image>() : child.GetComponentInChildren<Image>(true);
                        if (img != null)
                        {
                            timerButton = child;
                            break;
                        }
                    }
                }

                if (timerButton != null)
                {
                    timerButton.name = "DailyRewardButton";
                    var iconTransform = timerButton.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "Icon");
                    var iconImage = iconTransform != null ? iconTransform.GetComponent<Image>() : timerButton.GetComponent<Image>();
                    if (iconImage != null && giftIcon != null)
                    {
                        iconImage.sprite = giftIcon;
                    }

                    var timerTexts = timerButton.GetComponentsInChildren<TMP_Text>(true);
                    foreach (var txt in timerTexts)
                    {
                        if (txt.text.Contains("d") || txt.text.Contains("h"))
                        {
                            txt.gameObject.SetActive(false);
                        }
                        else
                        {
                            txt.text = "Reward";
                            if (font != null)
                            {
                                txt.font = font;
                            }
                        }
                    }

                    dailyRewardButton = EnsureButtonComponent(timerButton.gameObject);
                    dailyRewardBadge = CreateAlertBadge(dailyRewardButton.transform, new Vector2(30, 30));
                }

                // 5. upgradeTabButton: 좌측 AD Skip 버튼
                Transform adSkipButton = null;
                foreach (var child in allChildren)
                {
                    if (child.name.Contains("ADSkip") || child.name.Contains("Skip") || child.name.Contains("AD"))
                    {
                        var btn = child.GetComponent<Button>();
                        var img = child.GetComponent<Image>();
                        if (btn != null || img != null)
                        {
                            adSkipButton = child;
                            break;
                        }
                    }
                }

                if (adSkipButton != null)
                {
                    adSkipButton.name = "UpgradeTabButton";
                    SwapIconSprite(adSkipButton, hammerIcon);

                    var labelText = adSkipButton.GetComponentInChildren<TMP_Text>(true);
                    if (labelText != null)
                    {
                        labelText.text = "Upgrade";
                        if (font != null)
                        {
                            labelText.font = font;
                        }
                    }

                    var sampleEffects = adSkipButton.GetComponentsInChildren<Transform>(true);
                    foreach (var eff in sampleEffects)
                    {
                        if (eff.name.Contains("SampleEffect") || eff.name.Contains("Effect") || eff.name.Contains("Glow"))
                        {
                            eff.gameObject.SetActive(false);
                        }
                    }

                    tabButtons[1] = EnsureButtonComponent(adSkipButton.gameObject);
                }

                // 6. missionsTabButton: 우측 Mission 두루마리 버튼
                Transform missionButton = null;
                foreach (var child in allChildren)
                {
                    if (child.name.Contains("Mission") || child.name.Contains("Scroll"))
                    {
                        var btn = child.GetComponent<Button>();
                        var img = child.GetComponent<Image>();
                        if (btn != null || img != null)
                        {
                            missionButton = child;
                            break;
                        }
                    }
                }

                if (missionButton != null)
                {
                    missionButton.name = "MissionsTabButton";
                    tabButtons[2] = EnsureButtonComponent(missionButton.gameObject);
                    missionsBadge = CreateAlertBadge(tabButtons[2].transform, new Vector2(30, 30));
                }

                // 7. deckTabButton: 우측 Inventory 가방 버튼
                Transform inventoryButton = null;
                foreach (var child in allChildren)
                {
                    if (child.name.Contains("Inventory") || child.name.Contains("Bag"))
                    {
                        var btn = child.GetComponent<Button>();
                        var img = child.GetComponent<Image>();
                        if (btn != null || img != null)
                        {
                            inventoryButton = child;
                            break;
                        }
                    }
                }

                if (inventoryButton != null)
                {
                    inventoryButton.name = "DeckTabButton";
                    SwapIconSprite(inventoryButton, cardIcon);

                    var labelText = inventoryButton.GetComponentInChildren<TMP_Text>(true);
                    if (labelText != null)
                    {
                        labelText.text = "Deck";
                        if (font != null)
                        {
                            labelText.font = font;
                        }
                    }

                    tabButtons[0] = EnsureButtonComponent(inventoryButton.gameObject);

                    // 12. shopTabButton: Inventory 버튼 복제
                    if (inventoryButton.parent != null)
                    {
                        var shopButtonClone = Object.Instantiate(inventoryButton.gameObject);
                        shopButtonClone.name = "ShopTabButton";
                        shopButtonClone.transform.SetParent(inventoryButton.parent, false);

                        var shopRect = shopButtonClone.GetComponent<RectTransform>();
                        var inventoryRect = inventoryButton.GetComponent<RectTransform>();
                        if (shopRect != null && inventoryRect != null)
                        {
                            shopRect.anchoredPosition = inventoryRect.anchoredPosition + new Vector2(0, -150);
                        }

                        SwapIconSprite(shopButtonClone.transform, shopIcon);

                        var shopLabelText = shopButtonClone.GetComponentInChildren<TMP_Text>(true);
                        if (shopLabelText != null)
                        {
                            shopLabelText.text = "Shop";
                            if (font != null)
                            {
                                shopLabelText.font = font;
                            }
                        }

                        tabButtons[3] = EnsureButtonComponent(shopButtonClone);
                    }
                }

                // 8. stageTabButton: 중앙 맵 디오라마 버튼화
                Transform mapDiorama = null;
                foreach (var child in allChildren)
                {
                    if (child.name.Contains("Map") || child.name.Contains("Diorama") || child.name.Contains("SampleImage"))
                    {
                        mapDiorama = child;
                        break;
                    }
                }

                if (mapDiorama != null)
                {
                    mapDiorama.name = "StageTabButton";
                    var mapTexts = mapDiorama.GetComponentsInChildren<TMP_Text>(true);
                    foreach (var txt in mapTexts)
                    {
                        if (txt.text.Contains("Battle"))
                        {
                            txt.text = "STAGE 1";
                            if (font != null)
                            {
                                txt.font = font;
                            }
                        }
                    }

                    var stageButton = EnsureButtonComponent(mapDiorama.gameObject);
                    playButton = stageButton;
                }

                // 9. playButton: 빨간 START 버튼
                Transform startButton = null;
                foreach (var child in allChildren)
                {
                    if (child.name.Contains("START") || child.name.Contains("Start") || child.name.Contains("Play"))
                    {
                        var btn = child.GetComponent<Button>();
                        if (btn != null)
                        {
                            startButton = child;
                            break;
                        }
                    }
                }

                if (startButton != null)
                {
                    startButton.name = "PlayButton";
                    var labelText = startButton.GetComponentInChildren<TMP_Text>(true);
                    if (labelText != null)
                    {
                        labelText.text = "PLAY";
                        if (font != null)
                        {
                            labelText.font = font;
                        }
                    }

                    if (playButton == null)
                    {
                        playButton = EnsureButtonComponent(startButton.gameObject);
                    }
                }

                // 10. 비활성: 하단 채팅·탭바
                var disableNames = new[] { "Chat", "chat", "Tab_", "Bubble" };
                foreach (var child in allChildren)
                {
                    if (disableNames.Any(n => child.name.Contains(n)))
                    {
                        child.gameObject.SetActive(false);
                    }
                }

                if (goldText == null)
                {
                    GameLogger.LogWarning("[UIPrefabBuilder] goldText not found in Lobby_Default, searching alternatives...");
                    var allTexts = lobbyInstance.GetComponentsInChildren<TMP_Text>(true);
                    foreach (var txt in allTexts)
                    {
                        if (txt.text.Contains("Gold") || txt.text.Contains("Coin") || txt.gameObject.name.Contains("Gold"))
                        {
                            goldText = txt;
                            break;
                        }
                    }
                }

                if (bestStageText == null || unitCountText == null)
                {
                    GameLogger.LogWarning("[UIPrefabBuilder] Profile texts not found in Lobby_Default.");
                }

                if (settingsButton == null)
                {
                    GameLogger.LogWarning("[UIPrefabBuilder] settingsButton not found in Lobby_Default.");
                }

                if (dailyRewardButton == null)
                {
                    GameLogger.LogWarning("[UIPrefabBuilder] dailyRewardButton not found in Lobby_Default.");
                }

                if (tabButtons[0] == null || tabButtons[1] == null || tabButtons[2] == null || tabButtons[3] == null)
                {
                    GameLogger.LogWarning("[UIPrefabBuilder] Some tab buttons not found in Lobby_Default.");
                }

                if (playButton == null)
                {
                    GameLogger.LogWarning("[UIPrefabBuilder] playButton not found in Lobby_Default.");
                }

                return true;
            }
            catch (System.Exception ex)
            {
                GameLogger.LogError($"[UIPrefabBuilder] TryBuildHomeFromLobbyDefault failed: {ex.Message}");
                return false;
            }
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
            scrollViewRect.sizeDelta = new Vector2(1000, 1200);

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

            const int columns = 4;
            const float buttonWidth = 230f;
            const float buttonHeight = 100f;
            const float spacingX = 250f;
            const float spacingY = 120f;

            var rows = (stageIds.Length + columns - 1) / columns;
            var contentHeight = rows * spacingY + 50f;
            contentRect.sizeDelta = new Vector2(1000, contentHeight);

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

        private static GameObject CreatePlayerStatusPanel(RectTransform parent, TMP_FontAsset font)
        {
            // UserInfo_03 template produces broken layout (yellow frame + torn text) — always use fallback
            return CreatePlayerStatusPanelFallback(parent, font);

            /*
            const string templatePath = KitRoot + "Theme_Light/Prefabs/Prefabs~DemoLayout/UserInfo_03.prefab";
            var templateInstance = InstantiateKitPrefab(templatePath, parent);

            if (templateInstance == null)
            {
                GameLogger.LogWarning("[UIPrefabBuilder] UserInfo_03 not found, using fallback.");
                return CreatePlayerStatusPanelFallback(parent, font);
            }

            templateInstance.name = "PlayerStatusPanel";
            var panelRect = templateInstance.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 1);
            panelRect.anchorMax = new Vector2(0.5f, 1);
            panelRect.pivot = new Vector2(0.5f, 1);
            panelRect.anchoredPosition = new Vector2(-100, -50);
            panelRect.sizeDelta = new Vector2(380, 110);

            var iconTrophy = templateInstance.transform.Find("Icon_Trophy");
            var iconLeague = templateInstance.transform.Find("Icon_League");
            var bgTop = templateInstance.transform.Find("BgTop");

            if (iconTrophy != null)
            {
                iconTrophy.gameObject.SetActive(false);
            }
            if (iconLeague != null)
            {
                iconLeague.gameObject.SetActive(false);
            }
            if (bgTop != null)
            {
                bgTop.gameObject.SetActive(false);
            }

            // 프로필 프레임 안의 킷 데모 캐릭터 초상화 비활성화 (UiAuditor)
            var demoCharacter = templateInstance.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name == "Character");
            if (demoCharacter != null)
            {
                demoCharacter.gameObject.SetActive(false);
            }

            var userNameText = templateInstance.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name == "Text_UserName")?.GetComponent<TMP_Text>();
            var trophyTransform = templateInstance.transform.Find("Trophy");
            var textTMP = templateInstance.transform.Find("Text (TMP)")?.GetComponent<TMP_Text>();

            TMP_Text bestStageText = null;
            if (userNameText != null)
            {
                bestStageText = userNameText;
                bestStageText.text = "BEST STAGE -";
                bestStageText.font = font;
            }
            else if (trophyTransform != null)
            {
                bestStageText = trophyTransform.GetComponentInChildren<TMP_Text>();
                if (bestStageText != null)
                {
                    bestStageText.text = "BEST STAGE -";
                    bestStageText.font = font;
                }
            }

            TMP_Text unitCountText = null;
            if (textTMP != null)
            {
                unitCountText = textTMP;
                unitCountText.text = "UNITS 10/15";
                unitCountText.font = font;
            }

            if (bestStageText == null)
            {
                var bestStageTextObject = new GameObject("BestStageText");
                bestStageTextObject.transform.SetParent(templateInstance.transform, false);
                var bestStageRect = bestStageTextObject.AddComponent<RectTransform>();
                bestStageRect.anchorMin = new Vector2(0.5f, 1);
                bestStageRect.anchorMax = new Vector2(0.5f, 1);
                bestStageRect.pivot = new Vector2(0.5f, 1);
                bestStageRect.anchoredPosition = new Vector2(0, -20);
                bestStageRect.sizeDelta = new Vector2(350, 40);

                bestStageText = bestStageTextObject.AddComponent<TextMeshProUGUI>();
                bestStageText.text = "BEST STAGE -";
                bestStageText.fontSize = 30;
                bestStageText.alignment = TextAlignmentOptions.Center;
                bestStageText.font = font;
            }

            if (bestStageText != null)
            {
                bestStageText.gameObject.name = "BestStageText";
            }

            if (unitCountText == null)
            {
                var unitCountTextObject = new GameObject("UnitCountText");
                unitCountTextObject.transform.SetParent(templateInstance.transform, false);
                var unitCountRect = unitCountTextObject.AddComponent<RectTransform>();
                unitCountRect.anchorMin = new Vector2(0.5f, 1);
                unitCountRect.anchorMax = new Vector2(0.5f, 1);
                unitCountRect.pivot = new Vector2(0.5f, 1);
                unitCountRect.anchoredPosition = new Vector2(0, -65);
                unitCountRect.sizeDelta = new Vector2(350, 35);

                unitCountText = unitCountTextObject.AddComponent<TextMeshProUGUI>();
                unitCountText.text = "UNITS 10/15";
                unitCountText.fontSize = 26;
                unitCountText.alignment = TextAlignmentOptions.Center;
                unitCountText.font = font;
            }

            unitCountText.gameObject.name = "UnitCountText";

            return templateInstance;
            */
        }

        private static GameObject CreatePlayerStatusPanelFallback(RectTransform parent, TMP_FontAsset font)
        {
            var profileFramePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ProfileFrameBluePath);
            GameObject panelInstance;

            if (profileFramePrefab != null)
            {
                panelInstance = PrefabUtility.InstantiatePrefab(profileFramePrefab) as GameObject;
                panelInstance.transform.SetParent(parent, false);
            }
            else
            {
                GameLogger.LogWarning("[UIPrefabBuilder] ProfileFrame_01_l_Blue not found, using BasicFrame fallback.");
                var basicFramePath = KitRoot + "Theme_Light/Prefabs/Prefabs_Frame/BasicFrame/BasicFrame_02_Blue.prefab";
                var basicFrame = AssetDatabase.LoadAssetAtPath<GameObject>(basicFramePath);
                if (basicFrame != null)
                {
                    panelInstance = PrefabUtility.InstantiatePrefab(basicFrame) as GameObject;
                    panelInstance.transform.SetParent(parent, false);
                }
                else
                {
                    panelInstance = new GameObject("PlayerStatusPanel");
                    panelInstance.transform.SetParent(parent, false);
                    var bgImage = panelInstance.AddComponent<Image>();
                    bgImage.color = new Color(0.2f, 0.3f, 0.5f, 0.8f);
                }
            }

            panelInstance.name = "PlayerStatusPanel";
            DisableDemoDescendants(panelInstance, new[] { "Character" });
            var panelRect = panelInstance.GetComponent<RectTransform>();
            if (panelRect == null)
            {
                panelRect = panelInstance.AddComponent<RectTransform>();
            }

            panelRect.anchorMin = new Vector2(0.5f, 1);
            panelRect.anchorMax = new Vector2(0.5f, 1);
            panelRect.pivot = new Vector2(0.5f, 1);
            panelRect.anchoredPosition = new Vector2(-100, -50);
            panelRect.sizeDelta = new Vector2(380, 110);

            foreach (Transform child in panelInstance.transform)
            {
                if (child.name.Contains("Icon") || child.name.Contains("Text") || child.name.Contains("Image"))
                {
                    child.gameObject.SetActive(false);
                }
            }

            var bestStageTextObject = new GameObject("BestStageText");
            bestStageTextObject.transform.SetParent(panelInstance.transform, false);
            var bestStageRect = bestStageTextObject.AddComponent<RectTransform>();
            bestStageRect.anchorMin = new Vector2(0.5f, 1);
            bestStageRect.anchorMax = new Vector2(0.5f, 1);
            bestStageRect.pivot = new Vector2(0.5f, 1);
            bestStageRect.anchoredPosition = new Vector2(0, -20);
            bestStageRect.sizeDelta = new Vector2(350, 40);

            var bestStageText = bestStageTextObject.AddComponent<TextMeshProUGUI>();
            bestStageText.text = "BEST STAGE -";
            bestStageText.fontSize = 30;
            bestStageText.alignment = TextAlignmentOptions.Center;
            bestStageText.font = font;

            var unitCountTextObject = new GameObject("UnitCountText");
            unitCountTextObject.transform.SetParent(panelInstance.transform, false);
            var unitCountRect = unitCountTextObject.AddComponent<RectTransform>();
            unitCountRect.anchorMin = new Vector2(0.5f, 1);
            unitCountRect.anchorMax = new Vector2(0.5f, 1);
            unitCountRect.pivot = new Vector2(0.5f, 1);
            unitCountRect.anchoredPosition = new Vector2(0, -65);
            unitCountRect.sizeDelta = new Vector2(350, 35);

            var unitCountText = unitCountTextObject.AddComponent<TextMeshProUGUI>();
            unitCountText.text = "UNITS 10/15";
            unitCountText.fontSize = 26;
            unitCountText.alignment = TextAlignmentOptions.Center;
            unitCountText.font = font;

            return panelInstance;
        }

        private static Button CreatePlayButton(RectTransform parent, TMP_FontAsset font, Sprite battleIcon)
        {
            var buttonInstance = InstantiateKitPrefab(ButtonOrangePath, parent);
            if (buttonInstance == null)
            {
                GameLogger.LogError("[UIPrefabBuilder] Failed to create PLAY button.");
                return null;
            }

            buttonInstance.name = "PlayButton";
            var buttonRect = buttonInstance.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.5f, 0);
            buttonRect.anchorMax = new Vector2(0.5f, 0);
            buttonRect.pivot = new Vector2(0.5f, 0);
            buttonRect.anchoredPosition = new Vector2(0, 140);
            buttonRect.sizeDelta = new Vector2(360, 176);

            var button = EnsureButtonComponent(buttonInstance);

            var labelTMP = buttonInstance.GetComponentInChildren<TMP_Text>();
            if (labelTMP != null)
            {
                labelTMP.text = "PLAY";
                labelTMP.fontSize = 60;
                labelTMP.alignment = TextAlignmentOptions.Center;
                labelTMP.font = font;
                var labelRect = labelTMP.GetComponent<RectTransform>();
                labelRect.anchoredPosition = new Vector2(20, 0);
            }

            if (battleIcon != null)
            {
                var iconObject = new GameObject("BattleIcon");
                iconObject.transform.SetParent(buttonInstance.transform, false);
                var iconRect = iconObject.AddComponent<RectTransform>();
                iconRect.anchorMin = new Vector2(0, 0.5f);
                iconRect.anchorMax = new Vector2(0, 0.5f);
                iconRect.pivot = new Vector2(0, 0.5f);
                iconRect.anchoredPosition = new Vector2(30, 0);
                iconRect.sizeDelta = new Vector2(60, 60);

                var iconImage = iconObject.AddComponent<Image>();
                iconImage.sprite = battleIcon;
                iconImage.raycastTarget = false;
            }

            return button;
        }

        private static Button[] CreateBottomDockButtons(RectTransform parent, TMP_FontAsset font, Sprite cardIcon, Sprite hammerIcon, Sprite checkIcon, Sprite shopIcon)
        {
            var buttons = new Button[4];
            var labels = new[] { "DECK", "UPGRADE", "MISSIONS", "SHOP" };
            var icons = new[] { cardIcon, hammerIcon, checkIcon, shopIcon };
            var xPositions = new[] { -280f, -430f, 280f, 430f };
            var yPosition = 120f;

            for (var i = 0; i < 4; i++)
            {
                var buttonInstance = InstantiateKitPrefab(ButtonBluePath, parent);
                if (buttonInstance == null)
                {
                    GameLogger.LogError($"[UIPrefabBuilder] Failed to create dock button {i}");
                    continue;
                }

                buttonInstance.name = $"DockButton_{labels[i]}";
                var buttonRect = buttonInstance.GetComponent<RectTransform>();
                buttonRect.anchorMin = new Vector2(0.5f, 0);
                buttonRect.anchorMax = new Vector2(0.5f, 0);
                buttonRect.pivot = new Vector2(0.5f, 0);
                buttonRect.anchoredPosition = new Vector2(xPositions[i], yPosition);
                buttonRect.sizeDelta = new Vector2(150, 120);

                var button = EnsureButtonComponent(buttonInstance);
                buttons[i] = button;

                if (icons[i] != null)
                {
                    var iconObject = new GameObject("Icon");
                    iconObject.transform.SetParent(buttonInstance.transform, false);
                    var iconRect = iconObject.AddComponent<RectTransform>();
                    iconRect.anchorMin = new Vector2(0.5f, 1);
                    iconRect.anchorMax = new Vector2(0.5f, 1);
                    iconRect.pivot = new Vector2(0.5f, 1);
                    iconRect.anchoredPosition = new Vector2(0, -10);
                    iconRect.sizeDelta = new Vector2(48, 48);

                    var iconImage = iconObject.AddComponent<Image>();
                    iconImage.sprite = icons[i];
                    iconImage.raycastTarget = false;
                }

                var labelTextObject = new GameObject("Label");
                labelTextObject.transform.SetParent(buttonInstance.transform, false);
                var labelRect = labelTextObject.AddComponent<RectTransform>();
                labelRect.anchorMin = new Vector2(0, 0);
                labelRect.anchorMax = new Vector2(1, 0);
                labelRect.pivot = new Vector2(0.5f, 0);
                labelRect.anchoredPosition = new Vector2(0, 10);
                labelRect.sizeDelta = new Vector2(-10, 30);

                var labelText = labelTextObject.AddComponent<TextMeshProUGUI>();
                labelText.text = labels[i];
                labelText.fontSize = 24;
                labelText.alignment = TextAlignmentOptions.Center;
                labelText.font = font;
            }

            return buttons;
        }

        private static Button CreateDailyRewardButton(RectTransform parent, Sprite giftIcon)
        {
            var buttonInstance = InstantiateKitPrefab(ButtonCirclePath, parent);
            if (buttonInstance == null)
            {
                GameLogger.LogError("[UIPrefabBuilder] Failed to create daily reward button.");
                return null;
            }

            buttonInstance.name = "DailyRewardButton";
            var buttonRect = buttonInstance.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(1, 1);
            buttonRect.anchorMax = new Vector2(1, 1);
            buttonRect.pivot = new Vector2(1, 1);
            buttonRect.anchoredPosition = new Vector2(-80, -190);
            buttonRect.sizeDelta = new Vector2(90, 90);

            var button = EnsureButtonComponent(buttonInstance);

            if (giftIcon != null)
            {
                var iconObject = new GameObject("GiftIcon");
                iconObject.transform.SetParent(buttonInstance.transform, false);
                var iconRect = iconObject.AddComponent<RectTransform>();
                iconRect.anchorMin = new Vector2(0.5f, 0.5f);
                iconRect.anchorMax = new Vector2(0.5f, 0.5f);
                iconRect.pivot = new Vector2(0.5f, 0.5f);
                iconRect.anchoredPosition = Vector2.zero;
                iconRect.sizeDelta = new Vector2(50, 50);

                var iconImage = iconObject.AddComponent<Image>();
                iconImage.sprite = giftIcon;
                iconImage.raycastTarget = false;
            }

            return button;
        }

        private static GameObject CreateAlertBadge(Transform parent, Vector2 offset)
        {
            var badgeInstance = InstantiateKitPrefab(AlertDotRedPath, parent);
            if (badgeInstance == null)
            {
                GameLogger.LogWarning("[UIPrefabBuilder] Alert_Dot_01_Red not found, creating fallback badge.");
                badgeInstance = new GameObject("AlertBadge");
                badgeInstance.transform.SetParent(parent, false);
                var bgImage = badgeInstance.AddComponent<Image>();
                bgImage.color = Color.red;
            }

            badgeInstance.name = "AlertBadge";
            var badgeRect = badgeInstance.GetComponent<RectTransform>();
            if (badgeRect == null)
            {
                badgeRect = badgeInstance.AddComponent<RectTransform>();
            }

            badgeRect.anchorMin = new Vector2(1, 1);
            badgeRect.anchorMax = new Vector2(1, 1);
            badgeRect.pivot = new Vector2(0.5f, 0.5f);
            badgeRect.anchoredPosition = offset;
            badgeRect.sizeDelta = new Vector2(34, 34);

            badgeInstance.SetActive(false);

            return badgeInstance;
        }

        private static Button[] CreateTabButtons(RectTransform parent, TMP_FontAsset font)
        {
            var buttons = new Button[5];
            var labels = new[] { "STAGES", "DECK", "UPGRADE", "MISSIONS", "SHOP" };
            var xPositions = new[] { -440f, -220f, 0f, 220f, 440f };

            for (var i = 0; i < 5; i++)
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
            const string templatePath = KitRoot + "Theme_Light/Prefabs/Prefabs~DemoScenes/World_Dungeon_List.prefab";
            var templateInstance = InstantiateKitPrefab(templatePath, parent);

            if (templateInstance == null)
            {
                GameLogger.LogWarning("[UIPrefabBuilder] World_Dungeon_List not found, using fallback.");
                return CreateStagePanelFallback(parent, font, starIcon, lockIcon);
            }

            templateInstance.name = "StagePanel";
            var panelRect = templateInstance.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 0.22f);
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            DisableDemoDescendants(templateInstance, new[] { "ResourceBar", "Tab_", "Dropdown", "Info" });

            var scrollRectTransform = templateInstance.GetComponentsInChildren<ScrollRect>(true)
                .FirstOrDefault()?.transform;
            var viewportTransform = scrollRectTransform?.Find("Viewport");
            var contentTransform = viewportTransform?.Find("Content");

            if (contentTransform == null)
            {
                GameLogger.LogWarning("[UIPrefabBuilder] Content not found in World_Dungeon_List, using fallback.");
                Object.DestroyImmediate(templateInstance);
                return CreateStagePanelFallback(parent, font, starIcon, lockIcon);
            }

            var demoItems = contentTransform.GetComponentsInChildren<Transform>(true)
                .Where(t => t != contentTransform && t.parent == contentTransform && t.GetComponent<RectTransform>() != null)
                .Select(t => t.gameObject)
                .ToArray();

            if (demoItems.Length == 0)
            {
                GameLogger.LogWarning("[UIPrefabBuilder] No demo items found in World_Dungeon_List Content, using fallback.");
                Object.DestroyImmediate(templateInstance);
                return CreateStagePanelFallback(parent, font, starIcon, lockIcon);
            }

            var firstItem = demoItems[0];
            var stageIds = GenerateStageIds();
            var stageButtons = new UI_StageButtonView[stageIds.Length];

            foreach (Transform directChild in contentTransform)
            {
                var isStageItem = false;
                foreach (var item in demoItems)
                {
                    if (directChild.gameObject == item)
                    {
                        isStageItem = true;
                        break;
                    }
                }
                if (!isStageItem)
                {
                    directChild.gameObject.SetActive(false);
                }
            }

            for (var i = 0; i < stageIds.Length; i++)
            {
                GameObject itemInstance;
                if (i < demoItems.Length)
                {
                    itemInstance = demoItems[i];
                }
                else
                {
                    itemInstance = Object.Instantiate(firstItem, contentTransform);
                }

                itemInstance.name = $"StageButton_{stageIds[i]}";
                itemInstance.SetActive(true);

                DisableDemoDescendants(itemInstance, new[] { "ResourceStats", "Key" });

                var demoLocks = itemInstance.GetComponentsInChildren<Transform>(true)
                    .Where(t => t.name == "Lock" && t.parent.name != "LockedOverlay").ToArray();
                foreach (var lockTr in demoLocks)
                {
                    lockTr.gameObject.SetActive(false);
                }

                var button = EnsureButtonComponent(itemInstance);

                var stageNameText = itemInstance.GetComponentsInChildren<TMP_Text>(true)
                    .FirstOrDefault(txt => txt.name.Contains("Text") || txt.name.Contains("Title") || txt.name.Contains("Name"));

                if (stageNameText == null)
                {
                    var textObject = new GameObject("StageNameText");
                    textObject.transform.SetParent(itemInstance.transform, false);
                    var textRect = textObject.AddComponent<RectTransform>();
                    textRect.anchorMin = Vector2.zero;
                    textRect.anchorMax = Vector2.one;
                    textRect.offsetMin = Vector2.zero;
                    textRect.offsetMax = Vector2.zero;
                    stageNameText = textObject.AddComponent<TextMeshProUGUI>();
                    stageNameText.fontSize = 48;
                    stageNameText.alignment = TextAlignmentOptions.Center;
                }
                else
                {
                    stageNameText.gameObject.name = "StageNameText";
                }

                stageNameText.text = $"STAGE {i + 1}";
                stageNameText.font = font;

                var enterButton = itemInstance.GetComponentsInChildren<Transform>(true)
                    .FirstOrDefault(t => t.name.Contains("Button") && t != itemInstance.transform);
                if (enterButton != null)
                {
                    enterButton.gameObject.SetActive(false);
                }

                var clearedMarkObject = itemInstance.GetComponentsInChildren<Transform>(true)
                    .FirstOrDefault(t => t.name.Contains("Alert") || t.name.Contains("Dot") || t.name.Contains("Check"));
                GameObject clearedMark;
                if (clearedMarkObject != null)
                {
                    clearedMark = clearedMarkObject.gameObject;
                    clearedMark.name = "ClearedMark";
                    var markImage = clearedMark.GetComponent<Image>();
                    if (markImage != null && starIcon != null)
                    {
                        markImage.sprite = starIcon;
                    }
                    clearedMark.SetActive(false);
                }
                else
                {
                    clearedMark = new GameObject("ClearedMark");
                    clearedMark.transform.SetParent(itemInstance.transform, false);
                    var markRect = clearedMark.AddComponent<RectTransform>();
                    markRect.anchorMin = new Vector2(1, 0.5f);
                    markRect.anchorMax = new Vector2(1, 0.5f);
                    markRect.pivot = new Vector2(1, 0.5f);
                    markRect.anchoredPosition = new Vector2(-20, 0);
                    markRect.sizeDelta = new Vector2(64, 64);
                    var markImage = clearedMark.AddComponent<Image>();
                    markImage.sprite = starIcon;
                    markImage.raycastTarget = false;
                    clearedMark.SetActive(false);
                }

                var lockedOverlayObject = new GameObject("LockedOverlay");
                lockedOverlayObject.transform.SetParent(itemInstance.transform, false);
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

                var buttonView = itemInstance.AddComponent<UI_StageButtonView>();
                var buttonSerializer = new SerializedObject(buttonView);
                buttonSerializer.FindProperty("button").objectReferenceValue = button;
                buttonSerializer.FindProperty("stageNameText").objectReferenceValue = stageNameText;
                buttonSerializer.FindProperty("clearedMark").objectReferenceValue = clearedMark;
                buttonSerializer.FindProperty("lockedOverlay").objectReferenceValue = lockedOverlayObject;
                buttonSerializer.FindProperty("stageId").stringValue = stageIds[i];
                buttonSerializer.ApplyModifiedProperties();

                stageButtons[i] = buttonView;
            }

            var contentRect = contentTransform.GetComponent<RectTransform>();
            var verticalFit = contentRect.GetComponent<ContentSizeFitter>();
            if (verticalFit == null)
            {
                verticalFit = contentRect.gameObject.AddComponent<ContentSizeFitter>();
                verticalFit.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }

            var closeButtonInstance = InstantiateKitPrefab(ButtonCirclePath, templateInstance.transform);
            if (closeButtonInstance != null)
            {
                closeButtonInstance.name = "CloseButton";
                var closeButtonRect = closeButtonInstance.GetComponent<RectTransform>();
                closeButtonRect.anchorMin = new Vector2(1, 1);
                closeButtonRect.anchorMax = new Vector2(1, 1);
                closeButtonRect.pivot = new Vector2(1, 1);
                closeButtonRect.anchoredPosition = new Vector2(-70, -70);
                closeButtonRect.sizeDelta = new Vector2(90, 90);

                var closeIconObject = new GameObject("CloseIcon");
                closeIconObject.transform.SetParent(closeButtonInstance.transform, false);
                var closeIconRect = closeIconObject.AddComponent<RectTransform>();
                closeIconRect.anchorMin = new Vector2(0.5f, 0.5f);
                closeIconRect.anchorMax = new Vector2(0.5f, 0.5f);
                closeIconRect.pivot = new Vector2(0.5f, 0.5f);
                closeIconRect.anchoredPosition = Vector2.zero;
                closeIconRect.sizeDelta = new Vector2(50, 50);

                var closeIconImage = closeIconObject.AddComponent<Image>();
                var closeIcon = LoadSprite("Assets/Layer Lab/GUI Pro-MinimalGame/Shared/Icons/PictoIcon/128/arrow_back.png");
                closeIconImage.sprite = closeIcon;
                closeIconImage.raycastTarget = false;
            }

            var closeButton = EnsureButtonComponent(closeButtonInstance);

            var stageSelectView = templateInstance.AddComponent<UI_StageSelectView>();
            var serializedObject = new SerializedObject(stageSelectView);
            serializedObject.FindProperty("stageButtons").arraySize = stageButtons.Length;
            for (var i = 0; i < stageButtons.Length; i++)
            {
                serializedObject.FindProperty("stageButtons").GetArrayElementAtIndex(i).objectReferenceValue = stageButtons[i];
            }
            serializedObject.FindProperty("closeButton").objectReferenceValue = closeButton;
            serializedObject.FindProperty("root").objectReferenceValue = templateInstance;
            serializedObject.ApplyModifiedProperties();

            return templateInstance;
        }

        private static GameObject CreateStagePanelFallback(RectTransform parent, TMP_FontAsset font, Sprite starIcon, Sprite lockIcon)
        {
            var panelObject = new GameObject("StagePanel");
            panelObject.transform.SetParent(parent, false);

            var panelRect = panelObject.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 0.22f);
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            var bgImage = panelObject.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);

            AddPanelBackgroundFrame(panelObject);

            CreateStageTitle(panelRect, font);
            var stageButtons = CreateStageButtons(panelRect, font, starIcon, lockIcon);

            var closeButtonInstance = InstantiateKitPrefab(ButtonCirclePath, panelObject.transform);
            if (closeButtonInstance != null)
            {
                closeButtonInstance.name = "CloseButton";
                var closeButtonRect = closeButtonInstance.GetComponent<RectTransform>();
                closeButtonRect.anchorMin = new Vector2(1, 1);
                closeButtonRect.anchorMax = new Vector2(1, 1);
                closeButtonRect.pivot = new Vector2(1, 1);
                closeButtonRect.anchoredPosition = new Vector2(-70, -70);
                closeButtonRect.sizeDelta = new Vector2(90, 90);

                var closeIconObject = new GameObject("CloseIcon");
                closeIconObject.transform.SetParent(closeButtonInstance.transform, false);
                var closeIconRect = closeIconObject.AddComponent<RectTransform>();
                closeIconRect.anchorMin = new Vector2(0.5f, 0.5f);
                closeIconRect.anchorMax = new Vector2(0.5f, 0.5f);
                closeIconRect.pivot = new Vector2(0.5f, 0.5f);
                closeIconRect.anchoredPosition = Vector2.zero;
                closeIconRect.sizeDelta = new Vector2(50, 50);

                var closeIconImage = closeIconObject.AddComponent<Image>();
                var closeIcon = LoadSprite("Assets/Layer Lab/GUI Pro-MinimalGame/Shared/Icons/PictoIcon/128/arrow_back.png");
                closeIconImage.sprite = closeIcon;
                closeIconImage.raycastTarget = false;
            }

            var closeButton = EnsureButtonComponent(closeButtonInstance);

            var stageSelectView = panelObject.AddComponent<UI_StageSelectView>();
            var serializedObject = new SerializedObject(stageSelectView);
            serializedObject.FindProperty("stageButtons").arraySize = stageButtons.Length;
            for (var i = 0; i < stageButtons.Length; i++)
            {
                serializedObject.FindProperty("stageButtons").GetArrayElementAtIndex(i).objectReferenceValue = stageButtons[i];
            }
            serializedObject.FindProperty("closeButton").objectReferenceValue = closeButton;
            serializedObject.FindProperty("root").objectReferenceValue = panelObject;
            serializedObject.ApplyModifiedProperties();

            return panelObject;
        }

        private static GameObject CreateDeckPanel(RectTransform parent, TMP_FontAsset font, Sprite starIcon)
        {
            const string templatePath = KitRoot + "Theme_Light/Prefabs/Prefabs~DemoScenes/Character_Hero_List_01.prefab";
            var templateInstance = InstantiateKitPrefab(templatePath, parent);

            if (templateInstance == null)
            {
                GameLogger.LogWarning("[UIPrefabBuilder] Character_Hero_List_01 not found, using fallback.");
                return CreateDeckPanelFallback(parent, font, starIcon);
            }

            templateInstance.name = "DeckPanel";
            var panelRect = templateInstance.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 0.22f);
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            DisableDemoDescendants(templateInstance, new[] { "Dropdown", "Option", "Button_Info", "Info", "ResourceBar", "Group_Money", "Tab_" });

            var middleTransform = templateInstance.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name == "Middle");
            var scrollRectTransform = middleTransform?.Find("ScrollRect");
            var viewportTransform = scrollRectTransform?.Find("Viewport");
            var contentTransform = viewportTransform?.Find("Content");

            if (contentTransform == null)
            {
                GameLogger.LogWarning("[UIPrefabBuilder] Content not found in Character_Hero_List_01, using fallback.");
                Object.DestroyImmediate(templateInstance);
                return CreateDeckPanelFallback(parent, font, starIcon);
            }

            var demoItems = contentTransform.GetComponentsInChildren<Transform>(true)
                .Where(t => t.name.StartsWith("ListItem_Hero_01") && t != contentTransform)
                .Select(t => t.gameObject)
                .ToArray();

            if (demoItems.Length == 0)
            {
                GameLogger.LogWarning("[UIPrefabBuilder] No demo items found in Character_Hero_List_01, using fallback.");
                Object.DestroyImmediate(templateInstance);
                return CreateDeckPanelFallback(parent, font, starIcon);
            }

            var cards = new UI_UnitCardView[15];
            var firstItem = demoItems[0];

            for (var i = 0; i < 15; i++)
            {
                GameObject itemInstance;
                if (i < demoItems.Length)
                {
                    itemInstance = demoItems[i];
                }
                else
                {
                    itemInstance = Object.Instantiate(firstItem, contentTransform);
                }

                itemInstance.name = $"DeckCard_{i}";
                itemInstance.SetActive(true);

                DisableDemoDescendants(itemInstance, new[] { "Character", "Iocn_Up", "RoleArea", "Slider", "Gauge" });

                // 데모 캐릭터 아트는 오브젝트명이 아니라 스프라이트명(Sample_Cha* 류)으로 식별해 제거
                foreach (var demoArtImage in itemInstance.GetComponentsInChildren<Image>(true))
                {
                    if (demoArtImage.sprite != null && (demoArtImage.sprite.name.Contains("Cha") || demoArtImage.sprite.name.Contains("Sample")))
                    {
                        demoArtImage.gameObject.SetActive(false);
                    }
                }

                var demoGaugeTexts = itemInstance.GetComponentsInChildren<TMP_Text>(true)
                    .Where(txt => txt.text.Contains("/") && txt.name != "LevelText").ToArray();
                foreach (var txt in demoGaugeTexts)
                {
                    var parentGroup = txt.transform.parent;
                    if (parentGroup != null && parentGroup != itemInstance.transform)
                    {
                        parentGroup.gameObject.SetActive(false);
                    }
                }

                var button = EnsureButtonComponent(itemInstance);

                var iconImage = itemInstance.GetComponentsInChildren<Image>(true)
                    .FirstOrDefault(img => img.name.Contains("Icon") && !img.name.Contains("Check") && !img.name.Contains("Up"));
                var nameText = itemInstance.GetComponentsInChildren<TMP_Text>(true)
                    .FirstOrDefault(txt => txt.name.Contains("Name"));
                var levelText = itemInstance.GetComponentsInChildren<TMP_Text>(true)
                    .FirstOrDefault(txt => txt.name.Contains("Level"));

                if (iconImage == null)
                {
                    var iconObject = new GameObject("UnitIcon");
                    iconObject.transform.SetParent(itemInstance.transform, false);
                    var iconRect = iconObject.AddComponent<RectTransform>();
                    iconRect.anchorMin = new Vector2(0.5f, 0.5f);
                    iconRect.anchorMax = new Vector2(0.5f, 0.5f);
                    iconRect.anchoredPosition = new Vector2(0, 20);
                    iconRect.sizeDelta = new Vector2(150, 150);
                    iconImage = iconObject.AddComponent<Image>();
                    iconImage.preserveAspect = true;
                    iconImage.raycastTarget = false;
                }
                else
                {
                    iconImage.gameObject.name = "UnitIcon";
                    var iconRect = iconImage.GetComponent<RectTransform>();
                    iconRect.anchorMin = new Vector2(0.5f, 0.5f);
                    iconRect.anchorMax = new Vector2(0.5f, 0.5f);
                    iconRect.anchoredPosition = new Vector2(0, 20);
                    iconRect.sizeDelta = new Vector2(150, 150);
                }

                if (nameText == null)
                {
                    var nameObject = new GameObject("NameText");
                    nameObject.transform.SetParent(itemInstance.transform, false);
                    var nameRect = nameObject.AddComponent<RectTransform>();
                    nameRect.anchorMin = new Vector2(0.5f, 0);
                    nameRect.anchorMax = new Vector2(0.5f, 0);
                    nameRect.pivot = new Vector2(0.5f, 0);
                    nameRect.anchoredPosition = new Vector2(0, 10);
                    nameRect.sizeDelta = new Vector2(230, 40);
                    var nameTmp = nameObject.AddComponent<TextMeshProUGUI>();
                    nameTmp.fontSize = 30;
                    nameTmp.alignment = TextAlignmentOptions.Center;
                    nameText = nameTmp;
                }
                else
                {
                    nameText.gameObject.name = "NameText";
                    var nameRect = nameText.GetComponent<RectTransform>();
                    nameRect.anchorMin = new Vector2(0.5f, 0);
                    nameRect.anchorMax = new Vector2(0.5f, 0);
                    nameRect.pivot = new Vector2(0.5f, 0);
                    nameRect.anchoredPosition = new Vector2(0, 10);
                }
                nameText.text = "Unit";
                nameText.font = font;
                if (levelText != null)
                {
                    levelText.gameObject.name = "LevelText";
                    levelText.text = "Lv.1";
                    levelText.font = font;
                }

                var selectedMarkObject = itemInstance.GetComponentsInChildren<Transform>(true)
                    .FirstOrDefault(t => t.name.Contains("Check") || t.name.Contains("Selected"));
                GameObject selectedMarkGO;
                if (selectedMarkObject != null)
                {
                    selectedMarkGO = selectedMarkObject.gameObject;
                    selectedMarkGO.name = "SelectedMark";
                    selectedMarkGO.SetActive(false);
                }
                else
                {
                    selectedMarkGO = new GameObject("SelectedMark");
                    selectedMarkGO.transform.SetParent(itemInstance.transform, false);
                    var markRect = selectedMarkGO.AddComponent<RectTransform>();
                    markRect.anchorMin = new Vector2(1, 1);
                    markRect.anchorMax = new Vector2(1, 1);
                    markRect.pivot = new Vector2(1, 1);
                    markRect.anchoredPosition = new Vector2(-5, -5);
                    markRect.sizeDelta = new Vector2(40, 40);
                    var markImage = selectedMarkGO.AddComponent<Image>();
                    markImage.sprite = starIcon;
                    markImage.raycastTarget = false;
                    selectedMarkGO.SetActive(false);
                }

                var dimOverlayObject = new GameObject("DimOverlay");
                dimOverlayObject.transform.SetParent(itemInstance.transform, false);
                var dimRect = dimOverlayObject.AddComponent<RectTransform>();
                dimRect.anchorMin = Vector2.zero;
                dimRect.anchorMax = Vector2.one;
                dimRect.offsetMin = Vector2.zero;
                dimRect.offsetMax = Vector2.zero;
                var dimImage = dimOverlayObject.AddComponent<Image>();
                dimImage.color = new Color(0, 0, 0, 0.5f);
                dimImage.raycastTarget = false;
                dimOverlayObject.SetActive(false);

                var cardView = itemInstance.AddComponent<UI_UnitCardView>();
                var cardSerializer = new SerializedObject(cardView);
                cardSerializer.FindProperty("button").objectReferenceValue = button;
                cardSerializer.FindProperty("iconImage").objectReferenceValue = iconImage;
                cardSerializer.FindProperty("nameText").objectReferenceValue = nameText;
                cardSerializer.FindProperty("levelText").objectReferenceValue = levelText;
                cardSerializer.FindProperty("selectedMark").objectReferenceValue = selectedMarkGO;
                cardSerializer.FindProperty("dimOverlay").objectReferenceValue = dimOverlayObject;
                cardSerializer.FindProperty("unitId").stringValue = "";
                cardSerializer.ApplyModifiedProperties();

                cards[i] = cardView;
            }

            var topTransform = templateInstance.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name == "Top");
            if (topTransform != null)
            {
                var dropdownTransform = topTransform.GetComponentsInChildren<Transform>(true)
                    .FirstOrDefault(t => t.name.Contains("Dropdown"));
                if (dropdownTransform != null)
                {
                    dropdownTransform.gameObject.SetActive(false);
                }
            }

            var deckCountText = topTransform?.GetComponentsInChildren<TMP_Text>(true)
                .FirstOrDefault(txt => txt.text.Contains("/"));
            if (deckCountText != null)
            {
                deckCountText.gameObject.name = "DeckCountText";
                deckCountText.text = "10/10";
                deckCountText.font = font;
            }
            else
            {
                var deckCountTextObject = new GameObject("DeckCountText");
                deckCountTextObject.transform.SetParent(templateInstance.transform, false);
                var deckCountRect = deckCountTextObject.AddComponent<RectTransform>();
                deckCountRect.anchorMin = new Vector2(0.5f, 0);
                deckCountRect.anchorMax = new Vector2(0.5f, 0);
                deckCountRect.pivot = new Vector2(0.5f, 0);
                deckCountRect.anchoredPosition = new Vector2(0, 120);
                deckCountRect.sizeDelta = new Vector2(200, 50);

                deckCountText = deckCountTextObject.AddComponent<TextMeshProUGUI>();
                deckCountText.text = "10/10";
                deckCountText.fontSize = 36;
                deckCountText.alignment = TextAlignmentOptions.Center;
                deckCountText.font = font;
            }

            // 마지막 카드 줄이 CONFIRM/도크에 가리지 않게 그리드 하단 여백 확보
            var contentGrid = contentTransform != null ? contentTransform.GetComponent<GridLayoutGroup>() : null;
            if (contentGrid != null)
            {
                contentGrid.padding.bottom += 200;
            }

            var confirmButtonTransform = templateInstance.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name.Contains("Confirm"));
            Button confirmButton;
            if (confirmButtonTransform != null)
            {
                var confirmRect = confirmButtonTransform.GetComponent<RectTransform>();
                confirmRect.anchorMin = new Vector2(0.5f, 0);
                confirmRect.anchorMax = new Vector2(0.5f, 0);
                confirmRect.pivot = new Vector2(0.5f, 0);
                confirmRect.anchoredPosition = new Vector2(0, 40);
                confirmButton = EnsureButtonComponent(confirmButtonTransform.gameObject);
            }
            else
            {
                confirmButton = CreatePausePopupButton(templateInstance.transform, "ConfirmButton", new Vector2(0, 60), "CONFIRM", ButtonGreenPath, font);
                var createdConfirmRect = confirmButton.GetComponent<RectTransform>();
                createdConfirmRect.anchorMin = new Vector2(0.5f, 0);
                createdConfirmRect.anchorMax = new Vector2(0.5f, 0);
                createdConfirmRect.pivot = new Vector2(0.5f, 0);
                createdConfirmRect.anchoredPosition = new Vector2(0, 60);
            }

            // 덱 카운터는 좌상단 고정, 템플릿의 데모 카운터("7/30" 류)는 비활성
            if (deckCountText != null)
            {
                var deckCountRect = deckCountText.GetComponent<RectTransform>();
                deckCountRect.anchorMin = new Vector2(0, 1);
                deckCountRect.anchorMax = new Vector2(0, 1);
                deckCountRect.pivot = new Vector2(0, 1);
                deckCountRect.anchoredPosition = new Vector2(60, -90);
                deckCountRect.sizeDelta = new Vector2(240, 50);
                deckCountText.alignment = TextAlignmentOptions.Left;

                foreach (var demoCounter in templateInstance.GetComponentsInChildren<TMP_Text>(true))
                {
                    if (demoCounter != deckCountText && demoCounter.name != "LevelText" && demoCounter.name != "NameText" && demoCounter.text.Contains("/"))
                    {
                        demoCounter.gameObject.SetActive(false);
                    }
                }
            }

            var closeButtonTransform = templateInstance.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name.Contains("Close") || t.name.Contains("Back"));
            Button closeButton;
            if (closeButtonTransform != null)
            {
                closeButton = EnsureButtonComponent(closeButtonTransform.gameObject);
            }
            else
            {
                var closeButtonInstance = InstantiateKitPrefab(ButtonCirclePath, templateInstance.transform);
                if (closeButtonInstance != null)
                {
                    closeButtonInstance.name = "CloseButton";
                    var closeButtonRect = closeButtonInstance.GetComponent<RectTransform>();
                    closeButtonRect.anchorMin = new Vector2(1, 1);
                    closeButtonRect.anchorMax = new Vector2(1, 1);
                    closeButtonRect.pivot = new Vector2(1, 1);
                    closeButtonRect.anchoredPosition = new Vector2(-70, -70);
                    closeButtonRect.sizeDelta = new Vector2(90, 90);

                    var closeIconObject = new GameObject("CloseIcon");
                    closeIconObject.transform.SetParent(closeButtonInstance.transform, false);
                    var closeIconRect = closeIconObject.AddComponent<RectTransform>();
                    closeIconRect.anchorMin = new Vector2(0.5f, 0.5f);
                    closeIconRect.anchorMax = new Vector2(0.5f, 0.5f);
                    closeIconRect.pivot = new Vector2(0.5f, 0.5f);
                    closeIconRect.anchoredPosition = Vector2.zero;
                    closeIconRect.sizeDelta = new Vector2(50, 50);

                    var closeIconImage = closeIconObject.AddComponent<Image>();
                    var closeIcon = LoadSprite("Assets/Layer Lab/GUI Pro-MinimalGame/Shared/Icons/PictoIcon/128/arrow_back.png");
                    closeIconImage.sprite = closeIcon;
                    closeIconImage.raycastTarget = false;
                }

                closeButton = EnsureButtonComponent(closeButtonInstance);
            }

            var deckPanelView = templateInstance.AddComponent<UI_DeckPanelView>();
            var serializedObject = new SerializedObject(deckPanelView);
            serializedObject.FindProperty("root").objectReferenceValue = templateInstance;
            serializedObject.FindProperty("unitCards").arraySize = cards.Length;
            for (var i = 0; i < cards.Length; i++)
            {
                serializedObject.FindProperty("unitCards").GetArrayElementAtIndex(i).objectReferenceValue = cards[i];
            }
            serializedObject.FindProperty("deckCountText").objectReferenceValue = deckCountText;
            serializedObject.FindProperty("confirmButton").objectReferenceValue = confirmButton;
            serializedObject.FindProperty("closeButton").objectReferenceValue = closeButton;
            serializedObject.ApplyModifiedProperties();

            templateInstance.SetActive(false);

            return templateInstance;
        }

        private static GameObject CreateDeckPanelFallback(RectTransform parent, TMP_FontAsset font, Sprite starIcon)
        {
            var panelObject = new GameObject("DeckPanel");
            panelObject.transform.SetParent(parent, false);

            var panelRect = panelObject.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 0.22f);
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            var bgImage = panelObject.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);

            AddPanelBackgroundFrame(panelObject);

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

            var closeButtonInstance = InstantiateKitPrefab(ButtonCirclePath, panelObject.transform);
            if (closeButtonInstance != null)
            {
                closeButtonInstance.name = "CloseButton";
                var closeButtonRect = closeButtonInstance.GetComponent<RectTransform>();
                closeButtonRect.anchorMin = new Vector2(1, 1);
                closeButtonRect.anchorMax = new Vector2(1, 1);
                closeButtonRect.pivot = new Vector2(1, 1);
                closeButtonRect.anchoredPosition = new Vector2(-70, -70);
                closeButtonRect.sizeDelta = new Vector2(90, 90);

                var closeIconObject = new GameObject("CloseIcon");
                closeIconObject.transform.SetParent(closeButtonInstance.transform, false);
                var closeIconRect = closeIconObject.AddComponent<RectTransform>();
                closeIconRect.anchorMin = new Vector2(0.5f, 0.5f);
                closeIconRect.anchorMax = new Vector2(0.5f, 0.5f);
                closeIconRect.pivot = new Vector2(0.5f, 0.5f);
                closeIconRect.anchoredPosition = Vector2.zero;
                closeIconRect.sizeDelta = new Vector2(50, 50);

                var closeIconImage = closeIconObject.AddComponent<Image>();
                var closeIcon = LoadSprite("Assets/Layer Lab/GUI Pro-MinimalGame/Shared/Icons/PictoIcon/128/arrow_back.png");
                closeIconImage.sprite = closeIcon;
                closeIconImage.raycastTarget = false;
            }

            var closeButton = EnsureButtonComponent(closeButtonInstance);

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
            serializedObject.FindProperty("closeButton").objectReferenceValue = closeButton;
            serializedObject.ApplyModifiedProperties();

            panelObject.SetActive(false);

            return panelObject;
        }

        private static GameObject CreateUpgradePanel(RectTransform parent, TMP_FontAsset font, Sprite starIcon, Sprite goldIcon, Sprite arrowIcon)
        {
            var panelObject = new GameObject("UpgradePanel");
            panelObject.transform.SetParent(parent, false);

            var panelRect = panelObject.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 0.22f);
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            var bgImage = panelObject.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);

            AddPanelBackgroundFrame(panelObject);

            var cards = new UI_UnitCardView[15];
            const int columns = 5;
            const int rows = 3;
            const float cardWidth = 130f;
            const float cardHeight = 160f;
            const float spacingX = 133f;
            const float spacingY = 180f;
            const float startX = -385f;
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

            var detailChildren = detailPanel.GetComponentsInChildren<Transform>(true);
            var detailNameText = detailChildren.FirstOrDefault(t => t.name == "DetailNameText")?.GetComponent<TMP_Text>();
            var levelText = detailChildren.FirstOrDefault(t => t.name == "LevelText")?.GetComponent<TMP_Text>();
            var hpText = detailChildren.FirstOrDefault(t => t.name == "HPText")?.GetComponent<TMP_Text>();
            var attackText = detailChildren.FirstOrDefault(t => t.name == "AttackText")?.GetComponent<TMP_Text>();
            var costText = detailChildren.FirstOrDefault(t => t.name == "CostText")?.GetComponent<TMP_Text>();
            var upgradeButton = detailChildren.FirstOrDefault(t => t.name == "UpgradeButton")?.GetComponent<Button>();
            var maxLevelMark = detailChildren.FirstOrDefault(t => t.name == "MaxLevelMark")?.gameObject;

            var closeButtonInstance = InstantiateKitPrefab(ButtonCirclePath, panelObject.transform);
            if (closeButtonInstance != null)
            {
                closeButtonInstance.name = "CloseButton";
                var closeButtonRect = closeButtonInstance.GetComponent<RectTransform>();
                closeButtonRect.anchorMin = new Vector2(1, 1);
                closeButtonRect.anchorMax = new Vector2(1, 1);
                closeButtonRect.pivot = new Vector2(1, 1);
                closeButtonRect.anchoredPosition = new Vector2(-70, -70);
                closeButtonRect.sizeDelta = new Vector2(90, 90);

                var closeIconObject = new GameObject("CloseIcon");
                closeIconObject.transform.SetParent(closeButtonInstance.transform, false);
                var closeIconRect = closeIconObject.AddComponent<RectTransform>();
                closeIconRect.anchorMin = new Vector2(0.5f, 0.5f);
                closeIconRect.anchorMax = new Vector2(0.5f, 0.5f);
                closeIconRect.pivot = new Vector2(0.5f, 0.5f);
                closeIconRect.anchoredPosition = Vector2.zero;
                closeIconRect.sizeDelta = new Vector2(50, 50);

                var closeIconImage = closeIconObject.AddComponent<Image>();
                var closeIcon = LoadSprite("Assets/Layer Lab/GUI Pro-MinimalGame/Shared/Icons/PictoIcon/128/arrow_back.png");
                closeIconImage.sprite = closeIcon;
                closeIconImage.raycastTarget = false;
            }

            var closeButton = EnsureButtonComponent(closeButtonInstance);

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
            serializedObject.FindProperty("closeButton").objectReferenceValue = closeButton;
            serializedObject.ApplyModifiedProperties();

            panelObject.SetActive(false);

            return panelObject;
        }

        private static UI_UnitCardView CreateUnitCard(RectTransform parent, Vector2 position, Vector2 size, TMP_FontAsset font, Sprite starIcon, string cardName)
        {
            const string templatePath = KitRoot + "Theme_Light/Prefabs/Prefabs~DemoLayout/ListItem_Hero_02.prefab";
            var templateInstance = InstantiateKitPrefab(templatePath, parent);

            if (templateInstance == null)
            {
                GameLogger.LogWarning($"[UIPrefabBuilder] ListItem_Hero_02 not found, using fallback for {cardName}.");
                return CreateUnitCardFallback(parent, position, size, font, starIcon, cardName);
            }

            templateInstance.name = cardName;
            var cardRect = templateInstance.GetComponent<RectTransform>();
            cardRect.anchorMin = new Vector2(0.5f, 0.5f);
            cardRect.anchorMax = new Vector2(0.5f, 0.5f);
            cardRect.pivot = new Vector2(0.5f, 0.5f);
            cardRect.anchoredPosition = position;
            cardRect.sizeDelta = size;

            var iconCheckTransform = templateInstance.transform.Find("Icon_Check");
            var nameText = templateInstance.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name == "Text_Name")?.GetComponent<TMP_Text>();
            var star1 = templateInstance.transform.Find("Star (1)");
            var star2 = templateInstance.transform.Find("Star (2)");
            var levelTransform = templateInstance.transform.Find("Level");
            var roleArea = templateInstance.transform.Find("RoleArea");

            if (star1 != null)
            {
                star1.gameObject.SetActive(false);
            }
            if (star2 != null)
            {
                star2.gameObject.SetActive(false);
            }
            if (roleArea != null)
            {
                roleArea.gameObject.SetActive(false);
            }

            var button = EnsureButtonComponent(templateInstance);

            var iconObject = new GameObject("UnitIcon");
            iconObject.transform.SetParent(templateInstance.transform, false);
            var iconRect = iconObject.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.5f, 1);
            iconRect.anchorMax = new Vector2(0.5f, 1);
            iconRect.pivot = new Vector2(0.5f, 1);
            iconRect.anchoredPosition = new Vector2(0, -10);
            iconRect.sizeDelta = new Vector2(size.x * 0.6f, size.x * 0.6f);

            var iconImage = iconObject.AddComponent<Image>();
            iconImage.raycastTarget = false;

            TMP_Text levelText = null;
            if (levelTransform != null)
            {
                levelText = levelTransform.GetComponentInChildren<TMP_Text>();
                if (levelText != null)
                {
                    levelText.text = "Lv.1";
                    levelText.font = font;
                }
            }

            if (levelText == null)
            {
                var levelTextObject = new GameObject("LevelText");
                levelTextObject.transform.SetParent(templateInstance.transform, false);
                var levelTextRect = levelTextObject.AddComponent<RectTransform>();
                levelTextRect.anchorMin = new Vector2(0, 0);
                levelTextRect.anchorMax = new Vector2(1, 0);
                levelTextRect.pivot = new Vector2(0.5f, 0);
                levelTextRect.anchoredPosition = new Vector2(0, 5);
                levelTextRect.sizeDelta = new Vector2(-10, 25);

                levelText = levelTextObject.AddComponent<TextMeshProUGUI>();
                levelText.text = "Lv.1";
                levelText.fontSize = 18;
                levelText.alignment = TextAlignmentOptions.Center;
                levelText.font = font;
            }

            if (nameText != null)
            {
                nameText.text = "Unit";
                nameText.font = font;
            }

            GameObject selectedMarkObject = null;
            if (iconCheckTransform != null)
            {
                selectedMarkObject = iconCheckTransform.gameObject;
                selectedMarkObject.SetActive(false);
            }
            else
            {
                selectedMarkObject = new GameObject("SelectedMark");
                selectedMarkObject.transform.SetParent(templateInstance.transform, false);
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
            }

            var dimOverlayObject = new GameObject("DimOverlay");
            dimOverlayObject.transform.SetParent(templateInstance.transform, false);
            var dimOverlayRect = dimOverlayObject.AddComponent<RectTransform>();
            dimOverlayRect.anchorMin = Vector2.zero;
            dimOverlayRect.anchorMax = Vector2.one;
            dimOverlayRect.offsetMin = Vector2.zero;
            dimOverlayRect.offsetMax = Vector2.zero;

            var dimOverlayImage = dimOverlayObject.AddComponent<Image>();
            dimOverlayImage.color = new Color(0, 0, 0, 0.5f);
            dimOverlayImage.raycastTarget = false;
            dimOverlayObject.SetActive(false);

            var cardView = templateInstance.AddComponent<UI_UnitCardView>();
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

        private static UI_UnitCardView CreateUnitCardFallback(RectTransform parent, Vector2 position, Vector2 size, TMP_FontAsset font, Sprite starIcon, string cardName)
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
            const string templatePath = KitRoot + "Theme_Light/Prefabs/Prefabs~DemoScenes/Character_Hero_Detail_02_Common.prefab";
            var templateInstance = InstantiateKitPrefab(templatePath, parent);

            if (templateInstance == null)
            {
                GameLogger.LogWarning("[UIPrefabBuilder] Character_Hero_Detail_02_Common not found, using fallback.");
                return CreateUpgradeDetailPanelFallback(parent, font, goldIcon, arrowIcon);
            }

            templateInstance.name = "DetailPanel";
            var detailRect = templateInstance.GetComponent<RectTransform>();
            detailRect.anchorMin = new Vector2(0.5f, 0.5f);
            detailRect.anchorMax = new Vector2(0.5f, 0.5f);
            detailRect.pivot = new Vector2(0.5f, 0.5f);
            detailRect.anchoredPosition = Vector2.zero;
            detailRect.sizeDelta = new Vector2(600, 900);

            var allTexts = templateInstance.GetComponentsInChildren<TMP_Text>(true);
            TMP_Text detailNameText = allTexts.FirstOrDefault(t => t.name.Contains("Name") || t.name.Contains("Title"));
            TMP_Text levelText = allTexts.FirstOrDefault(t => t.name.Contains("Level"));

            if (detailNameText == null)
            {
                var nameObject = new GameObject("DetailNameText");
                nameObject.transform.SetParent(templateInstance.transform, false);
                var nameRect = nameObject.AddComponent<RectTransform>();
                nameRect.anchorMin = new Vector2(0.5f, 1);
                nameRect.anchorMax = new Vector2(0.5f, 1);
                nameRect.pivot = new Vector2(0.5f, 1);
                nameRect.anchoredPosition = new Vector2(0, -80);
                nameRect.sizeDelta = new Vector2(500, 60);
                detailNameText = nameObject.AddComponent<TextMeshProUGUI>();
                detailNameText.fontSize = 40;
                detailNameText.alignment = TextAlignmentOptions.Center;
            }
            else
            {
                detailNameText.gameObject.name = "DetailNameText";
            }
            detailNameText.text = "Unit Name";
            detailNameText.font = font;

            if (levelText == null)
            {
                var lvlObject = new GameObject("LevelText");
                lvlObject.transform.SetParent(templateInstance.transform, false);
                var lvlRect = lvlObject.AddComponent<RectTransform>();
                lvlRect.anchorMin = new Vector2(0.5f, 1);
                lvlRect.anchorMax = new Vector2(0.5f, 1);
                lvlRect.pivot = new Vector2(0.5f, 1);
                lvlRect.anchoredPosition = new Vector2(0, -150);
                lvlRect.sizeDelta = new Vector2(400, 40);
                levelText = lvlObject.AddComponent<TextMeshProUGUI>();
                levelText.fontSize = 32;
                levelText.alignment = TextAlignmentOptions.Center;
            }
            else
            {
                levelText.gameObject.name = "LevelText";
            }
            levelText.text = "Lv.1";
            levelText.font = font;

            var statsListTransform = templateInstance.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name.Contains("StatsList") || t.name.Contains("Group_Stats"));
            TMP_Text hpText = null;
            TMP_Text attackText = null;

            if (statsListTransform != null)
            {
                var statTexts = statsListTransform.GetComponentsInChildren<TMP_Text>(true).ToArray();
                if (statTexts.Length >= 2)
                {
                    hpText = statTexts[0];
                    hpText.gameObject.name = "HPText";
                    hpText.text = "HP: 100 -> 120";
                    hpText.font = font;

                    attackText = statTexts[1];
                    attackText.gameObject.name = "AttackText";
                    attackText.text = "ATK: 10 -> 12";
                    attackText.font = font;
                }

                for (var i = 2; i < statTexts.Length; i++)
                {
                    statTexts[i].gameObject.SetActive(false);
                }
            }

            if (hpText == null)
            {
                var hpObject = new GameObject("HPText");
                hpObject.transform.SetParent(templateInstance.transform, false);
                var hpRect = hpObject.AddComponent<RectTransform>();
                hpRect.anchorMin = new Vector2(0.5f, 0.5f);
                hpRect.anchorMax = new Vector2(0.5f, 0.5f);
                hpRect.pivot = new Vector2(0.5f, 0.5f);
                hpRect.anchoredPosition = new Vector2(0, 80);
                hpRect.sizeDelta = new Vector2(500, 40);
                hpText = hpObject.AddComponent<TextMeshProUGUI>();
                hpText.text = "HP: 100 -> 120";
                hpText.fontSize = 28;
                hpText.alignment = TextAlignmentOptions.Center;
                hpText.font = font;
            }

            if (attackText == null)
            {
                var atkObject = new GameObject("AttackText");
                atkObject.transform.SetParent(templateInstance.transform, false);
                var atkRect = atkObject.AddComponent<RectTransform>();
                atkRect.anchorMin = new Vector2(0.5f, 0.5f);
                atkRect.anchorMax = new Vector2(0.5f, 0.5f);
                atkRect.pivot = new Vector2(0.5f, 0.5f);
                atkRect.anchoredPosition = new Vector2(0, 30);
                atkRect.sizeDelta = new Vector2(500, 40);
                attackText = atkObject.AddComponent<TextMeshProUGUI>();
                attackText.text = "ATK: 10 -> 12";
                attackText.fontSize = 28;
                attackText.alignment = TextAlignmentOptions.Center;
                attackText.font = font;
            }

            var skillInfoTransform = templateInstance.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name.Contains("SkillInfo") || t.name.Contains("Skill"));
            if (skillInfoTransform != null)
            {
                skillInfoTransform.gameObject.SetActive(false);
            }

            var combatAreaTransform = templateInstance.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name.Contains("Combat") || t.name.Contains("ButtonArea"));
            Button upgradeButton = null;

            if (combatAreaTransform != null)
            {
                var combatButton = combatAreaTransform.GetComponentInChildren<Button>(true);
                if (combatButton != null)
                {
                    upgradeButton = combatButton;
                    upgradeButton.gameObject.name = "UpgradeButton";
                    var btnText = upgradeButton.GetComponentInChildren<TMP_Text>();
                    if (btnText != null)
                    {
                        btnText.text = "UPGRADE";
                        btnText.font = font;
                    }
                }
                else
                {
                    upgradeButton = EnsureButtonComponent(combatAreaTransform.gameObject);
                    upgradeButton.gameObject.name = "UpgradeButton";
                }
            }

            if (upgradeButton == null)
            {
                upgradeButton = CreatePausePopupButton(templateInstance.transform, "UpgradeButton", new Vector2(0, -300), "UPGRADE", ButtonGreenPath, font);
            }

            TMP_Text costText = null;
            var nearButtonTexts = upgradeButton.GetComponentsInChildren<TMP_Text>(true)
                .Where(t => t.gameObject != upgradeButton.gameObject)
                .ToArray();

            if (nearButtonTexts.Length > 0)
            {
                costText = nearButtonTexts[0];
                costText.gameObject.name = "CostText";
                costText.text = "100";
                costText.font = font;
            }
            else
            {
                var costObject = new GameObject("CostText");
                costObject.transform.SetParent(templateInstance.transform, false);
                var costRect = costObject.AddComponent<RectTransform>();
                costRect.anchorMin = new Vector2(0.5f, 0);
                costRect.anchorMax = new Vector2(0.5f, 0);
                costRect.pivot = new Vector2(0.5f, 0);
                costRect.anchoredPosition = new Vector2(40, 200);
                costRect.sizeDelta = new Vector2(100, 40);
                costText = costObject.AddComponent<TextMeshProUGUI>();
                costText.text = "100";
                costText.fontSize = 32;
                costText.alignment = TextAlignmentOptions.Left;
                costText.font = font;
            }

            var maxLevelMarkObject = new GameObject("MaxLevelMark");
            maxLevelMarkObject.transform.SetParent(templateInstance.transform, false);
            var maxLevelMarkRect = maxLevelMarkObject.AddComponent<RectTransform>();
            maxLevelMarkRect.anchorMin = new Vector2(0.5f, 0);
            maxLevelMarkRect.anchorMax = new Vector2(0.5f, 0);
            maxLevelMarkRect.pivot = new Vector2(0.5f, 0);
            maxLevelMarkRect.anchoredPosition = new Vector2(0, 200);
            maxLevelMarkRect.sizeDelta = new Vector2(400, 80);
            var maxLevelMarkText = maxLevelMarkObject.AddComponent<TextMeshProUGUI>();
            maxLevelMarkText.text = "MAX LEVEL";
            maxLevelMarkText.fontSize = 40;
            maxLevelMarkText.alignment = TextAlignmentOptions.Center;
            maxLevelMarkText.font = font;
            maxLevelMarkText.color = new Color(1f, 0.8f, 0.2f);
            maxLevelMarkObject.SetActive(false);

            return templateInstance;
        }

        private static GameObject CreateUpgradeDetailPanelFallback(RectTransform parent, TMP_FontAsset font, Sprite goldIcon, Sprite arrowIcon)
        {
            var detailPanel = new GameObject("DetailPanel");
            detailPanel.transform.SetParent(parent, false);

            var detailRect = detailPanel.AddComponent<RectTransform>();
            detailRect.anchorMin = new Vector2(1, 0.5f);
            detailRect.anchorMax = new Vector2(1, 0.5f);
            detailRect.pivot = new Vector2(1, 0.5f);
            detailRect.anchoredPosition = new Vector2(-20, 0);
            detailRect.sizeDelta = new Vector2(300, 600);

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

        private static GameObject CreateShopPanel(RectTransform parent, TMP_FontAsset font, Sprite goldIcon)
        {
            const string templatePath = KitRoot + "Theme_Light/Prefabs/Prefabs~DemoScenes/Shop_List.prefab";
            var templateInstance = InstantiateKitPrefab(templatePath, parent);

            if (templateInstance == null)
            {
                GameLogger.LogWarning("[UIPrefabBuilder] Shop_List not found, using fallback.");
                return CreateShopPanelFallback(parent, font, goldIcon);
            }

            templateInstance.name = "ShopPanel";
            var panelRect = templateInstance.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 0.22f);
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            DisableDemoDescendants(templateInstance, new[] { "ResourceBar", "Tab_", "Dropdown", "Option", "Button_Info" });

            var topTransform = templateInstance.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name == "Top");
            if (topTransform != null)
            {
                topTransform.gameObject.SetActive(false);
            }

            var titleGemTransform = templateInstance.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name.Contains("Title_Gem"));
            if (titleGemTransform != null)
            {
                titleGemTransform.gameObject.SetActive(false);
            }

            var groupGem1 = templateInstance.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name.Contains("Group_Gem1"));
            if (groupGem1 != null)
            {
                groupGem1.gameObject.SetActive(false);
            }

            var groupGem2 = templateInstance.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name.Contains("Group_Gem2"));
            if (groupGem2 != null)
            {
                groupGem2.gameObject.SetActive(false);
            }

            var titleGold = templateInstance.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name.Contains("Title_Gold"));
            if (titleGold != null)
            {
                titleGold.gameObject.SetActive(false);
            }

            var groupGold = templateInstance.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name.Contains("Group_Gold"));
            if (groupGold != null)
            {
                groupGold.gameObject.SetActive(false);
            }

            var titleSilver = templateInstance.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name.Contains("Title_Silver"));
            if (titleSilver != null)
            {
                titleSilver.gameObject.SetActive(false);
            }

            var groupSilver = templateInstance.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name.Contains("Group_Silver"));
            if (groupSilver != null)
            {
                groupSilver.gameObject.SetActive(false);
            }

            var titleDailyDeals = templateInstance.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name.Contains("Title_DailyDeals"));
            if (titleDailyDeals != null)
            {
                titleDailyDeals.gameObject.SetActive(false);
            }

            var groupChest = templateInstance.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name.Contains("Group_Chest"));
            if (groupChest != null)
            {
                groupChest.gameObject.SetActive(false);
            }

            var bottomTransform = templateInstance.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name == "Bottom");
            if (bottomTransform != null)
            {
                bottomTransform.gameObject.SetActive(false);
            }

            var groupItemTransform = templateInstance.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name == "Group_Item");

            if (groupItemTransform == null)
            {
                GameLogger.LogWarning("[UIPrefabBuilder] Group_Item not found in Shop_List, using fallback.");
                Object.DestroyImmediate(templateInstance);
                return CreateShopPanelFallback(parent, font, goldIcon);
            }

            var demoItems = groupItemTransform.GetComponentsInChildren<Transform>(true)
                .Where(t => t != groupItemTransform && t.parent == groupItemTransform && t.GetComponent<RectTransform>() != null)
                .Select(t => t.gameObject)
                .ToArray();

            if (demoItems.Length == 0)
            {
                GameLogger.LogWarning("[UIPrefabBuilder] No demo items found in Group_Item, using fallback.");
                Object.DestroyImmediate(templateInstance);
                return CreateShopPanelFallback(parent, font, goldIcon);
            }

            var firstItem = demoItems[0];
            var shopItems = new UI_ShopItemView[5];

            for (var i = 0; i < 5; i++)
            {
                GameObject itemInstance;
                if (i < demoItems.Length)
                {
                    itemInstance = demoItems[i];
                }
                else
                {
                    itemInstance = Object.Instantiate(firstItem, groupItemTransform);
                }

                itemInstance.name = $"ShopItem_{i}";
                itemInstance.SetActive(true);

                var iconImage = itemInstance.GetComponentsInChildren<Image>(true)
                    .FirstOrDefault(img => img.name.Contains("Icon") && !img.name.Contains("Button"));
                var nameText = itemInstance.GetComponentsInChildren<TMP_Text>(true)
                    .FirstOrDefault(txt => txt.name.Contains("Title") || txt.name.Contains("Name"));
                var buttonArea = itemInstance.GetComponentsInChildren<Transform>(true)
                    .FirstOrDefault(t => t.name.Contains("Button"));

                if (iconImage == null)
                {
                    var iconObject = new GameObject("UnitIcon");
                    iconObject.transform.SetParent(itemInstance.transform, false);
                    var iconRect = iconObject.AddComponent<RectTransform>();
                    iconRect.anchorMin = new Vector2(0, 0.5f);
                    iconRect.anchorMax = new Vector2(0, 0.5f);
                    iconRect.pivot = new Vector2(0, 0.5f);
                    iconRect.anchoredPosition = new Vector2(20, 0);
                    iconRect.sizeDelta = new Vector2(100, 100);
                    iconImage = iconObject.AddComponent<Image>();
                    iconImage.raycastTarget = false;
                }
                else
                {
                    iconImage.gameObject.name = "UnitIcon";
                }

                if (nameText == null)
                {
                    var nameObject = new GameObject("NameText");
                    nameObject.transform.SetParent(itemInstance.transform, false);
                    var nameRect = nameObject.AddComponent<RectTransform>();
                    nameRect.anchorMin = new Vector2(0, 0.5f);
                    nameRect.anchorMax = new Vector2(0, 0.5f);
                    nameRect.pivot = new Vector2(0, 0.5f);
                    nameRect.anchoredPosition = new Vector2(140, 0);
                    nameRect.sizeDelta = new Vector2(200, 50);
                    nameText = nameObject.AddComponent<TextMeshProUGUI>();
                    nameText.fontSize = 32;
                    nameText.alignment = TextAlignmentOptions.Left;
                }
                else
                {
                    nameText.gameObject.name = "NameText";
                }
                nameText.text = "Unit Name";
                nameText.font = font;

                Button buyButton = null;
                TMP_Text priceText = null;

                if (buttonArea != null)
                {
                    buyButton = EnsureButtonComponent(buttonArea.gameObject);
                    priceText = buttonArea.GetComponentInChildren<TMP_Text>();
                    if (priceText != null)
                    {
                        priceText.gameObject.name = "PriceText";
                        priceText.text = "100";
                        priceText.font = font;
                    }
                }

                if (buyButton == null)
                {
                    var btnInstance = InstantiateKitPrefab(ButtonGreenPath, itemInstance.transform);
                    if (btnInstance != null)
                    {
                        btnInstance.name = "BuyButton";
                        var btnRect = btnInstance.GetComponent<RectTransform>();
                        btnRect.anchorMin = new Vector2(1, 0.5f);
                        btnRect.anchorMax = new Vector2(1, 0.5f);
                        btnRect.pivot = new Vector2(1, 0.5f);
                        btnRect.anchoredPosition = new Vector2(-30, 0);
                        btnRect.sizeDelta = new Vector2(150, 70);
                        buyButton = EnsureButtonComponent(btnInstance);

                        var btnText = btnInstance.GetComponentInChildren<TMP_Text>();
                        if (btnText != null)
                        {
                            btnText.text = "BUY";
                            btnText.font = font;
                        }
                    }
                }

                if (priceText == null)
                {
                    var priceObject = new GameObject("PriceText");
                    priceObject.transform.SetParent(itemInstance.transform, false);
                    var priceRect = priceObject.AddComponent<RectTransform>();
                    priceRect.anchorMin = new Vector2(1, 0.5f);
                    priceRect.anchorMax = new Vector2(1, 0.5f);
                    priceRect.pivot = new Vector2(1, 0.5f);
                    priceRect.anchoredPosition = new Vector2(-200, 0);
                    priceRect.sizeDelta = new Vector2(100, 40);
                    priceText = priceObject.AddComponent<TextMeshProUGUI>();
                    priceText.text = "100";
                    priceText.fontSize = 28;
                    priceText.alignment = TextAlignmentOptions.Center;
                    priceText.font = font;
                }

                var ownedMarkObject = new GameObject("OwnedMark");
                ownedMarkObject.transform.SetParent(itemInstance.transform, false);
                var ownedMarkRect = ownedMarkObject.AddComponent<RectTransform>();
                ownedMarkRect.anchorMin = new Vector2(1, 0.5f);
                ownedMarkRect.anchorMax = new Vector2(1, 0.5f);
                ownedMarkRect.pivot = new Vector2(1, 0.5f);
                ownedMarkRect.anchoredPosition = new Vector2(-60, 0);
                ownedMarkRect.sizeDelta = new Vector2(120, 50);
                var ownedMarkText = ownedMarkObject.AddComponent<TextMeshProUGUI>();
                ownedMarkText.text = "OWNED";
                ownedMarkText.fontSize = 28;
                ownedMarkText.alignment = TextAlignmentOptions.Center;
                ownedMarkText.font = font;
                ownedMarkText.color = new Color(0.3f, 0.8f, 0.3f);
                ownedMarkObject.SetActive(false);

                var shopItemView = itemInstance.AddComponent<UI_ShopItemView>();
                var itemSerializer = new SerializedObject(shopItemView);
                itemSerializer.FindProperty("iconImage").objectReferenceValue = iconImage;
                itemSerializer.FindProperty("nameText").objectReferenceValue = nameText;
                itemSerializer.FindProperty("priceText").objectReferenceValue = priceText;
                itemSerializer.FindProperty("buyButton").objectReferenceValue = buyButton;
                itemSerializer.FindProperty("ownedMark").objectReferenceValue = ownedMarkObject;
                itemSerializer.ApplyModifiedProperties();

                shopItems[i] = shopItemView;
            }

            var closeButtonInstance = InstantiateKitPrefab(ButtonCirclePath, templateInstance.transform);
            if (closeButtonInstance != null)
            {
                closeButtonInstance.name = "CloseButton";
                var closeButtonRect = closeButtonInstance.GetComponent<RectTransform>();
                closeButtonRect.anchorMin = new Vector2(1, 1);
                closeButtonRect.anchorMax = new Vector2(1, 1);
                closeButtonRect.pivot = new Vector2(1, 1);
                closeButtonRect.anchoredPosition = new Vector2(-70, -70);
                closeButtonRect.sizeDelta = new Vector2(90, 90);

                var closeIconObject = new GameObject("CloseIcon");
                closeIconObject.transform.SetParent(closeButtonInstance.transform, false);
                var closeIconRect = closeIconObject.AddComponent<RectTransform>();
                closeIconRect.anchorMin = new Vector2(0.5f, 0.5f);
                closeIconRect.anchorMax = new Vector2(0.5f, 0.5f);
                closeIconRect.pivot = new Vector2(0.5f, 0.5f);
                closeIconRect.anchoredPosition = Vector2.zero;
                closeIconRect.sizeDelta = new Vector2(50, 50);

                var closeIconImage = closeIconObject.AddComponent<Image>();
                var closeIcon = LoadSprite("Assets/Layer Lab/GUI Pro-MinimalGame/Shared/Icons/PictoIcon/128/arrow_back.png");
                closeIconImage.sprite = closeIcon;
                closeIconImage.raycastTarget = false;
            }

            var closeButton = EnsureButtonComponent(closeButtonInstance);

            var shopPanelView = templateInstance.AddComponent<UI_ShopPanelView>();
            var serializedObject = new SerializedObject(shopPanelView);
            serializedObject.FindProperty("root").objectReferenceValue = templateInstance;
            serializedObject.FindProperty("shopItems").arraySize = shopItems.Length;
            for (var i = 0; i < shopItems.Length; i++)
            {
                serializedObject.FindProperty("shopItems").GetArrayElementAtIndex(i).objectReferenceValue = shopItems[i];
            }
            serializedObject.FindProperty("closeButton").objectReferenceValue = closeButton;
            serializedObject.ApplyModifiedProperties();

            templateInstance.SetActive(false);

            return templateInstance;
        }

        private static GameObject CreateShopPanelFallback(RectTransform parent, TMP_FontAsset font, Sprite goldIcon)
        {
            var panelObject = new GameObject("ShopPanel");
            panelObject.transform.SetParent(parent, false);

            var panelRect = panelObject.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 0.22f);
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            var bgImage = panelObject.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);

            AddPanelBackgroundFrame(panelObject);

            var shopItems = new UI_ShopItemView[5];
            const float itemHeight = 120f;
            const float itemWidth = 700f;
            const float spacingY = 140f;
            const float startY = 250f;

            for (var i = 0; i < 5; i++)
            {
                var y = startY - i * spacingY;
                shopItems[i] = CreateShopItem(panelRect, new Vector2(0, y), new Vector2(itemWidth, itemHeight), font, goldIcon, i);
            }

            var closeButtonInstance = InstantiateKitPrefab(ButtonCirclePath, panelObject.transform);
            if (closeButtonInstance != null)
            {
                closeButtonInstance.name = "CloseButton";
                var closeButtonRect = closeButtonInstance.GetComponent<RectTransform>();
                closeButtonRect.anchorMin = new Vector2(1, 1);
                closeButtonRect.anchorMax = new Vector2(1, 1);
                closeButtonRect.pivot = new Vector2(1, 1);
                closeButtonRect.anchoredPosition = new Vector2(-70, -70);
                closeButtonRect.sizeDelta = new Vector2(90, 90);

                var closeIconObject = new GameObject("CloseIcon");
                closeIconObject.transform.SetParent(closeButtonInstance.transform, false);
                var closeIconRect = closeIconObject.AddComponent<RectTransform>();
                closeIconRect.anchorMin = new Vector2(0.5f, 0.5f);
                closeIconRect.anchorMax = new Vector2(0.5f, 0.5f);
                closeIconRect.pivot = new Vector2(0.5f, 0.5f);
                closeIconRect.anchoredPosition = Vector2.zero;
                closeIconRect.sizeDelta = new Vector2(50, 50);

                var closeIconImage = closeIconObject.AddComponent<Image>();
                var closeIcon = LoadSprite("Assets/Layer Lab/GUI Pro-MinimalGame/Shared/Icons/PictoIcon/128/arrow_back.png");
                closeIconImage.sprite = closeIcon;
                closeIconImage.raycastTarget = false;
            }

            var closeButton = EnsureButtonComponent(closeButtonInstance);

            var shopPanelView = panelObject.AddComponent<UI_ShopPanelView>();
            var serializedObject = new SerializedObject(shopPanelView);
            serializedObject.FindProperty("root").objectReferenceValue = panelObject;
            serializedObject.FindProperty("shopItems").arraySize = shopItems.Length;
            for (var i = 0; i < shopItems.Length; i++)
            {
                serializedObject.FindProperty("shopItems").GetArrayElementAtIndex(i).objectReferenceValue = shopItems[i];
            }
            serializedObject.FindProperty("closeButton").objectReferenceValue = closeButton;
            serializedObject.ApplyModifiedProperties();

            panelObject.SetActive(false);

            return panelObject;
        }

        private static UI_ShopItemView CreateShopItem(RectTransform parent, Vector2 position, Vector2 size, TMP_FontAsset font, Sprite goldIcon, int index)
        {
            const string templatePath = KitRoot + "Theme_Light/Prefabs/Prefabs~DemoLayout/ListItem_ShopItem.prefab";
            var templateInstance = InstantiateKitPrefab(templatePath, parent);

            if (templateInstance == null)
            {
                GameLogger.LogWarning($"[UIPrefabBuilder] ListItem_ShopItem not found, using fallback for ShopItem_{index}.");
                return CreateShopItemFallback(parent, position, size, font, goldIcon, index);
            }

            templateInstance.name = $"ShopItem_{index}";
            var itemRect = templateInstance.GetComponent<RectTransform>();
            itemRect.anchorMin = new Vector2(0.5f, 0.5f);
            itemRect.anchorMax = new Vector2(0.5f, 0.5f);
            itemRect.pivot = new Vector2(0.5f, 0.5f);
            itemRect.anchoredPosition = position;
            itemRect.sizeDelta = size;

            var icon = templateInstance.transform.Find("Icon")?.GetComponent<Image>();
            var nameText = templateInstance.transform.Find("Text_Title")?.GetComponent<TMP_Text>();
            var priceButtonTransform = templateInstance.transform.Find("Button_Price");
            var textItemNum = templateInstance.transform.Find("Text_ItemNum");
            var textLimit = templateInstance.transform.Find("Text_Limit");
            var groupArea = templateInstance.transform.Find("Group/GroupArea");

            if (textItemNum != null)
            {
                textItemNum.gameObject.SetActive(false);
            }
            if (textLimit != null)
            {
                textLimit.gameObject.SetActive(false);
            }
            if (groupArea != null)
            {
                groupArea.gameObject.SetActive(false);
            }

            TMP_Text priceText = null;
            if (priceButtonTransform != null)
            {
                priceText = priceButtonTransform.GetComponentInChildren<TMP_Text>();
                if (priceText != null)
                {
                    priceText.font = font;
                }
            }
            else
            {
                var textTMP = templateInstance.transform.Find("Text (TMP)")?.GetComponent<TMP_Text>();
                if (textTMP != null)
                {
                    priceText = textTMP;
                    priceText.font = font;
                }
            }

            Button buyButton = null;
            if (priceButtonTransform != null)
            {
                buyButton = EnsureButtonComponent(priceButtonTransform.gameObject);
            }

            if (nameText != null)
            {
                nameText.text = "Unit Name";
                nameText.font = font;
            }

            var ownedMarkObject = new GameObject("OwnedMark");
            ownedMarkObject.transform.SetParent(templateInstance.transform, false);
            var ownedMarkRect = ownedMarkObject.AddComponent<RectTransform>();
            ownedMarkRect.anchorMin = new Vector2(1, 0.5f);
            ownedMarkRect.anchorMax = new Vector2(1, 0.5f);
            ownedMarkRect.pivot = new Vector2(1, 0.5f);
            ownedMarkRect.anchoredPosition = new Vector2(-60, 0);
            ownedMarkRect.sizeDelta = new Vector2(120, 50);

            var ownedMarkText = ownedMarkObject.AddComponent<TextMeshProUGUI>();
            ownedMarkText.text = "OWNED";
            ownedMarkText.fontSize = 24;
            ownedMarkText.alignment = TextAlignmentOptions.Center;
            ownedMarkText.font = font;
            ownedMarkText.color = new Color(0.3f, 0.8f, 0.3f);
            ownedMarkObject.SetActive(false);

            var shopItemView = templateInstance.AddComponent<UI_ShopItemView>();
            var serializedObject = new SerializedObject(shopItemView);
            serializedObject.FindProperty("iconImage").objectReferenceValue = icon;
            serializedObject.FindProperty("nameText").objectReferenceValue = nameText;
            serializedObject.FindProperty("priceText").objectReferenceValue = priceText;
            serializedObject.FindProperty("buyButton").objectReferenceValue = buyButton;
            serializedObject.FindProperty("ownedMark").objectReferenceValue = ownedMarkObject;
            serializedObject.ApplyModifiedProperties();

            return shopItemView;
        }

        private static UI_ShopItemView CreateShopItemFallback(RectTransform parent, Vector2 position, Vector2 size, TMP_FontAsset font, Sprite goldIcon, int index)
        {
            var itemObject = new GameObject($"ShopItem_{index}");
            itemObject.transform.SetParent(parent, false);

            var itemRect = itemObject.AddComponent<RectTransform>();
            itemRect.anchorMin = new Vector2(0.5f, 0.5f);
            itemRect.anchorMax = new Vector2(0.5f, 0.5f);
            itemRect.pivot = new Vector2(0.5f, 0.5f);
            itemRect.anchoredPosition = position;
            itemRect.sizeDelta = size;

            var frameInstance = InstantiateKitPrefab(CardFrameBluePath, itemObject.transform);
            if (frameInstance != null)
            {
                var frameRect = frameInstance.GetComponent<RectTransform>();
                frameRect.anchorMin = Vector2.zero;
                frameRect.anchorMax = Vector2.one;
                frameRect.offsetMin = Vector2.zero;
                frameRect.offsetMax = Vector2.zero;
            }

            var iconObject = new GameObject("UnitIcon");
            iconObject.transform.SetParent(itemObject.transform, false);
            var iconRect = iconObject.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0, 0.5f);
            iconRect.anchorMax = new Vector2(0, 0.5f);
            iconRect.pivot = new Vector2(0, 0.5f);
            iconRect.anchoredPosition = new Vector2(20, 0);
            iconRect.sizeDelta = new Vector2(100, 100);

            var iconImage = iconObject.AddComponent<Image>();
            iconImage.raycastTarget = false;

            var nameTextObject = new GameObject("NameText");
            nameTextObject.transform.SetParent(itemObject.transform, false);
            var nameTextRect = nameTextObject.AddComponent<RectTransform>();
            nameTextRect.anchorMin = new Vector2(0, 0.5f);
            nameTextRect.anchorMax = new Vector2(0, 0.5f);
            nameTextRect.pivot = new Vector2(0, 0.5f);
            nameTextRect.anchoredPosition = new Vector2(140, 0);
            nameTextRect.sizeDelta = new Vector2(200, 50);

            var nameText = nameTextObject.AddComponent<TextMeshProUGUI>();
            nameText.text = "Unit Name";
            nameText.fontSize = 28;
            nameText.alignment = TextAlignmentOptions.Left;
            nameText.font = font;

            var priceContainerObject = new GameObject("PriceContainer");
            priceContainerObject.transform.SetParent(itemObject.transform, false);
            var priceContainerRect = priceContainerObject.AddComponent<RectTransform>();
            priceContainerRect.anchorMin = new Vector2(0.5f, 0.5f);
            priceContainerRect.anchorMax = new Vector2(0.5f, 0.5f);
            priceContainerRect.pivot = new Vector2(0.5f, 0.5f);
            priceContainerRect.anchoredPosition = new Vector2(50, 0);
            priceContainerRect.sizeDelta = new Vector2(150, 50);

            var goldIconObject = new GameObject("GoldIcon");
            goldIconObject.transform.SetParent(priceContainerObject.transform, false);
            var goldIconRect = goldIconObject.AddComponent<RectTransform>();
            goldIconRect.anchorMin = new Vector2(0, 0.5f);
            goldIconRect.anchorMax = new Vector2(0, 0.5f);
            goldIconRect.pivot = new Vector2(0, 0.5f);
            goldIconRect.anchoredPosition = new Vector2(0, 0);
            goldIconRect.sizeDelta = new Vector2(40, 40);

            var goldIconImage = goldIconObject.AddComponent<Image>();
            goldIconImage.sprite = goldIcon;
            goldIconImage.raycastTarget = false;

            var priceTextObject = new GameObject("PriceText");
            priceTextObject.transform.SetParent(priceContainerObject.transform, false);
            var priceTextRect = priceTextObject.AddComponent<RectTransform>();
            priceTextRect.anchorMin = new Vector2(0, 0.5f);
            priceTextRect.anchorMax = new Vector2(0, 0.5f);
            priceTextRect.pivot = new Vector2(0, 0.5f);
            priceTextRect.anchoredPosition = new Vector2(50, 0);
            priceTextRect.sizeDelta = new Vector2(100, 50);

            var priceText = priceTextObject.AddComponent<TextMeshProUGUI>();
            priceText.text = "0";
            priceText.fontSize = 26;
            priceText.alignment = TextAlignmentOptions.Left;
            priceText.font = font;

            var buyButtonInstance = InstantiateKitPrefab(ButtonGreenPath, itemObject.transform);
            Button buyButton = null;
            if (buyButtonInstance != null)
            {
                buyButtonInstance.name = "BuyButton";
                var buyButtonRect = buyButtonInstance.GetComponent<RectTransform>();
                buyButtonRect.anchorMin = new Vector2(1, 0.5f);
                buyButtonRect.anchorMax = new Vector2(1, 0.5f);
                buyButtonRect.pivot = new Vector2(1, 0.5f);
                buyButtonRect.anchoredPosition = new Vector2(-20, 0);
                buyButtonRect.sizeDelta = new Vector2(120, 70);

                buyButton = EnsureButtonComponent(buyButtonInstance);
                var buyButtonText = buyButtonInstance.GetComponentInChildren<TMP_Text>();
                if (buyButtonText != null)
                {
                    buyButtonText.text = "BUY";
                    buyButtonText.fontSize = 26;
                    buyButtonText.alignment = TextAlignmentOptions.Center;
                    buyButtonText.font = font;
                }
            }

            var ownedMarkObject = new GameObject("OwnedMark");
            ownedMarkObject.transform.SetParent(itemObject.transform, false);
            var ownedMarkRect = ownedMarkObject.AddComponent<RectTransform>();
            ownedMarkRect.anchorMin = new Vector2(1, 0.5f);
            ownedMarkRect.anchorMax = new Vector2(1, 0.5f);
            ownedMarkRect.pivot = new Vector2(1, 0.5f);
            ownedMarkRect.anchoredPosition = new Vector2(-60, 0);
            ownedMarkRect.sizeDelta = new Vector2(120, 50);

            var ownedMarkText = ownedMarkObject.AddComponent<TextMeshProUGUI>();
            ownedMarkText.text = "OWNED";
            ownedMarkText.fontSize = 24;
            ownedMarkText.alignment = TextAlignmentOptions.Center;
            ownedMarkText.font = font;
            ownedMarkText.color = new Color(0.3f, 0.8f, 0.3f);
            ownedMarkObject.SetActive(false);

            var shopItemView = itemObject.AddComponent<UI_ShopItemView>();
            var serializedObject = new SerializedObject(shopItemView);
            serializedObject.FindProperty("iconImage").objectReferenceValue = iconImage;
            serializedObject.FindProperty("nameText").objectReferenceValue = nameText;
            serializedObject.FindProperty("priceText").objectReferenceValue = priceText;
            serializedObject.FindProperty("buyButton").objectReferenceValue = buyButton;
            serializedObject.FindProperty("ownedMark").objectReferenceValue = ownedMarkObject;
            serializedObject.ApplyModifiedProperties();

            return shopItemView;
        }

        private static GameObject CreateMissionPanel(RectTransform parent, TMP_FontAsset font, Sprite goldIcon)
        {
            const string templatePath = KitRoot + "Theme_Light/Prefabs/Prefabs~DemoScenes/Progression_Mission_02.prefab";
            var templateInstance = InstantiateKitPrefab(templatePath, parent);

            if (templateInstance == null)
            {
                GameLogger.LogWarning("[UIPrefabBuilder] Progression_Mission_02 not found, using fallback.");
                return CreateMissionPanelFallback(parent, font, goldIcon);
            }

            templateInstance.name = "MissionPanel";

            DisableDemoDescendants(templateInstance, new[] { "ResourceBar", "Tab_", "Dropdown", "Option", "Button_Info" });

            var dimmedTransform = templateInstance.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name == "Dimmed");
            var popupTransform = templateInstance.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name == "Popup");

            if (dimmedTransform == null || popupTransform == null)
            {
                GameLogger.LogWarning("[UIPrefabBuilder] Dimmed or Popup not found in Progression_Mission_02, using fallback.");
                Object.DestroyImmediate(templateInstance);
                return CreateMissionPanelFallback(parent, font, goldIcon);
            }

            var contentTransform = popupTransform.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name == "Content");

            if (contentTransform == null)
            {
                GameLogger.LogWarning("[UIPrefabBuilder] Content not found in Progression_Mission_02, using fallback.");
                Object.DestroyImmediate(templateInstance);
                return CreateMissionPanelFallback(parent, font, goldIcon);
            }

            var demoItems = contentTransform.GetComponentsInChildren<Transform>(true)
                .Where(t => t.name.StartsWith("ListItem_Mission_02") && t != contentTransform)
                .Select(t => t.gameObject)
                .ToArray();

            if (demoItems.Length == 0)
            {
                GameLogger.LogWarning("[UIPrefabBuilder] No demo items found in Progression_Mission_02, using fallback.");
                Object.DestroyImmediate(templateInstance);
                return CreateMissionPanelFallback(parent, font, goldIcon);
            }

            var rows = new UI_MissionRowView[10];
            var firstItem = demoItems[0];

            for (var i = 0; i < 10; i++)
            {
                GameObject itemInstance;
                if (i < demoItems.Length)
                {
                    itemInstance = demoItems[i];
                }
                else
                {
                    itemInstance = Object.Instantiate(firstItem, contentTransform);
                }

                itemInstance.name = $"MissionRow_{i}";
                itemInstance.SetActive(true);

                var descriptionText = itemInstance.GetComponentsInChildren<TMP_Text>(true)
                    .FirstOrDefault(txt => txt.name.Contains("Title") || txt.name.Contains("Description"));
                if (descriptionText == null)
                {
                    // 템플릿에 제목 요소가 없으면 아이템의 기본 텍스트를 설명으로 재활용
                    descriptionText = itemInstance.GetComponentsInChildren<TMP_Text>(true).FirstOrDefault();
                }
                var progressText = itemInstance.GetComponentsInChildren<TMP_Text>(true)
                    .FirstOrDefault(txt => txt.text.Contains("/") && txt != descriptionText);

                if (descriptionText == null)
                {
                    var descriptionObject = new GameObject("DescriptionText");
                    descriptionObject.transform.SetParent(itemInstance.transform, false);
                    var descriptionRect = descriptionObject.AddComponent<RectTransform>();
                    descriptionRect.anchorMin = new Vector2(0.5f, 1);
                    descriptionRect.anchorMax = new Vector2(0.5f, 1);
                    descriptionRect.pivot = new Vector2(0.5f, 1);
                    descriptionRect.anchoredPosition = new Vector2(0, -30);
                    descriptionRect.sizeDelta = new Vector2(400, 60);
                    var descriptionTmp = descriptionObject.AddComponent<TextMeshProUGUI>();
                    descriptionTmp.fontSize = 26;
                    descriptionTmp.alignment = TextAlignmentOptions.Center;
                    descriptionText = descriptionTmp;
                }
                descriptionText.gameObject.name = "DescriptionText";
                descriptionText.text = "Mission Description";
                descriptionText.font = font;

                if (progressText == null)
                {
                    var progressTextObject = new GameObject("ProgressText");
                    progressTextObject.transform.SetParent(itemInstance.transform, false);
                    var progressTextRect = progressTextObject.AddComponent<RectTransform>();
                    progressTextRect.anchorMin = new Vector2(0.5f, 0.5f);
                    progressTextRect.anchorMax = new Vector2(0.5f, 0.5f);
                    progressTextRect.pivot = new Vector2(0.5f, 0.5f);
                    progressTextRect.anchoredPosition = new Vector2(100, 0);
                    progressTextRect.sizeDelta = new Vector2(100, 40);

                    progressText = progressTextObject.AddComponent<TextMeshProUGUI>();
                    progressText.text = "0/10";
                    progressText.fontSize = 22;
                    progressText.alignment = TextAlignmentOptions.Center;
                    progressText.font = font;
                }
                else
                {
                    progressText.gameObject.name = "ProgressText";
                    progressText.text = "0/10";
                    progressText.font = font;
                }

                var claimButtonTransform = itemInstance.GetComponentsInChildren<Transform>(true)
                    .FirstOrDefault(t => t.name.Contains("Claim") && !t.name.Contains("Disabled"));
                Button claimButton;
                if (claimButtonTransform != null)
                {
                    claimButton = EnsureButtonComponent(claimButtonTransform.gameObject);
                    claimButtonTransform.gameObject.name = "ClaimButton";
                }
                else
                {
                    // 템플릿에 수령 버튼이 없으면 킷 버튼으로 생성
                    claimButton = CreatePausePopupButton(itemInstance.transform, "ClaimButton", new Vector2(0, -95), "CLAIM", ButtonBluePath, font);
                    var claimRect = claimButton.GetComponent<RectTransform>();
                    claimRect.sizeDelta = new Vector2(220, 70);
                }

                var unusedButtons = itemInstance.GetComponentsInChildren<Transform>(true)
                    .Where(t => (t.name.Contains("Ad") || t.name.Contains("Timer") || t.name.Contains("ClaimDisabled")) && t != claimButtonTransform);
                foreach (var unused in unusedButtons)
                {
                    unused.gameObject.SetActive(false);
                }

                var claimedMarkObject = new GameObject("ClaimedMark");
                claimedMarkObject.transform.SetParent(itemInstance.transform, false);
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

                var rowView = itemInstance.AddComponent<UI_MissionRowView>();
                var rowSerializer = new SerializedObject(rowView);
                rowSerializer.FindProperty("descriptionText").objectReferenceValue = descriptionText;
                rowSerializer.FindProperty("progressText").objectReferenceValue = progressText;
                rowSerializer.FindProperty("claimButton").objectReferenceValue = claimButton;
                rowSerializer.FindProperty("claimedMark").objectReferenceValue = claimedMarkObject;
                rowSerializer.ApplyModifiedProperties();

                rows[i] = rowView;
            }

            var topTransform = popupTransform.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name == "Top");
            var closeButtonTransform = topTransform?.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name.Contains("Close") || t.name.Contains("Back"));
            Button closeButton;
            if (closeButtonTransform != null)
            {
                closeButton = EnsureButtonComponent(closeButtonTransform.gameObject);
            }
            else
            {
                var closeButtonInstance = InstantiateKitPrefab(ButtonCirclePath, popupTransform);
                if (closeButtonInstance != null)
                {
                    closeButtonInstance.name = "CloseButton";
                    var closeButtonRect = closeButtonInstance.GetComponent<RectTransform>();
                    closeButtonRect.anchorMin = new Vector2(1, 1);
                    closeButtonRect.anchorMax = new Vector2(1, 1);
                    closeButtonRect.pivot = new Vector2(1, 1);
                    closeButtonRect.anchoredPosition = new Vector2(-70, -70);
                    closeButtonRect.sizeDelta = new Vector2(90, 90);

                    var closeIconObject = new GameObject("CloseIcon");
                    closeIconObject.transform.SetParent(closeButtonInstance.transform, false);
                    var closeIconRect = closeIconObject.AddComponent<RectTransform>();
                    closeIconRect.anchorMin = new Vector2(0.5f, 0.5f);
                    closeIconRect.anchorMax = new Vector2(0.5f, 0.5f);
                    closeIconRect.pivot = new Vector2(0.5f, 0.5f);
                    closeIconRect.anchoredPosition = Vector2.zero;
                    closeIconRect.sizeDelta = new Vector2(50, 50);

                    var closeIconImage = closeIconObject.AddComponent<Image>();
                    var closeIcon = LoadSprite("Assets/Layer Lab/GUI Pro-MinimalGame/Shared/Icons/PictoIcon/128/arrow_back.png");
                    closeIconImage.sprite = closeIcon;
                    closeIconImage.raycastTarget = false;
                }

                closeButton = EnsureButtonComponent(closeButtonInstance);
            }

            var missionPanelView = templateInstance.AddComponent<UI_MissionPanelView>();
            var serializedObject = new SerializedObject(missionPanelView);
            serializedObject.FindProperty("root").objectReferenceValue = templateInstance;
            serializedObject.FindProperty("missionRows").arraySize = rows.Length;
            for (var i = 0; i < rows.Length; i++)
            {
                serializedObject.FindProperty("missionRows").GetArrayElementAtIndex(i).objectReferenceValue = rows[i];
            }
            serializedObject.FindProperty("closeButton").objectReferenceValue = closeButton;
            serializedObject.ApplyModifiedProperties();

            templateInstance.SetActive(false);

            return templateInstance;
        }

        private static GameObject CreateMissionPanelFallback(RectTransform parent, TMP_FontAsset font, Sprite goldIcon)
        {
            var panelObject = new GameObject("MissionPanel");
            panelObject.transform.SetParent(parent, false);

            var panelRect = panelObject.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 0.22f);
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            var bgImage = panelObject.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);

            AddPanelBackgroundFrame(panelObject);

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

            var closeButtonInstance = InstantiateKitPrefab(ButtonCirclePath, panelObject.transform);
            if (closeButtonInstance != null)
            {
                closeButtonInstance.name = "CloseButton";
                var closeButtonRect = closeButtonInstance.GetComponent<RectTransform>();
                closeButtonRect.anchorMin = new Vector2(1, 1);
                closeButtonRect.anchorMax = new Vector2(1, 1);
                closeButtonRect.pivot = new Vector2(1, 1);
                closeButtonRect.anchoredPosition = new Vector2(-70, -70);
                closeButtonRect.sizeDelta = new Vector2(90, 90);

                var closeIconObject = new GameObject("CloseIcon");
                closeIconObject.transform.SetParent(closeButtonInstance.transform, false);
                var closeIconRect = closeIconObject.AddComponent<RectTransform>();
                closeIconRect.anchorMin = new Vector2(0.5f, 0.5f);
                closeIconRect.anchorMax = new Vector2(0.5f, 0.5f);
                closeIconRect.pivot = new Vector2(0.5f, 0.5f);
                closeIconRect.anchoredPosition = Vector2.zero;
                closeIconRect.sizeDelta = new Vector2(50, 50);

                var closeIconImage = closeIconObject.AddComponent<Image>();
                var closeIcon = LoadSprite("Assets/Layer Lab/GUI Pro-MinimalGame/Shared/Icons/PictoIcon/128/arrow_back.png");
                closeIconImage.sprite = closeIcon;
                closeIconImage.raycastTarget = false;
            }

            var closeButton = EnsureButtonComponent(closeButtonInstance);

            var missionPanelView = panelObject.AddComponent<UI_MissionPanelView>();
            var serializedObject = new SerializedObject(missionPanelView);
            serializedObject.FindProperty("root").objectReferenceValue = panelObject;
            serializedObject.FindProperty("missionRows").arraySize = rows.Length;
            for (var i = 0; i < rows.Length; i++)
            {
                serializedObject.FindProperty("missionRows").GetArrayElementAtIndex(i).objectReferenceValue = rows[i];
            }
            serializedObject.FindProperty("closeButton").objectReferenceValue = closeButton;
            serializedObject.ApplyModifiedProperties();

            panelObject.SetActive(false);

            return panelObject;
        }

        private static UI_MissionRowView CreateMissionRow(RectTransform parent, Vector2 position, Vector2 size, TMP_FontAsset font, Sprite goldIcon, int index)
        {
            const string templatePath = KitRoot + "Theme_Light/Prefabs/Prefabs~DemoLayout/ListItem_Mission_01.prefab";
            var templateInstance = InstantiateKitPrefab(templatePath, parent);

            if (templateInstance == null)
            {
                GameLogger.LogWarning($"[UIPrefabBuilder] ListItem_Mission_01 not found, using fallback for MissionRow_{index}.");
                return CreateMissionRowFallback(parent, position, size, font, goldIcon, index);
            }

            templateInstance.name = $"MissionRow_{index}";
            var rowRect = templateInstance.GetComponent<RectTransform>();
            rowRect.anchorMin = new Vector2(0.5f, 0.5f);
            rowRect.anchorMax = new Vector2(0.5f, 0.5f);
            rowRect.pivot = new Vector2(0.5f, 0.5f);
            rowRect.anchoredPosition = position;
            rowRect.sizeDelta = size;

            var descriptionText = templateInstance.transform.Find("Text_Title")?.GetComponent<TMP_Text>();
            var progressSlider = templateInstance.transform.Find("Slider_Border_Rectangle");
            var rewardIcon = templateInstance.transform.Find("Icon");
            var rewardText = templateInstance.transform.Find("Text (TMP)")?.GetComponent<TMP_Text>();
            var claimButton = templateInstance.transform.Find("Button_Claim");
            var claimDisabledButton = templateInstance.transform.Find("Button_ClaimDisabled");
            var buttonAd = templateInstance.transform.Find("Button_Ad");
            var buttonTimer = templateInstance.transform.Find("Button_Timer");
            var stampe = templateInstance.transform.Find("Stampe");

            if (buttonAd != null)
            {
                buttonAd.gameObject.SetActive(false);
            }
            if (buttonTimer != null)
            {
                buttonTimer.gameObject.SetActive(false);
            }
            if (stampe != null)
            {
                stampe.gameObject.SetActive(false);
            }
            if (claimDisabledButton != null)
            {
                claimDisabledButton.gameObject.SetActive(false);
            }
            if (progressSlider != null)
            {
                progressSlider.gameObject.SetActive(false);
            }
            if (rewardIcon != null)
            {
                rewardIcon.gameObject.SetActive(false);
            }

            var progressTextObject = new GameObject("ProgressText");
            progressTextObject.transform.SetParent(templateInstance.transform, false);
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

            if (descriptionText != null)
            {
                descriptionText.text = "Mission Description";
                descriptionText.font = font;
            }

            Button claimButtonComponent = null;
            if (claimButton != null)
            {
                claimButtonComponent = EnsureButtonComponent(claimButton.gameObject);
            }

            var claimedMarkObject = new GameObject("ClaimedMark");
            claimedMarkObject.transform.SetParent(templateInstance.transform, false);
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

            var rowView = templateInstance.AddComponent<UI_MissionRowView>();
            var serializedObject = new SerializedObject(rowView);
            serializedObject.FindProperty("descriptionText").objectReferenceValue = descriptionText;
            serializedObject.FindProperty("progressText").objectReferenceValue = progressText;
            serializedObject.FindProperty("claimButton").objectReferenceValue = claimButtonComponent;
            serializedObject.FindProperty("claimedMark").objectReferenceValue = claimedMarkObject;
            serializedObject.ApplyModifiedProperties();

            return rowView;
        }

        private static UI_MissionRowView CreateMissionRowFallback(RectTransform parent, Vector2 position, Vector2 size, TMP_FontAsset font, Sprite goldIcon, int index)
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

            DisableDemoDescendants(popupRoot, new[] { "ResourceBar", "Tab_", "Dropdown", "Option", "Button_Info" });

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
            const string templatePath = KitRoot + "Theme_Light/Prefabs/Prefabs~DemoScenes/Settings.prefab";
            var templateInstance = InstantiateKitPrefab(templatePath, parent);

            if (templateInstance == null)
            {
                GameLogger.LogWarning("[UIPrefabBuilder] Settings.prefab not found, using fallback.");
                return CreateSettingsPopupFallback(parent, font);
            }

            templateInstance.name = "SettingsPopupRoot";

            DisableDemoDescendants(templateInstance, new[] { "ResourceBar", "Tab_", "Dropdown", "Option", "Button_Info" });

            var dimmedTransform = templateInstance.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name == "Dimmed");
            var popupTransform = templateInstance.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name == "Popup");

            if (dimmedTransform == null || popupTransform == null)
            {
                GameLogger.LogWarning("[UIPrefabBuilder] Dimmed or Popup not found in Settings.prefab, using fallback.");
                Object.DestroyImmediate(templateInstance);
                return CreateSettingsPopupFallback(parent, font);
            }

            var sfxGroup = popupTransform.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name.Contains("SFX"));
            var bgmGroup = popupTransform.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name.Contains("BGM") || t.name.Contains("Music"));
            var languageGroup = popupTransform.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name.Contains("Language"));
            var uidGroup = popupTransform.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name.Contains("UID") || t.name.Contains("UserID"));
            var privacyButton = popupTransform.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name.Contains("Privacy"));
            var termsButton = popupTransform.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name.Contains("Terms"));

            if (sfxGroup != null)
            {
                sfxGroup.gameObject.SetActive(false);
            }
            if (bgmGroup != null)
            {
                bgmGroup.gameObject.SetActive(false);
            }
            if (languageGroup != null)
            {
                languageGroup.gameObject.SetActive(false);
            }
            if (uidGroup != null)
            {
                uidGroup.gameObject.SetActive(false);
            }
            if (privacyButton != null)
            {
                privacyButton.gameObject.SetActive(false);
            }
            if (termsButton != null)
            {
                termsButton.gameObject.SetActive(false);
            }

            var hapticToggle = popupTransform.GetComponentsInChildren<Toggle>(true)
                .FirstOrDefault(tog => tog.name.Contains("Haptic") || tog.name.Contains("Vibration"));
            Toggle vibrationToggle = hapticToggle;
            if (vibrationToggle != null)
            {
                vibrationToggle.gameObject.name = "VibrationToggle";
            }
            else
            {
                var togglePath = KitRoot + "Theme_Light/Prefabs/Prefabs_Control/Toggle_Check_02.prefab";
                var toggleInstance = InstantiateKitPrefab(togglePath, popupTransform);
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
                }
            }

            var versionText = popupTransform.GetComponentsInChildren<TMP_Text>(true)
                .FirstOrDefault(txt => txt.name.Contains("Version"));
            if (versionText != null)
            {
                versionText.gameObject.name = "VersionText";
                versionText.text = "v1.0.0";
                versionText.font = font;
            }
            else
            {
                var versionTextObject = new GameObject("VersionText");
                versionTextObject.transform.SetParent(popupTransform, false);
                var versionTextRect = versionTextObject.AddComponent<RectTransform>();
                versionTextRect.anchorMin = new Vector2(0.5f, 0);
                versionTextRect.anchorMax = new Vector2(0.5f, 0);
                versionTextRect.pivot = new Vector2(0.5f, 0);
                versionTextRect.anchoredPosition = new Vector2(0, 80);
                versionTextRect.sizeDelta = new Vector2(300, 40);

                versionText = versionTextObject.AddComponent<TextMeshProUGUI>();
                versionText.text = "v1.0.0";
                versionText.fontSize = 24;
                versionText.alignment = TextAlignmentOptions.Center;
                versionText.font = font;
                versionText.color = new Color(0.7f, 0.7f, 0.7f);
            }

            var resetButtonTransform = popupTransform.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name.Contains("Reset"));
            Button resetButton;
            if (resetButtonTransform != null)
            {
                resetButton = EnsureButtonComponent(resetButtonTransform.gameObject);
                resetButtonTransform.gameObject.name = "ResetButton";
            }
            else
            {
                resetButton = CreatePausePopupButton(popupTransform, "ResetButton", new Vector2(0, -20), "RESET PROGRESS", ButtonBluePath, font);
                if (resetButton != null)
                {
                    var resetButtonRect = resetButton.GetComponent<RectTransform>();
                    resetButtonRect.sizeDelta = new Vector2(350, 80);
                }
            }

            var resetConfirmRoot = new GameObject("ResetConfirmRoot");
            resetConfirmRoot.transform.SetParent(popupTransform, false);
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

            var closeButtonTransform = popupTransform.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name.Contains("Close") || t.name.Contains("Back"));
            Button closeButton;
            if (closeButtonTransform != null)
            {
                closeButton = EnsureButtonComponent(closeButtonTransform.gameObject);
            }
            else
            {
                closeButton = CreatePausePopupButton(templateInstance.transform, "CloseButton", new Vector2(0, 20), "CLOSE", ButtonBluePath, font);
            }

            var settingsView = templateInstance.AddComponent<UI_SettingsPopupView>();
            var serializedObject = new SerializedObject(settingsView);
            serializedObject.FindProperty("root").objectReferenceValue = templateInstance;
            serializedObject.FindProperty("vibrationToggle").objectReferenceValue = vibrationToggle;
            serializedObject.FindProperty("resetButton").objectReferenceValue = resetButton;
            serializedObject.FindProperty("resetConfirmRoot").objectReferenceValue = resetConfirmRoot;
            serializedObject.FindProperty("resetConfirmButton").objectReferenceValue = resetConfirmButton;
            serializedObject.FindProperty("resetCancelButton").objectReferenceValue = resetCancelButton;
            serializedObject.FindProperty("closeButton").objectReferenceValue = closeButton;
            serializedObject.FindProperty("versionText").objectReferenceValue = versionText;
            serializedObject.ApplyModifiedProperties();

            templateInstance.SetActive(false);

            return templateInstance;
        }

        private static GameObject CreateSettingsPopupFallback(RectTransform parent, TMP_FontAsset font)
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
