using Game.Core;
using Game.Data;
using Game.Model;
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
        private readonly StageProgressService _stageProgressService;
        private readonly MapBuilderService _mapBuilderService;
        private readonly GoldService _goldService;
        private readonly GameResultModel _resultModel;
        private readonly UnitFactory _unitFactory;
        private readonly EventBus _eventBus;

        public StageService(
            StageTableSO stageTable,
            BaseService baseService,
            WalletService walletService,
            WaveProgressService waveProgressService,
            StageProgressService stageProgressService,
            MapBuilderService mapBuilderService,
            GoldService goldService,
            GameResultModel resultModel,
            UnitFactory unitFactory,
            EventBus eventBus)
        {
            _stageTable = stageTable;
            _baseService = baseService;
            _walletService = walletService;
            _waveProgressService = waveProgressService;
            _stageProgressService = stageProgressService;
            _mapBuilderService = mapBuilderService;
            _goldService = goldService;
            _resultModel = resultModel;
            _unitFactory = unitFactory;
            _eventBus = eventBus;
        }

        public void Start()
        {
            var selectedStageId = _stageProgressService.GetSelectedStageId();
            CurrentStage = _stageTable.GetById(selectedStageId);

            if (CurrentStage == null)
            {
                GameLogger.LogError($"[StageService] Stage '{selectedStageId}' not found, fallback to default");
                CurrentStage = _stageTable.GetById(Const.DefaultStageId);
                if (CurrentStage == null)
                {
                    GameLogger.LogError($"[StageService] Default stage '{Const.DefaultStageId}' not found");
                    return;
                }
            }

            _unitFactory.SetEnemyStatMultiplier(CurrentStage.enemyStatMultiplier);
            _mapBuilderService.BuildMap(CurrentStage.mapId);
            _baseService.Initialize(CurrentStage);
            _walletService.Initialize(CurrentStage.startMoney, CurrentStage.moneyPerSecond);
            _waveProgressService.SetStage(CurrentStage);

            GameLogger.Log($"[StageService] Stage loaded: {CurrentStage.id}, enemy stat multiplier: {CurrentStage.enemyStatMultiplier}");
        }

        public void MarkCurrentStageCleared()
        {
            if (CurrentStage == null)
            {
                GameLogger.LogWarning("[StageService] MarkCurrentStageCleared called but CurrentStage is null");
                return;
            }

            _stageProgressService.MarkStageCleared(CurrentStage.id);
            _goldService.Add(CurrentStage.goldReward);
            _resultModel.SetEarnedGold(CurrentStage.goldReward);

            _eventBus.Publish(new StageClearedEvent { StageId = CurrentStage.id });

            GameLogger.Log($"[StageService] Stage cleared: {CurrentStage.id}, gold rewarded: {CurrentStage.goldReward}");
        }
    }
}
