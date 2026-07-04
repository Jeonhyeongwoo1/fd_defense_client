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

        private const string UnitTablePath = "Assets/Game/03.Resources/Data/UnitTable.asset";
        private const string EnemyTablePath = "Assets/Game/03.Resources/Data/EnemyTable.asset";
        private const string StageTablePath = "Assets/Game/03.Resources/Data/StageTable.asset";

        public static void ImportAllSheets()
        {
            var importedCount = 0;

            importedCount += ImportUnitData();
            importedCount += ImportEnemyData();
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
                if (columns.Length < 10)
                {
                    GameLogger.LogError($"[CsvSheetImporter] Invalid UnitData row {i}: insufficient columns");
                    skippedCount++;
                    continue;
                }

                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(columns[9]);
                if (prefab == null)
                {
                    GameLogger.LogError($"[CsvSheetImporter] Failed to load prefab: {columns[9]} (row {i})");
                    skippedCount++;
                    continue;
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
                if (columns.Length < 6)
                {
                    GameLogger.LogError($"[CsvSheetImporter] Invalid StageData row {i}: insufficient columns");
                    skippedCount++;
                    continue;
                }

                var spawnEntryList = new List<SpawnEntry>();
                var spawnEntriesStr = columns[5];
                if (!string.IsNullOrEmpty(spawnEntriesStr))
                {
                    var entryPairs = spawnEntriesStr.Split(';');
                    foreach (var pair in entryPairs)
                    {
                        var parts = pair.Split(':');
                        if (parts.Length == 2)
                        {
                            var spawnEntry = new SpawnEntry
                            {
                                enemyId = parts[0],
                                interval = float.Parse(parts[1], CultureInfo.InvariantCulture)
                            };
                            spawnEntryList.Add(spawnEntry);
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
                    SpawnEntryList = spawnEntryList
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
    }
}
