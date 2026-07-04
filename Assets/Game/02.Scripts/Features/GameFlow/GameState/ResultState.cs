using System;
using Game.Core;
using Game.Model;

namespace Game.GameState
{
    public class ResultState : BaseGameState
    {
        public override GameStateType StateType => GameStateType.Result;

        private readonly GameResultModel _resultModel;

        public ResultState(
            Action<GameStateType> requestStateChange,
            GameResultModel resultModel) : base(requestStateChange)
        {
            _resultModel = resultModel;
        }

        public override void Enter()
        {
            if (_resultModel.Winner == null)
            {
                GameLogger.LogWarning("[ResultState] Winner is null");
                return;
            }

            var message = _resultModel.Winner == UnitSide.Ally ? "VICTORY" : "DEFEAT";
            GameLogger.Log($"[ResultState] {message}");
        }
    }
}
