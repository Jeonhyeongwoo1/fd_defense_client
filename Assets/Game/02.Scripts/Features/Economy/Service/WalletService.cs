using Game.GameState;
using Game.Model;
using UnityEngine;
using VContainer.Unity;

namespace Game.Service
{
    public class WalletService : ITickable
    {
        private readonly WalletModel _walletModel;
        private readonly GameStateModel _gameStateModel;

        private float _moneyPerSecond;
        private float _fractionalMoney;

        public WalletService(WalletModel walletModel, GameStateModel gameStateModel)
        {
            _walletModel = walletModel;
            _gameStateModel = gameStateModel;
        }

        public void Initialize(int startMoney, float ratePerSecond)
        {
            _walletModel.SetMoney(startMoney);
            _moneyPerSecond = ratePerSecond;
            _fractionalMoney = 0f;
        }

        public void Tick()
        {
            if (_gameStateModel.CurrentStateType.Value != GameStateType.WavePlaying)
            {
                return;
            }

            _fractionalMoney += _moneyPerSecond * Time.deltaTime;

            if (_fractionalMoney >= 1f)
            {
                var integerPart = Mathf.FloorToInt(_fractionalMoney);
                _walletModel.AddMoney(integerPart);
                _fractionalMoney -= integerPart;
            }
        }

        public bool TrySpend(int cost)
        {
            return _walletModel.TrySpend(cost);
        }
    }
}
