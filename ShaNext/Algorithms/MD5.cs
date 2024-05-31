using System;
using System.Linq;
using System.Text;

namespace ShaNext.ShaNext
{
    public class MD5 : IHashAlgorithm
    {
        private const int BlockSize = 64;
        private const int HashSize = 16; 

        private static readonly uint[] S = {
            7, 12, 17, 22, 7, 12, 17, 22, 7, 12, 17, 22, 7, 12, 17, 22,
            5, 9, 14, 20, 5, 9, 14, 20, 5, 9, 14, 20, 5, 9, 14, 20,
            4, 11, 16, 23, 4, 11, 16, 23, 4, 11, 16, 23, 4, 11, 16, 23,
            6, 10, 15, 21, 6, 10, 15, 21, 6, 10, 15, 21, 6, 10, 15, 21
        };

        private static readonly uint[] K = {
            0xd76aa478, 0xe8c7b756, 0x242070db, 0xc1bdceee,
            0xf57c0faf, 0x4787c62a, 0xa8304613, 0xfd469501,
            0x698098d8, 0x8b44f7af, 0xffff5bb1, 0x895cd7be,
            0x6b901122, 0xfd987193, 0xa679438e, 0x49b40821,
            0xf61e2562, 0xc040b340, 0x265e5a51, 0xe9b6c7aa,
            0xd62f105d, 0x02441453, 0xd8a1e681, 0xe7d3fbc8,
            0x21e1cde6, 0xc33707d6, 0xf4d50d87, 0x455a14ed,
            0xa9e3e905, 0xfcefa3f8, 0x676f02d9, 0x8d2a4c8a,
            0xfffa3942, 0x8771f681, 0x6d9d6122, 0xfde5380c,
            0xa4beea44, 0x4bdecfa9, 0xf6bb4b60, 0xbebfbc70,
            0x289b7ec6, 0xeaa127fa, 0xd4ef3085, 0x04881d05,
            0xd9d4d039, 0xe6db99e5, 0x1fa27cf8, 0xc4ac5665,
            0xf4292244, 0x432aff97, 0xab9423a7, 0xfc93a039,
            0x655b59c3, 0x8f0ccc92, 0xffeff47d, 0x85845dd1,
            0x6fa87e4f, 0xfe2ce6e0, 0xa3014314, 0x4e0811a1,
            0xf7537e82, 0xbd3af235, 0x2ad7d2bb, 0xeb86d391
        };

        public static byte[] ShaNext_MD5(string input)
        {
            uint[] H = {
                0x67452301, 0xefcdab89, 0x98badcfe, 0x10325476
            };

            byte[] paddedMessage = PadMessage(Encoding.UTF8.GetBytes(input));
            int blockCount = paddedMessage.Length / BlockSize;

            for (int i = 0; i < blockCount; i++)
            {
                uint[] M = new uint[16];
                for (int j = 0; j < 16; j++)
                {
                    M[j] = BitConverter.ToUInt32(paddedMessage, i * BlockSize + j * 4);
                }

                uint a = H[0];
                uint b = H[1];
                uint c = H[2];
                uint d = H[3];

                for (int j = 0; j < 64; j++)
                {
                    uint F, g;
                    if (j < 16)
                    {
                        F = (b & c) | (~b & d);
                        g = (uint)j;
                    }
                    else if (j < 32)
                    {
                        F = (d & b) | (~d & c);
                        g = (uint)((5 * j + 1) % 16);
                    }
                    else if (j < 48)
                    {
                        F = b ^ c ^ d;
                        g = (uint)((3 * j + 5) % 16);
                    }
                    else
                    {
                        F = c ^ (b | ~d);
                        g = (uint)((7 * j) % 16);
                    }

                    F = F + a + K[j] + M[g];
                    a = d;
                    d = c;
                    c = b;
                    b = b + LeftRotate(F, (int)S[j]);
                }

                H[0] += a;
                H[1] += b;
                H[2] += c;
                H[3] += d;
            }

            byte[] hash = new byte[HashSize];
            for (int i = 0; i < 4; i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(H[i]), 0, hash, i * 4, 4);
            }

            return hash;
        }

        private static byte[] PadMessage(byte[] message)
        {
            ulong bitLength = (ulong)message.Length * 8;
            int padLength = (message.Length % BlockSize < 56) ? (56 - message.Length % BlockSize) : (120 - message.Length % BlockSize);
            byte[] paddedMessage = new byte[message.Length + padLength + 8];
            Buffer.BlockCopy(message, 0, paddedMessage, 0, message.Length);
            paddedMessage[message.Length] = 0x80;
            for (int i = paddedMessage.Length - 8; i < paddedMessage.Length; i++)
            {
                paddedMessage[i] = 0x00;
            }
            Buffer.BlockCopy(BitConverter.GetBytes(bitLength), 0, paddedMessage, paddedMessage.Length - 8, 8);
            return paddedMessage;
        }

        private static uint LeftRotate(uint x, int n)
        {
            return (x << n) | (x >> (32 - n));
        }

        public byte[] ComputeHash(string input)
        {
            return ShaNext_MD5(input);
        }
    }
}
