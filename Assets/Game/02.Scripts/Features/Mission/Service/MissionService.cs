using Game.Core;
using Game.Data;

namespace Game.Service
{
    public class MissionService
    {
        private readonly GoldService _goldService;
        private readonly UserDataService _userDataService;

        public MissionService(GoldService goldService, UserDataService userDataService)
        {
            _goldService = goldService;
            _userDataService = userDataService;
        }

        public void AddProgress(MissionType type, int amount = 1)
        {
            var missionType = type.ToString();
            var currentCount = _userDataService.GetMissionCount(missionType);
            var newCount = currentCount + amount;
            _userDataService.SetMissionCount(missionType, newCount);
            _userDataService.Save();
        }

        public int GetProgress(MissionType type)
        {
            var missionType = type.ToString();
            return _userDataService.GetMissionCount(missionType);
        }

        public bool IsCompleted(MissionData data)
        {
            var progress = GetProgress(data.missionType);
            return progress >= data.targetCount;
        }

        public bool IsClaimed(string missionId)
        {
            return _userDataService.IsMissionClaimed(missionId);
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

            _userDataService.MarkMissionClaimed(missionId);
            _userDataService.Save();

            GameLogger.Log($"[MissionService] Mission {missionId} claimed: {data.goldReward} gold");
            return true;
        }
    }
}
