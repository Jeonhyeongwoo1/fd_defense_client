using System;
using Game.Core;
using Game.Data;
using UnityEngine;

namespace Game.Service
{
    public class DailyRewardService
    {
        private const string LastClaimDateKey = "LastClaimDate";
        private const string DailyStreakKey = "DailyStreak";
        private const int MaxStreakDays = 7;

        private readonly DailyRewardTableSO _rewardTable;
        private readonly GoldService _goldService;

        public DailyRewardService(DailyRewardTableSO rewardTable, GoldService goldService)
        {
            _rewardTable = rewardTable;
            _goldService = goldService;
        }

        public bool CanClaim()
        {
            var lastClaimDateStr = PlayerPrefs.GetString(LastClaimDateKey, string.Empty);
            if (string.IsNullOrEmpty(lastClaimDateStr))
            {
                return true;
            }

            if (!DateTime.TryParseExact(lastClaimDateStr, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var lastClaimDate))
            {
                return true;
            }

            var today = DateTime.Now.Date;
            return lastClaimDate < today;
        }

        public int GetCurrentDay()
        {
            var lastClaimDateStr = PlayerPrefs.GetString(LastClaimDateKey, string.Empty);
            var currentStreak = PlayerPrefs.GetInt(DailyStreakKey, 0);

            if (string.IsNullOrEmpty(lastClaimDateStr))
            {
                return 1;
            }

            if (!DateTime.TryParseExact(lastClaimDateStr, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var lastClaimDate))
            {
                return 1;
            }

            var today = DateTime.Now.Date;
            var daysDifference = (today - lastClaimDate).Days;

            if (daysDifference == 1)
            {
                var nextDay = currentStreak + 1;
                return nextDay > MaxStreakDays ? 1 : nextDay;
            }
            else if (daysDifference > 1)
            {
                return 1;
            }
            else
            {
                return currentStreak;
            }
        }

        public int Claim()
        {
            if (!CanClaim())
            {
                GameLogger.LogWarning("[DailyRewardService] Reward already claimed today");
                return 0;
            }

            var day = GetCurrentDay();
            var rewardData = _rewardTable.GetByDay(day);
            if (rewardData == null)
            {
                GameLogger.LogWarning($"[DailyRewardService] No reward data for day {day}");
                return 0;
            }

            _goldService.Add(rewardData.gold);

            var today = DateTime.Now.Date;
            PlayerPrefs.SetString(LastClaimDateKey, today.ToString("yyyyMMdd"));
            PlayerPrefs.SetInt(DailyStreakKey, day);
            PlayerPrefs.Save();

            GameLogger.Log($"[DailyRewardService] Claimed day {day} reward: {rewardData.gold} gold");
            return rewardData.gold;
        }
    }
}
