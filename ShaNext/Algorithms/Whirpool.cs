using System;
using System.Text;

namespace ShaNext.ShaNext
{
    public class Whirlpool : IHashAlgorithm
    {
        private const int HashSize = 64;
        private const int BlockSize = 64;

        private static readonly ulong[,] C = new ulong[8, 256];
        private static readonly ulong[] rc = new ulong[10];

        static Whirlpool()
        {
            Initialize();
        }

        public static void Initialize()
        {
            ulong[] S = {
                0x18, 0x23, 0xC6, 0xE8, 0x87, 0xB8, 0x01, 0x4F,
                0x36, 0xA6, 0xD2, 0xF5, 0x79, 0x6F, 0x91, 0x52,
                0x60, 0xBC, 0x9B, 0x8E, 0xA3, 0x0C, 0x7B, 0x35,
                0x1D, 0xE0, 0xD7, 0xC2, 0x2E, 0x4B, 0xFE, 0x57,
                0x15, 0x77, 0x37, 0xE5, 0x9F, 0xF0, 0x4A, 0xDA,
                0x58, 0xC9, 0x29, 0x0A, 0xB1, 0xA0, 0x6B, 0x85,
                0xBD, 0x5D, 0x10, 0xF4, 0xCB, 0x3E, 0x05, 0x67,
                0xE4, 0x27, 0x41, 0x8B, 0xA7, 0x7D, 0x95, 0xD8,
                0xFB, 0xEE, 0x7C, 0x66, 0xDD, 0x17, 0x47, 0x9E,
                0xCA, 0x2D, 0xBF, 0x07, 0xAD, 0x5A, 0x83, 0x33,
                0x63, 0x02, 0xAA, 0x71, 0xC8, 0x19, 0x49, 0xD9,
                0xF2, 0xE3, 0x5B, 0x88, 0x9A, 0x26, 0x32, 0xB0,
                0xE9, 0x0F, 0xD5, 0x80, 0xBE, 0xCD, 0x34, 0x48,
                0xFF, 0x7A, 0x90, 0x5F, 0x20, 0x68, 0x1A, 0xAE,
                0xB4, 0x54, 0x93, 0x22, 0x64, 0xF1, 0x73, 0x12,
                0x40, 0x08, 0xC3, 0xEC, 0xDB, 0xA1, 0x8D, 0x3D,
                0x97, 0x00, 0xCF, 0x2B, 0x76, 0x82, 0xD6, 0x1B,
                0xB5, 0xAF, 0x6A, 0x50, 0x45, 0xF3, 0x30, 0xEF,
                0x3F, 0x55, 0xA2, 0xEA, 0x65, 0xBA, 0x2F, 0xC0,
                0xDE, 0x1C, 0xFD, 0x4D, 0x92, 0x75, 0x06, 0x8A,
                0xB2, 0xE6, 0x0E, 0x1F, 0x62, 0xD4, 0xA8, 0x96,
                0xF9, 0xC5, 0x25, 0x59, 0x84, 0x72, 0x39, 0x4C,
                0x5E, 0x78, 0x38, 0x8C, 0xD1, 0xA5, 0xE2, 0x61,
                0xB3, 0x21, 0x9C, 0x1E, 0x43, 0xC7, 0xFC, 0x04,
                0x51, 0x99, 0x6D, 0x0D, 0xFA, 0xDF, 0x7E, 0x24,
                0x3B, 0xAB, 0xCE, 0x11, 0x8F, 0x4E, 0xB7, 0xEB,
                0x3C, 0x81, 0x94, 0xF7, 0xB9, 0x13, 0x2C, 0xD3,
                0xE7, 0x6E, 0xC4, 0x03, 0x56, 0x44, 0x7F, 0xA9,
                0x2A, 0xBB, 0xC1, 0x53, 0xDC, 0x0B, 0x9D, 0x6C,
                0x31, 0x74, 0xF6, 0x46, 0xAC, 0x89, 0x14, 0xE1,
                0x16, 0x3A, 0x69, 0x09, 0x70, 0xB6, 0xD0, 0xED,
                0xCC, 0x42, 0x98, 0xA4, 0x28, 0x5C, 0xF8, 0x86
            };

            for (int x = 0; x < 256; x++)
            {
                ulong v1 = S[x];
                ulong v2 = Rot(v1, 8);
                ulong v3 = Rot(v1, 16);
                ulong v4 = Rot(v1, 24);
                ulong v5 = v1 ^ v3;
                ulong v6 = v2 ^ v4;
                ulong v7 = Rot(v5, 32);

                C[0, x] = v1 << 56 | v2 << 48 | v3 << 40 | v4 << 32 | v5 << 24 | v6 << 16 | v7 << 8 | v5;
                C[1, x] = v2 << 56 | v3 << 48 | v4 << 40 | v5 << 32 | v6 << 24 | v7 << 16 | v5 << 8 | v1;
                C[2, x] = v3 << 56 | v4 << 48 | v5 << 40 | v6 << 32 | v7 << 24 | v5 << 16 | v1 << 8 | v2;
                C[3, x] = v4 << 56 | v5 << 48 | v6 << 40 | v7 << 32 | v5 << 24 | v1 << 16 | v2 << 8 | v3;
                C[4, x] = v5 << 56 | v6 << 48 | v7 << 40 | v5 << 32 | v1 << 24 | v2 << 16 | v3 << 8 | v4;
                C[5, x] = v6 << 56 | v7 << 48 | v5 << 40 | v1 << 32 | v2 << 24 | v3 << 16 | v4 << 8 | v5;
                C[6, x] = v7 << 56 | v5 << 48 | v1 << 40 | v2 << 32 | v3 << 24 | v4 << 16 | v5 << 8 | v6;
                C[7, x] = v5 << 56 | v1 << 48 | v2 << 40 | v3 << 32 | v4 << 24 | v5 << 16 | v6 << 8 | v7;
            }

            rc[0] = 0x1823C6E887B8014F;
            for (int r = 1; r < 10; r++)
            {
                rc[r] = (ulong)((rc[r - 1] >> 8) ^ C[0, (byte)rc[r - 1]]);
            }
        }

        public static ulong Rot(ulong x, int s)
        {
            return (x << s) | (x >> (64 - s));
        }

        public static byte[] PadMessage(byte[] message)
        {
            ulong bitLength = (ulong)message.Length * 8;
            int padLength = (message.Length % BlockSize < 32) ? (32 - message.Length % BlockSize) : (96 - message.Length % BlockSize);
            byte[] paddedMessage = new byte[message.Length + padLength + 32];
            Buffer.BlockCopy(message, 0, paddedMessage, 0, message.Length);
            paddedMessage[message.Length] = 0x80;
            for (int i = paddedMessage.Length - 32; i < paddedMessage.Length - 16; i++)
            {
                paddedMessage[i] = 0x00;
            }
            Buffer.BlockCopy(BitConverter.GetBytes(ReverseBytes(bitLength)), 0, paddedMessage, paddedMessage.Length - 16, 8);
            return paddedMessage;
        }

        public static ulong ReverseBytes(ulong x)
        {
            return ((x & 0x00000000000000FFUL) << 56) |
                   ((x & 0x000000000000FF00UL) << 40) |
                   ((x & 0x0000000000FF0000UL) << 24) |
                   ((x & 0x00000000FF000000UL) << 8)  |
                   ((x & 0x000000FF00000000UL) >> 8)  |
                   ((x & 0x0000FF0000000000UL) >> 24) |
                   ((x & 0x00FF000000000000UL) >> 40) |
                   ((x & 0xFF00000000000000UL) >> 56);
        }

        public static byte[] ComputeHash(byte[] input)
        {
            byte[] paddedMessage = PadMessage(input);
            ulong[] H = new ulong[8];

            for (int i = 0; i < paddedMessage.Length / BlockSize; i++)
            {
                ulong[] K = new ulong[8];
                ulong[] block = new ulong[8];
                ulong[] state = new ulong[8];

                for (int j = 0; j < 8; j++)
                {
                    state[j] = H[j] ^ BitConverter.ToUInt64(paddedMessage, (i * BlockSize) + (j * 8));
                    K[j] = state[j];
                }

                for (int r = 0; r < 10; r++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        block[j] = 0;
                        block[j] ^= C[0, (byte)(K[j] >> 56)];
                        block[j] ^= C[1, (byte)(K[(j + 7) % 8] >> 48)];
                        block[j] ^= C[2, (byte)(K[(j + 6) % 8] >> 40)];
                        block[j] ^= C[3, (byte)(K[(j + 5) % 8] >> 32)];
                        block[j] ^= C[4, (byte)(K[(j + 4) % 8] >> 24)];
                        block[j] ^= C[5, (byte)(K[(j + 3) % 8] >> 16)];
                        block[j] ^= C[6, (byte)(K[(j + 2) % 8] >> 8)];
                        block[j] ^= C[7, (byte)(K[(j + 1) % 8])];
                    }

                    for (int j = 0; j < 8; j++)
                    {
                        K[j] = block[j];
                    }

                    K[0] ^= rc[r];
                }

                for (int j = 0; j < 8; j++)
                {
                    H[j] ^= K[j] ^ state[j];
                }
            }

            byte[] hash = new byte[HashSize];
            for (int i = 0; i < 8; i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(ReverseBytes(H[i])), 0, hash, i * 8, 8);
            }
            return hash;
        }

        public byte[] ComputeHash(string input)
        {
            return ComputeHash(Encoding.UTF8.GetBytes(input));
        }
    }
}
