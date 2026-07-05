using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Data
{
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
