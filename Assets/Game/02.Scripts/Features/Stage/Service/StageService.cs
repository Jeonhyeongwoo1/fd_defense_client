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
        private readonly EnemySpawnService _enemySpawnService;

        public StageService(
            StageTableSO stageTable,
            BaseService baseService,
            WalletService walletService,
            EnemySpawnService enemySpawnService)
        {
            _stageTable = stageTable;
            _baseService = baseService;
            _walletService = walletService;
            _enemySpawnService = enemySpawnService;
        }

        public void Start()
        {
            CurrentStage = _stageTable.GetById("stage_001");

            if (CurrentStage == null)
            {
                GameLogger.LogError("[StageService] Stage 'stage_001' not found");
                return;
            }

            _baseService.Initialize(CurrentStage);
            _walletService.Initialize(CurrentStage.startMoney, CurrentStage.moneyPerSecond);
            _enemySpawnService.SetStage(CurrentStage);

            GameLogger.Log($"[StageService] Stage loaded: {CurrentStage.id}");
        }
    }
}
