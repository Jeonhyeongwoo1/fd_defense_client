using Game.Core;
using Game.Data;
using UnityEngine;

namespace Game.Service
{
    public class MissionService
    {
        private const string MissionCountPrefix = "MissionCount_";
        private const string MissionClaimedPrefix = "MissionClaimed_";

        private readonly GoldService _goldService;

        public MissionService(GoldService goldService)
        {
            _goldService = goldService;
        }

        public void AddProgress(MissionType type, int amount = 1)
        {
            var key = MissionCountPrefix + type.ToString();
            var currentCount = PlayerPrefs.GetInt(key, 0);
            var newCount = currentCount + amount;
            PlayerPrefs.SetInt(key, newCount);
            PlayerPrefs.Save();
        }

        public int GetProgress(MissionType type)
        {
            var key = MissionCountPrefix + type.ToString();
            return PlayerPrefs.GetInt(key, 0);
        }

        public bool IsCompleted(MissionData data)
        {
            var progress = GetProgress(data.missionType);
            return progress >= data.targetCount;
        }

        public bool IsClaimed(string missionId)
        {
            var key = MissionClaimedPrefix + missionId;
            return PlayerPrefs.GetInt(key, 0) == 1;
        }

        public bool TryClaim(string missionId, MissionData data)
        {
            if (!IsCompleted(data))
            {
                GameLogger.LogWarning($"[MissionService] Mission {missionId} is not completed yet");
                return false;
            }

            if (IsClaimed(missionId))
            {
                GameLogger.LogWarning($"[MissionService] Mission {missionId} already claimed");
                return false;
            }

            _goldService.Add(data.goldReward);

            var key = MissionClaimedPrefix + missionId;
            PlayerPrefs.SetInt(key, 1);
            PlayerPrefs.Save();

            GameLogger.Log($"[MissionService] Mission {missionId} claimed: {data.goldReward} gold");
            return true;
        }
    }
}
