using System;
using Game.Data;
using Game.Service;
using Game.View;
using UniRx;
using VContainer.Unity;

namespace Game.Presenter
{
    public class OutGameHomePresenter : IStartable, ITickable, IDisposable
    {
        private const float BadgeRefreshIntervalSeconds = 1f;

        private readonly UI_OutGameHomeView _view;
        private readonly GoldService _goldService;
        private readonly UI_DeckPanelView _deckPanelView;
        private readonly UI_UpgradePanelView _upgradePanelView;
        private readonly UI_MissionPanelView _missionPanelView;
        private readonly UI_ShopPanelView _shopPanelView;
        private readonly DeckPresenter _deckPresenter;
        private readonly UpgradePresenter _upgradePresenter;
        private readonly MissionPanelPresenter _missionPanelPresenter;
        private readonly ShopPanelPresenter _shopPanelPresenter;
        private readonly SettingsPresenter _settingsPresenter;
        private readonly DailyRewardPresenter _dailyRewardPresenter;
        private readonly DailyRewardService _dailyRewardService;
        private readonly MissionService _missionService;
        private readonly MissionTableSO _missionTable;
        private readonly StageTableSO _stageTable;
        private readonly StageProgressService _stageProgressService;
        private readonly UnitTableSO _unitTable;
        private readonly UserDataService _userDataService;
        private readonly CompositeDisposable _disposables = new();
        private float _badgeRefreshTimer;

        public OutGameHomePresenter(
            UI_OutGameHomeView view,
            GoldService goldService,
            UI_DeckPanelView deckPanelView,
            UI_UpgradePanelView upgradePanelView,
            UI_MissionPanelView missionPanelView,
            UI_ShopPanelView shopPanelView,
            DeckPresenter deckPresenter,
            UpgradePresenter upgradePresenter,
            MissionPanelPresenter missionPanelPresenter,
            ShopPanelPresenter shopPanelPresenter,
            SettingsPresenter settingsPresenter,
            DailyRewardPresenter dailyRewardPresenter,
            DailyRewardService dailyRewardService,
            MissionService missionService,
            MissionTableSO missionTable,
            StageTableSO stageTable,
            StageProgressService stageProgressService,
            UnitTableSO unitTable,
            UserDataService userDataService)
        {
            _view = view;
            _goldService = goldService;
            _deckPanelView = deckPanelView;
            _upgradePanelView = upgradePanelView;
            _missionPanelView = missionPanelView;
            _shopPanelView = shopPanelView;
            _deckPresenter = deckPresenter;
            _upgradePresenter = upgradePresenter;
            _missionPanelPresenter = missionPanelPresenter;
            _shopPanelPresenter = shopPanelPresenter;
            _settingsPresenter = settingsPresenter;
            _dailyRewardPresenter = dailyRewardPresenter;
            _dailyRewardService = dailyRewardService;
            _missionService = missionService;
            _missionTable = missionTable;
            _stageTable = stageTable;
            _stageProgressService = stageProgressService;
            _unitTable = unitTable;
            _userDataService = userDataService;
        }

        public void Start()
        {
            _goldService.Gold.Subscribe(gold => _view.UpdateGold(gold)).AddTo(_disposables);

            _view.StageTabButton.onClick.AsObservable()
                .Subscribe(_ => OnStageTabClicked())
                .AddTo(_disposables);

            _view.PlayButton.onClick.AsObservable()
                .Subscribe(_ => OnStageTabClicked())
                .AddTo(_disposables);

            _view.DeckTabButton.onClick.AsObservable()
                .Subscribe(_ => OnDeckTabClicked())
                .AddTo(_disposables);

            _view.UpgradeTabButton.onClick.AsObservable()
                .Subscribe(_ => OnUpgradeTabClicked())
                .AddTo(_disposables);

            _view.MissionsTabButton.onClick.AsObservable()
                .Subscribe(_ => OnMissionsTabClicked())
                .AddTo(_disposables);

            _view.ShopTabButton.onClick.AsObservable()
                .Subscribe(_ => OnShopTabClicked())
                .AddTo(_disposables);

            _view.SettingsButton.onClick.AsObservable()
                .Subscribe(_ => OnSettingsClicked())
                .AddTo(_disposables);

            _view.DailyRewardButton.onClick.AsObservable()
                .Subscribe(_ => OnDailyRewardClicked())
                .AddTo(_disposables);

            UpdatePlayerStats();
            UpdateSelectedStageDisplay();
            RefreshBadges();

            if (_view.StagePanelRoot != null)
            {
                _view.StagePanelRoot.SetActive(false);
            }

            _deckPanelView.Hide();
            _upgradePanelView.Hide();
            _missionPanelView.Hide();
            _shopPanelView.Hide();
        }

        public void Tick()
        {
            _badgeRefreshTimer += UnityEngine.Time.deltaTime;

            if (_badgeRefreshTimer >= BadgeRefreshIntervalSeconds)
            {
                _badgeRefreshTimer = 0f;
                RefreshBadges();
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void OnStageTabClicked()
        {
            if (_view.StagePanelRoot != null)
            {
                _view.StagePanelRoot.SetActive(true);
            }

            _deckPanelView.Hide();
            _upgradePanelView.Hide();
            _missionPanelView.Hide();
            _shopPanelView.Hide();
        }

        private void OnDeckTabClicked()
        {
            if (_view.StagePanelRoot != null)
            {
                _view.StagePanelRoot.SetActive(false);
            }

            _deckPanelView.Show();
            _upgradePanelView.Hide();
            _missionPanelView.Hide();
            _shopPanelView.Hide();

            _deckPresenter.Refresh();
        }

        private void OnUpgradeTabClicked()
        {
            if (_view.StagePanelRoot != null)
            {
                _view.StagePanelRoot.SetActive(false);
            }

            _deckPanelView.Hide();
            _upgradePanelView.Show();
            _missionPanelView.Hide();
            _shopPanelView.Hide();

            _upgradePresenter.Refresh();
        }

        private void OnMissionsTabClicked()
        {
            if (_view.StagePanelRoot != null)
            {
                _view.StagePanelRoot.SetActive(false);
            }

            _deckPanelView.Hide();
            _upgradePanelView.Hide();
            _missionPanelView.Show();
            _shopPanelView.Hide();

            _missionPanelPresenter.Refresh();
        }

        private void OnShopTabClicked()
        {
            if (_view.StagePanelRoot != null)
            {
                _view.StagePanelRoot.SetActive(false);
            }

            _deckPanelView.Hide();
            _upgradePanelView.Hide();
            _missionPanelView.Hide();
            _shopPanelView.Show();

            _shopPanelPresenter.Refresh();
            UpdatePlayerStats();
        }

        private void OnSettingsClicked()
        {
            _settingsPresenter.Show();
        }

        private void OnDailyRewardClicked()
        {
            _dailyRewardPresenter.Show();
        }

        private void UpdatePlayerStats()
        {
            var bestStageNumber = 0;

            foreach (var stageData in _stageTable.StageDataList)
            {
                if (_stageProgressService.IsStageCleared(stageData.id))
                {
                    if (int.TryParse(stageData.id.Replace("stage_", ""), out var stageNumber))
                    {
                        if (stageNumber > bestStageNumber)
                        {
                            bestStageNumber = stageNumber;
                        }
                    }
                }
            }

            var bestStageLabel = bestStageNumber > 0 ? $"BEST STAGE {bestStageNumber}" : "BEST STAGE -";

            var ownedCount = 0;
            foreach (var unitData in _unitTable.UnitDataList)
            {
                if (_userDataService.IsUnitOwned(unitData.id))
                {
                    ownedCount++;
                }
            }

            var totalCount = _unitTable.UnitDataList.Count;

            _view.UpdatePlayerStats(bestStageLabel, ownedCount, totalCount);
        }

        private void UpdateSelectedStageDisplay()
        {
            var selectedStageId = _stageProgressService.GetSelectedStageId();
            var stageData = _stageTable.GetById(selectedStageId);

            if (stageData == null)
            {
                Game.Core.GameLogger.LogWarning($"[OutGameHomePresenter] Selected stage '{selectedStageId}' not found in StageTable.");
                return;
            }

            var stageNumber = 0;
            if (int.TryParse(stageData.id.Replace("stage_", ""), out stageNumber))
            {
                _view.UpdateSelectedStage($"STAGE {stageNumber}", stageData.stageName);
            }
            else
            {
                Game.Core.GameLogger.LogWarning($"[OutGameHomePresenter] Failed to parse stage number from '{stageData.id}'.");
            }
        }

        private void RefreshBadges()
        {
            var canClaimDaily = _dailyRewardService.CanClaim();
            if (_view.DailyRewardBadge != null)
            {
                _view.DailyRewardBadge.SetActive(canClaimDaily);
            }

            var hasClaimableMission = false;
            foreach (var missionData in _missionTable.MissionDataList)
            {
                var isCompleted = _missionService.IsCompleted(missionData);
                var isClaimed = _missionService.IsClaimed(missionData.id);

                if (isCompleted && !isClaimed)
                {
                    hasClaimableMission = true;
                    break;
                }
            }

            if (_view.MissionsBadge != null)
            {
                _view.MissionsBadge.SetActive(hasClaimableMission);
            }
        }
    }
}
