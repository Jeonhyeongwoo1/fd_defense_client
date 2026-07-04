using System;
using System.Collections.Generic;
using System.IO;
using Game.Core;
using Game.Model;
using UnityEngine;

namespace Game.Service
{
    public class UserDataService
    {
        private const string FileName = "userdata.json";

        private static readonly string[] AllUnitIds = new[]
        {
            "pet_goldfish", "pet_chick", "pet_bat", "pet_pig", "pet_heart",
            "pet_bomb", "pet_flower", "pet_pug", "pet_ghost", "pet_puppis",
            "pet_melody", "pet_sword", "pet_pumpkin", "pet_blackhole", "pet_titan"
        };

        private static readonly string[] AllMissionTypes = new[]
        {
            "KillEnemies", "KillBosses", "ClearStages", "UpgradeUnits"
        };

        private static readonly string[] AllMissionIds = new[]
        {
            "mission_kill_100", "mission_kill_500", "mission_kill_2000",
            "mission_boss_5", "mission_boss_25",
            "mission_clear_5", "mission_clear_20", "mission_clear_50",
            "mission_upgrade_5", "mission_upgrade_30"
        };

        private readonly string _filePath;
        private UserDataModel _data;

        public UserDataModel Data => _data;

        public UserDataService()
        {
            _filePath = Path.Combine(Application.persistentDataPath, FileName);
            Load();
        }

        private void Load()
        {
            if (File.Exists(_filePath))
            {
                try
                {
                    var json = File.ReadAllText(_filePath);
                    _data = JsonUtility.FromJson<UserDataModel>(json);

                    // FromJson은 일부 입력에서 예외 없이 null을 반환할 수 있다 — 손상으로 취급
                    if (_data == null)
                    {
                        throw new Exception("Deserialized data is null");
                    }

                    GameLogger.Log($"[UserDataService] Loaded data from {_filePath}");
                }
                catch (Exception e)
                {
                    GameLogger.LogError($"[UserDataService] Failed to parse {_filePath}: {e.Message}");
                    BackupCorruptedFile();

                    // 손상 파일이 남으면 매 실행 실패가 반복되고 레거시 마이그레이션도 영구 차단되므로 제거 후 복구 경로 진입
                    File.Delete(_filePath);

                    if (TryMigrateLegacy())
                    {
                        GameLogger.LogWarning("[UserDataService] Recovered via legacy PlayerPrefs migration");
                    }
                    else
                    {
                        _data = CreateDefault();
                    }

                    Save();
                }
            }
            else
            {
                if (TryMigrateLegacy())
                {
                    GameLogger.LogWarning("[UserDataService] Migrated legacy PlayerPrefs data");
                    Save();
                }
                else
                {
                    _data = CreateDefault();
                    Save();
                    GameLogger.Log("[UserDataService] Created default data");
                }
            }
        }

        public void Save()
        {
            try
            {
                var json = JsonUtility.ToJson(_data, true);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception e)
            {
                GameLogger.LogError($"[UserDataService] Failed to save {_filePath}: {e.Message}");
            }
        }

        public void ResetToDefault()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }

            _data = CreateDefault();
            Save();
        }

        public int GetUnitLevel(string unitId)
        {
            foreach (var entry in _data.unitLevels)
            {
                if (entry.unitId == unitId)
                {
                    return entry.level;
                }
            }
            return 1;
        }

        public void SetUnitLevel(string unitId, int level)
        {
            for (var i = 0; i < _data.unitLevels.Count; i++)
            {
                if (_data.unitLevels[i].unitId == unitId)
                {
                    _data.unitLevels[i].level = level;
                    return;
                }
            }

            _data.unitLevels.Add(new UnitLevelEntry { unitId = unitId, level = level });
        }

        public int GetMissionCount(string missionType)
        {
            foreach (var entry in _data.missionCounts)
            {
                if (entry.missionType == missionType)
                {
                    return entry.count;
                }
            }
            return 0;
        }

        public void SetMissionCount(string missionType, int count)
        {
            for (var i = 0; i < _data.missionCounts.Count; i++)
            {
                if (_data.missionCounts[i].missionType == missionType)
                {
                    _data.missionCounts[i].count = count;
                    return;
                }
            }

            _data.missionCounts.Add(new MissionCountEntry { missionType = missionType, count = count });
        }

        public bool IsStageCleared(string stageId)
        {
            foreach (var id in _data.clearedStageIds)
            {
                if (id == stageId)
                {
                    return true;
                }
            }
            return false;
        }

        public void MarkStageCleared(string stageId)
        {
            if (!IsStageCleared(stageId))
            {
                _data.clearedStageIds.Add(stageId);
            }
        }

        public bool IsMissionClaimed(string missionId)
        {
            foreach (var id in _data.claimedMissionIds)
            {
                if (id == missionId)
                {
                    return true;
                }
            }
            return false;
        }

        public void MarkMissionClaimed(string missionId)
        {
            if (!IsMissionClaimed(missionId))
            {
                _data.claimedMissionIds.Add(missionId);
            }
        }

        private UserDataModel CreateDefault()
        {
            var data = new UserDataModel
            {
                gold = 0,
                ownedUnitIds = new List<string>(AllUnitIds),
                selectedStageId = Const.DefaultStageId,
                settings = new SettingsData { isVibrationEnabled = true }
            };

            return data;
        }

        private bool TryMigrateLegacy()
        {
            if (!PlayerPrefs.HasKey("Gold"))
            {
                return false;
            }

            var data = new UserDataModel();

            data.gold = PlayerPrefs.GetInt("Gold", 0);

            data.ownedUnitIds = new List<string>(AllUnitIds);

            data.unitLevels = new List<UnitLevelEntry>();
            foreach (var unitId in AllUnitIds)
            {
                var key = "UnitLevel_" + unitId;
                if (PlayerPrefs.HasKey(key))
                {
                    var level = PlayerPrefs.GetInt(key, 1);
                    data.unitLevels.Add(new UnitLevelEntry { unitId = unitId, level = level });
                }
            }

            var deckStr = PlayerPrefs.GetString("Deck", string.Empty);
            if (!string.IsNullOrEmpty(deckStr))
            {
                var deckArray = deckStr.Split(';');
                data.deckUnitIds = new List<string>(deckArray);
            }

            data.selectedStageId = PlayerPrefs.GetString("SelectedStageId", Const.DefaultStageId);

            data.clearedStageIds = new List<string>();
            for (var i = 1; i <= 50; i++)
            {
                var stageId = $"stage_{i:D3}";
                var key = "StageCleared_" + stageId;
                if (PlayerPrefs.HasKey(key) && PlayerPrefs.GetInt(key, 0) == 1)
                {
                    data.clearedStageIds.Add(stageId);
                }
            }

            data.daily = new DailyData
            {
                lastClaimDate = PlayerPrefs.GetString("LastClaimDate", string.Empty),
                streak = PlayerPrefs.GetInt("DailyStreak", 0)
            };

            data.missionCounts = new List<MissionCountEntry>();
            foreach (var missionType in AllMissionTypes)
            {
                var key = "MissionCount_" + missionType;
                if (PlayerPrefs.HasKey(key))
                {
                    var count = PlayerPrefs.GetInt(key, 0);
                    data.missionCounts.Add(new MissionCountEntry { missionType = missionType, count = count });
                }
            }

            data.claimedMissionIds = new List<string>();
            foreach (var missionId in AllMissionIds)
            {
                var key = "MissionClaimed_" + missionId;
                if (PlayerPrefs.HasKey(key) && PlayerPrefs.GetInt(key, 0) == 1)
                {
                    data.claimedMissionIds.Add(missionId);
                }
            }

            data.settings = new SettingsData
            {
                isVibrationEnabled = PlayerPrefs.GetInt("Vibration", 1) == 1
            };

            _data = data;

            PlayerPrefs.DeleteKey("Gold");
            foreach (var unitId in AllUnitIds)
            {
                PlayerPrefs.DeleteKey("UnitLevel_" + unitId);
            }
            PlayerPrefs.DeleteKey("Deck");
            PlayerPrefs.DeleteKey("SelectedStageId");
            for (var i = 1; i <= 50; i++)
            {
                var stageId = $"stage_{i:D3}";
                PlayerPrefs.DeleteKey("StageCleared_" + stageId);
            }
            PlayerPrefs.DeleteKey("LastClaimDate");
            PlayerPrefs.DeleteKey("DailyStreak");
            foreach (var missionType in AllMissionTypes)
            {
                PlayerPrefs.DeleteKey("MissionCount_" + missionType);
            }
            foreach (var missionId in AllMissionIds)
            {
                PlayerPrefs.DeleteKey("MissionClaimed_" + missionId);
            }
            PlayerPrefs.DeleteKey("Vibration");
            PlayerPrefs.Save();

            return true;
        }

        private void BackupCorruptedFile()
        {
            try
            {
                var backupPath = _filePath + ".bak";
                File.Copy(_filePath, backupPath, true);
                GameLogger.LogWarning($"[UserDataService] Backed up corrupted file to {backupPath}");
            }
            catch (Exception e)
            {
                GameLogger.LogError($"[UserDataService] Failed to backup corrupted file: {e.Message}");
            }
        }
    }
}
