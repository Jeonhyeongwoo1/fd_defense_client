using System;
using System.Collections.Generic;

namespace Game.Model
{
    [Serializable]
    public class UserDataModel
    {
        public int version = 1;
        public int gold;
        public List<string> ownedUnitIds = new();
        public List<UnitLevelEntry> unitLevels = new();
        public List<string> deckUnitIds = new();
        public string selectedStageId;
        public List<string> clearedStageIds = new();
        public DailyData daily = new();
        public List<MissionCountEntry> missionCounts = new();
        public List<string> claimedMissionIds = new();
        public SettingsData settings = new();
        public List<string> purchaseHistory = new();
    }

    [Serializable]
    public class UnitLevelEntry
    {
        public string unitId;
        public int level;
    }

    [Serializable]
    public class MissionCountEntry
    {
        public string missionType;
        public int count;
    }

    [Serializable]
    public class DailyData
    {
        public string lastClaimDate;
        public int streak;
    }

    [Serializable]
    public class SettingsData
    {
        public bool isVibrationEnabled = true;
    }
}
