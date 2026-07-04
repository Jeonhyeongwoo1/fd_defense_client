Create a new epic and working branch pair for the given name.

## Argument Parsing

The argument format is: `{name} [base-branch]`

- First token → branch name (e.g. `bottle-system`)
- Second token (optional) → base branch to start from (e.g. `feature/prototype`)
- If no base branch is provided, default to `main`

Examples:
- `/new-branch bottle-system` → starts from `main`
- `/new-branch bottle-system feature/prototype` → starts from `feature/prototype`

## Steps

1. Parse `$ARGUMENTS`:
   - Split by whitespace
   - `name` = first token
   - `base` = second token if present, otherwise `main`

2. Run the following in order:
   ```
   git checkout {base}
   git pull origin {base}
   git checkout -b epic/{name}
   git push -u origin epic/{name}
   git checkout -b claude/{name}
   git push -u origin claude/{name}
   git checkout claude/{name}
   ```

3. Verify the current branch with:
   ```
   git branch --show-current
   ```

4. Report:
   - Base branch used: `{base}`
   - Created: `epic/{name}`
   - Created: `claude/{name}`
   - Current branch: `claude/{name}` (confirmed)

The arguments are: $ARGUMENTS
