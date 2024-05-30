namespace ShaNext.ShaNext
{
    public static class ShaNextUtilities
    {
        public static bool TimeSafeCompare(string a, string b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            bool result = true;
            for (int i = 0; i < a.Length; i++)
            {
                result &= (a[i] == b[i]);
            }

            return result;
        }
    }
}
