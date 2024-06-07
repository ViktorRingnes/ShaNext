using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace ShaNext.ShaNext
{
    public static partial class ShaNextUtilities
    {
        private const int MinimumLength = 16;
        private const int EntropyBits = 100;
        private static readonly char[] Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private static readonly char[] Lowercase = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        private static readonly char[] Digits = "0123456789".ToCharArray();
        private static readonly char[] Special = "!@#$%^&*()-_=+[]{}|;:',.<>/?".ToCharArray();
        private static readonly char[] AllChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+[]{}|;:',.<>/?".ToCharArray();
        private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();
        private static readonly List<string> PasswordHistory = new List<string>();

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

        public static string GenerateRandomString(int length)
        {
            using var rng = RandomNumberGenerator.Create();

            var passwordChars = new char[length];
            byte[] randomBytes = new byte[length];

            rng.GetBytes(randomBytes);

            for (int i = 0; i < length; i++)
            {
                int charIndex = randomBytes[i] % AllChars.Length;
                passwordChars[i] = AllChars[charIndex];
            }
            return new string(passwordChars);
        }

        public static string GeneratePassword(int length)
        {
            if (length < MinimumLength)
            {
                throw new ArgumentException($"Password length must be at least {MinimumLength} characters.");
            }

            while (true)
            {
                var passwordChars = new char[length];
                var randomBytes = new byte[length];

                Rng.GetBytes(randomBytes);

                passwordChars[0] = Uppercase[randomBytes[0] % Uppercase.Length];
                passwordChars[1] = Lowercase[randomBytes[1] % Lowercase.Length];
                passwordChars[2] = Digits[randomBytes[2] % Digits.Length];
                passwordChars[3] = Special[randomBytes[3] % Special.Length];

                for (int i = 4; i < length; i++)
                {
                    passwordChars[i] = AllChars[randomBytes[i] % AllChars.Length];
                }

                Shuffle(passwordChars);

                string password = new(passwordChars);

                if (IsValidPassword(password))
                {
                    if (CalculateEntropy(password) >= EntropyBits)
                    {
                        PasswordHistory.Add(password);
                        if (PasswordHistory.Count > 12)
                        {
                            PasswordHistory.RemoveAt(0);
                        }
                        return password;
                    }
                }
            }
        }

        private static void Shuffle(char[] array)
        {
            byte[] randomBytes = new byte[array.Length];
            Rng.GetBytes(randomBytes);

            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = randomBytes[i] % (i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }
        }

        private static bool IsValidPassword(string password)
        {
            if (RepeatedCharactersRegex().IsMatch(password) ||
                CommonPatternsRegex().IsMatch(password))
            {
                return false;
            }

            if (!UppercaseRegex().IsMatch(password) ||
                !LowercaseRegex().IsMatch(password) ||
                !DigitRegex().IsMatch(password) ||
                !SpecialCharacterRegex().IsMatch(password))
            {
                return false;
            }

            return true;
        }

        private static double CalculateEntropy(string password)
        {
            double entropyPerChar = Math.Log2(95);
            return entropyPerChar * password.Length;
        }

        [GeneratedRegex(@"(.)\1\1")]
        private static partial Regex RepeatedCharactersRegex();
        [GeneratedRegex(@"(qwerty|asdfgh|123456|password)", RegexOptions.IgnoreCase, "en-US")]
        private static partial Regex CommonPatternsRegex();
        [GeneratedRegex(@"[A-Z]")]
        private static partial Regex UppercaseRegex();
        [GeneratedRegex(@"[a-z]")]
        private static partial Regex LowercaseRegex();
        [GeneratedRegex(@"\d")]
        private static partial Regex DigitRegex();
        [GeneratedRegex(@"[!@#$%^&*()\-_=+\[\]{}|;:',.<>/?]")]
        private static partial Regex SpecialCharacterRegex();
    }
}
