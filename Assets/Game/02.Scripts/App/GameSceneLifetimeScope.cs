using Game.Controller;
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
        [SerializeField] private BossTableSO bossTable;
        [SerializeField] private StageTableSO stageTable;
        [SerializeField] private WaveTableSO waveTable;
        [SerializeField] private EffectConfigSO effectConfig;
        [SerializeField] private MapTableSO mapTable;
        [SerializeField] private UpgradeTableSO upgradeTable;
        [SerializeField] private MissionTableSO missionTable;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<EventBus>(Lifetime.Scoped);
            builder.Register<PoolManager>(Lifetime.Scoped);
            builder.Register<ResourceService>(Lifetime.Scoped);
            builder.Register<StageProgressService>(Lifetime.Scoped);
            builder.Register<SceneLoadService>(Lifetime.Scoped);

            builder.RegisterInstance(unitTable);
            builder.RegisterInstance(enemyTable);
            builder.RegisterInstance(bossTable);
            builder.RegisterInstance(stageTable);
            builder.RegisterInstance(waveTable);
            builder.RegisterInstance(effectConfig);
            builder.RegisterInstance(mapTable);
            builder.RegisterInstance(upgradeTable);
            builder.RegisterInstance(missionTable);

            builder.Register<UnitRegistry>(Lifetime.Scoped);
            builder.Register<UnitFactory>(Lifetime.Scoped);
            builder.Register<UnitSpawnService>(Lifetime.Scoped);
            builder.Register<GameResultModel>(Lifetime.Scoped);
            builder.Register<GameStateModel>(Lifetime.Scoped);
            builder.Register<WalletModel>(Lifetime.Scoped);
            builder.Register<BaseService>(Lifetime.Scoped);
            builder.Register<EffectService>(Lifetime.Scoped);
            builder.Register<MapBuilderService>(Lifetime.Scoped);
            builder.Register<PauseService>(Lifetime.Scoped);
            builder.Register<GoldService>(Lifetime.Scoped);
            builder.Register<UpgradeService>(Lifetime.Scoped);
            builder.Register<DeckService>(Lifetime.Scoped);
            builder.Register<MissionService>(Lifetime.Scoped);

            builder.RegisterComponentInHierarchy<UI_GameHudView>();
            builder.RegisterComponentInHierarchy<UI_ResultPopupView>();
            builder.RegisterComponentInHierarchy<UI_PausePopupView>();

            builder.RegisterEntryPoint<StageService>(Lifetime.Scoped).AsSelf();
            builder.RegisterEntryPoint<WalletService>(Lifetime.Scoped).AsSelf();
            builder.RegisterEntryPoint<WaveProgressService>(Lifetime.Scoped).AsSelf();
            builder.RegisterEntryPoint<ProjectileService>(Lifetime.Scoped).AsSelf();
            builder.RegisterEntryPoint<BossSkillService>(Lifetime.Scoped).AsSelf();
            builder.RegisterEntryPoint<UnitBattleService>(Lifetime.Scoped).AsSelf();
            builder.RegisterEntryPoint<GameFlowService>(Lifetime.Scoped);
            builder.RegisterEntryPoint<GameHudPresenter>(Lifetime.Scoped);
            builder.RegisterEntryPoint<ResultPresenter>(Lifetime.Scoped);
            builder.RegisterEntryPoint<PausePresenter>(Lifetime.Scoped);
            builder.RegisterEntryPoint<MissionProgressController>(Lifetime.Scoped);
        }
    }
}
