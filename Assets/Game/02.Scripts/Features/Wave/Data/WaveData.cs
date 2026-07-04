using System;
using System.Collections.Generic;

namespace Game.Data
{
    [Serializable]
    public class WaveData
    {
        public string id;
        public List<WaveSpawnEntry> SpawnEntryList = new();
    }

    [Serializable]
    public class WaveSpawnEntry
    {
        public string enemyId;
        public int count;
        public float interval;
    }
}
