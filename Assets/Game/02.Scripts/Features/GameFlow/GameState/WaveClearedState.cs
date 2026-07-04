using System;
using Game.Core;
using Game.Model;
using Game.Service;

namespace Game.GameState
{
    public class WaveClearedState : BaseGameState
    {
        public override GameStateType StateType => GameStateType.WaveCleared;

        private readonly WaveProgressService _waveProgressService;
        private readonly StageService _stageService;
        private readonly GameResultModel _resultModel;
        private float _elapsedTime;

        public WaveClearedState(
            Action<GameStateType> requestStateChange,
            WaveProgressService waveProgressService,
            StageService stageService,
            GameResultModel resultModel) : base(requestStateChange)
        {
            _waveProgressService = waveProgressService;
            _stageService = stageService;
            _resultModel = resultModel;
        }

        public override void Enter()
        {
            GameLogger.Log($"[WaveClearedState] Wave {_waveProgressService.CurrentWaveIndex.Value + 1} cleared");
            _elapsedTime = 0f;
        }

        public override void Tick(float deltaTime)
        {
            _elapsedTime += deltaTime;

            if (_elapsedTime >= _stageService.CurrentStage.waveIntervalSeconds)
            {
                if (_waveProgressService.HasNextWave)
                {
                    RequestStateChange(GameStateType.WavePlaying);
                }
                else if (!_waveProgressService.IsStageCompleted)
                {
                    _waveProgressService.ScheduleBossWave();
                    RequestStateChange(GameStateType.WavePlaying);
                }
                else
                {
                    _resultModel.SetWinner(UnitSide.Ally);
                    RequestStateChange(GameStateType.Result);
                }
            }
        }
    }
}
