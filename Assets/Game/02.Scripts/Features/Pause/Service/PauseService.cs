using System;
using UniRx;
using UnityEngine;

namespace Game.Service
{
    public class PauseService : IDisposable
    {
        public IReadOnlyReactiveProperty<bool> IsPaused => _isPaused;

        private readonly ReactiveProperty<bool> _isPaused = new(false);

        public void SetPaused(bool isPaused)
        {
            _isPaused.Value = isPaused;
            Time.timeScale = isPaused ? 0f : 1f;
        }

        public void Dispose()
        {
            Time.timeScale = 1f;
        }
    }
}
