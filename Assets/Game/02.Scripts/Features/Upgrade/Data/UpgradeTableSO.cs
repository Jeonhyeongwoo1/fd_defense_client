using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(menuName = "Game/UpgradeTable")]
    public class UpgradeTableSO : ScriptableObject
    {
        public List<UpgradeData> UpgradeDataList = new();

        public UpgradeData GetByLevel(int level)
        {
            foreach (var data in UpgradeDataList)
            {
                if (data.level == level)
                {
                    return data;
                }
            }

            return null;
        }

        public int MaxLevel
        {
            get
            {
                if (UpgradeDataList.Count == 0)
                {
                    return 1;
                }

                var max = 1;
                foreach (var data in UpgradeDataList)
                {
                    if (data.level > max)
                    {
                        max = data.level;
                    }
                }
                return max;
            }
        }
    }
}
