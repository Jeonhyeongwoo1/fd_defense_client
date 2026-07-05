using Game.Core;
using Game.Data;
using Game.Service;
using VContainer.Unity;

namespace Game.Service
{
    public class LobbyBackgroundService : IStartable
    {
        private readonly MapBuilderService _mapBuilderService;
        private readonly StageProgressService _stageProgressService;
        private readonly StageTableSO _stageTable;

        public LobbyBackgroundService(
            MapBuilderService mapBuilderService,
            StageProgressService stageProgressService,
            StageTableSO stageTable)
        {
            _mapBuilderService = mapBuilderService;
            _stageProgressService = stageProgressService;
            _stageTable = stageTable;
        }

        public void Start()
        {
            var selectedStageId = _stageProgressService.GetSelectedStageId();
            var stageData = _stageTable.GetById(selectedStageId);

            if (stageData == null)
            {
                GameLogger.LogWarning($"[LobbyBackgroundService] StageData not found for {selectedStageId}. Skipping background build.");
                return;
            }

            if (string.IsNullOrEmpty(stageData.mapId))
            {
                GameLogger.LogWarning($"[LobbyBackgroundService] Stage {selectedStageId} has no mapId. Skipping background build.");
                return;
            }

            _mapBuilderService.BuildMap(stageData.mapId);
            GameLogger.Log($"[LobbyBackgroundService] Built lobby background map: {stageData.mapId}");
        }
    }
}
