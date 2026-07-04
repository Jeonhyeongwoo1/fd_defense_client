using System.Collections.Generic;
using Game.Core;
using Game.Data;
using Game.GameState;
using UnityEngine;
using VContainer.Unity;

namespace Game.Service
{
    public class EnemySpawnService : ITickable
    {
        private readonly EnemyTableSO _enemyTable;
        private readonly UnitFactory _unitFactory;
        private readonly GameFlowService _gameFlowService;

        private StageData _currentStage;
        private List<float> _spawnTimerList;
        private HashSet<int> _disabledEntrySet = new();

        public EnemySpawnService(
            EnemyTableSO enemyTable,
            UnitFactory unitFactory,
            GameFlowService gameFlowService)
        {
            _enemyTable = enemyTable;
            _unitFactory = unitFactory;
            _gameFlowService = gameFlowService;
        }

        public void SetStage(StageData data)
        {
            _currentStage = data;
            _spawnTimerList = new List<float>();
            _disabledEntrySet.Clear();

            for (var i = 0; i < data.SpawnEntryList.Count; i++)
            {
                _spawnTimerList.Add(0f);
            }
        }

        public void Tick()
        {
            if (_gameFlowService.CurrentStateType.Value != GameStateType.WavePlaying)
            {
                return;
            }

            if (_currentStage == null)
            {
                return;
            }

            var deltaTime = Time.deltaTime;

            for (var i = 0; i < _currentStage.SpawnEntryList.Count; i++)
            {
                if (_disabledEntrySet.Contains(i))
                {
                    continue;
                }

                var entry = _currentStage.SpawnEntryList[i];
                _spawnTimerList[i] += deltaTime;

                if (_spawnTimerList[i] >= entry.interval)
                {
                    var enemyData = _enemyTable.GetById(entry.enemyId);

                    if (enemyData == null)
                    {
                        GameLogger.LogError($"[EnemySpawnService] Enemy not found: {entry.enemyId}. Disabling spawn entry.");
                        _disabledEntrySet.Add(i);
                        continue;
                    }

                    var position = new Vector3(Const.EnemyBaseX - 1f, Const.GroundY, 0);
                    _unitFactory.SpawnEnemy(enemyData, position);

                    _spawnTimerList[i] = 0f;
                }
            }
        }
    }
}
