using System.Text;

namespace ShaNext.ShaNext
{
    public class ShaNextHashing
    {
        private static IHashAlgorithm hashAlgorithm = HashAlgorithmFactory.Create();

        public static string Hash(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("Input cannot be null or empty");

            byte[] hash = hashAlgorithm.ComputeHash(input);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        public static string HashWithSalt(string input, string salt)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(salt))
                throw new ArgumentException("Input and salt cannot be null or empty");

            string saltedInput = salt + input;
            return Hash(saltedInput);
        }

        public static string GenerateSaltedHash(string input)
        {
            string salt = ShaNextSalt.NewSalt();
            string hash = HashWithSalt(input, salt);
            return $"{hash}:{salt}";
        }

        public static string HashWithCustomIterations(string input, string salt, int iterations)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(salt))
                throw new ArgumentException("Input and salt cannot be null or empty");

            string hash = salt + input;
            for (int i = 0; i < iterations; i++)
            {
                hash = Hash(hash);
            }
            return hash;
        }

        public static string HashFile(string filePath)
        {
            using (Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return HashStream(stream);
            }
        }

        public static string HashStream(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                byte[] fileBytes = memoryStream.ToArray();
                string input = Encoding.UTF8.GetString(fileBytes);
                return Hash(input);
            }
        }

        public static async Task<string> HashAsync(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("Input cannot be null or empty");

            byte[] hash = await Task.Run(() => hashAlgorithm.ComputeHash(input));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        public static async Task<string> HashWithSaltAsync(string input, string salt)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(salt))
                throw new ArgumentException("Input and salt cannot be null or empty");

            string saltedInput = salt + input;
            return await HashAsync(saltedInput);
        }

        public static async Task<string> GenerateSaltedHashAsync(string input)
        {
            string salt = ShaNextSalt.NewSalt();
            string hash = await HashWithSaltAsync(input, salt);
            return $"{hash}:{salt}";
        }

        public static async Task<string> HashWithCustomIterationsAsync(string input, string salt, int iterations)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(salt))
                throw new ArgumentException("Input and salt cannot be null or empty");

            string hash = salt + input;
            for (int i = 0; i < iterations; i++)
            {
                hash = await HashAsync(hash);
            }
            return hash;
        }

        public static async Task<string> HashFileAsync(string filePath)
        {
            using Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return await HashStreamAsync(stream);
        }

        public static async Task<string> HashStreamAsync(Stream stream)
        {
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            byte[] fileBytes = memoryStream.ToArray();
            string input = Encoding.UTF8.GetString(fileBytes);
            return await HashAsync(input);
        }
    }
}
