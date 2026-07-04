Create a new git worktree with an `epic/{name}` + `claude/{name}` branch pair, starting from the given target branch.

This keeps worktree work aligned with the project's `main ← epic/{name} ← claude/{name}` convention so `/pr` and `/epic-pr` continue to work unchanged.

## Argument Parsing

The argument format is: `{name} [target-branch]`

- First token → worktree / branch name (e.g. `refactoring`)
- Second token (optional) → target branch to start from (e.g. `epic/prototype`)
- If no target branch is provided, default to `main`

Examples:
- `/new-worktree refactoring` → starts from `main`
- `/new-worktree refactoring epic/prototype` → starts from `epic/prototype`

## Steps

1. Parse `$ARGUMENTS`:
   - Split by whitespace
   - `name` = first token
   - `target` = second token if present, otherwise `main`

2. Pre-checks:
   - `git fetch origin {target}` — ensure we have the latest target tip.
   - Verify `origin/{target}` exists; if not, stop and report.
   - Verify neither `epic/{name}` nor `claude/{name}` already exists locally or on remote; if either does, stop and report.
   - Verify `.claude/worktrees/{name}` does not already exist; if it does, stop and report.

3. Run the following in order:
   ```
   git branch epic/{name} origin/{target}
   git push -u origin epic/{name}
   git worktree add -b claude/{name} .claude/worktrees/{name} epic/{name}
   git -C .claude/worktrees/{name} push -u origin claude/{name}
   ```

4. Verify:
   ```
   git worktree list
   git -C .claude/worktrees/{name} branch --show-current
   ```

5. Report:
   - Target branch used: `{target}`
   - Created branch: `epic/{name}`
   - Created branch: `claude/{name}` (checked out inside the worktree)
   - Worktree path: `.claude/worktrees/{name}`
   - Current branch inside worktree: `claude/{name}` (confirmed)
   - Next:
     - Start a Claude session pinned to the worktree: `claude --resume {name}` or `cd .claude/worktrees/{name} && claude`
     - Work on `claude/{name}`, then run `/pr` inside that session — PR will target `epic/{name}`.
     - To merge the epic into another epic (or `main`) later, use `/epic-pr {name} <target-epic>`.

The arguments are: $ARGUMENTS
