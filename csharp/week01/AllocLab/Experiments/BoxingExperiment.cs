using System.Collections;

namespace AllocLab.Experiments
{
    internal static class BoxingExperiment
    {
        private const int N = 1_000_000;
        private static long _sink;

        public static void ArrayListBoxing()
        {
            var arr = new ArrayList { Capacity = N };
            for (int i = 0; i < N; i++)
                arr.Add(i);

            long sum = 0;
            for (int i = 0; i < arr.Count; i++)
                sum += (int)arr[i];

            _sink = sum;
        }

        public static void GenericListNoBoxing()
        {
            var list = new List<int>(capacity: N);
            for (int i = 0; i < N; i++)
                list.Add(i);

            long sum = 0;
            for (int i = 0; i < list.Count; i++)
                sum += list[i];

            _sink = sum;
        }
    }
}
