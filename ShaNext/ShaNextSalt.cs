using System;
using System.Security.Cryptography;

namespace ShaNext.ShaNext
{
    public class ShaNextSalt
    {
        private const int SaltSize = 64;  

        public static string Salt(string providedSalt = null)
        {
            if (!string.IsNullOrEmpty(providedSalt))
            {
                return providedSalt;
            }

            byte[] saltBytes = new byte[SaltSize];
            RandomNumberGenerator.Fill(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }
    }
}
