namespace AllocLab.Experiments
{
    internal static class StructClassExperiment
    {
        private const int N = 1_000_000;
        private static double _sink;

        private readonly struct SmallStruct
        {
            public readonly float X, Y, Z;
            public SmallStruct(float x, float y, float z) { X = x; Y = y; Z = z; }
        }

        private sealed class SmallClass
        {
            public readonly float X, Y, Z;
            public SmallClass(float x, float y, float z) { X = x; Y = y; Z = z; }
        }

        public static void StructListAddAndSum()
        {
            var list = new List<SmallStruct>(capacity: N);
            for (int i = 0; i < N; i++)
                list.Add(new SmallStruct(i, i + 1, i + 2));

            double sum = 0;
            for (int i = 0; i < list.Count; i++)
            {
                var v = list[i];
                sum += v.X + v.Y + v.Z;
            }

            _sink = sum;
        }

        public static void ClassListAddAndSum()
        {
            var list = new List<SmallClass>(capacity: N);
            for (int i = 0; i < N; i++)
                list.Add(new SmallClass(i, i + 1, i + 2));

            double sum = 0;
            for (int i = 0; i < list.Count; i++)
            {
                var v = list[i];
                sum += v.X + v.Y + v.Z;
            }

            _sink = sum;
        }
    }
}
