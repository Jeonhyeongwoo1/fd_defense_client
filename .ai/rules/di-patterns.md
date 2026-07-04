# Dependency Injection Patterns (VContainer)

## Registration Rules
- Register all non-MonoBehaviour classes via constructor injection
- Register MonoBehaviour instances via `RegisterComponentInHierarchy` or `RegisterComponentOnNewGameObject`
- Entry points (classes that need lifecycle hooks) use `RegisterEntryPoint<T>()`

## Lifetime Rules
- Use `Lifetime.Singleton` for services, models, presenters, and registries
- Use `Lifetime.Scoped` for scene-local instances
- Do not use transient unless objects are truly stateless and short-lived

## VContainer Lifecycle Interfaces
| Interface | When to use |
|-----------|-------------|
| `IInitializable` | One-time setup after injection |
| `IStartable` | Start logic equivalent to Unity Start() |
| `ITickable` | Per-frame update logic |
| `IDisposable` | Cleanup on scope disposal |

Use `IInitializable` for most setup. Use `IStartable` only when Unity scene objects must exist first.

## Constructor Injection Rule
- Always prefer constructor injection over field/property injection
- Do not use `[Inject]` on fields unless injecting into MonoBehaviour
- MonoBehaviour uses `[Inject]` method injection when needed

## GameSceneLifetimeScope Pattern
```csharp
// Register models
builder.Register<WaveModel>(Lifetime.Singleton);

// Register services
builder.Register<TowerRuleService>(Lifetime.Singleton);

// Register presenters as entry points
builder.RegisterEntryPoint<TowerPresenter>();

// Register MonoBehaviour
builder.RegisterComponentInHierarchy<TowerController>();
```

## Dependency Direction
- LifetimeScope only knows about registration
- High-level classes (Presenter, Service) depend on low-level abstractions
- Low-level classes must not receive high-level classes as dependencies
