using System;
using Game.Core;
using Game.Data;
using UnityEngine;

namespace Game.Service
{
    public class MapBuilderService
    {
        private const string MapRootName = "MapRoot";
        private const float TileXMin = -12f;
        private const float TileXMax = 12f;
        private const float DecorXMin = -9.5f;
        private const float DecorXMax = 9.5f;
        private const float DecorY = -2.4f;
        private const float FieldY = -3.2f;
        private const int FieldSortingOrder = -10;
        private const int DecorSortingOrder = -5;
        private const float BaseExclusionRadius = 1.2f;
        private const int MaxRetries = 8;

        private readonly MapTableSO _mapTable;

        public MapBuilderService(MapTableSO mapTable)
        {
            _mapTable = mapTable;
        }

        public void BuildMap(string mapId)
        {
            var existingRoot = GameObject.Find(MapRootName);
            if (existingRoot != null)
            {
                UnityEngine.Object.Destroy(existingRoot);
            }

            if (string.IsNullOrEmpty(mapId))
            {
                return;
            }

            var mapData = _mapTable.GetById(mapId);
            if (mapData == null)
            {
                GameLogger.LogError($"[MapBuilderService] MapData not found: {mapId}");
                return;
            }

            var mapRoot = new GameObject(MapRootName);

            SetCameraBackgroundColor(mapData.skyColor);
            BuildField(mapData.fieldPrefab, mapRoot.transform);
            BuildDecor(mapId, mapData.DecorEntryList, mapRoot.transform);

            GameLogger.Log($"[MapBuilderService] Map built: {mapId}");
        }

        private void SetCameraBackgroundColor(Color skyColor)
        {
            if (Camera.main == null)
            {
                GameLogger.LogWarning("[MapBuilderService] Camera.main is null, cannot set background color");
                return;
            }

            Camera.main.backgroundColor = skyColor;
        }

        private void BuildField(GameObject fieldPrefab, Transform parent)
        {
            if (fieldPrefab == null)
            {
                GameLogger.LogWarning("[MapBuilderService] Field prefab is null");
                return;
            }

            var firstInstance = UnityEngine.Object.Instantiate(fieldPrefab, parent);
            var spriteRenderer = firstInstance.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                GameLogger.LogWarning("[MapBuilderService] Field prefab has no SpriteRenderer");
                return;
            }

            var tileWidth = spriteRenderer.bounds.size.x;
            if (tileWidth <= Mathf.Epsilon)
            {
                GameLogger.LogWarning("[MapBuilderService] Field tile width is zero or negative — sprite not assigned or invalid bounds. Skipping tiling.");
                return;
            }

            firstInstance.transform.position = new Vector3(TileXMin, FieldY, 0f);
            spriteRenderer.sortingOrder = FieldSortingOrder;

            for (var x = TileXMin + tileWidth; x <= TileXMax; x += tileWidth)
            {
                var instance = UnityEngine.Object.Instantiate(fieldPrefab, parent);
                instance.transform.position = new Vector3(x, FieldY, 0f);
                var sr = instance.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sortingOrder = FieldSortingOrder;
                }
            }
        }

        private void BuildDecor(string mapId, System.Collections.Generic.List<MapData.MapDecorEntry> decorEntryList, Transform parent)
        {
            var entryIndex = 0;
            foreach (var entry in decorEntryList)
            {
                if (entry.PrefabList.Count == 0)
                {
                    continue;
                }

                for (var i = 0; i < entry.count; i++)
                {
                    var prefab = entry.PrefabList[i % entry.PrefabList.Count];
                    if (prefab == null)
                    {
                        continue;
                    }

                    var position = GenerateDecorPosition(mapId, entryIndex, i);
                    var instance = UnityEngine.Object.Instantiate(prefab, parent);
                    instance.transform.position = position;

                    var spriteRenderer = instance.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.sortingOrder = DecorSortingOrder;
                    }
                }

                entryIndex++;
            }
        }

        private Vector3 GenerateDecorPosition(string mapId, int entryIndex, int index)
        {
            // 결정적 배치 유지를 위해 entryIndex를 시드에 혼합 — 서로 다른 엔트리의 장식이 같은 x에 겹치지 않도록 분리
            var seed = HashSeed(mapId, entryIndex, index);
            var random = new System.Random(seed);

            var x = 0f;
            var retries = 0;

            while (retries < MaxRetries)
            {
                x = (float)(random.NextDouble() * (DecorXMax - DecorXMin) + DecorXMin);

                var distToAllyBase = Mathf.Abs(x - Const.AllyBaseX);
                var distToEnemyBase = Mathf.Abs(x - Const.EnemyBaseX);

                if (distToAllyBase >= BaseExclusionRadius && distToEnemyBase >= BaseExclusionRadius)
                {
                    break;
                }

                retries++;
            }

            return new Vector3(x, DecorY, 0f);
        }

        private int HashSeed(string mapId, int entryIndex, int index)
        {
            var hash = mapId.GetHashCode();
            return hash ^ (entryIndex * 397) ^ index;
        }
    }
}
