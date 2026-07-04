using System;
using Game.App;
using Game.Core;
using Game.Service;
using Game.View;
using UniRx;
using UnityEngine;
using VContainer.Unity;

namespace Game.Presenter
{
    public class SettingsPresenter : IStartable, IDisposable
    {
        private readonly UI_SettingsPopupView _view;
        private readonly SettingsService _settingsService;
        private readonly SceneLoadService _sceneLoadService;
        private readonly CompositeDisposable _disposables = new();

        public SettingsPresenter(
            UI_SettingsPopupView view,
            SettingsService settingsService,
            SceneLoadService sceneLoadService)
        {
            _view = view;
            _settingsService = settingsService;
            _sceneLoadService = sceneLoadService;
        }

        public void Start()
        {
            _view.VibrationToggle.isOn = _settingsService.IsVibrationEnabled;

            _view.VibrationToggle.onValueChanged.AsObservable()
                .Subscribe(OnVibrationToggleChanged)
                .AddTo(_disposables);

            _view.ResetButton.onClick.AsObservable()
                .Subscribe(_ => OnResetClicked())
                .AddTo(_disposables);

            _view.ResetConfirmButton.onClick.AsObservable()
                .Subscribe(_ => OnResetConfirmClicked())
                .AddTo(_disposables);

            _view.ResetCancelButton.onClick.AsObservable()
                .Subscribe(_ => OnResetCancelClicked())
                .AddTo(_disposables);

            _view.CloseButton.onClick.AsObservable()
                .Subscribe(_ => OnCloseClicked())
                .AddTo(_disposables);

            _view.VersionText.text = $"v{Application.version}";
            _view.ResetConfirmRoot.SetActive(false);
            _view.Hide();
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        public void Show()
        {
            _view.Show();
        }

        private void OnVibrationToggleChanged(bool isOn)
        {
            _settingsService.IsVibrationEnabled = isOn;
            GameLogger.Log($"[SettingsPresenter] Vibration {(isOn ? "enabled" : "disabled")}");
        }

        private void OnResetClicked()
        {
            _view.ResetConfirmRoot.SetActive(true);
        }

        private void OnResetConfirmClicked()
        {
            _settingsService.ResetAllProgress();
            GameLogger.LogWarning("[SettingsPresenter] All progress reset");
            _sceneLoadService.LoadOutGameScene();
        }

        private void OnResetCancelClicked()
        {
            _view.ResetConfirmRoot.SetActive(false);
        }

        private void OnCloseClicked()
        {
            _view.ResetConfirmRoot.SetActive(false);
            _view.Hide();
        }
    }
}
