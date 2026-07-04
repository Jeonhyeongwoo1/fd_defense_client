using System.Collections.Generic;
using Game.Core;
using Game.GameState;
using Game.Model;
using UniRx;
using UnityEngine;
using VContainer.Unity;

namespace Game.Service
{
    public class GameFlowService : IStartable, ITickable
    {
        public IReadOnlyReactiveProperty<GameStateType> CurrentStateType => _currentStateType;

        private readonly ReactiveProperty<GameStateType> _currentStateType = new();
        private readonly Dictionary<GameStateType, BaseGameState> _stateDict = new();
        private readonly BaseService _baseService;
        private readonly GameResultModel _resultModel;
        private BaseGameState _currentState;

        public GameFlowService(BaseService baseService, GameResultModel resultModel)
        {
            _baseService = baseService;
            _resultModel = resultModel;
        }

        public void Start()
        {
            _stateDict[GameStateType.Ready] = new ReadyState(ChangeState);
            _stateDict[GameStateType.WavePlaying] = new WavePlayingState(ChangeState, _baseService, _resultModel);
            _stateDict[GameStateType.WaveCleared] = new WaveClearedState(ChangeState);
            _stateDict[GameStateType.Result] = new ResultState(ChangeState, _resultModel);

            ChangeState(GameStateType.Ready);
        }

        public void Tick()
        {
            _currentState.Tick(Time.deltaTime);
        }

        private void ChangeState(GameStateType next)
        {
            if (_currentState != null && _currentState.StateType == next)
            {
                GameLogger.LogWarning($"[GameFlowService] Requested same state: {next}");
                return;
            }

            var prev = _currentState?.StateType.ToString() ?? "None";

            _currentState?.Exit();

            GameLogger.Log($"[GameFlowService] {prev} -> {next}");

            _currentState = _stateDict[next];
            _currentStateType.Value = next;

            _currentState.Enter();
        }
    }
}
