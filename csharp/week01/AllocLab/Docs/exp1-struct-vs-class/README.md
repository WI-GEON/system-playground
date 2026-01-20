# Exp1 - struct-only vs class-only (List add + sum)

## Goal
값 타입(struct)과 참조 타입(class) 선택이 **할당량(Allocated bytes), GC, 실행 시간**에 미치는 영향을 비교한다.  
핫패스에서 “대량 객체 생성”이 왜 위험한지(프레임 드랍/GC 스파이크)를 근거로 설명할 수 있게 만든다.

## Method
- 동일한 데이터(3 floats)를 N개 생성하여 List에 넣고, 다시 순회하며 합산한다.
- Exp1A: `List<SmallStruct>` (struct-only)
- Exp1B: `List<SmallClass>` (class-only)

## Environment / Parameters
- Build: Release
- Runs: warmup 1, measure 3 (Force GC before each run)
- N = 1,000,000
- Metrics: Time(ms), Alloc, GC(0/1/2), HeapΔ

## Results

| Variant | Run | Time (ms) | Alloc | GC(0/1/2) | HeapΔ |
|---|---:|---:|---:|---:|---:|
| struct-only | 1 | 3.049 | 11.44 MiB | 0/0/0 | 11.46 MiB |
| struct-only | 2 | 3.241 | 11.44 MiB | 0/0/0 | 11.46 MiB |
| struct-only | 3 | 3.134 | 11.44 MiB | 0/0/0 | 11.46 MiB |
| class-only  | 1 | 27.256 | 38.15 MiB | 3/2/1 | 38.14 MiB |
| class-only  | 2 | 27.226 | 38.15 MiB | 3/2/1 | 38.14 MiB |
| class-only  | 3 | 26.191 | 38.15 MiB | 3/2/1 | 38.14 MiB |

### Summary
- struct-only: median **3.134 ms**, median alloc **11.44 MiB**, total GC **0/0/0**
- class-only : median **27.226 ms**, median alloc **38.15 MiB**, total GC **9/6/3**

## Key Findings
- class-only는 struct-only 대비:
  - 시간: **약 8.7배 느림** (27.226 ms vs 3.134 ms)
  - 할당: **약 3.3배 많음** (38.15 MiB vs 11.44 MiB)
  - GC: **Gen2 포함 GC 발생** (매 run마다 3/2/1)

## Interpretation
- class는 원소마다 **힙 객체가 생성**되고, List 내부에는 참조 배열이 유지된다.  
  대량 생성 시 힙 압박이 커져 세대 승격이 발생하고 Gen2 GC까지 도달할 수 있다.
- struct는 개별 객체 할당이 없고 List 내부 배열에 값이 연속 저장된다(주요 할당은 내부 버퍼).

## Practical Takeaways (Game/Unity)
- 핫패스(매 프레임/대량 업데이트/대량 스폰)에서는:
  - 대량 객체 생성 회피
  - 데이터 연속화(배열/struct/SoA) 또는 풀링(pooling) 우선 고려
- 단, struct도 무조건 정답은 아니다:
  - 큰 struct는 복사 비용/캐시 효율 문제가 생길 수 있어 크기와 접근 패턴을 함께 본다.

## What I Learned
- “대량 class 생성 → GC 스파이크”가 수치로 확인되었다.
- 할당/GC를 회피해야 하는 이유가 설계 원칙으로 명확해졌다.

## Interview-ready Answer (short)
> 참조 타입을 대량 생성하면 힙 압박이 커져 GC(특히 Gen2)로 이어질 수 있다.  
> 그래서 핫패스에서는 대량 객체 생성과 컬렉션 생성을 피하고, 연속 데이터 구조/풀링으로 설계한다.  
> struct도 크기/복사 비용을 고려해 선택한다.

## Console Output (raw)
```txt
--- Exp1A: struct-only (List add + sum) ---
Run #1:    3.049 ms | Alloc:  11.44 MiB | GC(0/1/2): 0/0/0 | HeapΔ: 11.46 MiB
Run #2:    3.241 ms | Alloc:  11.44 MiB | GC(0/1/2): 0/0/0 | HeapΔ: 11.46 MiB
Run #3:    3.134 ms | Alloc:  11.44 MiB | GC(0/1/2): 0/0/0 | HeapΔ: 11.46 MiB
Summary: median 3.134 ms | median alloc 11.44 MiB | total GC(0/1/2) 0/0/0

--- Exp1B: class-only  (List add + sum) ---
Run #1:   27.256 ms | Alloc:  38.15 MiB | GC(0/1/2): 3/2/1 | HeapΔ: 38.14 MiB
Run #2:   27.226 ms | Alloc:  38.15 MiB | GC(0/1/2): 3/2/1 | HeapΔ: 38.14 MiB
Run #3:   26.191 ms | Alloc:  38.15 MiB | GC(0/1/2): 3/2/1 | HeapΔ: 38.14 MiB
Summary: median 27.226 ms | median alloc 38.15 MiB | total GC(0/1/2) 9/6/3
```

## Next
- 큰 struct(예: 64B ~ 128B) 케이스를 추가하여 "복사 비용 vs 할당 절감" 트레이드오프 확인
- 접근 패턴(순차 vs 랜덤) 실험으로 캐시 영향 관찰