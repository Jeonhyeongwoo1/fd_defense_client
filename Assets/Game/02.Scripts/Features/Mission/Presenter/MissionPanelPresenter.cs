using System;
using Game.Core;
using Game.Data;
using Game.Service;
using Game.View;
using UniRx;
using VContainer.Unity;

namespace Game.Presenter
{
    public class MissionPanelPresenter : IStartable, IDisposable
    {
        private readonly UI_MissionPanelView _view;
        private readonly MissionTableSO _missionTable;
        private readonly MissionService _missionService;
        private readonly GoldService _goldService;
        private readonly CompositeDisposable _disposables = new();

        public MissionPanelPresenter(
            UI_MissionPanelView view,
            MissionTableSO missionTable,
            MissionService missionService,
            GoldService goldService)
        {
            _view = view;
            _missionTable = missionTable;
            _missionService = missionService;
            _goldService = goldService;
        }

        public void Start()
        {
            var missionDataList = _missionTable.MissionDataList;

            for (var i = 0; i < _view.MissionRows.Length && i < missionDataList.Count; i++)
            {
                var row = _view.MissionRows[i];
                var missionData = missionDataList[i];

                var current = _missionService.GetProgress(missionData.missionType);
                var isCompleted = _missionService.IsCompleted(missionData);
                var isClaimed = _missionService.IsClaimed(missionData.id);

                row.Bind(missionData.description, current, missionData.targetCount, isCompleted, isClaimed);

                var capturedMissionData = missionData;
                row.ClaimButton.onClick.AsObservable()
                    .Subscribe(_ => OnClaimClicked(capturedMissionData))
                    .AddTo(_disposables);
            }

            for (var i = missionDataList.Count; i < _view.MissionRows.Length; i++)
            {
                _view.MissionRows[i].gameObject.SetActive(false);
            }

            _view.CloseButton.onClick.AsObservable()
                .Subscribe(_ => OnCloseButtonClicked())
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        public void Refresh()
        {
            var missionDataList = _missionTable.MissionDataList;

            for (var i = 0; i < _view.MissionRows.Length && i < missionDataList.Count; i++)
            {
                var row = _view.MissionRows[i];
                var missionData = missionDataList[i];

                var current = _missionService.GetProgress(missionData.missionType);
                var isCompleted = _missionService.IsCompleted(missionData);
                var isClaimed = _missionService.IsClaimed(missionData.id);

                row.Bind(missionData.description, current, missionData.targetCount, isCompleted, isClaimed);
            }
        }

        private void OnClaimClicked(MissionData missionData)
        {
            var success = _missionService.TryClaim(missionData.id, missionData);

            if (success)
            {
                GameLogger.Log($"[MissionPanelPresenter] Claimed mission {missionData.id}: {missionData.goldReward} gold");
                Refresh();
            }
        }

        private void OnCloseButtonClicked()
        {
            _view.Hide();
        }
    }
}
