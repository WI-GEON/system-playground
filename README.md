# systems-playground

**C#/.NET과 C++에서 실무에 직접 연결되는 시스템 지식**을 “실험 → 측정 → 기록” 형태로 쌓는 저장소

- 대상: C# 런타임/GC/할당 최적화, C++ 메모리/RAII/성능/스레딩, 네트워크/OS/컴구 기초 실험
- 목표: 면접에서 “왜 이렇게 설계했는지”를 수치와 코드로 설명할 수 있게 만들기

---

## Quick Start

### C# (Console)
- 경로: `csharp/weekXX/`
- 실행: `dotnet run`

### C++ (CMake)
- 경로: `cpp/weekXX/`
- 빌드/실행(예시):
  - `cmake -S . -B build`
  - `cmake --build build`
  - `./build/app` (환경에 따라 경로 상이)

> 각 주차 폴더의 `README.md` 또는 `notes.md`에 실행/결과가 정리되어 있습니다.

---

## Repository Structure

```txt
systems-playground/
  csharp/
    week01/
    week02/
  cpp/
    week01/
    week02/
  notes/
    week01.md
    week02.md
