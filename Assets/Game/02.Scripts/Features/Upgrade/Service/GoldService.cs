using Game.Core;
using UniRx;
using UnityEngine;

namespace Game.Service
{
    public class GoldService
    {
        private const string GoldKey = "Gold";

        private readonly ReactiveProperty<int> _gold = new();

        public IReadOnlyReactiveProperty<int> Gold => _gold;

        public GoldService()
        {
            var savedGold = PlayerPrefs.GetInt(GoldKey, 0);
            _gold.Value = savedGold;
        }

        public void Add(int amount)
        {
            if (amount < 0)
            {
                GameLogger.LogWarning($"[GoldService] Attempted to add negative amount: {amount}");
                return;
            }

            _gold.Value += amount;
            PlayerPrefs.SetInt(GoldKey, _gold.Value);
            PlayerPrefs.Save();
        }

        public bool TrySpend(int amount)
        {
            if (_gold.Value < amount)
            {
                return false;
            }

            _gold.Value -= amount;
            PlayerPrefs.SetInt(GoldKey, _gold.Value);
            PlayerPrefs.Save();
            return true;
        }
    }
}
