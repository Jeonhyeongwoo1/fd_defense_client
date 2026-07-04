Create a PR from one epic branch into another epic branch, then merge it.

Usage: `/epic-pr [from-epic] [to-epic]`

Direction semantics:
- `$1` = from-epic → becomes **head** (source of changes)
- `$2` = to-epic → becomes **base** (destination that receives changes)
- Merge flow: `from-epic` is merged **into** `to-epic`

Arguments may be given with or without the `epic/` prefix. Both forms work:
- `/epic-pr update-resource main-feature`
- `/epic-pr epic/update-resource epic/main-feature`

Steps:
1. Read `$1` and `$2`.
   - If either is missing, stop and report usage.
2. Normalize both names: if the value does not start with `epic/`, prepend `epic/`.
   - `head = $1` (normalized)
   - `base = $2` (normalized)
3. Verify both branches exist on remote:
   ```
   git ls-remote --heads origin {head}
   git ls-remote --heads origin {base}
   ```
   - If either does not exist, stop and report which branch is missing.
4. Run:
   ```
   gh pr create --base {base} --head {head} --title "..." --body "..."
   ```
   - Title: concise summary of what `{head}` brings into `{base}` (e.g. `Merge epic/X into epic/Y`).
   - Body: include what changed, source (`head`), and target (`base`).
5. Immediately merge:
   ```
   gh pr merge {number} --merge
   ```
6. Report:
   - PR URL
   - Merged: `{head}` → `{base}`
   - Status: MERGED

Do not ask for confirmation. Create and merge in one flow.
