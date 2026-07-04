using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "WaveTable", menuName = "Game/WaveTable")]
    public class WaveTableSO : ScriptableObject
    {
        public List<WaveData> WaveDataList = new();

        public WaveData GetById(string id)
        {
            foreach (var data in WaveDataList)
            {
                if (data.id == id)
                {
                    return data;
                }
            }

            return null;
        }
    }
}
