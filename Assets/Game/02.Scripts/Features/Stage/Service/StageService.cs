using Game.Core;
using Game.Data;
using VContainer.Unity;

namespace Game.Service
{
    public class StageService : IStartable
    {
        public StageData CurrentStage { get; private set; }

        private readonly StageTableSO _stageTable;
        private readonly BaseService _baseService;
        private readonly WalletService _walletService;
        private readonly WaveProgressService _waveProgressService;

        public StageService(
            StageTableSO stageTable,
            BaseService baseService,
            WalletService walletService,
            WaveProgressService waveProgressService)
        {
            _stageTable = stageTable;
            _baseService = baseService;
            _walletService = walletService;
            _waveProgressService = waveProgressService;
        }

        public void Start()
        {
            CurrentStage = _stageTable.GetById(Const.DefaultStageId);

            if (CurrentStage == null)
            {
                GameLogger.LogError($"[StageService] Stage '{Const.DefaultStageId}' not found");
                return;
            }

            _baseService.Initialize(CurrentStage);
            _walletService.Initialize(CurrentStage.startMoney, CurrentStage.moneyPerSecond);
            _waveProgressService.SetStage(CurrentStage);

            GameLogger.Log($"[StageService] Stage loaded: {CurrentStage.id}");
        }
    }
}
