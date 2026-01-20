## 테스트 목표
boxing/unboxing이 **할당량(Allocated bytes), GC, 실행 시간**에 미치는 영향을 비교한다.  
게임 개발에서 `object` 기반 API/컬렉션이 왜 핫패스에서 위험한지(프레임 드랍/GC 스파이크)를 근거로 설명할 수 있게 만든다.

<br>

## 테스트 방법
- Exp2A: `ArrayList`에 `int`를 추가하고 다시 꺼내 합산한다. (boxing + unboxing)
- Exp2B: `List<int>`에 `int`를 추가하고 다시 꺼내 합산한다. (no boxing)
- 동일한 N(반복 횟수)로 두 케이스를 분리 실행한다.

<br>

## 테스트 환경 및 파라미터
- Build: Release
- Runs: warmup 1, measure 3 (Force GC before each run)
- N = 1,000,000
- Metrics: Time(ms), Alloc, GC(0/1/2), HeapΔ

<br>

## 테스트 결과

| Variant | Run | Time (ms) | Alloc | GC(0/1/2) | HeapΔ |
|---|---:|---:|---:|---:|---:|
| ArrayList (boxing) | 1 | 17.090 | 30.52 MiB | 2/1/1 | 30.56 MiB |
| ArrayList (boxing) | 2 | 26.106 | 30.52 MiB | 2/1/1 | 30.56 MiB |
| ArrayList (boxing) | 3 | 26.903 | 30.52 MiB | 2/1/1 | 30.56 MiB |
| List<int> (no boxing) | 1 | 1.295 | 3.81 MiB | 0/0/0 | 3.83 MiB |
| List<int> (no boxing) | 2 | 1.288 | 3.81 MiB | 0/0/0 | 3.83 MiB |
| List<int> (no boxing) | 3 | 1.633 | 3.81 MiB | 0/0/0 | 3.83 MiB |

<br>

### 요약
- ArrayList(박싱): median **26.106 ms**, median alloc **30.52 MiB**, total GC **6/3/3**
- List<int>(노박싱): median **1.295 ms**, median alloc **3.81 MiB**, total GC **0/0/0**

<br>

## 핵심 결과
- ArrayList(박싱)는 List<int>(노박싱) 대비:
  - 시간: **약 20배 느림** (26.106 ms vs 1.295 ms)
  - 할당: **약 8배 많음** (30.52 MiB vs 3.81 MiB)
  - GC: **Gen2 포함 GC 발생** (매 run마다 2/1/1)

<br>

## 결과 분석
- `ArrayList`는 내부 저장 타입이 `object`라서 `int`를 넣는 순간 boxing이 발생한다.  
  이 과정에서 추가 비용(변환 + 관리 힙 압박)이 발생하고, 대량 반복 시 GC(세대 승격 포함)로 이어질 수 있다.
- 반대로 `List<int>`는 제네릭이라 값 타입이 내부 배열에 직접 저장되어 boxing/unboxing이 없다.
- 이번 결과에서 ArrayList는 매 회차 **Gen2 GC가 1회씩** 발생했고, 그 영향이 실행 시간과 안정성(프레임 타임)에 직접적인 리스크가 된다.

<br>

## 게임 개발에 적용한다면?
- 원칙적으로 **제네릭 컬렉션(List<T>, Dictionary<K,V>)을 기본으로 사용**한다.
- 핫패스에서 다음 패턴을 특히 경계한다:
  - `object` 파라미터/반환, `params object[]`
  - 인터페이스/비제네릭 컬렉션 경유로 값 타입이 object로 승격되는 경로
  - 로그/문자열 포맷팅(구조체/숫자 값이 object로 포장되는 경우)
- “작은 boxing”이라도 매 프레임/대량 루프에서 누적되면 GC 스파이크로 이어질 수 있으므로, Alloc(0B) 목표로 점검한다.

<br>

## 느낀점
- “boxing은 느리다”가 단순한 격언이 아니라, **Alloc/GC로 명확히 증명**되어서 설계 기준이 생김.
- object 경로가 보이면 ‘혹시 여기서 boxing이 터지는가?’를 의심하는 습관이 필요하다고 느낌.

<br>

## 면접을 본다면?
> boxing은 값 타입이 object로 변환되며 추가 비용과 힙 압박을 만들 수 있고, 누적되면 GC로 이어질 수 있다.  
> 그래서 실무에서는 제네릭을 사용하고, 핫패스의 object/인터페이스 경유 경로를 점검해 boxing을 회피한다.

<br>

## 콘솔 출력 원본
```txt
--- Exp2A: ArrayList<int> boxing/unboxing ---
Run #1:    17.09 ms | Alloc:  30.52 MiB | GC(0/1/2): 2/1/1 | HeapΔ: 30.56 MiB
Run #2:   26.106 ms | Alloc:  30.52 MiB | GC(0/1/2): 2/1/1 | HeapΔ: 30.56 MiB
Run #3:   26.903 ms | Alloc:  30.52 MiB | GC(0/1/2): 2/1/1 | HeapΔ: 30.56 MiB
Summary: median 26.106 ms | median alloc 30.52 MiB | total GC(0/1/2) 6/3/3

--- Exp2B: List<int> no boxing ---
Run #1:    1.295 ms | Alloc:   3.81 MiB | GC(0/1/2): 0/0/0 | HeapΔ: 3.83 MiB
Run #2:    1.288 ms | Alloc:   3.81 MiB | GC(0/1/2): 0/0/0 | HeapΔ: 3.83 MiB
Run #3:    1.633 ms | Alloc:   3.81 MiB | GC(0/1/2): 0/0/0 | HeapΔ: 3.83 MiB
Summary: median 1.295 ms | median alloc 3.81 MiB | total GC(0/1/2) 0/0/0
```

<br>

## 다음에 해볼 테스트
- params object[](예: 로그 API)에서 boxing이 얼마나 발생하는지 Alloc 측정
- 인터페이스 기반 호출에서 boxing이 생기는 케이스(예: IComparable, IEnumerator) 분리 실험
- “값 타입을 object로 담는 컨테이너”를 직접 만들고(예: object[]), 할당/GC 변화 확인