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
        private readonly DeckPresenter _deckPresenter;
        private readonly UpgradePresenter _upgradePresenter;
        private readonly CompositeDisposable _disposables = new();

        public OutGameHomePresenter(
            UI_OutGameHomeView view,
            GoldService goldService,
            UI_DeckPanelView deckPanelView,
            UI_UpgradePanelView upgradePanelView,
            DeckPresenter deckPresenter,
            UpgradePresenter upgradePresenter)
        {
            _view = view;
            _goldService = goldService;
            _deckPanelView = deckPanelView;
            _upgradePanelView = upgradePanelView;
            _deckPresenter = deckPresenter;
            _upgradePresenter = upgradePresenter;
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
        }

        private void OnDeckTabClicked()
        {
            if (_view.StagePanelRoot != null)
            {
                _view.StagePanelRoot.SetActive(false);
            }

            _deckPanelView.Show();
            _upgradePanelView.Hide();

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

            _upgradePresenter.Refresh();
        }
    }
}
