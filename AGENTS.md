# AGENTS.md

## Overview
Unity defense game. Goal: ship a complete playable game.

## Stack
- Unity C# (URP 2D, Input System)
- VContainer (DI)
- UniRx (reactive state)
- UniTask (async; Coroutine only inside MonoBehaviour)

## Architecture (project-specific)

### Game Flow State Machine
`GameFlowService` (entry point: `IStartable`, `ITickable`) owns the state machine. Each state is a `BaseGameState` subclass. States receive a `RequestStateChange` callback from `GameFlowService` — they do not call `GameFlowService` directly.

Expected states: Ready → WavePlaying → WaveCleared → (next wave | Result).

### DI Root
`GameSceneLifetimeScope` is the single VContainer registration file for the game scene. All services, models, registries, factories, and entry points are registered here with `Lifetime.Scoped`.

### Key Data Flow
1. `ResourceService` loads `WaveDataSO` / `TowerDataSO`
2. `EnemySpawnService` → `EnemyFactory` → `PoolManager` instantiates enemies
3. `EnemyRegistry` tracks live enemy instances by id
4. `TowerPresenter` (entry point) bridges `EventBus` (input) ↔ `TowerRuleService` + `TowerModel` (state)
5. `WaveProgressService` tracks current wave; `GameFlowService` drives state transitions

### Core Domains
- **Tower** — 배치·타겟팅·공격·업그레이드
- **Enemy** — 스폰·이동(경로)·HP·사망 (풀링 필수)
- **Wave** — 웨이브 스케줄·스폰 테이블·클리어 판정
- **Projectile / Combat** — 투사체·데미지 계산·hit 판정 (풀링 필수)

## Roles
- View — rendering and input only
- Model — state only
- Service — domain logic and state changes
- Presenter / Controller — coordination and flow
- Factory — creation and initialization
- Registry — runtime instance tracking
- Event — limited cross-system messaging only
- Interface — interface definitions only
- Data — SheetData or GameData only
- Manager — minimal use; prefer concrete names (e.g. `UISpawner` over `UIManager`)

## Folder Rules
Base folders: `Assets/Game/01.Scene`, `Assets/Game/02.Scripts`, `Assets/Game/03.Resources`

C# paths under `Assets/Game/02.Scripts/`:
`View · Model · Service · Presenter · Controller · Factory · Registry · Event · Interface · Data · Utils · GameState · Manager · Editor · Const`

Create missing folders if needed. Always specify target file path.

## Git Workflow (brief)
- Branch: `main` ← `epic/{name}` ← `claude/{name}` or `codex/{name}` (working branch)
- PR target: working branch → matching `epic/{name}`; merge immediately after creation
- `epic → main` merge requires explicit user instruction
- Stage only specific files — never `git add .`

## Output Rules
On every implementation/modification, provide:
1. File path + name
2. Full copy-pasteable code
3. Brief explanation: what was modified / added / important notes

Keep it short and concrete.

## Prototype Rules
- Prioritize playable flow
- Avoid over-engineering
- Use the simplest valid structure in this stack

## Modification Rules
- Change only what is necessary
- Explain what changed and why
- Do not rewrite unrelated parts

---

## Shared Rules

@.ai/rules/architecture.md
@.ai/rules/code-style.md
@.ai/rules/di-patterns.md
@.ai/rules/reactive-patterns.md
@.ai/rules/creation-patterns.md
@.ai/rules/event-bus-rules.md
@.ai/rules/error-handling.md
@.ai/rules/scope-discipline.md
@.ai/rules/verify-existing-code.md
@.ai/rules/test-before-claim.md
@.ai/rules/git-workflow.md
