using System;

namespace Game.Data
{
    [Serializable]
    public class UpgradeData
    {
        public int level;
        public int goldCost;
        public float hpMultiplier;
        public float attackMultiplier;
    }
}
