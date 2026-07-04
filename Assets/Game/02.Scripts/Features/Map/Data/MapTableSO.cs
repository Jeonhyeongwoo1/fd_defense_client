using System.Collections.Generic;
using Game.Core;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(menuName = "Game/MapTable")]
    public class MapTableSO : ScriptableObject
    {
        public List<MapData> MapDataList = new();

        public MapData GetById(string id)
        {
            foreach (var data in MapDataList)
            {
                if (data.id == id)
                {
                    return data;
                }
            }

            GameLogger.LogError($"[MapTableSO] MapData not found: {id}");
            return null;
        }
    }
}
