# Test-Before-Claim Rules

알고리즘 구현·수정 후 "동작합니다" / "이게 원인일 거예요" 라고 보고하기 전에 반드시 **검증 가능한 테스트 케이스로 확인**한다. 추측과 검증을 분리한다.

## Principles

- 코드를 "써놓은 것"과 "동작 확인된 것"은 다르다.
- validator(static review)는 알고리즘 정확성을 입증하지 못한다. 스타일·아키텍처만 본다.
- 사용자에게 런타임 테스트를 떠넘기는 건 디버그 비용을 사용자에게 전가하는 것.
- 추측은 "추측"이라고 명시. 검증된 사실과 분리해서 보고.

## 적용 대상

다음 중 하나라도 해당되면 `algorithm-tester` subagent 호출 의무.

- Loop / 반복문 ≥ 1개 있는 메서드
- Branch / 조건 분기 ≥ 2개 (`if`/`switch`) 있는 메서드
- State machine, generator, parser
- 타겟팅·경로탐색·데미지 계산·웨이브 스폰 스케줄·업그레이드 수치 계산
- Shuffle, sort, packing, BFS/DFS, graph traversal
- 입출력 형변환 (예: 배열 변환, deep copy)
- 새로 작성된 비순수 함수 (외부 상태 의존)

## 비적용

다음 작업은 tester 호출 불필요.

- View 렌더 (시각 결과는 사용자 검증 영역)
- DI 등록만 변경
- 단순 필드/속성 추가
- 주석/리네이밍/포맷팅
- 1-2줄 직선 코드 수정

## 절차

1. **implementer**가 알고리즘 구현/수정.
2. **algorithm-tester** 호출 (auto-route).
3. tester가 3-5개 케이스를 hand-trace로 시뮬레이션.
4. PASS면 → validator로 진행 → 사용자에게 결과 + 테스트 케이스 출력.
5. FAIL이면 → fixer 호출 (실패한 케이스 명시) → tester 재호출.

## Anti-Patterns

- "이게 원인일 거예요" 추측만 하고 사용자에게 테스트 떠넘기기
- 단순 케이스 동작 확인 없이 복잡 케이스/엣지 케이스로 바로 가기
- validator 통과 = 알고리즘 정확성 입증이라고 착각
- "이 알고리즘은 명백하니까 테스트 불필요" 같은 자기 정당화
- 사용자가 "안 된다"고 보고할 때마다 새 가설 세우기 (테스트로 검증된 게 없는 상태에서 가설만 늘림)

## Required Behavior

- 알고리즘 변경 시: `implementer → algorithm-tester → (FAIL 시 fixer → algorithm-tester) → validator → 보고` 흐름 강제.
- 보고 시 hand-trace 결과(케이스별 input → output)를 사용자에게 함께 제시. 사용자가 검증 과정을 확인 가능.
- 검증되지 않은 가설은 "추측" 명시. 예: "추측: 사거리 판정이 제곱 거리 비교라 false일 가능성" — 검증 전이면 절대 단언 X.
- 단정형 발화("X는 Y다", "X는 Z만 가능") 전 자문: 근거가 직접 확인된 코드인가, 추측·기억인가? 추측이면 "추측:" 라벨 또는 코드 확인 후 답변. (도메인 동작 단정의 anti-pattern은 `verify-existing-code.md`의 "Domain Behavior" 섹션 참조)
- 사용자가 "동작 안 한다"고 보고하면 **가장 단순한 케이스부터** tester로 다시 검증. 새 가설 세우지 말 것.

## Good

```
사용자: "타겟팅이 작동 안 해요"
→ algorithm-tester 호출:
  - Case 1: enemy 1기, 사거리 내 → trace → PASS
  - Case 2: enemy 3기, 가장 앞선 enemy 선택 → trace → PASS
  - Case 3: 사거리 경계선상 enemy → trace → FAIL at line N (경계 포함/제외 불일치)
→ fixer: line N의 비교 연산자 수정
→ tester 재호출 → 전부 PASS
→ 사용자에게 보고 (트레이스 포함)
```

## Bad

```
사용자: "타겟팅이 작동 안 해요"
→ "사거리 임계값이 문제일 것 같습니다" (추측)
→ 임계값 변경
→ "이번엔 갱신 주기가 문제일 것 같습니다" (또 추측)
→ 주기 변경
→ 5번 추측 반복, 단 한 번도 실제 케이스 검증 X
```
