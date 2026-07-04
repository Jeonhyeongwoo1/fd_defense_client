using System;
using Game.Core;

namespace Game.GameState
{
    public class ReadyState : BaseGameState
    {
        public override GameStateType StateType => GameStateType.Ready;

        private const float TransitionDelay = 1.0f;
        private float _elapsedTime;

        public ReadyState(Action<GameStateType> requestStateChange) : base(requestStateChange)
        {
        }

        public override void Enter()
        {
            GameLogger.Log("[ReadyState] Entered");
            _elapsedTime = 0f;
        }

        public override void Tick(float deltaTime)
        {
            // Phase 3: Replace with actual wave start trigger
            _elapsedTime += deltaTime;

            if (_elapsedTime >= TransitionDelay)
            {
                RequestStateChange(GameStateType.WavePlaying);
            }
        }
    }
}
