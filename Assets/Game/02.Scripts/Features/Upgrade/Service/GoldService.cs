using Game.Core;
using UniRx;

namespace Game.Service
{
    public class GoldService
    {
        private readonly UserDataService _userDataService;
        private readonly ReactiveProperty<int> _gold = new();

        public IReadOnlyReactiveProperty<int> Gold => _gold;

        public GoldService(UserDataService userDataService)
        {
            _userDataService = userDataService;
            _gold.Value = _userDataService.Data.gold;
        }

        public void Add(int amount)
        {
            if (amount < 0)
            {
                GameLogger.LogWarning($"[GoldService] Attempted to add negative amount: {amount}");
                return;
            }

            _gold.Value += amount;
            _userDataService.Data.gold = _gold.Value;
            _userDataService.Save();
        }

        public bool TrySpend(int amount)
        {
            if (_gold.Value < amount)
            {
                return false;
            }

            _gold.Value -= amount;
            _userDataService.Data.gold = _gold.Value;
            _userDataService.Save();
            return true;
        }
    }
}
