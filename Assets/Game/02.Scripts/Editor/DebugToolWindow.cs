using System;
using System.IO;
using Game.App;
using Game.Core;
using Game.Model;
using Game.Service;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace Game.Editor
{
    public class DebugToolWindow : EditorWindow
    {
        private int _currentTab;
        private readonly string[] _tabNames = { "User Data", "Daily & Missions", "In-Game Cheats", "Pipeline" };

        private int _goldInputAmount = 1000;
        private int _missionInputKillEnemies;
        private int _missionInputKillBosses;
        private int _missionInputClearStages;
        private int _missionInputUpgradeUnits;

        private float _timeScaleSlider = 1f;

        private Vector2 _scrollPosition;

        [MenuItem("FD Defense/Debug Tools")]
        public static void ShowWindow()
        {
            var window = GetWindow<DebugToolWindow>("Debug Tools");
            window.minSize = new Vector2(500, 600);
            window.Show();
        }

        private void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            _currentTab = GUILayout.Toolbar(_currentTab, _tabNames);

            EditorGUILayout.Space(10);

            switch (_currentTab)
            {
                case 0:
                    DrawUserDataTab();
                    break;
                case 1:
                    DrawDailyMissionsTab();
                    break;
                case 2:
                    DrawInGameCheatsTab();
                    break;
                case 3:
                    DrawPipelineTab();
                    break;
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawUserDataTab()
        {
            EditorGUILayout.LabelField("User Data", EditorStyles.boldLabel);

            var filePath = Path.Combine(Application.persistentDataPath, "userdata.json");
            EditorGUILayout.LabelField("File Path:", filePath);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Open File Location", GUILayout.Width(150)))
            {
                EditorUtility.RevealInFinder(filePath);
            }
            if (GUILayout.Button("Print JSON to Console", GUILayout.Width(150)))
            {
                PrintUserDataToConsole(filePath);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Gold", EditorStyles.boldLabel);
            var currentGold = GetCurrentGold();
            EditorGUILayout.LabelField("Current Gold:", currentGold.ToString());
            _goldInputAmount = EditorGUILayout.IntField("Amount:", _goldInputAmount);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add", GUILayout.Width(80)))
            {
                ModifyGold(_goldInputAmount);
            }
            if (GUILayout.Button("Subtract", GUILayout.Width(80)))
            {
                ModifyGold(-_goldInputAmount);
            }
            if (GUILayout.Button("Set to 0", GUILayout.Width(80)))
            {
                SetGold(0);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Unit Levels", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("All to Level 1", GUILayout.Width(150)))
            {
                SetAllUnitLevels(1);
            }
            if (GUILayout.Button("All to Level 5", GUILayout.Width(150)))
            {
                SetAllUnitLevels(5);
            }
            if (GUILayout.Button("All to MAX (10)", GUILayout.Width(150)))
            {
                SetAllUnitLevels(10);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Stages", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Unlock All Stages (1-49)", GUILayout.Width(200)))
            {
                UnlockAllStages();
            }
            if (GUILayout.Button("Reset All Stages", GUILayout.Width(200)))
            {
                ResetAllStages();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Shop", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Own All Units", GUILayout.Width(150)))
            {
                OwnAllUnits();
            }
            if (GUILayout.Button("Reset to Default (10 Units)", GUILayout.Width(200)))
            {
                ResetOwnedUnits();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            if (GUILayout.Button("Reset All User Data", GUILayout.Height(40)))
            {
                ResetAllUserData();
            }

            EditorGUILayout.Space(5);
            EditorGUILayout.HelpBox("Non-play modifications will take effect on next game launch. Play-mode changes are immediate.", MessageType.Info);
        }

        private void DrawDailyMissionsTab()
        {
            EditorGUILayout.LabelField("Daily Reward", EditorStyles.boldLabel);

            var dailyData = GetDailyData();
            EditorGUILayout.LabelField("Last Claim Date:", string.IsNullOrEmpty(dailyData.lastClaimDate) ? "Never" : dailyData.lastClaimDate);
            EditorGUILayout.LabelField("Streak:", dailyData.streak.ToString());

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set to Yesterday", GUILayout.Width(150)))
            {
                SetDailyDate(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
            }
            if (GUILayout.Button("Set to 2 Days Ago", GUILayout.Width(150)))
            {
                SetDailyDate(DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd"));
            }
            if (GUILayout.Button("Reset Claim Data", GUILayout.Width(150)))
            {
                ResetDailyData();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Mission Counters", EditorStyles.boldLabel);

            var missionCounts = GetMissionCounts();
            EditorGUILayout.LabelField("KillEnemies:", missionCounts.killEnemies.ToString());
            _missionInputKillEnemies = EditorGUILayout.IntField("Set KillEnemies:", _missionInputKillEnemies);
            if (GUILayout.Button("Apply KillEnemies", GUILayout.Width(150)))
            {
                SetMissionCount("KillEnemies", _missionInputKillEnemies);
            }

            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("KillBosses:", missionCounts.killBosses.ToString());
            _missionInputKillBosses = EditorGUILayout.IntField("Set KillBosses:", _missionInputKillBosses);
            if (GUILayout.Button("Apply KillBosses", GUILayout.Width(150)))
            {
                SetMissionCount("KillBosses", _missionInputKillBosses);
            }

            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("ClearStages:", missionCounts.clearStages.ToString());
            _missionInputClearStages = EditorGUILayout.IntField("Set ClearStages:", _missionInputClearStages);
            if (GUILayout.Button("Apply ClearStages", GUILayout.Width(150)))
            {
                SetMissionCount("ClearStages", _missionInputClearStages);
            }

            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("UpgradeUnits:", missionCounts.upgradeUnits.ToString());
            _missionInputUpgradeUnits = EditorGUILayout.IntField("Set UpgradeUnits:", _missionInputUpgradeUnits);
            if (GUILayout.Button("Apply UpgradeUnits", GUILayout.Width(150)))
            {
                SetMissionCount("UpgradeUnits", _missionInputUpgradeUnits);
            }

            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set All to Target - 1", GUILayout.Width(200)))
            {
                SetAllMissionsNearComplete();
            }
            if (GUILayout.Button("Reset All Mission Claims", GUILayout.Width(200)))
            {
                ResetMissionClaims();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawInGameCheatsTab()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("In-game cheats are only available during Play Mode.", MessageType.Warning);
                return;
            }

            if (SceneManager.GetActiveScene().name != Const.GameSceneName)
            {
                EditorGUILayout.HelpBox("In-game cheats are only available in GameScene.", MessageType.Warning);
                return;
            }

            EditorGUILayout.LabelField("In-Game Cheats", EditorStyles.boldLabel);

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Combat Money", EditorStyles.boldLabel);
            var walletModel = ResolveService<WalletModel>();
            if (walletModel != null)
            {
                EditorGUILayout.LabelField("Current Money:", walletModel.Money.Value.ToString());
                if (GUILayout.Button("+500 Money", GUILayout.Width(150)))
                {
                    walletModel.AddMoney(500);
                    GameLogger.Log("[DebugToolWindow] Added 500 combat money");
                }
            }

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Base HP", EditorStyles.boldLabel);
            var baseService = ResolveService<BaseService>();
            if (baseService != null)
            {
                var allyBase = baseService.GetBase(UnitSide.Ally);
                var enemyBase = baseService.GetBase(UnitSide.Enemy);

                EditorGUILayout.LabelField("Ally Base HP:", $"{allyBase.CurrentHp.Value} / {allyBase.MaxHp}");
                EditorGUILayout.LabelField("Enemy Base HP:", $"{enemyBase.CurrentHp.Value} / {enemyBase.MaxHp}");

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Destroy Enemy Base (Instant Win)", GUILayout.Width(250)))
                {
                    baseService.ApplyDamage(UnitSide.Enemy, enemyBase.CurrentHp.Value);
                    GameLogger.Log("[DebugToolWindow] Destroyed enemy base");
                }
                if (GUILayout.Button("Destroy Ally Base (Instant Lose)", GUILayout.Width(250)))
                {
                    baseService.ApplyDamage(UnitSide.Ally, allyBase.CurrentHp.Value);
                    GameLogger.Log("[DebugToolWindow] Destroyed ally base");
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Wave", EditorStyles.boldLabel);
            var waveProgressService = ResolveService<WaveProgressService>();
            if (waveProgressService != null)
            {
                EditorGUILayout.LabelField("Current Wave:", $"{waveProgressService.CurrentWaveIndex.Value + 1} / {waveProgressService.TotalWaveCount}");
                if (GUILayout.Button("Kill All Enemies (Clear Current Wave)", GUILayout.Width(300)))
                {
                    KillAllEnemies();
                }
            }

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Time Scale", EditorStyles.boldLabel);
            _timeScaleSlider = EditorGUILayout.Slider("Time Scale:", _timeScaleSlider, 0.1f, 5f);
            if (GUILayout.Button("Apply Time Scale", GUILayout.Width(150)))
            {
                Time.timeScale = _timeScaleSlider;
                GameLogger.Log($"[DebugToolWindow] Set timeScale to {_timeScaleSlider}");
            }
            if (GUILayout.Button("Reset to 1.0", GUILayout.Width(150)))
            {
                Time.timeScale = 1f;
                _timeScaleSlider = 1f;
                GameLogger.Log("[DebugToolWindow] Reset timeScale to 1.0");
            }
        }

        private void DrawPipelineTab()
        {
            EditorGUILayout.LabelField("Build Pipeline", EditorStyles.boldLabel);

            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Pipeline operations are only available in Edit Mode.", MessageType.Warning);
                return;
            }

            EditorGUILayout.Space(10);

            if (GUILayout.Button("CSV Import All Sheets", GUILayout.Height(30)))
            {
                CsvSheetImporter.ImportAllSheets();
            }

            if (GUILayout.Button("Rebuild UI Prefabs", GUILayout.Height(30)))
            {
                UIPrefabBuilder.BuildAllUiPrefabs();
            }

            if (GUILayout.Button("Rebuild GameScene", GUILayout.Height(30)))
            {
                VerticalSliceSceneBuilder.BuildGameScene();
            }

            if (GUILayout.Button("Rebuild OutGameScene", GUILayout.Height(30)))
            {
                OutGameSceneBuilder.BuildOutGameScene();
            }

            if (GUILayout.Button("Rebuild Unit Animations", GUILayout.Height(30)))
            {
                UnitAnimationAssetBuilder.BuildAllUnitAssets();
            }

            if (GUILayout.Button("Extract Map Layouts", GUILayout.Height(30)))
            {
                DemoSceneLayoutExtractor.ExtractAllLayouts();
            }

            if (GUILayout.Button("Clean Animation Events", GUILayout.Height(30)))
            {
                AnimationEventCleaner.CleanEmptyAnimationEvents();
            }

            EditorGUILayout.Space(10);

            if (GUILayout.Button("Run Full Pipeline", GUILayout.Height(50)))
            {
                RunFullPipeline();
            }
        }

        private void PrintUserDataToConsole(string filePath)
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                GameLogger.Log($"[DebugToolWindow] User Data JSON:\n{json}");
            }
            else
            {
                GameLogger.LogWarning("[DebugToolWindow] User data file does not exist");
            }
        }

        private int GetCurrentGold()
        {
            if (Application.isPlaying)
            {
                var userDataService = ResolveService<UserDataService>();
                if (userDataService != null)
                {
                    return userDataService.Data.gold;
                }
            }

            var data = LoadUserDataDirect();
            return data?.gold ?? 0;
        }

        private void ModifyGold(int amount)
        {
            if (Application.isPlaying)
            {
                var goldService = ResolveService<GoldService>();
                if (goldService != null)
                {
                    if (amount > 0)
                    {
                        goldService.Add(amount);
                    }
                    else
                    {
                        goldService.TrySpend(-amount);
                    }
                    GameLogger.Log($"[DebugToolWindow] Modified gold by {amount}");
                    return;
                }
            }

            ModifyUserDataDirect(data =>
            {
                data.gold = Mathf.Max(0, data.gold + amount);
            });
            GameLogger.Log($"[DebugToolWindow] Modified gold by {amount} (non-play)");
        }

        private void SetGold(int value)
        {
            if (Application.isPlaying)
            {
                var userDataService = ResolveService<UserDataService>();
                if (userDataService != null)
                {
                    userDataService.Data.gold = value;
                    userDataService.Save();
                    GameLogger.Log($"[DebugToolWindow] Set gold to {value}");
                    return;
                }
            }

            ModifyUserDataDirect(data =>
            {
                data.gold = value;
            });
            GameLogger.Log($"[DebugToolWindow] Set gold to {value} (non-play)");
        }

        private void SetAllUnitLevels(int level)
        {
            if (Application.isPlaying)
            {
                var userDataService = ResolveService<UserDataService>();
                if (userDataService != null)
                {
                    foreach (var unitId in GetAllUnitIds())
                    {
                        userDataService.SetUnitLevel(unitId, level);
                    }
                    userDataService.Save();
                    GameLogger.Log($"[DebugToolWindow] Set all unit levels to {level}");
                    return;
                }
            }

            ModifyUserDataDirect(data =>
            {
                data.unitLevels.Clear();
                foreach (var unitId in GetAllUnitIds())
                {
                    data.unitLevels.Add(new UnitLevelEntry { unitId = unitId, level = level });
                }
            });
            GameLogger.Log($"[DebugToolWindow] Set all unit levels to {level} (non-play)");
        }

        private void UnlockAllStages()
        {
            if (Application.isPlaying)
            {
                var userDataService = ResolveService<UserDataService>();
                if (userDataService != null)
                {
                    for (var i = 1; i <= 49; i++)
                    {
                        userDataService.MarkStageCleared($"stage_{i:D3}");
                    }
                    userDataService.Save();
                    GameLogger.Log("[DebugToolWindow] Unlocked all stages");
                    return;
                }
            }

            ModifyUserDataDirect(data =>
            {
                data.clearedStageIds.Clear();
                for (var i = 1; i <= 49; i++)
                {
                    data.clearedStageIds.Add($"stage_{i:D3}");
                }
            });
            GameLogger.Log("[DebugToolWindow] Unlocked all stages (non-play)");
        }

        private void ResetAllStages()
        {
            if (Application.isPlaying)
            {
                var userDataService = ResolveService<UserDataService>();
                if (userDataService != null)
                {
                    userDataService.Data.clearedStageIds.Clear();
                    userDataService.Save();
                    GameLogger.Log("[DebugToolWindow] Reset all stages");
                    return;
                }
            }

            ModifyUserDataDirect(data =>
            {
                data.clearedStageIds.Clear();
            });
            GameLogger.Log("[DebugToolWindow] Reset all stages (non-play)");
        }

        private void OwnAllUnits()
        {
            if (Application.isPlaying)
            {
                var userDataService = ResolveService<UserDataService>();
                if (userDataService != null)
                {
                    foreach (var unitId in GetAllUnitIds())
                    {
                        userDataService.AddOwnedUnit(unitId);
                    }
                    userDataService.Save();
                    GameLogger.Log("[DebugToolWindow] Granted ownership of all units");
                    return;
                }
            }

            ModifyUserDataDirect(data =>
            {
                data.ownedUnitIds.Clear();
                foreach (var unitId in GetAllUnitIds())
                {
                    data.ownedUnitIds.Add(unitId);
                }
            });
            GameLogger.Log("[DebugToolWindow] Granted ownership of all units (non-play)");
        }

        private void ResetOwnedUnits()
        {
            if (Application.isPlaying)
            {
                var userDataService = ResolveService<UserDataService>();
                if (userDataService != null)
                {
                    userDataService.Data.ownedUnitIds.Clear();
                    foreach (var unitId in GetDefaultOwnedUnitIds())
                    {
                        userDataService.AddOwnedUnit(unitId);
                    }
                    userDataService.Save();
                    GameLogger.Log("[DebugToolWindow] Reset owned units to default");
                    return;
                }
            }

            ModifyUserDataDirect(data =>
            {
                data.ownedUnitIds.Clear();
                foreach (var unitId in GetDefaultOwnedUnitIds())
                {
                    data.ownedUnitIds.Add(unitId);
                }
            });
            GameLogger.Log("[DebugToolWindow] Reset owned units to default (non-play)");
        }

        private void ResetAllUserData()
        {
            if (!EditorUtility.DisplayDialog("Reset User Data", "Are you sure you want to reset all user data? This cannot be undone.", "Yes", "Cancel"))
            {
                return;
            }

            if (Application.isPlaying)
            {
                var userDataService = ResolveService<UserDataService>();
                if (userDataService != null)
                {
                    userDataService.ResetToDefault();
                    GameLogger.Log("[DebugToolWindow] Reset all user data");
                    return;
                }
            }

            var filePath = Path.Combine(Application.persistentDataPath, "userdata.json");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            GameLogger.Log("[DebugToolWindow] Deleted user data file (non-play)");
        }

        private DailyData GetDailyData()
        {
            if (Application.isPlaying)
            {
                var userDataService = ResolveService<UserDataService>();
                if (userDataService != null)
                {
                    return userDataService.Data.daily;
                }
            }

            var data = LoadUserDataDirect();
            return data?.daily ?? new DailyData();
        }

        private void SetDailyDate(string date)
        {
            if (Application.isPlaying)
            {
                var userDataService = ResolveService<UserDataService>();
                if (userDataService != null)
                {
                    userDataService.Data.daily.lastClaimDate = date;
                    userDataService.Save();
                    GameLogger.Log($"[DebugToolWindow] Set daily claim date to {date}");
                    return;
                }
            }

            ModifyUserDataDirect(data =>
            {
                data.daily.lastClaimDate = date;
            });
            GameLogger.Log($"[DebugToolWindow] Set daily claim date to {date} (non-play)");
        }

        private void ResetDailyData()
        {
            if (Application.isPlaying)
            {
                var userDataService = ResolveService<UserDataService>();
                if (userDataService != null)
                {
                    userDataService.Data.daily.lastClaimDate = string.Empty;
                    userDataService.Data.daily.streak = 0;
                    userDataService.Save();
                    GameLogger.Log("[DebugToolWindow] Reset daily reward data");
                    return;
                }
            }

            ModifyUserDataDirect(data =>
            {
                data.daily.lastClaimDate = string.Empty;
                data.daily.streak = 0;
            });
            GameLogger.Log("[DebugToolWindow] Reset daily reward data (non-play)");
        }

        private (int killEnemies, int killBosses, int clearStages, int upgradeUnits) GetMissionCounts()
        {
            if (Application.isPlaying)
            {
                var userDataService = ResolveService<UserDataService>();
                if (userDataService != null)
                {
                    return (
                        userDataService.GetMissionCount("KillEnemies"),
                        userDataService.GetMissionCount("KillBosses"),
                        userDataService.GetMissionCount("ClearStages"),
                        userDataService.GetMissionCount("UpgradeUnits")
                    );
                }
            }

            var data = LoadUserDataDirect();
            if (data == null)
            {
                return (0, 0, 0, 0);
            }

            return (
                GetMissionCountFromData(data, "KillEnemies"),
                GetMissionCountFromData(data, "KillBosses"),
                GetMissionCountFromData(data, "ClearStages"),
                GetMissionCountFromData(data, "UpgradeUnits")
            );
        }

        private int GetMissionCountFromData(UserDataModel data, string missionType)
        {
            foreach (var entry in data.missionCounts)
            {
                if (entry.missionType == missionType)
                {
                    return entry.count;
                }
            }
            return 0;
        }

        private void SetMissionCount(string missionType, int count)
        {
            if (Application.isPlaying)
            {
                var userDataService = ResolveService<UserDataService>();
                if (userDataService != null)
                {
                    userDataService.SetMissionCount(missionType, count);
                    userDataService.Save();
                    GameLogger.Log($"[DebugToolWindow] Set {missionType} to {count}");
                    return;
                }
            }

            ModifyUserDataDirect(data =>
            {
                var found = false;
                for (var i = 0; i < data.missionCounts.Count; i++)
                {
                    if (data.missionCounts[i].missionType == missionType)
                    {
                        var entry = data.missionCounts[i];
                        entry.count = count;
                        data.missionCounts[i] = entry;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    data.missionCounts.Add(new MissionCountEntry { missionType = missionType, count = count });
                }
            });
            GameLogger.Log($"[DebugToolWindow] Set {missionType} to {count} (non-play)");
        }

        private void SetAllMissionsNearComplete()
        {
            var targets = new[]
            {
                ("KillEnemies", 99),
                ("KillBosses", 4),
                ("ClearStages", 4),
                ("UpgradeUnits", 4)
            };

            if (Application.isPlaying)
            {
                var userDataService = ResolveService<UserDataService>();
                if (userDataService != null)
                {
                    foreach (var (missionType, count) in targets)
                    {
                        userDataService.SetMissionCount(missionType, count);
                    }
                    userDataService.Save();
                    GameLogger.Log("[DebugToolWindow] Set all missions to near-complete");
                    return;
                }
            }

            ModifyUserDataDirect(data =>
            {
                data.missionCounts.Clear();
                foreach (var (missionType, count) in targets)
                {
                    data.missionCounts.Add(new MissionCountEntry { missionType = missionType, count = count });
                }
            });
            GameLogger.Log("[DebugToolWindow] Set all missions to near-complete (non-play)");
        }

        private void ResetMissionClaims()
        {
            if (Application.isPlaying)
            {
                var userDataService = ResolveService<UserDataService>();
                if (userDataService != null)
                {
                    userDataService.Data.claimedMissionIds.Clear();
                    userDataService.Save();
                    GameLogger.Log("[DebugToolWindow] Reset all mission claims");
                    return;
                }
            }

            ModifyUserDataDirect(data =>
            {
                data.claimedMissionIds.Clear();
            });
            GameLogger.Log("[DebugToolWindow] Reset all mission claims (non-play)");
        }

        private void KillAllEnemies()
        {
            var unitBattleService = ResolveService<UnitBattleService>();
            var unitRegistry = ResolveService<UnitRegistry>();

            if (unitBattleService == null || unitRegistry == null)
            {
                GameLogger.LogWarning("[DebugToolWindow] Could not resolve UnitBattleService or UnitRegistry");
                return;
            }

            var enemyList = unitRegistry.GetEntryList(UnitSide.Enemy);
            var killCount = 0;

            foreach (var entry in enemyList)
            {
                if (entry.Model.IsAlive)
                {
                    entry.Model.CurrentHp = 0;
                    killCount++;
                }
            }

            GameLogger.Log($"[DebugToolWindow] Set {killCount} enemies HP to 0 (will die next frame)");
        }

        private void RunFullPipeline()
        {
            if (!EditorUtility.DisplayDialog("Run Full Pipeline", "This will rebuild all data, UI, scenes, animations, and layouts. Continue?", "Yes", "Cancel"))
            {
                return;
            }

            CsvSheetImporter.ImportAllSheets();
            UIPrefabBuilder.BuildAllUiPrefabs();
            VerticalSliceSceneBuilder.BuildGameScene();
            OutGameSceneBuilder.BuildOutGameScene();
            UnitAnimationAssetBuilder.BuildAllUnitAssets();
            DemoSceneLayoutExtractor.ExtractAllLayouts();
            AnimationEventCleaner.CleanEmptyAnimationEvents();

            GameLogger.Log("[DebugToolWindow] Full pipeline completed");
        }

        private UserDataModel LoadUserDataDirect()
        {
            var filePath = Path.Combine(Application.persistentDataPath, "userdata.json");
            if (!File.Exists(filePath))
            {
                return null;
            }

            try
            {
                var json = File.ReadAllText(filePath);
                return JsonUtility.FromJson<UserDataModel>(json);
            }
            catch (Exception e)
            {
                GameLogger.LogError($"[DebugToolWindow] Failed to load user data: {e.Message}");
                return null;
            }
        }

        private void ModifyUserDataDirect(Action<UserDataModel> modifier)
        {
            var filePath = Path.Combine(Application.persistentDataPath, "userdata.json");
            var data = LoadUserDataDirect();

            if (data == null)
            {
                data = new UserDataModel
                {
                    gold = 0,
                    selectedStageId = Const.DefaultStageId,
                    settings = new SettingsData { isVibrationEnabled = true }
                };

                foreach (var unitId in GetDefaultOwnedUnitIds())
                {
                    data.ownedUnitIds.Add(unitId);
                }
            }

            modifier(data);

            try
            {
                var json = JsonUtility.ToJson(data, true);
                File.WriteAllText(filePath, json);
            }
            catch (Exception e)
            {
                GameLogger.LogError($"[DebugToolWindow] Failed to save user data: {e.Message}");
            }
        }

        private T ResolveService<T>() where T : class
        {
            if (!Application.isPlaying)
            {
                return null;
            }

            var lifetimeScope = FindFirstObjectByType<LifetimeScope>();
            if (lifetimeScope == null)
            {
                GameLogger.LogWarning("[DebugToolWindow] LifetimeScope not found in scene");
                return null;
            }

            try
            {
                return lifetimeScope.Container.Resolve<T>();
            }
            catch (Exception e)
            {
                GameLogger.LogWarning($"[DebugToolWindow] Failed to resolve {typeof(T).Name}: {e.Message}");
                return null;
            }
        }

        private string[] GetAllUnitIds()
        {
            return new[]
            {
                "pet_goldfish", "pet_chick", "pet_bat", "pet_pig", "pet_heart",
                "pet_bomb", "pet_flower", "pet_pug", "pet_ghost", "pet_puppis",
                "pet_melody", "pet_sword", "pet_pumpkin", "pet_blackhole", "pet_titan"
            };
        }

        private string[] GetDefaultOwnedUnitIds()
        {
            return new[]
            {
                "pet_goldfish", "pet_chick", "pet_bat", "pet_pig", "pet_heart",
                "pet_bomb", "pet_flower", "pet_pug", "pet_ghost", "pet_puppis"
            };
        }
    }
}
