using Game.Core;
using Game.Data;
using Game.Model;
using Game.Presenter;
using Game.Service;
using Game.View;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.App
{
    public class GameSceneLifetimeScope : LifetimeScope
    {
        [SerializeField] private UnitTableSO unitTable;
        [SerializeField] private EnemyTableSO enemyTable;
        [SerializeField] private StageTableSO stageTable;
        [SerializeField] private WaveTableSO waveTable;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<EventBus>(Lifetime.Scoped);
            builder.Register<PoolManager>(Lifetime.Scoped);
            builder.Register<ResourceService>(Lifetime.Scoped);

            builder.RegisterInstance(unitTable);
            builder.RegisterInstance(enemyTable);
            builder.RegisterInstance(stageTable);
            builder.RegisterInstance(waveTable);

            builder.Register<UnitRegistry>(Lifetime.Scoped);
            builder.Register<UnitFactory>(Lifetime.Scoped);
            builder.Register<UnitSpawnService>(Lifetime.Scoped);
            builder.Register<GameResultModel>(Lifetime.Scoped);
            builder.Register<GameStateModel>(Lifetime.Scoped);
            builder.Register<WalletModel>(Lifetime.Scoped);
            builder.Register<BaseService>(Lifetime.Scoped);

            builder.RegisterComponentInHierarchy<UI_GameHudView>();

            builder.RegisterEntryPoint<StageService>(Lifetime.Scoped).AsSelf();
            builder.RegisterEntryPoint<WalletService>(Lifetime.Scoped).AsSelf();
            builder.RegisterEntryPoint<WaveProgressService>(Lifetime.Scoped).AsSelf();
            builder.RegisterEntryPoint<ProjectileService>(Lifetime.Scoped).AsSelf();
            builder.RegisterEntryPoint<UnitBattleService>(Lifetime.Scoped).AsSelf();
            builder.RegisterEntryPoint<GameFlowService>(Lifetime.Scoped);
            builder.RegisterEntryPoint<GameHudPresenter>(Lifetime.Scoped);
        }
    }
}
