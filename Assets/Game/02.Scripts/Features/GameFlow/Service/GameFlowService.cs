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
        private readonly Dictionary<GameStateType, BaseGameState> _stateDict = new();
        private readonly BaseService _baseService;
        private readonly GameResultModel _resultModel;
        private readonly GameStateModel _gameStateModel;
        private readonly WaveProgressService _waveProgressService;
        private readonly StageService _stageService;
        private readonly UnitBattleService _unitBattleService;
        private BaseGameState _currentState;

        public GameFlowService(
            BaseService baseService,
            GameResultModel resultModel,
            GameStateModel gameStateModel,
            WaveProgressService waveProgressService,
            StageService stageService,
            UnitBattleService unitBattleService)
        {
            _baseService = baseService;
            _resultModel = resultModel;
            _gameStateModel = gameStateModel;
            _waveProgressService = waveProgressService;
            _stageService = stageService;
            _unitBattleService = unitBattleService;
        }

        public void Start()
        {
            _stateDict[GameStateType.Ready] = new ReadyState(ChangeState);
            _stateDict[GameStateType.WavePlaying] = new WavePlayingState(ChangeState, _baseService, _resultModel, _waveProgressService);
            _stateDict[GameStateType.WaveCleared] = new WaveClearedState(ChangeState, _waveProgressService, _stageService, _resultModel, _baseService);
            _stateDict[GameStateType.Result] = new ResultState(ChangeState, _resultModel, _unitBattleService.ReleaseAllUnits, OnVictory);

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
            _gameStateModel.SetState(next);

            _currentState.Enter();
        }

        private void OnVictory()
        {
            _stageService.MarkCurrentStageCleared();
        }
    }
}
