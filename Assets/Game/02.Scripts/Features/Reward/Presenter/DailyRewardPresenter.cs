using System;
using Game.Core;
using Game.Data;
using Game.Service;
using Game.View;
using UniRx;
using VContainer.Unity;

namespace Game.Presenter
{
    public class DailyRewardPresenter : IStartable, IDisposable
    {
        private readonly UI_DailyRewardPopupView _view;
        private readonly DailyRewardTableSO _rewardTable;
        private readonly DailyRewardService _dailyRewardService;
        private readonly CompositeDisposable _disposables = new();

        public DailyRewardPresenter(
            UI_DailyRewardPopupView view,
            DailyRewardTableSO rewardTable,
            DailyRewardService dailyRewardService)
        {
            _view = view;
            _rewardTable = rewardTable;
            _dailyRewardService = dailyRewardService;
        }

        public void Start()
        {
            _view.ClaimButton.onClick.AsObservable()
                .Subscribe(_ => OnClaimClicked())
                .AddTo(_disposables);

            _view.CloseButton.onClick.AsObservable()
                .Subscribe(_ => OnCloseClicked())
                .AddTo(_disposables);

            if (_dailyRewardService.CanClaim())
            {
                ShowPopup();
            }
            else
            {
                _view.Hide();
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void ShowPopup()
        {
            var currentDay = _dailyRewardService.GetCurrentDay();
            var canClaim = _dailyRewardService.CanClaim();

            for (var i = 0; i < 7; i++)
            {
                var day = i + 1;
                var rewardData = _rewardTable.GetByDay(day);

                if (rewardData == null)
                {
                    GameLogger.LogWarning($"[DailyRewardPresenter] No reward data for day {day}");
                    continue;
                }

                var isClaimed = day < currentDay || (day == currentDay && !canClaim);
                var isToday = day == currentDay && canClaim;

                _view.UpdateSlot(i, day, rewardData.gold, isClaimed, isToday);
            }

            _view.ClaimButton.interactable = canClaim;
            _view.Show();
        }

        private void OnClaimClicked()
        {
            var gold = _dailyRewardService.Claim();

            if (gold > 0)
            {
                GameLogger.Log($"[DailyRewardPresenter] Claimed daily reward: {gold} gold");
                RefreshAndClose();
            }
        }

        private void OnCloseClicked()
        {
            _view.Hide();
        }

        private void RefreshAndClose()
        {
            var currentDay = _dailyRewardService.GetCurrentDay();

            for (var i = 0; i < 7; i++)
            {
                var day = i + 1;
                var rewardData = _rewardTable.GetByDay(day);

                if (rewardData == null)
                {
                    continue;
                }

                var isClaimed = day <= currentDay;
                var isToday = false;

                _view.UpdateSlot(i, day, rewardData.gold, isClaimed, isToday);
            }

            _view.ClaimButton.interactable = false;
            _view.Hide();
        }
    }
}
