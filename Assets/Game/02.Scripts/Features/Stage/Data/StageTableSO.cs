using System.Collections.Generic;
using Game.Core;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(menuName = "Game/StageTable")]
    public class StageTableSO : ScriptableObject
    {
        public List<StageData> StageDataList = new();

        public StageData GetById(string id)
        {
            foreach (var data in StageDataList)
            {
                if (data.id == id)
                {
                    return data;
                }
            }

            GameLogger.LogError($"[StageTableSO] StageData not found: {id}");
            return null;
        }
    }
}
