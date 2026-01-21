using AllocLab.Benchmarks;
using AllocLab.Experiments;

namespace AllocLab
{
    internal static class BenchmarkSuite
    {
        public static void Run()
        {
            var opts = new BenchmarkOptions(
                WarmupRuns: 1,
                MeasureRuns: 3,
                ForceGcBeforeEachRun: true);

            LinqExperiment.Initialize();

            BenchmarkRunner.Run("Exp1A: struct-only (List add + sum)", StructClassExperiment.StructListAddAndSum, opts);
            BenchmarkRunner.Run("Exp1B: class-only  (List add + sum)", StructClassExperiment.ClassListAddAndSum, opts);

            BenchmarkRunner.Run("Exp2A: ArrayList<int> boxing/unboxing", BoxingExperiment.ArrayListBoxing, opts);
            BenchmarkRunner.Run("Exp2B: List<int> no boxing", BoxingExperiment.GenericListNoBoxing, opts);

            BenchmarkRunner.Run("Exp3A: LINQ Where/Select/ToList", LinqExperiment.LinqPipeline, opts);
            BenchmarkRunner.Run("Exp3B: manual loop (no extra collection)", LinqExperiment.ManualLoop, opts);
        }
    }
}
