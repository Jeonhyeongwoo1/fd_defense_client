using Game.Controller;
using Game.Core;
using Game.Data;
using Game.Presenter;
using Game.Service;
using Game.View;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.App
{
    public class OutGameLifetimeScope : LifetimeScope
    {
        [SerializeField] private StageTableSO stageTable;
        [SerializeField] private UnitTableSO unitTable;
        [SerializeField] private UpgradeTableSO upgradeTable;
        [SerializeField] private DailyRewardTableSO dailyRewardTable;
        [SerializeField] private MissionTableSO missionTable;
        [SerializeField] private ShopTableSO shopTable;
        [SerializeField] private MapTableSO mapTable;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(stageTable);
            builder.RegisterInstance(unitTable);
            builder.RegisterInstance(upgradeTable);
            builder.RegisterInstance(dailyRewardTable);
            builder.RegisterInstance(missionTable);
            builder.RegisterInstance(shopTable);
            builder.RegisterInstance(mapTable);

            builder.Register<EventBus>(Lifetime.Scoped);
            builder.Register<UserDataService>(Lifetime.Scoped);
            builder.Register<StageProgressService>(Lifetime.Scoped);
            builder.Register<SceneLoadService>(Lifetime.Scoped);
            builder.Register<GoldService>(Lifetime.Scoped);
            builder.Register<UpgradeService>(Lifetime.Scoped);
            builder.Register<DeckService>(Lifetime.Scoped);
            builder.Register<DailyRewardService>(Lifetime.Scoped);
            builder.Register<MissionService>(Lifetime.Scoped);
            builder.Register<SettingsService>(Lifetime.Scoped);
            builder.Register<ShopService>(Lifetime.Scoped);
            builder.Register<MapBuilderService>(Lifetime.Scoped);
            builder.Register<LobbyBackgroundService>(Lifetime.Scoped);
            builder.Register<LobbyShowcaseService>(Lifetime.Scoped);

            builder.RegisterComponentInHierarchy<UI_StageSelectView>();
            builder.RegisterComponentInHierarchy<UI_OutGameHomeView>();
            builder.RegisterComponentInHierarchy<UI_DeckPanelView>();
            builder.RegisterComponentInHierarchy<UI_UpgradePanelView>();
            builder.RegisterComponentInHierarchy<UI_MissionPanelView>();
            builder.RegisterComponentInHierarchy<UI_DailyRewardPopupView>();
            builder.RegisterComponentInHierarchy<UI_SettingsPopupView>();
            builder.RegisterComponentInHierarchy<UI_ShopPanelView>();

            builder.RegisterEntryPoint<LobbyBackgroundService>(Lifetime.Scoped);
            builder.RegisterEntryPoint<LobbyShowcaseService>(Lifetime.Scoped);
            builder.RegisterEntryPoint<StageSelectPresenter>(Lifetime.Scoped);
            builder.RegisterEntryPoint<DeckPresenter>(Lifetime.Scoped);
            builder.RegisterEntryPoint<UpgradePresenter>(Lifetime.Scoped);
            builder.RegisterEntryPoint<MissionPanelPresenter>(Lifetime.Scoped);
            builder.RegisterEntryPoint<DailyRewardPresenter>(Lifetime.Scoped);
            builder.RegisterEntryPoint<SettingsPresenter>(Lifetime.Scoped);
            builder.RegisterEntryPoint<ShopPanelPresenter>(Lifetime.Scoped);
            builder.RegisterEntryPoint<OutGameHomePresenter>(Lifetime.Scoped);
            builder.RegisterEntryPoint<MissionProgressController>(Lifetime.Scoped);
        }
    }
}
