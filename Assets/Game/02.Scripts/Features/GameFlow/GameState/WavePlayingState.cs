using System;
using Game.Core;

namespace Game.GameState
{
    public class WavePlayingState : BaseGameState
    {
        public override GameStateType StateType => GameStateType.WavePlaying;

        private const float TransitionDelay = 2.0f;
        private float _elapsedTime;

        public WavePlayingState(Action<GameStateType> requestStateChange) : base(requestStateChange)
        {
        }

        public override void Enter()
        {
            GameLogger.Log("[WavePlayingState] Entered");
            _elapsedTime = 0f;
        }

        public override void Tick(float deltaTime)
        {
            // Phase 3: Replace with actual wave completion condition
            _elapsedTime += deltaTime;

            if (_elapsedTime >= TransitionDelay)
            {
                RequestStateChange(GameStateType.WaveCleared);
            }
        }
    }
}
