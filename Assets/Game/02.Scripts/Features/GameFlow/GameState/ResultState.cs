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
        private readonly Action _onVictory;

        public ResultState(
            Action<GameStateType> requestStateChange,
            GameResultModel resultModel,
            Action onEnterCleanup,
            Action onVictory) : base(requestStateChange)
        {
            _resultModel = resultModel;
            _onEnterCleanup = onEnterCleanup;
            _onVictory = onVictory;
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

            if (_resultModel.Winner == UnitSide.Ally)
            {
                _onVictory.Invoke();
            }
        }
    }
}
