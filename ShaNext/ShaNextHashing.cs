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

            byte[] paddedMessage = ShaNextUtilities.PadMessage(Encoding.UTF8.GetBytes(input));
            int blockCount = paddedMessage.Length / BlockSize;

            for (int i = 0; i < blockCount; i++)
            {
                uint[] W = new uint[MessageScheduleSize];
                for (int j = 0; j < 16; j++)
                {
                    W[j] = BitConverter.ToUInt32(paddedMessage, i * BlockSize + j * 4);
                    W[j] = ShaNextUtilities.ReverseBytes(W[j]);
                }

                for (int t = 16; t < MessageScheduleSize; t++)
                {
                    uint s0 = ShaNextUtilities.RightRotate(W[t - 15], 7) ^ ShaNextUtilities.RightRotate(W[t - 15], 18) ^ (W[t - 15] >> 3);
                    uint s1 = ShaNextUtilities.RightRotate(W[t - 2], 17) ^ ShaNextUtilities.RightRotate(W[t - 2], 19) ^ (W[t - 2] >> 10);
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
                    uint Σ1 = ShaNextUtilities.RightRotate(e, 6) ^ ShaNextUtilities.RightRotate(e, 11) ^ ShaNextUtilities.RightRotate(e, 25);
                    uint Ch = (e & f) ^ ((~e) & g);
                    uint temp1 = h + Σ1 + Ch + K[t] + W[t];
                    uint Σ0 = ShaNextUtilities.RightRotate(a, 2) ^ ShaNextUtilities.RightRotate(a, 13) ^ ShaNextUtilities.RightRotate(a, 22);
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
                Buffer.BlockCopy(BitConverter.GetBytes(ShaNextUtilities.ReverseBytes(H[i])), 0, hash, i * 4, 4);
            }

            return hash;
        }
    }
}