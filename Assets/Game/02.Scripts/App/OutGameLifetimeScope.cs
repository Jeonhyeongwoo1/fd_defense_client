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

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(stageTable);

            builder.Register<StageProgressService>(Lifetime.Scoped);
            builder.Register<SceneLoadService>(Lifetime.Scoped);

            builder.RegisterComponentInHierarchy<UI_StageSelectView>();

            builder.RegisterEntryPoint<StageSelectPresenter>(Lifetime.Scoped);
        }
    }
}
