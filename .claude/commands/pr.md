Create a PR from the current branch into the matching epic branch, then merge it.

Steps:
1. Get the current branch name
2. Extract the suffix (e.g. `claude/test` → `test`)
3. Check if `epic/{suffix}` exists on remote
   - If yes: use `epic/{suffix}` as base
   - If no: **STOP. Do not create a PR. Never fall back to `main`.** Report that `epic/{suffix}` is missing and ask the user how to proceed.

> The PR base must ALWAYS be an `epic/*` branch. A non-`epic/*` branch may never target `main`. Only an `epic/*` branch may merge into `main`, and that merge requires explicit user instruction (never automated). This is also enforced by `.claude/hooks/block-dangerous-git.sh`.
4. Run:
   ```
   gh pr create --base {base} --title "..." --body "..."
   ```
   - Title: concise summary of what changed
   - Body: include what changed, current branch, and target base
5. Immediately merge:
   ```
   gh pr merge {number} --merge
   ```
6. Report:
   - PR URL
   - Merged: `{current}` → `{base}`
   - Status: MERGED

Do not ask for confirmation. Create and merge in one flow.
