using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
        private readonly WaveProgressService _waveProgressService;
        private readonly BaseService _baseService;
        private readonly DeckService _deckService;
        private readonly CompositeDisposable _disposables = new();
        private readonly List<(UI_UnitSpawnButtonView view, UnitData data)> _buttonEntryList = new();

        public GameHudPresenter(
            UI_GameHudView view,
            WalletModel walletModel,
            UnitSpawnService unitSpawnService,
            UnitTableSO unitTable,
            WaveProgressService waveProgressService,
            BaseService baseService,
            DeckService deckService)
        {
            _view = view;
            _walletModel = walletModel;
            _unitSpawnService = unitSpawnService;
            _unitTable = unitTable;
            _waveProgressService = waveProgressService;
            _baseService = baseService;
            _deckService = deckService;
        }

        public void Start()
        {
            _walletModel.Money.Subscribe(_view.UpdateMoney).AddTo(_disposables);

            _waveProgressService.CurrentWaveIndex
                .Subscribe(index => _view.UpdateWave(index + 1, _waveProgressService.TotalWaveCount))
                .AddTo(_disposables);

            _waveProgressService.IsBossWave
                .Subscribe(isBoss =>
                {
                    if (isBoss)
                    {
                        _view.UpdateBossWave();
                        ShowBossBannerAsync().Forget();
                    }
                    else
                    {
                        _view.UpdateWave(_waveProgressService.CurrentWaveIndex.Value + 1, _waveProgressService.TotalWaveCount);
                    }
                })
                .AddTo(_disposables);

            var allyBase = _baseService.GetBase(UnitSide.Ally);
            allyBase.CurrentHp
                .Subscribe(hp => _view.UpdateAllyBaseHp((float)hp / allyBase.MaxHp))
                .AddTo(_disposables);

            var enemyBase = _baseService.GetBase(UnitSide.Enemy);
            enemyBase.CurrentHp
                .Subscribe(hp => _view.UpdateEnemyBaseHp((float)hp / enemyBase.MaxHp))
                .AddTo(_disposables);

            var deck = _deckService.GetDeck();

            for (var i = 0; i < _view.SpawnButtonList.Count; i++)
            {
                var button = _view.SpawnButtonList[i];

                if (i >= deck.Count)
                {
                    GameLogger.LogWarning($"[GameHudPresenter] Not enough deck entries ({deck.Count}) for button {i}");
                    break;
                }

                var unitId = deck[i];
                var data = _unitTable.GetById(unitId);
                if (data == null)
                {
                    GameLogger.LogError($"[GameHudPresenter] Unit data not found for deck entry: {unitId}");
                    continue;
                }

                button.Bind(unitId, data.unitName, data.cost, data.iconSprite);
                _buttonEntryList.Add((button, data));

                var boundUnitId = unitId;
                button.Button.onClick.AsObservable()
                    .Subscribe(_ => OnSpawnButtonClicked(boundUnitId))
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

        private async UniTaskVoid ShowBossBannerAsync()
        {
            _view.ShowBossBanner(true);
            await UniTask.Delay(TimeSpan.FromSeconds(Const.BossBannerSeconds), DelayType.UnscaledDeltaTime);
            _view.ShowBossBanner(false);
        }
    }
}
