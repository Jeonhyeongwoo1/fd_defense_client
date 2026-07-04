using System;
using Game.Core;

namespace Game.GameState
{
    public class WaveClearedState : BaseGameState
    {
        public override GameStateType StateType => GameStateType.WaveCleared;

        private const float TransitionDelay = 1.0f;
        private const int MaxWaves = 3;
        private float _elapsedTime;
        private int _clearedWaveCount;

        public WaveClearedState(Action<GameStateType> requestStateChange) : base(requestStateChange)
        {
        }

        public override void Enter()
        {
            _clearedWaveCount++;
            GameLogger.Log($"[WaveClearedState] Entered - Wave {_clearedWaveCount} cleared");
            _elapsedTime = 0f;
        }

        public override void Tick(float deltaTime)
        {
            // Phase 3: Replace with actual next-wave or result condition
            _elapsedTime += deltaTime;

            if (_elapsedTime >= TransitionDelay)
            {
                if (_clearedWaveCount < MaxWaves)
                {
                    RequestStateChange(GameStateType.Ready);
                }
                else
                {
                    RequestStateChange(GameStateType.Result);
                }
            }
        }
    }
}
