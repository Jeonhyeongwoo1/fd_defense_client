using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    [Serializable]
    public class MapLayoutItem
    {
        public GameObject prefab;
        public Vector3 position;
        public Vector3 scale;
        public bool isFlippedX;
        public int sortingOrder;
    }

    [Serializable]
    public class MapData
    {
        public string id;
        public Color skyColor;
        public GameObject fieldPrefab;
        public List<MapDecorEntry> DecorEntryList = new();
        public List<MapLayoutItem> LayoutItemList = new();
        public string demoSceneName;

        [Serializable]
        public class MapDecorEntry
        {
            public List<GameObject> PrefabList = new();
            public int count;
        }
    }
}
