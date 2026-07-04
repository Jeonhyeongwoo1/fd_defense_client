using System.Collections.Generic;
using Game.Core;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(menuName = "Game/UnitTable")]
    public class UnitTableSO : ScriptableObject
    {
        public List<UnitData> UnitDataList = new();

        public UnitData GetById(string id)
        {
            foreach (var data in UnitDataList)
            {
                if (data.id == id)
                {
                    return data;
                }
            }

            GameLogger.LogError($"[UnitTableSO] UnitData not found: {id}");
            return null;
        }
    }
}
