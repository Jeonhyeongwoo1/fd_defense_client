using System.Collections.Generic;
using Game.Core;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(menuName = "Game/EnemyTable")]
    public class EnemyTableSO : ScriptableObject
    {
        public List<EnemyData> EnemyDataList = new();

        public EnemyData GetById(string id)
        {
            foreach (var data in EnemyDataList)
            {
                if (data.id == id)
                {
                    return data;
                }
            }

            GameLogger.LogError($"[EnemyTableSO] EnemyData not found: {id}");
            return null;
        }
    }
}
