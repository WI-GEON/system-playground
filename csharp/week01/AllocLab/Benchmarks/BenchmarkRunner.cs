using System.Diagnostics;

namespace AllocLab.Benchmarks
{
    internal static class BenchmarkRunner
    {
        public static void Run(string title, Action action, BenchmarkOptions options)
        {
            Console.WriteLine();
            Console.WriteLine($"--- {title} ---");

            for (int i = 0; i < options.WarmupRuns; i++)
                action();

            var results = new List<BenchmarkResult>(options.MeasureRuns);

            for (int i = 1; i <= options.MeasureRuns; i++)
            {
                if (options.ForceGcBeforeEachRun)
                    ForceFullGC();

                int g0Before = GC.CollectionCount(0);
                int g1Before = GC.CollectionCount(1);
                int g2Before = GC.CollectionCount(2);

                long heapBefore = GC.GetTotalMemory(forceFullCollection: false);
                long allocBefore = GC.GetAllocatedBytesForCurrentThread();

                long start = Stopwatch.GetTimestamp();
                action();
                long end = Stopwatch.GetTimestamp();

                long allocAfter = GC.GetAllocatedBytesForCurrentThread();
                long heapAfter = GC.GetTotalMemory(forceFullCollection: false);

                int g0After = GC.CollectionCount(0);
                int g1After = GC.CollectionCount(1);
                int g2After = GC.CollectionCount(2);

                double elapsedMs = (end - start) * 1000.0 / Stopwatch.Frequency;

                var r = new BenchmarkResult(
                    ElapsedMs: elapsedMs,
                    AllocatedBytes: allocAfter - allocBefore,
                    Gen0: g0After - g0Before,
                    Gen1: g1After - g1Before,
                    Gen2: g2After - g2Before,
                    HeapDeltaBytes: heapAfter - heapBefore
                );

                results.Add(r);

                Console.WriteLine(
                    $"Run #{i}: {r.ElapsedMs,8:0.###} ms | " +
                    $"Alloc: {FormatUtil.Bytes(r.AllocatedBytes),10} | " +
                    $"GC(0/1/2): {r.Gen0}/{r.Gen1}/{r.Gen2} | " +
                    $"HeapΔ: {FormatUtil.Bytes(r.HeapDeltaBytes)}");
            }

            PrintSummary(results);
        }

        private static void PrintSummary(List<BenchmarkResult> results)
        {
            double medianMs = Median(results.Select(r => r.ElapsedMs));
            long medianAlloc = (long)Median(results.Select(r => (double)r.AllocatedBytes));
            int sumG0 = results.Sum(r => r.Gen0);
            int sumG1 = results.Sum(r => r.Gen1);
            int sumG2 = results.Sum(r => r.Gen2);

            Console.WriteLine($"Summary: median {medianMs:0.###} ms | median alloc {FormatUtil.Bytes(medianAlloc)} | total GC(0/1/2) {sumG0}/{sumG1}/{sumG2}");
        }

        private static double Median(IEnumerable<double> values)
        {
            var arr = values.OrderBy(x => x).ToArray();
            if (arr.Length == 0) return 0;

            int mid = arr.Length / 2;
            if (arr.Length % 2 == 1) return arr[mid];
            return (arr[mid - 1] + arr[mid]) / 2.0;
        }

        private static void ForceFullGC()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }
}