using System;
using Game.Service;
using Game.View;
using UniRx;
using VContainer.Unity;

namespace Game.Presenter
{
    public class OutGameHomePresenter : IStartable, IDisposable
    {
        private readonly UI_OutGameHomeView _view;
        private readonly GoldService _goldService;
        private readonly UI_DeckPanelView _deckPanelView;
        private readonly UI_UpgradePanelView _upgradePanelView;
        private readonly UI_MissionPanelView _missionPanelView;
        private readonly DeckPresenter _deckPresenter;
        private readonly UpgradePresenter _upgradePresenter;
        private readonly MissionPanelPresenter _missionPanelPresenter;
        private readonly SettingsPresenter _settingsPresenter;
        private readonly CompositeDisposable _disposables = new();

        public OutGameHomePresenter(
            UI_OutGameHomeView view,
            GoldService goldService,
            UI_DeckPanelView deckPanelView,
            UI_UpgradePanelView upgradePanelView,
            UI_MissionPanelView missionPanelView,
            DeckPresenter deckPresenter,
            UpgradePresenter upgradePresenter,
            MissionPanelPresenter missionPanelPresenter,
            SettingsPresenter settingsPresenter)
        {
            _view = view;
            _goldService = goldService;
            _deckPanelView = deckPanelView;
            _upgradePanelView = upgradePanelView;
            _missionPanelView = missionPanelView;
            _deckPresenter = deckPresenter;
            _upgradePresenter = upgradePresenter;
            _missionPanelPresenter = missionPanelPresenter;
            _settingsPresenter = settingsPresenter;
        }

        public void Start()
        {
            _goldService.Gold.Subscribe(gold => _view.UpdateGold(gold)).AddTo(_disposables);

            _view.StageTabButton.onClick.AsObservable()
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

            _view.SettingsButton.onClick.AsObservable()
                .Subscribe(_ => OnSettingsClicked())
                .AddTo(_disposables);

            OnStageTabClicked();
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

            _missionPanelPresenter.Refresh();
        }

        private void OnSettingsClicked()
        {
            _settingsPresenter.Show();
        }
    }
}
