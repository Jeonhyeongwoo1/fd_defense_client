# Git Workflow Rules

## Branch Strategy

When the user requests work on a feature (e.g. "test 브랜치로 작업해줘", "xxx 기능 만들어줘"):

1. Create `epic/{name}` branch from `main`
2. Create `claude/{name}` as the working branch from `epic/{name}`
3. Do all work on `claude/{name}`
4. Push both branches to remote

```
main
 └── epic/{name}        ← created first, base for PRs
      └── claude/{name} ← actual working branch
```

### Example

User says: "test 브랜치로 작업해줘"

```bash
git checkout main
git checkout -b epic/test
git push -u origin epic/test

git checkout -b claude/test
git push -u origin claude/test
```

---

## Branch Naming Rules

| Prefix | Used by | Example |
|--------|---------|---------|
| `epic/` | feature 집합 | `epic/tower-system` |
| `claude/` | Claude Code | `claude/tower-targeting` |
| `codex/` | GitHub Codex | `codex/tower-view` |
| `feature/` | manual work | `feature/wave-data` |
| `fix/` | bug fix | `fix/projectile-hit-bug` |

---

## Commit Rules

- Stage only relevant files — never `git add .` or `git add -A`
- One commit per logical change

```bash
git add {specific-file}
git commit -m "{message}"
git push origin claude/{name}
```

---

## PR Automation Rules

When the user says "PR 넣어줘", "PR 만들어줘", "PR 올려줘", or similar:

1. Detect current branch (e.g. `claude/test`)
2. Automatically find the matching `epic/*` as the base (e.g. `epic/test`)
   - Match by name suffix: `claude/test` → `epic/test`
   - If no matching `epic/*` exists: **STOP — never fall back to `main`.** Report the missing epic branch and ask the user.
3. Create PR: `claude/{name}` → `epic/{name}`
4. Merge immediately after creation
5. Report result with PR URL and merge status

**Do not ask for confirmation — create and merge in one flow.**

### Example

```
Current branch: claude/test
→ Detected base: epic/test
→ gh pr create --base epic/test ...
→ gh pr merge {number} --merge
→ Report: PR #X merged into epic/test ✓
```

> **Only an `epic/*` branch may target `main`.** A non-`epic/*` branch (`claude/*`, `codex/*`, `feature/*`, `fix/*`, …) may NEVER be PR'd or pushed into `main` — under any circumstance.
> `epic → main` merge is NOT automated. Always wait for explicit user instruction before merging into main.
> Hard-enforced by `.claude/hooks/block-dangerous-git.sh` (blocks `git push`, `gh pr create`, and `gh pr merge` that would put a non-epic branch into main).

---

## Why This Structure

| Problem | Solution |
|---------|----------|
| AI 두 개가 같은 파일 충돌 | 브랜치 분리로 격리 |
| 어느 AI가 뭘 했는지 추적 어려움 | 브랜치 prefix로 명확히 구분 |
| AI 코드 리뷰 없이 main 반영 | epic에서 한 번 더 검토 후 merge |
| 롤백 범위 불명확 | PR 단위로 revert 가능 |
