using Game.Core;

namespace Game.Service
{
    public class SettingsService
    {
        private readonly UserDataService _userDataService;

        public SettingsService(UserDataService userDataService)
        {
            _userDataService = userDataService;
        }

        public bool IsVibrationEnabled
        {
            get => _userDataService.Data.settings.isVibrationEnabled;
            set
            {
                _userDataService.Data.settings.isVibrationEnabled = value;
                _userDataService.Save();
            }
        }

        public void ResetAllProgress()
        {
            _userDataService.ResetToDefault();
            GameLogger.LogWarning("[SettingsService] All player progress has been reset (irreversible)");
        }
    }
}
