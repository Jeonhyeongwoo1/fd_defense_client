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
        private const string BossCsvPath = "Assets/Game/03.Resources/Data/Sheets/BossData.csv";
        private const string StageCsvPath = "Assets/Game/03.Resources/Data/Sheets/StageData.csv";
        private const string WaveCsvPath = "Assets/Game/03.Resources/Data/Sheets/WaveData.csv";
        private const string MapCsvPath = "Assets/Game/03.Resources/Data/Sheets/MapData.csv";

        private const string UnitTablePath = "Assets/Game/03.Resources/Data/UnitTable.asset";
        private const string EnemyTablePath = "Assets/Game/03.Resources/Data/EnemyTable.asset";
        private const string BossTablePath = "Assets/Game/03.Resources/Data/BossTable.asset";
        private const string StageTablePath = "Assets/Game/03.Resources/Data/StageTable.asset";
        private const string WaveTablePath = "Assets/Game/03.Resources/Data/WaveTable.asset";
        private const string MapTablePath = "Assets/Game/03.Resources/Data/MapTable.asset";

        public static void ImportAllSheets()
        {
            var importedCount = 0;

            importedCount += ImportUnitData();
            importedCount += ImportEnemyData();
            importedCount += ImportBossData();
            importedCount += ImportWaveData();
            importedCount += ImportMapData();
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

        private static int ImportBossData()
        {
            if (!File.Exists(BossCsvPath))
            {
                GameLogger.LogError($"[CsvSheetImporter] CSV not found: {BossCsvPath}");
                return 0;
            }

            var lines = File.ReadAllLines(BossCsvPath);
            if (lines.Length < 2)
            {
                GameLogger.LogWarning($"[CsvSheetImporter] No data rows in {BossCsvPath}");
                return 0;
            }

            var bossDataList = new List<BossData>();
            var skippedCount = 0;

            for (var i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                var columns = line.Split(',');
                if (columns.Length < 11)
                {
                    GameLogger.LogError($"[CsvSheetImporter] Invalid BossData row {i}: insufficient columns");
                    skippedCount++;
                    continue;
                }

                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(columns[10]);
                if (prefab == null)
                {
                    GameLogger.LogError($"[CsvSheetImporter] Failed to load prefab: {columns[10]} (row {i})");
                    skippedCount++;
                    continue;
                }

                var data = new BossData
                {
                    id = columns[0],
                    unitName = columns[1],
                    hp = int.Parse(columns[2]),
                    attackPower = int.Parse(columns[3]),
                    attackInterval = float.Parse(columns[4], CultureInfo.InvariantCulture),
                    attackRange = float.Parse(columns[5], CultureInfo.InvariantCulture),
                    moveSpeed = float.Parse(columns[6], CultureInfo.InvariantCulture),
                    skillDamage = int.Parse(columns[7]),
                    skillInterval = float.Parse(columns[8], CultureInfo.InvariantCulture),
                    skillRange = float.Parse(columns[9], CultureInfo.InvariantCulture),
                    prefab = prefab
                };

                bossDataList.Add(data);
            }

            var table = AssetDatabase.LoadAssetAtPath<BossTableSO>(BossTablePath);
            if (table == null)
            {
                table = ScriptableObject.CreateInstance<BossTableSO>();
                AssetDatabase.CreateAsset(table, BossTablePath);
            }

            table.BossDataList = bossDataList;
            EditorUtility.SetDirty(table);

            GameLogger.Log($"[CsvSheetImporter] BossData imported: {bossDataList.Count} entries ({skippedCount} skipped)");
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
                if (columns.Length < 9)
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
                    waveIntervalSeconds = float.Parse(columns[6], CultureInfo.InvariantCulture),
                    bossId = columns[7],
                    mapId = columns.Length > 8 ? columns[8] : string.Empty
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
                        else
                        {
                            GameLogger.LogWarning($"[CsvSheetImporter] Malformed WaveData spawn entry at row {i}: '{entry}' (expected format: enemyId:count:interval)");
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

        private static int ImportMapData()
        {
            if (!File.Exists(MapCsvPath))
            {
                GameLogger.LogError($"[CsvSheetImporter] CSV not found: {MapCsvPath}");
                return 0;
            }

            var lines = File.ReadAllLines(MapCsvPath);
            if (lines.Length < 2)
            {
                GameLogger.LogWarning($"[CsvSheetImporter] No data rows in {MapCsvPath}");
                return 0;
            }

            var table = AssetDatabase.LoadAssetAtPath<MapTableSO>(MapTablePath);
            var existingDataDict = new Dictionary<string, MapData>();
            if (table != null)
            {
                foreach (var existingData in table.MapDataList)
                {
                    existingDataDict[existingData.id] = existingData;
                }
            }

            var mapDataList = new List<MapData>();
            var skippedCount = 0;

            for (var i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                var columns = line.Split(',');
                if (columns.Length < 4)
                {
                    GameLogger.LogError($"[CsvSheetImporter] Invalid MapData row {i}: insufficient columns");
                    skippedCount++;
                    continue;
                }

                Color skyColor;
                if (!ColorUtility.TryParseHtmlString(columns[1], out skyColor))
                {
                    GameLogger.LogError($"[CsvSheetImporter] Failed to parse skyColor: {columns[1]} (row {i})");
                    skippedCount++;
                    continue;
                }

                var fieldPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(columns[2]);
                if (fieldPrefab == null)
                {
                    GameLogger.LogError($"[CsvSheetImporter] Failed to load field prefab: {columns[2]} (row {i})");
                    skippedCount++;
                    continue;
                }

                var decorEntryList = new List<MapData.MapDecorEntry>();
                var decorEntriesStr = columns[3];
                if (!string.IsNullOrEmpty(decorEntriesStr))
                {
                    var entries = decorEntriesStr.Split(';');
                    foreach (var entry in entries)
                    {
                        var parts = entry.Split(':');
                        if (parts.Length == 2)
                        {
                            var folderAndPrefix = parts[0];
                            var count = int.Parse(parts[1]);

                            var prefixParts = folderAndPrefix.Split('/');
                            var folder = prefixParts[0];
                            var prefix = prefixParts.Length > 1 ? prefixParts[1] : folderAndPrefix;

                            var searchFolder = $"Assets/Layer Lab/2D Minimal-Environment/Environment 1/Prefabs/{folder}";
                            var prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { searchFolder });

                            var prefabList = new List<GameObject>();
                            foreach (var guid in prefabGuids)
                            {
                                var path = AssetDatabase.GUIDToAssetPath(guid);
                                var fileName = System.IO.Path.GetFileNameWithoutExtension(path);
                                if (fileName.StartsWith(prefix))
                                {
                                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                                    if (prefab != null)
                                    {
                                        prefabList.Add(prefab);
                                    }
                                }
                            }

                            if (prefabList.Count == 0)
                            {
                                GameLogger.LogWarning($"[CsvSheetImporter] No prefabs found for decor entry: {folderAndPrefix} (row {i})");
                                continue;
                            }

                            var decorEntry = new MapData.MapDecorEntry
                            {
                                PrefabList = prefabList,
                                count = count
                            };
                            decorEntryList.Add(decorEntry);
                        }
                        else
                        {
                            GameLogger.LogWarning($"[CsvSheetImporter] Malformed MapData decor entry at row {i}: '{entry}' (expected format: folder/prefix:count)");
                        }
                    }
                }

                var demoSceneName = columns.Length > 4 ? columns[4] : string.Empty;

                MapData existingData = null;
                existingDataDict.TryGetValue(columns[0], out existingData);

                var data = new MapData
                {
                    id = columns[0],
                    skyColor = existingData != null ? existingData.skyColor : skyColor,
                    fieldPrefab = fieldPrefab,
                    DecorEntryList = decorEntryList,
                    LayoutItemList = existingData != null ? existingData.LayoutItemList : new List<MapLayoutItem>(),
                    demoSceneName = demoSceneName
                };

                mapDataList.Add(data);
            }

            if (table == null)
            {
                table = ScriptableObject.CreateInstance<MapTableSO>();
                AssetDatabase.CreateAsset(table, MapTablePath);
            }

            table.MapDataList = mapDataList;
            EditorUtility.SetDirty(table);

            GameLogger.Log($"[CsvSheetImporter] MapData imported: {mapDataList.Count} entries ({skippedCount} skipped)");
            return 1;
        }
    }
}
