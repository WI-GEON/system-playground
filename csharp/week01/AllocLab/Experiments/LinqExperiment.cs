namespace AllocLab.Experiments
{
    internal static class LinqExperiment
    {
        private const int N = 100_000;
        private static int[] _data = Array.Empty<int>();
        private static long _sink;

        public static void Initialize()
        {
            // _data = Enumerable.Range(0, N).ToArray();
            _data = [.. Enumerable.Range(0, N)];
        }

        public static void LinqPipeline()
        {
            var list = _data
                .Where(x => (x & 1) == 0)
                .Select(x => x * 2)
                .ToList();

            long sum = 0;
            for (int i = 0; i < list.Count; i++)
                sum += list[i];

            _sink = sum;
        }

        public static void ManualLoop()
        {
            long sum = 0;
            for (int i = 0; i < _data.Length; i++)
            {
                int x = _data[i];
                if ((x & 1) == 0)
                    sum += x * 2;
            }

            _sink = sum;
        }
    }
}
