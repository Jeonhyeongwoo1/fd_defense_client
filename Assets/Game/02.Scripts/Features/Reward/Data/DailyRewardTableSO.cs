using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "DailyRewardTable", menuName = "Game/Data/DailyRewardTable")]
    public class DailyRewardTableSO : ScriptableObject
    {
        public List<DailyRewardData> DailyRewardDataList = new();

        public DailyRewardData GetByDay(int day)
        {
            return DailyRewardDataList.FirstOrDefault(d => d.day == day);
        }

        public int MaxDay => DailyRewardDataList.Count > 0 ? DailyRewardDataList.Max(d => d.day) : 0;
    }
}
