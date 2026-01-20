namespace AllocLab.Benchmarks
{
    internal readonly record struct BenchmarkResult(
        double ElapsedMs,
        long AllocatedBytes,
        int Gen0,
        int Gen1,
        int Gen2,
        long HeapDeltaBytes);
}