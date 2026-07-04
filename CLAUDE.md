# CLAUDE.md

This file provides guidance to Claude Code when working with code in this repository.

## Overview
Unity defense game. Goal: ship a complete playable game.

## Stack
- Unity C# (URP 2D, Input System)
- VContainer (DI)
- UniRx (reactive state)
- UniTask (async; Coroutine only inside MonoBehaviour)

## Architecture (project-specific)

### Game Flow State Machine
`GameFlowService` (entry point: `IStartable`, `ITickable`) owns the state machine. Each state is a `BaseGameState` subclass and uses `RequestStateChange` callback — states never call `GameFlowService` directly.

Expected states: Ready → WavePlaying → WaveCleared → (next wave | Result).

### DI Root
`GameSceneLifetimeScope` is the single VContainer registration file for the game scene. All services/models/registries/factories/entry points register with `Lifetime.Scoped`.

### Core Domains (Features)
- **Tower** — 배치·타겟팅·공격·업그레이드. `TowerRuleService` (룰), `TowerFactory` + `TowerRegistry` (생성·추적)
- **Enemy** — 스폰·이동(경로)·HP·사망. `EnemyFactory` + `EnemyRegistry` + `PoolManager` (고빈도 스폰이므로 풀링 필수)
- **Wave** — 웨이브 스케줄·스폰 테이블(`WaveDataSO`)·클리어 판정. `WaveProgressService`
- **Projectile / Combat** — 투사체·데미지 계산·hit 판정. 풀링 필수

## Folder Rules
Base folders: `Assets/Game/01.Scene`, `Assets/Game/02.Scripts`, `Assets/Game/03.Resources`

Top-level under `Assets/Game/02.Scripts/`:
- `App/` — 부팅·LifetimeScope·Scene 전환
- `Core/` — Cross-cutting infra (Const, Event, Network, Resource, UI, Utils, ...)
- `Features/` — 게임 로직 도메인 (Tower, Enemy, Wave, Projectile, Shop, ...)
- `UI/` — Scene-level UI orchestrator (GameScene/OutGameScene HUD + CameraController). 도메인 popup은 `Features/<도메인>/Presenter/` + `View/`
- `Effect/` — 연출·애니메이션 (Hit, Explosion, Death)
- `Editor/` — 에디터 도구

Within each feature folder: classify by responsibility (View · Model · Service · Presenter · Controller · Factory · Registry · Event · Interface · Data · Utils · GameState). Sub-layer optional — apply only for large features.

Namespace는 폴더와 강제 일치하지 않음 — 기존 `Game.Service`, `Game.Model` 등 그대로 유지.

Create missing folders if needed. Always specify target file path.

## Git Workflow (brief)
- Branch: `main` ← `epic/{name}` ← `claude/{name}` (working branch)
- PR target: `claude/{name}` → `epic/{name}` (match by suffix); merge immediately after creation
- `epic → main` merge requires explicit user instruction
- Stage only specific files — never `git add .` (enforced by hook)

> Detailed branch/PR/commit rules are auto-injected when prompt mentions git keywords.

## Subagent Routing

### Implementer delegation threshold
- **Delegate to `implementer`** when the task touches **3 or more files** (read or modify), OR creates a new file, OR adds a new class/system.
- **Handle in main** when the task touches **fewer than 3 files** AND is a localized edit (signature change, small fix, naming, single-method refactor).
- Rationale: subagent has fixed overhead (~5–15K tokens for system prompt + briefing + summary). Below the 3-file threshold the overhead usually exceeds the context-isolation benefit. Above it, isolating file reads from main context is the clear win.

### Dispatch mode
- **Parallel** (3+ independent tasks, no file overlap, different layers): dispatch in single message
- **Sequential** (dependencies, shared file mods): chain calls

### Validator / fixer
- **After `implementer` delegation**: always run `validator` → if FAIL: `fixer` (minimal scope)
- **After main-handled edits**: run `validator` only when the change affects architecture, DI registration, or cross-layer contracts. Skip for purely local edits.

### Algorithm-tester (runtime correctness)
- **After implementer/fixer modifies algorithm code** (loop·branch·state machine·targeting·pathing·damage calc·wave scheduling): call `algorithm-tester` to hand-trace 3-5 test cases.
- tester reports PASS/FAIL per case + overall verdict. FAIL → fixer with case info → tester 재호출.
- 사용자에게 보고 시 hand-trace 결과를 함께 제시 (검증 가능성 확보).
- See `.ai/rules/test-before-claim.md` for detailed flow & anti-patterns.
- **Skip** for View/DI-only/rename/comment changes (non-algorithmic).

## Output Rules
On every implementation/modification, provide:
1. File path + name
2. Full copy-pasteable code
3. Brief explanation: what was modified / added / important notes

Keep it short and concrete.

---

## Shared Rules (always loaded)

@.ai/rules/code-style.md
@.ai/rules/error-handling.md
@.ai/rules/scope-discipline.md
@.ai/rules/verify-existing-code.md
@.ai/rules/test-before-claim.md

> Context-specific rules (Architecture / DI / Reactive / Creation / EventBus / Git workflow) are injected on demand by `.claude/hooks/inject-context-rules.sh` when the prompt mentions related keywords.
