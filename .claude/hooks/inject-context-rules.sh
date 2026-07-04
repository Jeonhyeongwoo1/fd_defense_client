#!/usr/bin/env bash
# UserPromptSubmit hook: inject context-specific rule files only when relevant.
#
# CLAUDE.md always loads code-style/error-handling/scope-discipline/verify-existing-code.
# This hook injects the other rule files (architecture/di/reactive/creation/eventbus/git)
# only when the prompt mentions their concerns — saving ~1400 tokens per turn when not needed.

set -u

prompt=$(jq -r '.prompt // empty' 2>/dev/null)
[ -z "$prompt" ] && exit 0

RULES_DIR="$CLAUDE_PROJECT_DIR/.ai/rules"
ctx=""

append_rule() {
  local file=$1
  local label=$2
  if [ -f "$RULES_DIR/$file" ]; then
    ctx+="

[Auto-injected: $label — prompt mentioned related keywords]
$(cat "$RULES_DIR/$file")"
  fi
}

# Lowercase for case-insensitive matching
lower=$(printf '%s' "$prompt" | tr '[:upper:]' '[:lower:]')

# Helper: match either English regex (with word boundaries) OR Korean substring
match_any() {
  local en_pattern=$1
  local ko_pattern=$2
  printf '%s' "$lower" | grep -qE "$en_pattern" && return 0
  [ -n "$ko_pattern" ] && printf '%s' "$prompt" | grep -qE "$ko_pattern" && return 0
  return 1
}

# DI / VContainer
if match_any \
  '\b(di|vcontainer|inject|register|lifetime|lifetimescope|gamescenelifetimescope|istartable|itickable|iinitializable|idisposable)\b' \
  '의존성|주입|컨테이너|등록'; then
  append_rule "di-patterns.md" "DI patterns"
fi

# Reactive / UniRx / UniTask
if match_any \
  '\b(unirx|unitask|reactive|reactiveproperty|subject|observable|subscribe|compositedisposable|addto|asyncawait|cancellationtoken|coroutine)\b' \
  '구독|반응형|옵저버블|비동기|취소|코루틴'; then
  append_rule "reactive-patterns.md" "Reactive patterns"
fi

# Factory / Pool / Registry / creation
if match_any \
  '\b(factory|pool|poolmanager|registry|spawn|create|instantiate|towerfactory|enemyfactory|towerregistry|enemyregistry)\b' \
  '팩토리|풀링|레지스트리|스폰|생성|인스턴스'; then
  append_rule "creation-patterns.md" "Creation patterns"
fi

# EventBus
if match_any \
  '\b(eventbus|gameevent|publish|towerplacedevent|enemykilledevent|wavestartedevent)\b' \
  '이벤트버스|이벤트 ?버스|이벤트 발행'; then
  append_rule "event-bus-rules.md" "EventBus rules"
fi

# Git workflow
if match_any \
  '\b(branch|epic|claude/|merge|rebase|worktree|pull request|commit)\b' \
  '브랜치|머지|머징|리베이스|워크트리|커밋|푸시|풀리퀘'; then
  append_rule "git-workflow.md" "Git workflow"
fi

# Architecture (Layer / Dependency / Flow rules) — moved from always-loaded to lazy
if match_any \
  '\b(architecture|layer|new class|new system|module|where should|responsibility|design pattern|coupling|dependency direction)\b' \
  '아키텍처|레이어|모듈|새 클래스|새 시스템|어디에 둬|어디에 둬야|설계|책임|결합도|의존성 방향'; then
  append_rule "architecture.md" "Architecture rules"
fi

if [ -z "$ctx" ]; then
  exit 0
fi

jq -nc --arg c "$ctx" '{hookSpecificOutput:{hookEventName:"UserPromptSubmit",additionalContext:$c}}'
