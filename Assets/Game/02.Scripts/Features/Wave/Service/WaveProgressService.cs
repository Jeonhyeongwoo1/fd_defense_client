using System.Collections.Generic;
using Game.Core;
using Game.Data;
using Game.GameState;
using Game.Model;
using Game.Service;
using UniRx;
using UnityEngine;
using VContainer.Unity;

namespace Game.Service
{
    public class WaveProgressService : ITickable
    {
        public IReadOnlyReactiveProperty<int> CurrentWaveIndex => _currentWaveIndex;
        public IReadOnlyReactiveProperty<bool> IsBossWave => _isBossWave;
        public int TotalWaveCount { get; private set; }
        public bool HasNextWave => _currentWaveIndex.Value + 1 < TotalWaveCount;
        public bool HasBossWave => !string.IsNullOrEmpty(_currentStage?.bossId);
        public bool IsStageCompleted
        {
            get
            {
                var allWavesCleared = _currentWaveIndex.Value >= TotalWaveCount - 1 && IsCurrentWaveCleared;
                if (!HasBossWave)
                {
                    return allWavesCleared;
                }
                return allWavesCleared && _isBossWaveCleared;
            }
        }
        public bool IsBossWavePending => _isBossWavePending;
        public bool IsCurrentWaveCleared
        {
            get
            {
                if (_currentWaveData == null && !_isBossWave.Value)
                {
                    return false;
                }

                foreach (var state in _entryStateList)
                {
                    if (state.SpawnedCount < state.Entry.count)
                    {
                        return false;
                    }
                }

                var enemyList = _unitRegistry.GetEntryList(UnitSide.Enemy);
                return enemyList.Count == 0;
            }
        }

        private readonly ReactiveProperty<int> _currentWaveIndex = new(-1);
        private readonly ReactiveProperty<bool> _isBossWave = new(false);
        private readonly WaveTableSO _waveTable;
        private readonly EnemyTableSO _enemyTable;
        private readonly BossTableSO _bossTable;
        private readonly UnitFactory _unitFactory;
        private readonly UnitRegistry _unitRegistry;
        private readonly GameStateModel _gameStateModel;

        private StageData _currentStage;
        private WaveData _currentWaveData;
        private readonly List<EntryState> _entryStateList = new();
        private bool _isBossWaveCleared;
        private bool _isBossWavePending;

        private class EntryState
        {
            public WaveSpawnEntry Entry;
            public int SpawnedCount;
            public float Timer;
        }

        public WaveProgressService(
            WaveTableSO waveTable,
            EnemyTableSO enemyTable,
            BossTableSO bossTable,
            UnitFactory unitFactory,
            UnitRegistry unitRegistry,
            GameStateModel gameStateModel)
        {
            _waveTable = waveTable;
            _enemyTable = enemyTable;
            _bossTable = bossTable;
            _unitFactory = unitFactory;
            _unitRegistry = unitRegistry;
            _gameStateModel = gameStateModel;
        }

        public void SetStage(StageData data)
        {
            _currentStage = data;
            TotalWaveCount = data.WaveIdList.Count;
            _currentWaveIndex.Value = -1;
            _isBossWave.Value = false;
            _isBossWaveCleared = false;
            _isBossWavePending = false;
        }

        public void StartBossWave()
        {
            if (string.IsNullOrEmpty(_currentStage?.bossId))
            {
                GameLogger.LogError("[WaveProgressService] No boss assigned to current stage.");
                return;
            }

            var bossData = _bossTable.GetById(_currentStage.bossId);
            if (bossData == null)
            {
                GameLogger.LogError($"[WaveProgressService] Boss not found: {_currentStage.bossId}");
                return;
            }

            var position = new Vector3(Const.EnemyBaseX - Const.UnitSpawnOffsetX, Const.GroundY, 0);
            _unitFactory.SpawnBoss(bossData, position);
            _isBossWave.Value = true;
            _isBossWavePending = false;
            _entryStateList.Clear();

            GameLogger.Log($"[WaveProgressService] Boss wave started: {_currentStage.bossId}");
        }

        public void StartNextPhase()
        {
            if (_isBossWavePending)
            {
                StartBossWave();
            }
            else
            {
                StartWave(_currentWaveIndex.Value + 1);
            }
        }

        public void ScheduleBossWave()
        {
            _isBossWavePending = true;
        }

        public void MarkBossWaveCleared()
        {
            _isBossWaveCleared = true;
        }

        public void StartWave(int index)
        {
            if (_currentStage == null)
            {
                GameLogger.LogError("[WaveProgressService] Stage not set.");
                return;
            }

            if (index < 0 || index >= _currentStage.WaveIdList.Count)
            {
                GameLogger.LogError($"[WaveProgressService] Invalid wave index: {index}");
                return;
            }

            var waveId = _currentStage.WaveIdList[index];
            _currentWaveData = _waveTable.GetById(waveId);

            if (_currentWaveData == null)
            {
                GameLogger.LogError($"[WaveProgressService] Wave not found: {waveId}");
                return;
            }

            _currentWaveIndex.Value = index;
            _entryStateList.Clear();

            foreach (var entry in _currentWaveData.SpawnEntryList)
            {
                _entryStateList.Add(new EntryState
                {
                    Entry = entry,
                    SpawnedCount = 0,
                    Timer = 0f
                });
            }

            GameLogger.Log($"[WaveProgressService] Wave {index + 1} started: {waveId}");
        }

        public void Tick()
        {
            if (_gameStateModel.CurrentStateType.Value != GameStateType.WavePlaying)
            {
                return;
            }

            if (_currentWaveData == null)
            {
                return;
            }

            var deltaTime = Time.deltaTime;

            foreach (var state in _entryStateList)
            {
                if (state.SpawnedCount >= state.Entry.count)
                {
                    continue;
                }

                state.Timer += deltaTime;

                if (state.Timer >= state.Entry.interval)
                {
                    var enemyData = _enemyTable.GetById(state.Entry.enemyId);

                    if (enemyData == null)
                    {
                        GameLogger.LogError($"[WaveProgressService] Enemy not found: {state.Entry.enemyId}. Skipping spawn.");
                        state.SpawnedCount = state.Entry.count;
                        continue;
                    }

                    var position = new Vector3(Const.EnemyBaseX - Const.UnitSpawnOffsetX, Const.GroundY, 0);
                    _unitFactory.SpawnEnemy(enemyData, position);

                    state.SpawnedCount++;
                    state.Timer = 0f;
                }
            }
        }
    }
}
