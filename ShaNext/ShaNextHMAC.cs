using System.Security.Cryptography;
using System.Text;

namespace ShaNext.ShaNext
{
    public static class ShaNextHMAC
    {
        public static string GenerateHMAC(string key, string data, HashAlgorithmName hashAlgorithmName)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        public static bool VerifyHMAC(string key, string data, string hmac, HashAlgorithmName hashAlgorithmName)
        {
            string generatedHmac = GenerateHMAC(key, data, hashAlgorithmName);
            return ShaNextUtilities.TimeSafeCompare(generatedHmac, hmac);
        }

        public static async Task<string> GenerateHMACAsync(string key, string data, HashAlgorithmName hashAlgorithmName)
        {
            return await Task.Run(() => GenerateHMAC(key, data, hashAlgorithmName));
        }

        public static async Task<bool> VerifyHMACAsync(string key, string data, string hmac, HashAlgorithmName hashAlgorithmName)
        {
            return await Task.Run(() => VerifyHMAC(key, data, hmac, hashAlgorithmName));
        }
    }
}
