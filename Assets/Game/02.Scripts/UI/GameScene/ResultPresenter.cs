using System;
using Game.App;
using Game.Core;
using Game.GameState;
using Game.Model;
using Game.View;
using UniRx;
using VContainer.Unity;

namespace Game.Presenter
{
    public class ResultPresenter : IStartable, IDisposable
    {
        private readonly UI_ResultPopupView _view;
        private readonly GameStateModel _gameStateModel;
        private readonly GameResultModel _gameResultModel;
        private readonly SceneLoadService _sceneLoadService;
        private readonly CompositeDisposable _disposables = new();

        public ResultPresenter(
            UI_ResultPopupView view,
            GameStateModel gameStateModel,
            GameResultModel gameResultModel,
            SceneLoadService sceneLoadService)
        {
            _view = view;
            _gameStateModel = gameStateModel;
            _gameResultModel = gameResultModel;
            _sceneLoadService = sceneLoadService;
        }

        public void Start()
        {
            _view.Hide();

            _gameStateModel.CurrentStateType
                .Where(state => state == GameStateType.Result)
                .Subscribe(_ => OnResultEntered())
                .AddTo(_disposables);

            _view.RetryButton.onClick.AsObservable()
                .Subscribe(_ => OnRetryClicked())
                .AddTo(_disposables);

            _view.StageSelectButton.onClick.AsObservable()
                .Subscribe(_ => OnStageSelectClicked())
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void OnResultEntered()
        {
            if (_gameResultModel.Winner == null)
            {
                GameLogger.LogWarning("[ResultPresenter] Winner is null, showing DEFEAT");
                _view.UpdateUI(false);
                return;
            }

            var isVictory = _gameResultModel.Winner == UnitSide.Ally;
            _view.UpdateUI(isVictory);
        }

        private void OnRetryClicked()
        {
            _sceneLoadService.LoadGameScene();
        }

        private void OnStageSelectClicked()
        {
            _sceneLoadService.LoadOutGameScene();
        }
    }
}
