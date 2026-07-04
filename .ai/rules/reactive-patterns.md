# Reactive Patterns (UniRx / UniTask)

## UniRx Rules

### ReactiveProperty vs Subject
| Use | When |
|-----|------|
| `ReactiveProperty<T>` | State that has a current value and emits on change |
| `Subject<T>` | One-time events with no persisted value |
| `IReadOnlyReactiveProperty<T>` | Expose state as read-only to outside classes |

### CompositeDisposable
- Declare `_disposables = new CompositeDisposable()` in each class that subscribes
- Add subscriptions with `.AddTo(_disposables)`
- Dispose in `Dispose()` or on scope cleanup
- Presenter owns subscription lifetime, not View or Model

### AddTo Rule
- Use `AddTo(this)` only in MonoBehaviour classes
- Use `AddTo(_disposables)` in non-MonoBehaviour classes

### Observable Patterns
```csharp
// Model exposes read-only reactive state
public IReadOnlyReactiveProperty<bool> IsWaveCleared => _isWaveCleared;

// Presenter subscribes and reacts
_model.IsWaveCleared
    .Where(cleared => cleared)
    .Subscribe(_ => OnWaveCleared())
    .AddTo(_disposables);
```

## UniTask Rules

### UniTask vs Coroutine
| Use | When |
|-----|------|
| `UniTask` | Async logic in non-MonoBehaviour (Service, Presenter) |
| `Coroutine` | Only inside MonoBehaviour |
| `async UniTaskVoid` | Fire-and-forget async methods |

### Cancellation
- Pass `CancellationToken` for long-running operations
- Use `this.GetCancellationTokenOnDestroy()` in MonoBehaviour
- Cancel on state exit in GameState classes (wave 진행 중 결과 화면 전환 등)

### Async Patterns
```csharp
// Service uses UniTask
public async UniTask LoadResourcesAsync(CancellationToken token)
{
    await Addressables.LoadAssetAsync<GameObject>(key).ToUniTask(cancellationToken: token);
}

// MonoBehaviour uses Coroutine
private IEnumerator PlayHitEffect()
{
    yield return new WaitForSeconds(0.3f);
}
```

## Naming
- Async methods end with `Async` suffix
- Observable properties use noun or adjective names, not verbs
