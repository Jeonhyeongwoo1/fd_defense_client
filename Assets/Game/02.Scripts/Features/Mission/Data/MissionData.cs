using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    [CreateAssetMenu(fileName = "MissionTable", menuName = "Game/Data/MissionTable")]
    public class MissionTableSO : ScriptableObject
    {
        public List<MissionData> MissionDataList = new();

        public MissionData GetById(string id)
        {
            return MissionDataList.FirstOrDefault(m => m.id == id);
        }
    }
}
