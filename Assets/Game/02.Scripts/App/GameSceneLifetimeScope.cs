using Game.Core;
using Game.Service;
using VContainer;
using VContainer.Unity;

namespace Game.App
{
    public class GameSceneLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<EventBus>(Lifetime.Scoped);
            builder.Register<PoolManager>(Lifetime.Scoped);
            builder.Register<ResourceService>(Lifetime.Scoped);

            builder.RegisterEntryPoint<GameFlowService>(Lifetime.Scoped).AsSelf();
        }
    }
}
