using System;

namespace Game.GameState
{
    public abstract class BaseGameState
    {
        public abstract GameStateType StateType { get; }

        protected Action<GameStateType> RequestStateChange { get; }

        protected BaseGameState(Action<GameStateType> requestStateChange)
        {
            RequestStateChange = requestStateChange;
        }

        public virtual void Enter()
        {
        }

        public virtual void Tick(float deltaTime)
        {
        }

        public virtual void Exit()
        {
        }
    }
}
