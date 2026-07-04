# Verify Existing Code Rules

기존 코드를 활용·참조하기 전에 직접 읽어 사실을 확인한다.
외부 보고(implementer 요약, 이전 세션 결정, 자신의 기억, 이전 작업의 패턴)는 출처 정보일 뿐
검증되지 않은 한 진실로 취급하지 않는다.

> **절차의 자동화**: 신규 `.cs` 파일 생성 시점의 "같은 키워드 기존 클래스 grep"은
> `.claude/hooks/verify-on-new-file.sh`가 PreToolUse(Write)에서 자동 실행한다.
> 이 문서는 **그 외 상황 — 기존 파일 수정·분류 재검토·구조 결정 — 에서 따라야 할 규범과 anti-pattern**이다.
> 즉 hook이 "강제할 수 있는 절차"를 맡고, 이 rule은 "강제할 수 없는 사고 흐름"을 맡는다.

## Principles

- **추상 분류보다 단어 그대로 grep**. 사용자가 명사로 시스템·클래스 이름을 줬다면 그 단어를 먼저 grep으로 검색한다. "리소스 = Addressables", "Animation orchestrator = Controller" 같은 자동 추상화로 갈음하지 않는다.
- **외부 보고는 출처 정보, 코드가 진실**. implementer 보고서·이전 세션 결정·자신의 기억은 모두 출처. 신뢰 전에 직접 Read.
- **이전 작업 답습 금지**. 이전 작업이 같은 일을 한 코드를 봤다고 해서 그게 정답이라는 보장 없다. 이전 작업이 동일 실수의 결과일 수 있다.
- **자동 주입 규칙 우선**. 시스템 프롬프트에 `.ai/rules/<file>.md`가 자동 주입되어 있으면 그 내용을 결정의 1차 근거로 삼는다. 무시하고 결정한 뒤 사후 발견은 잘못.
- **1차 실수 후 자기교정 범위를 좁히지 말 것**. 분류 지적을 받으면 같은 카테고리 안에서 다른 후보로 옮기지 말고 **카테고리 자체를 재검토**한다. (예: Controller→Presenter 이분법으로 좁히지 말고 Service까지 후보에 올림.)

## Anti-Patterns

- "이전 작업에서 이렇게 했으니 같은 패턴이다" — 이전 작업이 잘못이었을 수 있음
- "implementer 보고에 '다른 패턴'이라 적혀있어서 안 쓴다" — 그 판단이 틀렸을 수 있음
- "리소스 → Addressables", "연출 orchestrator → Controller/Presenter" 식 자동 추상화 — 단어를 그대로 grep 먼저
- "기억에 비슷한 코드 본 적 있다"로 진행 — 기억은 stale 가능
- 분류 정의(`architecture.md`)만 보고 결정 — 실제 그 폴더에 어떤 패턴의 클래스가 사는지 1회 확인 안 함
- 첫 분류 실수 후 같은 카테고리 안에서만 자기교정하기 — 카테고리 자체가 틀렸을 가능성 검토 안 함

## Required Behavior (Hook 외 상황)

- **기존 파일 수정 시**: 그 파일과 협력하는 sibling 클래스(같은 책임/폴더)를 1회 grep + Read.
- **분류·구조 결정 시**: 같은 책임의 reference 클래스를 grep으로 먼저 찾고, 그 위치/패턴을 따른다. 분류 정의는 그 다음.
- **implementer 위임 시**: 위임 명세에 "관련 기존 클래스 grep + Read 후 진행" 명시. 결과 보고서를 그대로 신뢰하지 말고 1차 코드 검증.
- **결정 포인트별 reference**:
  - prefab 인스턴스화 → `PoolManager.Get` (직접 Instantiate 아님)
  - 리소스 로드 → `ResourceService.Load<T>` (직접 Addressables 아님)
  - 연출 시퀀스 오케스트레이션 → `Service/` (기존 `*AnimationService` 패턴 참고)
  - 새 시스템 → `creation-patterns.md`의 Factory/Pool/Registry 검토

## Domain Behavior — RuleHandler/Service 직접 확인

게임 룰·시스템 동작(타워 공격 룰, 웨이브 스폰 룰, 데미지 계산 등)에 대해 단정형 발화를 하기 전, 해당 도메인의 Handler/Service 파일을 직접 grep + Read한다.
**한 곳의 코드(예: 시뮬레이터·에디터 보조 함수)를 시스템 전체 룰로 일반화하지 않는다** — 같은 동작이 여러 곳에 정의될 수 있고 의미가 다를 수 있다.

### Anti-Patterns
- 보조 함수만 보고 "실제 게임 룰도 같다"고 일반화
- 테스트·에디터 코드가 사용한 가정을 실제 게임 룰로 단정
- "비슷한 코드 본 적 있어" 기반 동작 단정 (기억은 stale)
- 코드 확인 없이 사용자에게 "X는 Y이다" 단정 → 정정 받음

### Required Behavior
- "X는 Y만 가능", "X는 Z이다" 형 단정 전: 해당 X의 Handler/Service 파일 1회 Read
- 시뮬레이션·에디터 코드와 실제 룰 코드 둘 다 확인 — 같은지 검증
- 단정 근거가 부분 코드면 "추측:" 라벨 명시

---

## Efficient Edits (리네이밍·이동)

기존 파일의 이름·위치만 바꾸는 작업은 신규 구현이 아니므로 도구 선택이 다르다.
이미 존재하는 코드를 다시 만들지 말고 옮기거나 치환한다.

### Principles
- **파일 이름·위치 변경**: `mv` 우선. `rm` + `Write`는 Unity `.meta` GUID 재발급 → prefab/scene 참조 깨질 위험.
- **식별자 치환**: `Edit replace_all` 우선. 같은 내용을 통째 `Write`로 다시 쓰지 않는다.
- **순수 리네이밍은 validator 생략**: 알고리즘·구조 변경 없으면 `grep`으로 잔재 참조만 확인하면 충분.
- **컨텍스트에 있는 파일 재 Read 금지**: 직전 turn에 Read 했거나 자기 자신이 방금 Write 한 파일은 다시 읽지 않는다.

### Anti-Patterns
- 수백 줄 파일을 식별자 한두 군데 바꾸려고 통째 `Write`로 재작성
- 리네이밍·이동 후 validator agent 호출 (수만 토큰 낭비)
- `mv` 대신 `rm` + `Write` 조합 (`.meta` GUID 깨짐)
- 같은 식별자를 여러 곳에서 분할 `Edit` (한 번의 `replace_all`로 가능)

### Required Behavior
- 작업 시작 전 "리네이밍·이동인가, 신규 구현인가?" 자문
- 리네이밍이면: `mv` → `Edit replace_all` → `grep` (잔재 확인) 3단계로 종결
- `.meta` 파일도 함께 `mv` (GUID 보존)
