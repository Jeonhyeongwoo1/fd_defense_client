using System;

namespace Game.Data
{
    public enum MissionType
    {
        KillEnemies,
        KillBosses,
        ClearStages,
        UpgradeUnits
    }

    [Serializable]
    public class MissionData
    {
        public string id;
        public MissionType missionType;
        public int targetCount;
        public int goldReward;
        public string description;
    }
}
