using System.Diagnostics;

namespace AllocLab
{
    internal static class Program
    {
        private static void Main()
        {
            // Console.WriteLine("=== AllocLab (Week01 v2) ===");
            Console.WriteLine($".NET: {Environment.Version}");
            Console.WriteLine($"64-bit Process: {Environment.Is64BitProcess}");
            Console.WriteLine($"Stopwatch Frequency: {Stopwatch.Frequency} ticks/sec");
            Console.WriteLine();

            BenchmarkSuite.Run();

            Console.WriteLine();
            Console.WriteLine("Done.");
        }
    }
}