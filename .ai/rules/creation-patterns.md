# Creation Patterns

## Creation Rules
- Separate creation from business logic
- Use Factory when creation has dependencies, setup steps, or reuse
- Service may request creation, but non-trivial creation should live in Factory
- Use Registry when created objects must be tracked

## Factory Pattern
- Factory encapsulates complex object construction
- Factory calls PoolManager to get or create instances
- Factory sets up stats, positions, and initial state
- Service calls Factory — Factory does not call Service

### Example flow
```
EnemySpawnService
  → EnemyFactory.CreateEnemy()
    → PoolManager.Get<EnemyController>()
    → EnemyController stat/path setup
    → EnemyRegistry.Register()
```

## Registry Pattern
- Registry tracks live instances by key (usually id or index)
- Register on spawn, unregister on despawn/death
- Used for fast lookup without scene traversal (targeting, cleanup, wave-clear checks)

## Pool Pattern
- PoolManager wraps Unity ObjectPool per resource key
- Get: pull from pool or instantiate if empty
- Release: return to pool (do not destroy)
- Reset state on Get, not on Release
- 스폰 빈도가 높은 오브젝트(enemy, projectile, hit effect)는 반드시 풀링
