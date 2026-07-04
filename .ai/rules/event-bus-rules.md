# EventBus Rules

## When to Use EventBus
Use EventBus only for cross-system events where direct callback wiring is impractical:
- `TowerPlacedEvent` — input layer to presenter layer
- `EnemyKilledEvent` — combat layer to wave/reward logic layer
- `WaveStartedEvent` — UI button to game flow

## When NOT to Use EventBus
- Communication within the same layer
- Parent-to-child or child-to-parent within one feature
- Any case where an `Action` or interface callback is sufficient
- 매 프레임/고빈도 이벤트 (enemy 이동, 투사체 hit 판정 등) — 직접 참조나 콜백으로 처리

## Declaration Rule
- All event structs must be declared inside `GameEvent.cs`
- Never create a separate file per event struct

## Structure Rule
```csharp
// GameEvent.cs
public static class GameEvent
{
    public struct TowerPlacedEvent { public int TowerIndex; }
    public struct EnemyKilledEvent { public int EnemyId; public int Reward; }
    public struct WaveStartedEvent { public int WaveIndex; }
}
```

## Injection Rule
- Do not inject `EventBus` into Service or low-level classes
- Only Presenter and Controller may subscribe to EventBus
- Prefer callback flow (Action/Func) over EventBus for local wiring

## Handler Signature
- Subscribe handlers receive events by `in` reference: `void OnEnemyKilled(in EnemyKilledEvent e)`
- Event structs are immutable, so `in` is always safe and avoids struct copy on each dispatch.

```csharp
// Good
private void OnEnemyKilled(in GameEvent.EnemyKilledEvent e) { ... }

// Bad — value copy on every dispatch
private void OnEnemyKilled(GameEvent.EnemyKilledEvent e) { ... }
```
