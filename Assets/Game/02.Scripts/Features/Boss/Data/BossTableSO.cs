using System.Collections.Generic;
using Game.Core;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "BossTable", menuName = "Game/BossTable")]
    public class BossTableSO : ScriptableObject
    {
        public List<BossData> BossDataList = new();

        public BossData GetById(string id)
        {
            foreach (var data in BossDataList)
            {
                if (data.id == id)
                {
                    return data;
                }
            }

            GameLogger.LogWarning($"[BossTableSO] Boss not found: {id}");
            return null;
        }
    }
}
