using System;
using Game.Core;
using Game.Model;

namespace Game.GameState
{
    public class ResultState : BaseGameState
    {
        public override GameStateType StateType => GameStateType.Result;

        private readonly GameResultModel _resultModel;
        private readonly Action _onEnterCleanup;

        public ResultState(
            Action<GameStateType> requestStateChange,
            GameResultModel resultModel,
            Action onEnterCleanup) : base(requestStateChange)
        {
            _resultModel = resultModel;
            _onEnterCleanup = onEnterCleanup;
        }

        public override void Enter()
        {
            _onEnterCleanup.Invoke();

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
