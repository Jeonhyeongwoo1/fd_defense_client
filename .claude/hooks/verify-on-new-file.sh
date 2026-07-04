#!/usr/bin/env bash
# PreToolUse hook for Write: when a NEW .cs file is being created under
# Assets/Game/02.Scripts/, grep for sibling classes with the same responsibility
# keywords and inject the result as additionalContext so the assistant can
# verify before declaring it a brand-new system.
#
# Background: `.ai/rules/verify-existing-code.md` mandates "grep existing
# classes before creating new ones" — but a text rule alone is unreliable
# (the rule was violated in the same session it was written). This hook
# enforces the procedural part at tool-call level. The textual rule retains
# the *normative* part (why/anti-patterns/learnings).
#
# Information-only: never blocks. False positives just add ~100 tokens once.

set -u

file_path=$(jq -r '.tool_input.file_path // empty' 2>/dev/null)
[ -z "$file_path" ] && exit 0

# Scope: Unity C# scripts only.
case "$file_path" in
  *Assets/Game/02.Scripts/*.cs) ;;
  *) exit 0 ;;
esac

# Skip if file already exists (Write to existing file = overwrite, not a new system).
[ -e "$file_path" ] && exit 0

class_name=$(basename "$file_path" .cs)

# PascalCase / UI_-prefix decomposition.
# "DecorationPurchaseAnimationService" -> "Decoration Purchase Animation Service"
# "UI_DecorationElement"               -> "UI Decoration Element"
tokens=$(printf '%s' "$class_name" \
  | sed -E 's/_/ /g; s/([a-z0-9])([A-Z])/\1 \2/g; s/([A-Z]+)([A-Z][a-z])/\1 \2/g')

# Generic suffixes/prefixes — too broad to give useful matches if grepped alone.
GENERIC='Service|Manager|Controller|Presenter|Factory|Registry|Handler|View|Model|Event|Data|UI|Util|Utils|Const|Popup|Element|Base|Game|Async'

keywords=$(printf '%s\n' $tokens \
  | grep -E '^[A-Z][A-Za-z0-9]+$' \
  | grep -vE "^($GENERIC)$" \
  | sort -u)

[ -z "$keywords" ] && exit 0

SEARCH_ROOT="$CLAUDE_PROJECT_DIR/Assets/Game/02.Scripts"
[ -d "$SEARCH_ROOT" ] || exit 0

candidates=""
while IFS= read -r kw; do
  [ -z "$kw" ] && continue
  matches=$(find "$SEARCH_ROOT" -type f -name "*${kw}*.cs" 2>/dev/null \
    | grep -v "/${class_name}\.cs$" \
    | head -8)
  if [ -n "$matches" ]; then
    candidates+=$'\n'"  [keyword: $kw]"
    while IFS= read -r m; do
      rel=${m#"$CLAUDE_PROJECT_DIR/"}
      candidates+=$'\n'"    - $rel"
    done <<<"$matches"
  fi
done <<<"$keywords"

[ -z "$candidates" ] && exit 0

ctx="[verify-hook] 신규 클래스 '${class_name}' 생성 직전.
같은 키워드를 가진 기존 클래스가 발견되어 알려둠 — 분류 결정 전에 1회 Read 검증 권장.$candidates

→ 같은 책임의 클래스가 이미 있다면 그 위치/패턴을 따른다. 분류 추론보다 코드 실증 우선.
   규범·anti-pattern 근거: .ai/rules/verify-existing-code.md"

jq -nc --arg c "$ctx" '{hookSpecificOutput:{hookEventName:"PreToolUse",additionalContext:$c}}'
