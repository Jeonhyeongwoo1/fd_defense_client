using Game.Core;
using UnityEngine;

namespace Game.Service
{
    public class SettingsService
    {
        private const string VibrationKey = "Vibration";

        public bool IsVibrationEnabled
        {
            get => PlayerPrefs.GetInt(VibrationKey, 1) == 1;
            set
            {
                PlayerPrefs.SetInt(VibrationKey, value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }

        public void ResetAllProgress()
        {
            // WARNING: This action is irreversible and will delete all player progress
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            GameLogger.LogWarning("[SettingsService] All player progress has been reset (irreversible)");
        }
    }
}
