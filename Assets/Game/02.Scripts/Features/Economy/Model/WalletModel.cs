using UniRx;

namespace Game.Model
{
    public class WalletModel
    {
        public IReadOnlyReactiveProperty<int> Money => _money;

        private readonly ReactiveProperty<int> _money = new();

        public void SetMoney(int value)
        {
            _money.Value = value;
        }

        public void AddMoney(int amount)
        {
            _money.Value += amount;
        }

        public bool TrySpend(int cost)
        {
            if (_money.Value < cost)
            {
                return false;
            }

            _money.Value -= cost;
            return true;
        }
    }
}
