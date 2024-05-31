using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ShaNext.ShaNext
{
    public static class ShaNextPBKDF2
    {
        public static byte[] GenerateKey(string password, byte[] salt, int iterations, int keyLength)
        {
            using var rfc2898 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            return rfc2898.GetBytes(keyLength);
        }

        public static bool VerifyKey(string password, byte[] salt, int iterations, byte[] expectedKey)
        {
            byte[] generatedKey = GenerateKey(password, salt, iterations, expectedKey.Length);
            return ShaNextUtilities.TimeSafeCompare(Convert.ToBase64String(generatedKey), Convert.ToBase64String(expectedKey));
        }

        public static async Task<byte[]> GenerateKeyAsync(string password, byte[] salt, int iterations, int keyLength)
        {
            return await Task.Run(() => GenerateKey(password, salt, iterations, keyLength));
        }

        public static async Task<bool> VerifyKeyAsync(string password, byte[] salt, int iterations, byte[] expectedKey)
        {
            return await Task.Run(() => VerifyKey(password, salt, iterations, expectedKey));
        }
    }
}
