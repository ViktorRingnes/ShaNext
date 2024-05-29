using System;

namespace ShaNext.ShaNext
{
    public class ShaNextCompare
    {
        public static bool Compare(string input, string hash)
        {
            string hashedInput = ShaNextHashing.Hash(input);

            return SecureEquals(hashedInput, hash);
        }

        private static bool SecureEquals(string a, string b)
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

        public static bool CompareSaltedHash(string input, string salt, string hash)
        {
            string hashedInput = ShaNextHashing.HashWithSalt(input, salt);
            return SecureEquals(hashedInput, hash);
        }
    }


}
