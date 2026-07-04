using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    [Serializable]
    public class MapData
    {
        public string id;
        public Color skyColor;
        public GameObject fieldPrefab;
        public List<MapDecorEntry> DecorEntryList = new();

        [Serializable]
        public class MapDecorEntry
        {
            public List<GameObject> PrefabList = new();
            public int count;
        }
    }
}
