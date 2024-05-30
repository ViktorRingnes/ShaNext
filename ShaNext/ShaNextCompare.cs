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

        public static bool CompareWithSalt(string input, string salt, string hash)
        {
            string hashedInput = ShaNextHashing.HashWithSalt(input, salt);
            return SecureEquals(hashedInput, hash);
        }

        public static bool VerifySaltedHash(string input, string storedHash)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(storedHash))
                throw new ArgumentException("Input and stored hash cannot be null or empty");

            string[] parts = storedHash.Split(':');
            if (parts.Length != 2)
                throw new ArgumentException("Stored hash is not in the correct format");

            string hash = parts[0];
            string salt = parts[1];
            string newHash = ShaNextHashing.HashWithSalt(input, salt);

            return SecureEquals(hash, newHash);
        }

        public static bool VerifyFileHash(string filePath, string expectedHash)
        {
            string fileHash = ShaNextHashing.HashFile(filePath);
            return SecureEquals(fileHash, expectedHash);
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
    }
}
