using System;
using System.Security.Cryptography;

namespace ShaNext.ShaNext
{
    public class ShaNextSalt
    {
        private const int SaltSize = 64;  

        public static string NewSalt(string providedSalt = null)
        {
            if (!string.IsNullOrEmpty(providedSalt))
            {
                return providedSalt;
            }

            byte[] saltBytes = new byte[SaltSize];
            RandomNumberGenerator.Fill(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        public static string NewFixedLengthSalt(int length)
        {
            byte[] saltBytes = new byte[length];
            RandomNumberGenerator.Fill(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }
    }
}
