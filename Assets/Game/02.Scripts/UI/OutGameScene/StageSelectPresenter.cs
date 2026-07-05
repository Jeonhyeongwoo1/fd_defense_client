using System;
using System.Collections.Generic;
using Game.App;
using Game.Core;
using Game.Data;
using Game.Service;
using Game.View;
using UniRx;
using VContainer.Unity;

namespace Game.Presenter
{
    public class StageSelectPresenter : IStartable, IDisposable
    {
        private readonly UI_StageSelectView _view;
        private readonly StageTableSO _stageTable;
        private readonly StageProgressService _stageProgressService;
        private readonly SceneLoadService _sceneLoadService;
        private readonly CompositeDisposable _disposables = new();

        public StageSelectPresenter(
            UI_StageSelectView view,
            StageTableSO stageTable,
            StageProgressService stageProgressService,
            SceneLoadService sceneLoadService)
        {
            _view = view;
            _stageTable = stageTable;
            _stageProgressService = stageProgressService;
            _sceneLoadService = sceneLoadService;
        }

        public void Start()
        {
            var stageDataList = _stageTable.StageDataList;
            var buttonList = _view.StageButtonList;

            for (var i = 0; i < buttonList.Count; i++)
            {
                var button = buttonList[i];
                var stageId = button.StageId;

                var stageData = _stageTable.GetById(stageId);
                if (stageData == null)
                {
                    GameLogger.LogError($"[StageSelectPresenter] StageData not found for button: {stageId}");
                    continue;
                }

                var isCleared = _stageProgressService.IsStageCleared(stageId);
                var isUnlocked = IsStageUnlocked(i, stageDataList);

                button.UpdateUI(stageData.id, isCleared, isUnlocked);

                var capturedStageId = stageId;
                button.Button.onClick.AsObservable()
                    .Subscribe(_ => OnStageButtonClicked(capturedStageId))
                    .AddTo(_disposables);
            }

            _view.CloseButton.onClick.AsObservable()
                .Subscribe(_ => OnCloseButtonClicked())
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private bool IsStageUnlocked(int stageIndex, List<StageData> stageDataList)
        {
            if (stageIndex == 0)
            {
                return true;
            }

            if (stageIndex >= stageDataList.Count)
            {
                return false;
            }

            var previousStageId = stageDataList[stageIndex - 1].id;
            return _stageProgressService.IsStageCleared(previousStageId);
        }

        private void OnStageButtonClicked(string stageId)
        {
            _stageProgressService.SetSelectedStageId(stageId);
            _sceneLoadService.LoadGameScene();
        }

        private void OnCloseButtonClicked()
        {
            _view.Hide();
        }
    }
}
