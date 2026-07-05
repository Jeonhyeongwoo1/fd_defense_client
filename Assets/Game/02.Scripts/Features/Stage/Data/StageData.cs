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
        public List<string> WaveIdList = new();
        public float waveIntervalSeconds;
        public string bossId;
        public string mapId;
        public int goldReward;
        public float enemyStatMultiplier;
        public string stageName;
    }
}
