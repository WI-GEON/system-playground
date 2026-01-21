## 테스트 목표
LINQ 파이프라인(특히 `ToList`)이 **할당량(Allocated bytes), GC, 실행 시간**에 미치는 영향을 확인한다.  
게임 개발에서 “LINQ를 무조건 금지”가 아니라, **어떤 사용 패턴이 비용(할당)을 만들고 왜 위험한지**를 근거로 설명할 수 있게 만든다.

<br>

## 테스트 방법
- Exp3A: `Where/Select/ToList`로 결과 컬렉션을 생성한 뒤 합산한다. (컬렉션 생성/할당 발생)
- Exp3B: 수동 루프로 조건을 처리하고 바로 합산한다. (추가 컬렉션 생성 없음)
- 두 케이스를 동일 데이터 크기에서 분리 실행한다.

<br>

## 테스트 환경 및 파라미터
- Build: Release
- Runs: warmup 1, measure 3 (Force GC before each run)
- N = 100,000
- Metrics: Time(ms), Alloc, GC(0/1/2), HeapΔ

<br>

## 테스트 결과

| Variant | Run | Time (ms) | Alloc | GC(0/1/2) | HeapΔ |
|---|---:|---:|---:|---:|---:|
| LINQ (Where/Select/ToList) | 1 | 0.193 | 195.47 KiB | 0/0/0 | 211.4 KiB |
| LINQ (Where/Select/ToList) | 2 | 0.112 | 195.47 KiB | 0/0/0 | 211.4 KiB |
| LINQ (Where/Select/ToList) | 3 | 0.114 | 195.47 KiB | 0/0/0 | 211.4 KiB |
| Manual loop (no collection) | 1 | 0.047 | 0 B | 0/0/0 | 8.03 KiB |
| Manual loop (no collection) | 2 | 0.047 | 0 B | 0/0/0 | 8.03 KiB |
| Manual loop (no collection) | 3 | 0.045 | 0 B | 0/0/0 | 8.03 KiB |

<br>

### 요약
- LINQ: median **0.114 ms**, median alloc **195.47 KiB**, total GC **0/0/0**
- Manual: median **0.047 ms**, median alloc **0 B**, total GC **0/0/0**

<br>

## 핵심 결과
- LINQ(ToList)는 매 실행마다 **약 195.47 KiB 할당**이 발생했다.
- 수동 루프는 **0B 할당**이며, 실행 시간도 더 빠른 편이었다.
- 이번 N(100,000)에서는 GC가 발생하지 않았지만, **빈번히 반복되는 경로에서는 누적 할당이 GC로 이어질 수 있다.**

<br>

## 결과 분석
- `ToList()`는 결과를 담는 새 `List<int>`를 만들기 때문에 **할당이 필연적**이다.  
  즉, LINQ의 “문법” 자체가 문제라기보다 **컬렉션 생성(ToList/ToArray 등)이 비용의 핵심**이다.
- Manual loop는 결과 컬렉션을 만들지 않고 “필터 + 변환 + 합산”을 바로 수행하여 **추가 할당이 없다(0B)**.
- GC가 당장 보이지 않는다고 안전한 것이 아니라, Update 같은 핫패스에서 누적되면:
  - 작은 할당이 계속 쌓이고
  - 어느 순간 Gen0/Gen1/Gen2 GC가 발생하며
  - 프레임 타임 스파이크(끊김)로 체감될 수 있다.

<br>

## 게임 개발에 적용한다면?
- 원칙: LINQ는 생산성이 높지만, **핫패스(매 프레임/빈번 호출)**에서 `ToList/ToArray` 같은 컬렉션 생성은 위험할 수 있다.
- 핫패스에서는 아래 방향을 우선 고려한다:
  - 수동 루프(조건/변환/누적을 한 번에)
  - 사전 할당(재사용 리스트/버퍼)
  - 풀링(컬렉션 풀)로 결과 버퍼 재사용
- 반대로 비핫패스(로딩/초기화/툴/에디터)에서는 LINQ를 적극 활용해도 된다.

<br>

## 느낀점
- “LINQ를 쓰지 말라”가 아니라, **ToList 같은 컬렉션 생성이 핵심 비용**이라는 점이 수치로 확인되어 기준이 생김.
- Update 경로에서는 “Alloc 0B”를 목표로 코드를 점검해야 한다는 확신이 생김.

<br>

## 면접을 본다면?
> LINQ 자체가 금지인 게 아니라, `ToList/ToArray` 같은 컬렉션 생성이 핫패스에서 할당을 만들고 누적되면 GC 스파이크로 이어질 수 있다.  
> 그래서 핫패스에서는 수동 루프/사전 할당/풀링으로 할당을 제어하고, LINQ는 비핫패스에서 제한적으로 사용한다.

<br>

## 콘솔 출력 원본
```txt
--- Exp3A: LINQ Where/Select/ToList ---
Run #1:    0.193 ms | Alloc: 195.47 KiB | GC(0/1/2): 0/0/0 | HeapΔ: 211.4 KiB
Run #2:    0.112 ms | Alloc: 195.47 KiB | GC(0/1/2): 0/0/0 | HeapΔ: 211.4 KiB
Run #3:    0.114 ms | Alloc: 195.47 KiB | GC(0/1/2): 0/0/0 | HeapΔ: 211.4 KiB
Summary: median 0.114 ms | median alloc 195.47 KiB | total GC(0/1/2) 0/0/0

--- Exp3B: manual loop (no extra collection) ---
Run #1:    0.047 ms | Alloc:        0 B | GC(0/1/2): 0/0/0 | HeapΔ: 8.03 KiB
Run #2:    0.047 ms | Alloc:        0 B | GC(0/1/2): 0/0/0 | HeapΔ: 8.03 KiB
Run #3:    0.045 ms | Alloc:        0 B | GC(0/1/2): 0/0/0 | HeapΔ: 8.03 KiB
Summary: median 0.047 ms | median alloc 0 B | total GC(0/1/2) 0/0/0
```

<br>

## 다음에 해볼 테스트
- `ToList()` 대신 "재사용 리스트(버퍼 재사용)"로 결과를 채우는 방식(풀링) 적용 후 Alloc/시간 비교
- `foreach` vs `for` 비교 및 Alloc 여부 확인
- 람다 캡처(closure)가 발생하는 LINQ 케이스를 추가하여 Alloc 증가를 확인