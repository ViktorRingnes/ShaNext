using System;
using System.Security.Cryptography;
using System.Text;

namespace ShaNext.ShaNext
{
    public class ShaNextHashing
    {
        private const int HashSize = 32;
        private const int BlockSize = 64;
        private const int MessageScheduleSize = 64;

        public static string Hash(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("Input cannot be null or empty");

            byte[] hash = SHA_Next(input);
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

        public static byte[] SHA_Next(string input)
        {
            uint[] H = {
                0x6a09e667, 0xbb67ae85, 0x3c6ef372, 0xa54ff53a,
                0x510e527f, 0x9b05688c, 0x1f83d9ab, 0x5be0cd19
            };

            uint[] K = {
                0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5,
                0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
                0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3,
                0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
                0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc,
                0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da,
                0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7,
                0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967,
                0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13,
                0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
                0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3,
                0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
                0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5,
                0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3,
                0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208,
                0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2
            };

            byte[] paddedMessage = PadMessage(Encoding.UTF8.GetBytes(input));
            int blockCount = paddedMessage.Length / BlockSize;

            for (int i = 0; i < blockCount; i++)
            {
                uint[] W = new uint[MessageScheduleSize];
                for (int j = 0; j < 16; j++)
                {
                    W[j] = BitConverter.ToUInt32(paddedMessage, i * BlockSize + j * 4);
                    W[j] = ReverseBytes(W[j]);
                }

                for (int t = 16; t < MessageScheduleSize; t++)
                {
                    uint s0 = RightRotate(W[t - 15], 7) ^ RightRotate(W[t - 15], 18) ^ (W[t - 15] >> 3);
                    uint s1 = RightRotate(W[t - 2], 17) ^ RightRotate(W[t - 2], 19) ^ (W[t - 2] >> 10);
                    W[t] = W[t - 16] + s0 + W[t - 7] + s1;
                }

                uint a = H[0];
                uint b = H[1];
                uint c = H[2];
                uint d = H[3];
                uint e = H[4];
                uint f = H[5];
                uint g = H[6];
                uint h = H[7];

                for (int t = 0; t < MessageScheduleSize; t++)
                {
                    uint Σ1 = RightRotate(e, 6) ^ RightRotate(e, 11) ^ RightRotate(e, 25);
                    uint Ch = (e & f) ^ ((~e) & g);
                    uint temp1 = h + Σ1 + Ch + K[t] + W[t];
                    uint Σ0 = RightRotate(a, 2) ^ RightRotate(a, 13) ^ RightRotate(a, 22);
                    uint Maj = (a & b) ^ (a & c) ^ (b & c);
                    uint temp2 = Σ0 + Maj;

                    h = g;
                    g = f;
                    f = e;
                    e = d + temp1;
                    d = c;
                    c = b;
                    b = a;
                    a = temp1 + temp2;
                }

                H[0] += a;
                H[1] += b;
                H[2] += c;
                H[3] += d;
                H[4] += e;
                H[5] += f;
                H[6] += g;
                H[7] += h;
            }

            byte[] hash = new byte[HashSize];
            for (int i = 0; i < 8; i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(ReverseBytes(H[i])), 0, hash, i * 4, 4);
            }

            return hash;
        }

        public static byte[] PadMessage(byte[] message)
        {
            ulong bitLength = (ulong)message.Length * 8;
            int padLength = (message.Length % 64 < 56) ? (56 - message.Length % 64) : (120 - message.Length % 64);
            byte[] paddedMessage = new byte[message.Length + padLength + 8];
            Buffer.BlockCopy(message, 0, paddedMessage, 0, message.Length);
            paddedMessage[message.Length] = 0x80;
            for (int i = paddedMessage.Length - 8; i < paddedMessage.Length - 4; i++)
            {
                paddedMessage[i] = 0x00;
            }
            Buffer.BlockCopy(BitConverter.GetBytes(ReverseBytes(bitLength)), 0, paddedMessage, paddedMessage.Length - 8, 8);
            return paddedMessage;
        }

        public static uint RightRotate(uint x, int n)
        {
            return (x >> n) | (x << (32 - n));
        }

        public static ulong RightRotate(ulong x, int n)
        {
            return (x >> n) | (x << (64 - n));
        }

        public static uint ReverseBytes(uint x)
        {
            return ((x & 0x000000FFU) << 24) |
                   ((x & 0x0000FF00U) << 8) |
                   ((x & 0x00FF0000U) >> 8) |
                   ((x & 0xFF000000U) >> 24);
        }

        public static ulong ReverseBytes(ulong x)
        {
            return ((x & 0x00000000000000FFUL) << 56) |
                   ((x & 0x000000000000FF00UL) << 40) |
                   ((x & 0x0000000000FF0000UL) << 24) |
                   ((x & 0x00000000FF000000UL) << 8) |
                   ((x & 0x000000FF00000000UL) >> 8) |
                   ((x & 0x0000FF0000000000UL) >> 24) |
                   ((x & 0x00FF000000000000UL) >> 40) |
                   ((x & 0xFF00000000000000UL) >> 56);
        }
    }
}
