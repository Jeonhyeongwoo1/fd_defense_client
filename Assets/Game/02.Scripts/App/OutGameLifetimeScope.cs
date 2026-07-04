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

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(stageTable);
            builder.RegisterInstance(unitTable);
            builder.RegisterInstance(upgradeTable);

            builder.Register<StageProgressService>(Lifetime.Scoped);
            builder.Register<SceneLoadService>(Lifetime.Scoped);
            builder.Register<GoldService>(Lifetime.Scoped);
            builder.Register<UpgradeService>(Lifetime.Scoped);
            builder.Register<DeckService>(Lifetime.Scoped);

            builder.RegisterComponentInHierarchy<UI_StageSelectView>();
            builder.RegisterComponentInHierarchy<UI_OutGameHomeView>();
            builder.RegisterComponentInHierarchy<UI_DeckPanelView>();
            builder.RegisterComponentInHierarchy<UI_UpgradePanelView>();

            builder.RegisterEntryPoint<StageSelectPresenter>(Lifetime.Scoped);
            builder.RegisterEntryPoint<DeckPresenter>(Lifetime.Scoped);
            builder.RegisterEntryPoint<UpgradePresenter>(Lifetime.Scoped);
            builder.RegisterEntryPoint<OutGameHomePresenter>(Lifetime.Scoped);
        }
    }
}
