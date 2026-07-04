using System;
using System.Collections.Generic;

namespace Game.Data
{
    [Serializable]
    public class StageData
    {
        public string id;
        public int allyBaseHp;
        public int enemyBaseHp;
        public int startMoney;
        public float moneyPerSecond;
        public List<SpawnEntry> SpawnEntryList = new();
    }

    [Serializable]
    public class SpawnEntry
    {
        public string enemyId;
        public float interval;
    }
}
