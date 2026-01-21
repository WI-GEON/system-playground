# Week01 Summary (C# AllocLab)

## 이번 주 결론 (3줄)
1) class 대량 생성은 Alloc/GC(Gen2)로 이어져 핫패스에서 위험함.
2) boxing은 Alloc과 GC를 크게 늘릴 수 있어 제네릭/타입 안정성으로 회피하는 것이 좋아보임.
3) LINQ 자체가 문제가 아니라 ToList 같은 컬렉션 생성이 핫패스에서 할당을 만듦.

## 측정 요약 (median)
- Exp1A struct-only: 3.134 ms / 11.44 MiB / GC 0/0/0
- Exp1B class-only : 27.226 ms / 38.15 MiB / GC total 9/6/3
- Exp2A ArrayList  : 26.106 ms / 30.52 MiB / GC total 6/3/3
- Exp2B List<int>  : 1.295 ms / 3.81 MiB / GC 0/0/0
- Exp3A LINQ       : 0.114 ms / 195.47 KiB / GC 0/0/0
- Exp3B Manual     : 0.047 ms / 0 B / GC 0/0/0

## 면접 대비?
- “핫패스에서 할당이 누적되면 GC 스파이크로 프레임 드랍이 난다. 그래서 풀링/연속 데이터 구조/컬렉션 생성 회피로 설계한다.”

## 다음 주(Week02) 계획
- (C#) 문자열/로그/클로저 캡처 할당 실험 2개 추가
- (C++) SimpleVector 시작(메모리/RAII)