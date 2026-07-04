using System;
using Game.App;
using Game.Service;
using Game.View;
using UniRx;
using VContainer.Unity;

namespace Game.Presenter
{
    public class PausePresenter : IStartable, IDisposable
    {
        private readonly UI_PausePopupView _view;
        private readonly PauseService _pauseService;
        private readonly SceneLoadService _sceneLoadService;
        private readonly CompositeDisposable _disposables = new();

        public PausePresenter(
            UI_PausePopupView view,
            PauseService pauseService,
            SceneLoadService sceneLoadService)
        {
            _view = view;
            _pauseService = pauseService;
            _sceneLoadService = sceneLoadService;
        }

        public void Start()
        {
            _view.Hide();

            _view.PauseButton.onClick.AsObservable()
                .Subscribe(_ => OnPauseClicked())
                .AddTo(_disposables);

            _view.ResumeButton.onClick.AsObservable()
                .Subscribe(_ => OnResumeClicked())
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

        private void OnPauseClicked()
        {
            _pauseService.SetPaused(true);
            _view.Show();
        }

        private void OnResumeClicked()
        {
            _pauseService.SetPaused(false);
            _view.Hide();
        }

        private void OnRetryClicked()
        {
            _pauseService.SetPaused(false);
            _sceneLoadService.LoadGameScene();
        }

        private void OnStageSelectClicked()
        {
            _pauseService.SetPaused(false);
            _sceneLoadService.LoadOutGameScene();
        }
    }
}
