## 개요
이 폴더는 Week01 AllocLab의 **실험 리포트(README)**와 **실행 로그**를 모아둔다.
목표는 할당(Alloc) / GC / 실행 시간을 수치로 기록하고, 게임 개발 관점에서 해석까지 남기는 것이다.

<br>

## 빠른 링크
- [Exp1: struct vs class](./exp1-struct-vs-class/README.md)
- [Exp2: boxing (ArrayList) vs no-boxing (List&lt;int&gt;)](./exp2-boxing/README.md)
- [Exp3: LINQ(ToList) vs manual loop](./exp3-linq/README.md)

<br>

## 실행 방법
### Visual Studio
- Configuration: **Release**
- Run: **Ctrl + F5**

### CLI
```bash
dotnet run -c Release --project csharp/week01/AllocLab/AllocLab.csproj
```

<br>

## 측정값이 흔들릴 때 체크할 것
- Debug로 실행하지 않았는지 (Release + Ctrl+F5 권장)
- 백그라운드 작업(브라우저/업데이트/인덱싱)이 과도하지 않은지
- N 값이 너무 커서 GC가 과도하게 발생하지는 않는지

<br>

## 이번 주 실험 결과 핵심 요약
- Exp1A struct-only: 3.134 ms, 11.44 MiB alloc, GC 0/0/0
- Exp1B class-only : 27.226 ms, 38.15 MiB alloc, GC total 9/6/3
- Exp2A ArrayList : 26.106 ms, 30.52 MiB alloc, GC total 6/3/3
- Exp2B List<int> : 1.295 ms, 3.81 MiB alloc, GC 0/0/0
- Exp3A LINQ : 0.114 ms, 195.47 KiB alloc, GC 0/0/0
- Exp3B Manual : 0.047 ms, 0 B alloc, GC 0/0/0