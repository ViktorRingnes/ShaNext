using System.Text;

namespace ShaNext.ShaNext
{
    public static class ShaNextBase64
    {
        public static string Encode(byte[] data)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("Data cannot be null or empty");

            return Convert.ToBase64String(data);
        }

        public static byte[] Decode(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
                throw new ArgumentException("Base64 string cannot be null or empty");

            return Convert.FromBase64String(base64String);
        }

        public static string EncodeString(string data)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentException("Data cannot be null or empty");

            byte[] bytes = Encoding.UTF8.GetBytes(data);
            return Convert.ToBase64String(bytes);
        }

        public static string DecodeToString(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
                throw new ArgumentException("Base64 string cannot be null or empty");

            byte[] bytes = Convert.FromBase64String(base64String);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
