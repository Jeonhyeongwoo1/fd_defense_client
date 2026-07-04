#!/usr/bin/env bash
# PreToolUse hook for Bash: block project-forbidden git/gh operations.
# Reads hook input JSON from stdin, emits deny decision JSON if blocked.
#
# Blocks:
#   1. `git add .` / `git add -A` / `git add --all`   (CLAUDE.md: stage specific files only)
#   2. `git push ... main|master`                     (epic→main merge is manual, PR-based)
#   3. `gh pr create --base main|master` from a non-epic head   (only epic/* may target main)
#   4. `gh pr merge` of a PR whose base is main|master and head is non-epic  (same rule)
#
# Rule: a non-`epic/*` branch may NEVER be PR'd/pushed into main, under any circumstance.
# Only an `epic/*` branch may target main (and that still needs explicit user instruction).

set -u

cmd=$(jq -r '.tool_input.command // empty' 2>/dev/null)
[ -z "$cmd" ] && exit 0

emit_deny() {
  reason=$1
  printf '{"hookSpecificOutput":{"hookEventName":"PreToolUse","permissionDecision":"deny","permissionDecisionReason":"%s"}}\n' "$reason"
  exit 0
}

strip_quotes() {
  local v=$1
  v=${v#\"}; v=${v%\"}
  v=${v#\'}; v=${v%\'}
  printf '%s' "$v"
}

# Return the value of a `--flag value` or `--flag=value` option from the token array.
get_flag_val() {
  local flag=$1; shift
  local prev="" t
  for t in "$@"; do
    case "$t" in
      "${flag}="*) strip_quotes "${t#*=}"; return 0 ;;
    esac
    if [ "$prev" = "$flag" ]; then strip_quotes "$t"; return 0; fi
    prev=$t
  done
  return 1
}

is_main() { [ "$1" = "main" ] || [ "$1" = "master" ]; }
is_epic() { case "$1" in epic/*) return 0 ;; *) return 1 ;; esac }

# Tokenize the command (branch/flag names have no spaces, so whitespace split is sufficient).
read -ra toks <<< "$cmd"

# Rule 1: git add with bulk-staging args (`.`, `-A`, `--all`).
if printf '%s' "$cmd" | grep -qE '(^|[^[:alnum:]_-])git[[:space:]]+add([[:space:]]+[^[:space:]]+)*[[:space:]]+(\.|-A|--all)([[:space:]]|$)'; then
  emit_deny "Project rule (CLAUDE.md): Stage only specific files — never \`git add .\` / \`-A\` / \`--all\`. Use explicit file paths."
fi

# Rule 2: git push referencing main/master as a branch token.
# Catches: `git push origin main`, `git push origin :main`, `git push origin HEAD:main`,
#          `git push origin feature:main`, `git push -f origin main`, etc.
if printf '%s' "$cmd" | grep -qE '\bgit[[:space:]]+push\b' \
   && printf '%s' "$cmd" | grep -qE '\bgit[[:space:]]+push\b.*[[:space:]:"'"'"'](main|master)([[:space:]:"'"'"']|$)'; then
  emit_deny "Project rule: direct push to main/master is blocked. epic→main goes through a PR with explicit user instruction."
fi

# Rule 3: gh pr create targeting main/master from a non-epic head.
if printf '%s' "$cmd" | grep -qE '\bgh[[:space:]]+pr[[:space:]]+create\b'; then
  base=$(get_flag_val --base "${toks[@]}") || base=""
  if is_main "$base"; then
    head=$(get_flag_val --head "${toks[@]}") || head=""
    [ -z "$head" ] && head=$(git rev-parse --abbrev-ref HEAD 2>/dev/null)
    if ! is_epic "$head"; then
      emit_deny "Project rule: only an \`epic/*\` branch may target \`main\`. Head \`${head:-?}\` is not an epic branch — create the PR into the matching \`epic/*\` instead. (.claude/hooks/block-dangerous-git.sh)"
    fi
  fi
fi

# Rule 4: gh pr merge of a PR whose base is main/master and head is non-epic.
# Resolves the PR's base/head via gh; fails open (allows) if it cannot be determined,
# so legitimate epic→main merges are never falsely blocked.
if printf '%s' "$cmd" | grep -qE '\bgh[[:space:]]+pr[[:space:]]+merge\b'; then
  num=$(printf '%s\n' "${toks[@]}" | grep -oE '^[0-9]+$' | head -1)
  [ -z "$num" ] && num=$(printf '%s\n' "${toks[@]}" | grep -oE 'pull/[0-9]+' | grep -oE '[0-9]+' | head -1)
  if [ -n "$num" ]; then
    info=$(gh pr view "$num" --json baseRefName,headRefName 2>/dev/null)
  else
    info=$(gh pr view --json baseRefName,headRefName 2>/dev/null)
  fi
  if [ -n "$info" ]; then
    base=$(printf '%s' "$info" | jq -r '.baseRefName // empty' 2>/dev/null)
    head=$(printf '%s' "$info" | jq -r '.headRefName // empty' 2>/dev/null)
    if is_main "$base" && ! is_epic "$head"; then
      emit_deny "Project rule: PR #${num:-current} merges non-epic head \`${head:-?}\` into \`$base\`. Only an \`epic/*\` branch may merge into main. (.claude/hooks/block-dangerous-git.sh)"
    fi
  fi
fi

exit 0
