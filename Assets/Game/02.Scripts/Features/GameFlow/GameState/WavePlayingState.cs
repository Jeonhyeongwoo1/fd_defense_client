using System;
using Game.Core;
using Game.Model;
using Game.Service;

namespace Game.GameState
{
    public class WavePlayingState : BaseGameState
    {
        public override GameStateType StateType => GameStateType.WavePlaying;

        private readonly BaseService _baseService;
        private readonly GameResultModel _resultModel;
        private readonly WaveProgressService _waveProgressService;

        public WavePlayingState(
            Action<GameStateType> requestStateChange,
            BaseService baseService,
            GameResultModel resultModel,
            WaveProgressService waveProgressService) : base(requestStateChange)
        {
            _baseService = baseService;
            _resultModel = resultModel;
            _waveProgressService = waveProgressService;
        }

        public override void Enter()
        {
            _waveProgressService.StartNextPhase();
            GameLogger.Log("[WavePlayingState] Entered");
        }

        public override void Tick(float deltaTime)
        {
            if (_baseService.IsBaseDestroyed(UnitSide.Enemy))
            {
                _resultModel.SetWinner(UnitSide.Ally);
                RequestStateChange(GameStateType.Result);
                return;
            }

            if (_baseService.IsBaseDestroyed(UnitSide.Ally))
            {
                _resultModel.SetWinner(UnitSide.Enemy);
                RequestStateChange(GameStateType.Result);
                return;
            }

            if (_waveProgressService.IsCurrentWaveCleared)
            {
                if (_waveProgressService.IsBossWave.Value)
                {
                    _waveProgressService.MarkBossWaveCleared();
                }
                RequestStateChange(GameStateType.WaveCleared);
            }
        }
    }
}
