namespace AllocLab.Benchmarks
{
    internal readonly record struct BenchmarkOptions(
        int WarmupRuns,
        int MeasureRuns,
        bool ForceGcBeforeEachRun);
}