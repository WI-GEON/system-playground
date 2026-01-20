namespace AllocLab.Benchmarks
{
    internal static class FormatUtil
    {
        public static string Bytes(long bytes)
        {
            double b = bytes;
            string[] units = { "B", "KiB", "MiB", "GiB" };
            int u = 0;

            while (Math.Abs(b) >= 1024 && u < units.Length - 1)
            {
                b /= 1024;
                u++;
            }

            return $"{b:0.##} {units[u]}";
        }
    }
}