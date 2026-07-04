using System;
using System.Collections.Generic;
using Game.Core;
using Game.Data;
using Game.Model;
using Game.Service;
using Game.View;
using UniRx;
using UnityEngine;
using VContainer.Unity;

namespace Game.Presenter
{
    public class GameHudPresenter : IStartable, ITickable, IDisposable
    {
        private readonly UI_GameHudView _view;
        private readonly WalletModel _walletModel;
        private readonly UnitSpawnService _unitSpawnService;
        private readonly UnitTableSO _unitTable;
        private readonly CompositeDisposable _disposables = new();
        private readonly List<(UI_UnitSpawnButtonView view, UnitData data)> _buttonEntryList = new();

        public GameHudPresenter(
            UI_GameHudView view,
            WalletModel walletModel,
            UnitSpawnService unitSpawnService,
            UnitTableSO unitTable)
        {
            _view = view;
            _walletModel = walletModel;
            _unitSpawnService = unitSpawnService;
            _unitTable = unitTable;
        }

        public void Start()
        {
            _walletModel.Money.Subscribe(_view.UpdateMoney).AddTo(_disposables);

            foreach (var button in _view.SpawnButtonList)
            {
                var data = _unitTable.GetById(button.UnitId);
                if (data == null)
                {
                    GameLogger.LogError($"[GameHudPresenter] Unit data not found for button: {button.UnitId}");
                    continue;
                }

                _buttonEntryList.Add((button, data));

                button.UpdateUI(data.unitName, data.cost);

                var unitId = button.UnitId;
                button.Button.onClick.AsObservable()
                    .Subscribe(_ => OnSpawnButtonClicked(unitId))
                    .AddTo(_disposables);
            }
        }

        public void Tick()
        {
            foreach (var (button, data) in _buttonEntryList)
            {
                var cooldownRemaining = _unitSpawnService.GetCooldownRemaining(button.UnitId);
                var cooldownRatio = data.cooldown > 0 ? cooldownRemaining / data.cooldown : 0f;
                button.SetCooldownRatio(cooldownRatio);

                var canSpawn = cooldownRemaining <= 0f && _walletModel.Money.Value >= data.cost;
                button.SetInteractable(canSpawn);
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void OnSpawnButtonClicked(string unitId)
        {
            _unitSpawnService.TrySpawnAlly(unitId);
        }
    }
}
