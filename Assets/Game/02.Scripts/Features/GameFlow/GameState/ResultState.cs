using System;
using Game.Core;

namespace Game.GameState
{
    public class ResultState : BaseGameState
    {
        public override GameStateType StateType => GameStateType.Result;

        public ResultState(Action<GameStateType> requestStateChange) : base(requestStateChange)
        {
        }

        public override void Enter()
        {
            GameLogger.Log("[ResultState] Entered - Game completed");
        }
    }
}
