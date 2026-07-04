using System.Collections.Generic;
using System.Linq;
using Game.Core;
using Game.Data;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Editor
{
    public static class DemoSceneLayoutExtractor
    {
        private const string MapTablePath = "Assets/Game/03.Resources/Data/MapTable.asset";
        private const string DemoSceneBasePath = "Assets/Layer Lab/2D Minimal-Environment/Environment 1/Scene";
        private const string EnvironmentPrefabPath = "2D Minimal-Environment";

        public static void ExtractAllLayouts()
        {
            var mapTable = AssetDatabase.LoadAssetAtPath<MapTableSO>(MapTablePath);
            if (mapTable == null)
            {
                GameLogger.LogWarning("[DemoSceneLayoutExtractor] MapTable.asset not found. Running ImportAllSheets first.");
                CsvSheetImporter.ImportAllSheets();
                mapTable = AssetDatabase.LoadAssetAtPath<MapTableSO>(MapTablePath);
                if (mapTable == null)
                {
                    GameLogger.LogError("[DemoSceneLayoutExtractor] MapTable.asset still not found after import.");
                    return;
                }
            }

            // OpenScene(Single)이 로드된 MapTableSO 참조를 파괴할 수 있으므로,
            // 씬 순회 동안은 순수 데이터로만 모으고 기록은 전 씬 처리 후 테이블을 재로드해 수행한다
            var scenePlanList = mapTable.MapDataList
                .Where(mapData => !string.IsNullOrEmpty(mapData.demoSceneName))
                .Select(mapData => (mapId: mapData.id, sceneName: mapData.demoSceneName))
                .ToList();

            // 씬 전환 스윕이 프리팹 참조도 파괴할 수 있어 순회 중에는 에셋 경로로 보관한다
            var extractedResultDict = new Dictionary<string, (List<(string prefabPath, Vector3 position, Vector3 scale, bool isFlippedX, int sortingOrder)> recordList, Color skyColor, bool hasSkyColor)>();

            foreach (var plan in scenePlanList)
            {
                var scenePath = $"{DemoSceneBasePath}/{plan.sceneName}.unity";
                if (!System.IO.File.Exists(scenePath))
                {
                    GameLogger.LogWarning($"[DemoSceneLayoutExtractor] Demo scene not found: {scenePath} (mapId: {plan.mapId})");
                    continue;
                }

                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                var layoutItemList = ExtractLayoutFromScene(scene);
                if (layoutItemList.Count == 0)
                {
                    GameLogger.LogWarning($"[DemoSceneLayoutExtractor] No prefab instances found in {plan.sceneName} (mapId: {plan.mapId})");
                    continue;
                }

                NormalizeLayout(layoutItemList);

                var recordList = new List<(string prefabPath, Vector3 position, Vector3 scale, bool isFlippedX, int sortingOrder)>();
                foreach (var item in layoutItemList)
                {
                    var prefabPath = AssetDatabase.GetAssetPath(item.prefab);
                    if (string.IsNullOrEmpty(prefabPath))
                    {
                        GameLogger.LogWarning($"[DemoSceneLayoutExtractor] Prefab path lost for an item in {plan.sceneName}, skipping.");
                        continue;
                    }

                    recordList.Add((prefabPath, item.position, item.scale, item.isFlippedX, item.sortingOrder));
                }

                var mainCamera = Camera.main;
                var hasSkyColor = mainCamera != null;
                var skyColor = hasSkyColor ? mainCamera.backgroundColor : Color.black;

                extractedResultDict[plan.mapId] = (recordList, skyColor, hasSkyColor);
                GameLogger.Log($"[DemoSceneLayoutExtractor] Extracted {recordList.Count} items from {plan.sceneName} (mapId: {plan.mapId})");
            }

            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            mapTable = AssetDatabase.LoadAssetAtPath<MapTableSO>(MapTablePath);
            if (mapTable == null)
            {
                GameLogger.LogError("[DemoSceneLayoutExtractor] MapTable.asset lost after scene traversal.");
                return;
            }

            foreach (var mapData in mapTable.MapDataList)
            {
                if (!extractedResultDict.TryGetValue(mapData.id, out var result))
                {
                    continue;
                }

                var layoutItemList = new List<MapLayoutItem>();
                foreach (var record in result.recordList)
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(record.prefabPath);
                    if (prefab == null)
                    {
                        GameLogger.LogWarning($"[DemoSceneLayoutExtractor] Prefab not found at write time: {record.prefabPath}");
                        continue;
                    }

                    layoutItemList.Add(new MapLayoutItem
                    {
                        prefab = prefab,
                        position = record.position,
                        scale = record.scale,
                        isFlippedX = record.isFlippedX,
                        sortingOrder = record.sortingOrder
                    });
                }

                mapData.LayoutItemList = layoutItemList;
                if (result.hasSkyColor)
                {
                    mapData.skyColor = result.skyColor;
                }
            }

            EditorUtility.SetDirty(mapTable);
            AssetDatabase.SaveAssets();

            GameLogger.Log($"[DemoSceneLayoutExtractor] Extraction completed. Processed {extractedResultDict.Count} maps.");
        }

        private static List<MapLayoutItem> ExtractLayoutFromScene(Scene scene)
        {
            var layoutItemList = new List<MapLayoutItem>();
            var rootObjects = scene.GetRootGameObjects();

            foreach (var rootObject in rootObjects)
            {
                CollectPrefabInstances(rootObject.transform, layoutItemList);
            }

            return layoutItemList;
        }

        private static void CollectPrefabInstances(Transform transform, List<MapLayoutItem> layoutItemList)
        {
            if (PrefabUtility.IsAnyPrefabInstanceRoot(transform.gameObject))
            {
                var sourcePrefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(transform.gameObject);
                if (sourcePrefab != null)
                {
                    var sourcePath = AssetDatabase.GetAssetPath(sourcePrefab);
                    if (sourcePath.Contains(EnvironmentPrefabPath))
                    {
                        var spriteRenderer = transform.GetComponentInChildren<SpriteRenderer>();
                        var isFlippedX = false;
                        var sortingOrder = 0;

                        if (spriteRenderer != null)
                        {
                            isFlippedX = spriteRenderer.flipX;
                            sortingOrder = spriteRenderer.sortingOrder;
                        }

                        var layoutItem = new MapLayoutItem
                        {
                            prefab = sourcePrefab,
                            position = transform.position,
                            scale = transform.lossyScale,
                            isFlippedX = isFlippedX,
                            sortingOrder = sortingOrder
                        };

                        layoutItemList.Add(layoutItem);
                    }
                }
            }

            foreach (Transform child in transform)
            {
                CollectPrefabInstances(child, layoutItemList);
            }
        }

        private static void NormalizeLayout(List<MapLayoutItem> layoutItemList)
        {
            if (layoutItemList.Count == 0)
            {
                return;
            }

            var avgX = layoutItemList.Average(item => item.position.x);

            var fieldOrRoadItems = layoutItemList.Where(item =>
            {
                if (item.prefab == null)
                {
                    return false;
                }

                var name = item.prefab.name;
                return name.Contains("Field") || name.Contains("Road");
            }).ToList();

            var yShift = 0f;
            if (fieldOrRoadItems.Count > 0)
            {
                var maxFieldY = fieldOrRoadItems.Max(item => item.position.y);
                yShift = Const.GroundY - maxFieldY;
            }
            else
            {
                var minY = layoutItemList.Min(item => item.position.y);
                yShift = (Const.GroundY - 1.2f) - minY;
            }

            foreach (var item in layoutItemList)
            {
                item.position = new Vector3(
                    item.position.x - avgX,
                    item.position.y + yShift,
                    item.position.z
                );
            }
        }
    }
}
