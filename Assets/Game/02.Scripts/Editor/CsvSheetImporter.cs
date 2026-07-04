using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Game.Core;
using Game.Data;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public static class CsvSheetImporter
    {
        private const string UnitCsvPath = "Assets/Game/03.Resources/Data/Sheets/UnitData.csv";
        private const string EnemyCsvPath = "Assets/Game/03.Resources/Data/Sheets/EnemyData.csv";
        private const string StageCsvPath = "Assets/Game/03.Resources/Data/Sheets/StageData.csv";
        private const string WaveCsvPath = "Assets/Game/03.Resources/Data/Sheets/WaveData.csv";

        private const string UnitTablePath = "Assets/Game/03.Resources/Data/UnitTable.asset";
        private const string EnemyTablePath = "Assets/Game/03.Resources/Data/EnemyTable.asset";
        private const string StageTablePath = "Assets/Game/03.Resources/Data/StageTable.asset";
        private const string WaveTablePath = "Assets/Game/03.Resources/Data/WaveTable.asset";

        public static void ImportAllSheets()
        {
            var importedCount = 0;

            importedCount += ImportUnitData();
            importedCount += ImportEnemyData();
            importedCount += ImportWaveData();
            importedCount += ImportStageData();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            GameLogger.Log($"[CsvSheetImporter] Import completed. Total sheets: {importedCount}");
        }

        private static int ImportUnitData()
        {
            if (!File.Exists(UnitCsvPath))
            {
                GameLogger.LogError($"[CsvSheetImporter] CSV not found: {UnitCsvPath}");
                return 0;
            }

            var lines = File.ReadAllLines(UnitCsvPath);
            if (lines.Length < 2)
            {
                GameLogger.LogWarning($"[CsvSheetImporter] No data rows in {UnitCsvPath}");
                return 0;
            }

            var unitDataList = new List<UnitData>();
            var skippedCount = 0;

            for (var i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                var columns = line.Split(',');
                if (columns.Length < 13)
                {
                    GameLogger.LogError($"[CsvSheetImporter] Invalid UnitData row {i}: insufficient columns");
                    skippedCount++;
                    continue;
                }

                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(columns[12]);
                if (prefab == null)
                {
                    GameLogger.LogError($"[CsvSheetImporter] Failed to load prefab: {columns[12]} (row {i})");
                    skippedCount++;
                    continue;
                }

                Sprite projectileSprite = null;
                if (!string.IsNullOrEmpty(columns[11]))
                {
                    projectileSprite = AssetDatabase.LoadAssetAtPath<Sprite>(columns[11]);
                }

                var data = new UnitData
                {
                    id = columns[0],
                    unitName = columns[1],
                    hp = int.Parse(columns[2]),
                    attackPower = int.Parse(columns[3]),
                    attackInterval = float.Parse(columns[4], CultureInfo.InvariantCulture),
                    attackRange = float.Parse(columns[5], CultureInfo.InvariantCulture),
                    moveSpeed = float.Parse(columns[6], CultureInfo.InvariantCulture),
                    cost = int.Parse(columns[7]),
                    cooldown = float.Parse(columns[8], CultureInfo.InvariantCulture),
                    isRanged = bool.Parse(columns[9]),
                    projectileSpeed = float.Parse(columns[10], CultureInfo.InvariantCulture),
                    projectileSprite = projectileSprite,
                    prefab = prefab
                };

                unitDataList.Add(data);
            }

            var table = AssetDatabase.LoadAssetAtPath<UnitTableSO>(UnitTablePath);
            if (table == null)
            {
                table = ScriptableObject.CreateInstance<UnitTableSO>();
                AssetDatabase.CreateAsset(table, UnitTablePath);
            }

            table.UnitDataList = unitDataList;
            EditorUtility.SetDirty(table);

            GameLogger.Log($"[CsvSheetImporter] UnitData imported: {unitDataList.Count} entries ({skippedCount} skipped)");
            return 1;
        }

        private static int ImportEnemyData()
        {
            if (!File.Exists(EnemyCsvPath))
            {
                GameLogger.LogError($"[CsvSheetImporter] CSV not found: {EnemyCsvPath}");
                return 0;
            }

            var lines = File.ReadAllLines(EnemyCsvPath);
            if (lines.Length < 2)
            {
                GameLogger.LogWarning($"[CsvSheetImporter] No data rows in {EnemyCsvPath}");
                return 0;
            }

            var enemyDataList = new List<EnemyData>();
            var skippedCount = 0;

            for (var i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                var columns = line.Split(',');
                if (columns.Length < 8)
                {
                    GameLogger.LogError($"[CsvSheetImporter] Invalid EnemyData row {i}: insufficient columns");
                    skippedCount++;
                    continue;
                }

                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(columns[7]);
                if (prefab == null)
                {
                    GameLogger.LogError($"[CsvSheetImporter] Failed to load prefab: {columns[7]} (row {i})");
                    skippedCount++;
                    continue;
                }

                var data = new EnemyData
                {
                    id = columns[0],
                    unitName = columns[1],
                    hp = int.Parse(columns[2]),
                    attackPower = int.Parse(columns[3]),
                    attackInterval = float.Parse(columns[4], CultureInfo.InvariantCulture),
                    attackRange = float.Parse(columns[5], CultureInfo.InvariantCulture),
                    moveSpeed = float.Parse(columns[6], CultureInfo.InvariantCulture),
                    prefab = prefab
                };

                enemyDataList.Add(data);
            }

            var table = AssetDatabase.LoadAssetAtPath<EnemyTableSO>(EnemyTablePath);
            if (table == null)
            {
                table = ScriptableObject.CreateInstance<EnemyTableSO>();
                AssetDatabase.CreateAsset(table, EnemyTablePath);
            }

            table.EnemyDataList = enemyDataList;
            EditorUtility.SetDirty(table);

            GameLogger.Log($"[CsvSheetImporter] EnemyData imported: {enemyDataList.Count} entries ({skippedCount} skipped)");
            return 1;
        }

        private static int ImportStageData()
        {
            if (!File.Exists(StageCsvPath))
            {
                GameLogger.LogError($"[CsvSheetImporter] CSV not found: {StageCsvPath}");
                return 0;
            }

            var lines = File.ReadAllLines(StageCsvPath);
            if (lines.Length < 2)
            {
                GameLogger.LogWarning($"[CsvSheetImporter] No data rows in {StageCsvPath}");
                return 0;
            }

            var stageDataList = new List<StageData>();
            var skippedCount = 0;

            for (var i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                var columns = line.Split(',');
                if (columns.Length < 7)
                {
                    GameLogger.LogError($"[CsvSheetImporter] Invalid StageData row {i}: insufficient columns");
                    skippedCount++;
                    continue;
                }

                var waveIdList = new List<string>();
                var waveIdsStr = columns[5];
                if (!string.IsNullOrEmpty(waveIdsStr))
                {
                    var ids = waveIdsStr.Split(';');
                    foreach (var id in ids)
                    {
                        if (!string.IsNullOrEmpty(id))
                        {
                            waveIdList.Add(id);
                        }
                    }
                }

                var data = new StageData
                {
                    id = columns[0],
                    allyBaseHp = int.Parse(columns[1]),
                    enemyBaseHp = int.Parse(columns[2]),
                    startMoney = int.Parse(columns[3]),
                    moneyPerSecond = float.Parse(columns[4], CultureInfo.InvariantCulture),
                    WaveIdList = waveIdList,
                    waveIntervalSeconds = float.Parse(columns[6], CultureInfo.InvariantCulture)
                };

                stageDataList.Add(data);
            }

            var table = AssetDatabase.LoadAssetAtPath<StageTableSO>(StageTablePath);
            if (table == null)
            {
                table = ScriptableObject.CreateInstance<StageTableSO>();
                AssetDatabase.CreateAsset(table, StageTablePath);
            }

            table.StageDataList = stageDataList;
            EditorUtility.SetDirty(table);

            GameLogger.Log($"[CsvSheetImporter] StageData imported: {stageDataList.Count} entries ({skippedCount} skipped)");
            return 1;
        }

        private static int ImportWaveData()
        {
            if (!File.Exists(WaveCsvPath))
            {
                GameLogger.LogError($"[CsvSheetImporter] CSV not found: {WaveCsvPath}");
                return 0;
            }

            var lines = File.ReadAllLines(WaveCsvPath);
            if (lines.Length < 2)
            {
                GameLogger.LogWarning($"[CsvSheetImporter] No data rows in {WaveCsvPath}");
                return 0;
            }

            var waveDataList = new List<WaveData>();
            var skippedCount = 0;

            for (var i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                var columns = line.Split(',');
                if (columns.Length < 2)
                {
                    GameLogger.LogError($"[CsvSheetImporter] Invalid WaveData row {i}: insufficient columns");
                    skippedCount++;
                    continue;
                }

                var spawnEntryList = new List<WaveSpawnEntry>();
                var spawnEntriesStr = columns[1];
                if (!string.IsNullOrEmpty(spawnEntriesStr))
                {
                    var entries = spawnEntriesStr.Split(';');
                    foreach (var entry in entries)
                    {
                        var parts = entry.Split(':');
                        if (parts.Length == 3)
                        {
                            var spawnEntry = new WaveSpawnEntry
                            {
                                enemyId = parts[0],
                                count = int.Parse(parts[1]),
                                interval = float.Parse(parts[2], CultureInfo.InvariantCulture)
                            };
                            spawnEntryList.Add(spawnEntry);
                        }
                    }
                }

                var data = new WaveData
                {
                    id = columns[0],
                    SpawnEntryList = spawnEntryList
                };

                waveDataList.Add(data);
            }

            var table = AssetDatabase.LoadAssetAtPath<WaveTableSO>(WaveTablePath);
            if (table == null)
            {
                table = ScriptableObject.CreateInstance<WaveTableSO>();
                AssetDatabase.CreateAsset(table, WaveTablePath);
            }

            table.WaveDataList = waveDataList;
            EditorUtility.SetDirty(table);

            GameLogger.Log($"[CsvSheetImporter] WaveData imported: {waveDataList.Count} entries ({skippedCount} skipped)");
            return 1;
        }
    }
}
