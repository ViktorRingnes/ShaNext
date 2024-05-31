using System;
using System.Linq;
using System.Text;

namespace ShaNext.ShaNext
{
    public class SHA_512 : IHashAlgorithm
    {
        private const int HashSize = 64;
        private const int BlockSize = 128; 
        private const int MessageScheduleSize = 80;

        private static readonly ulong[] H0 = {
            0x6a09e667f3bcc908, 0xbb67ae8584caa73b, 0x3c6ef372fe94f82b, 0xa54ff53a5f1d36f1,
            0x510e527fade682d1, 0x9b05688c2b3e6c1f, 0x1f83d9abfb41bd6b, 0x5be0cd19137e2179
        };

        private static readonly ulong[] K = {
            0x428a2f98d728ae22, 0x7137449123ef65cd, 0xb5c0fbcfec4d3b2f, 0xe9b5dba58189dbbc,
            0x3956c25bf348b538, 0x59f111f1b605d019, 0x923f82a4af194f9b, 0xab1c5ed5da6d8118,
            0xd807aa98a3030242, 0x12835b0145706fbe, 0x243185be4ee4b28c, 0x550c7dc3d5ffb4e2,
            0x72be5d74f27b896f, 0x80deb1fe3b1696b1, 0x9bdc06a725c71235, 0xc19bf174cf692694,
            0xe49b69c19ef14ad2, 0xefbe4786384f25e3, 0x0fc19dc68b8cd5b5, 0x240ca1cc77ac9c65,
            0x2de92c6f592b0275, 0x4a7484aa6ea6e483, 0x5cb0a9dcbd41fbd4, 0x76f988da831153b5,
            0x983e5152ee66dfab, 0xa831c66d2db43210, 0xb00327c898fb213f, 0xbf597fc7beef0ee4,
            0xc6e00bf33da88fc2, 0xd5a79147930aa725, 0x06ca6351e003826f, 0x142929670a0e6e70,
            0x27b70a8546d22ffc, 0x2e1b21385c26c926, 0x4d2c6dfc5ac42aed, 0x53380d139d95b3df,
            0x650a73548baf63de, 0x766a0abb3c77b2a8, 0x81c2c92e47edaee6, 0x92722c851482353b,
            0xa2bfe8a14cf10364, 0xa81a664bbc423001, 0xc24b8b70d0f89791, 0xc76c51a30654be30,
            0xd192e819d6ef5218, 0xd69906245565a910, 0xf40e35855771202a, 0x106aa07032bbd1b8,
            0x19a4c116b8d2d0c8, 0x1e376c085141ab53, 0x2748774cdf8eeb99, 0x34b0bcb5e19b48a8,
            0x391c0cb3c5c95a63, 0x4ed8aa4ae3418acb, 0x5b9cca4f7763e373, 0x682e6ff3d6b2b8a3,
            0x748f82ee5defb2fc, 0x78a5636f43172f60, 0x84c87814a1f0ab72, 0x8cc702081a6439ec,
            0x90befffa23631e28, 0xa4506cebde82bde9, 0xbef9a3f7b2c67915, 0xc67178f2e372532b,
            0xca273eceea26619c, 0xd186b8c721c0c207, 0xeada7dd6cde0eb1e, 0xf57d4f7fee6ed178,
            0x06f067aa72176fba, 0x0a637dc5a2c898a6, 0x113f9804bef90dae, 0x1b710b35131c471b,
            0x28db77f523047d84, 0x32caab7b40c72493, 0x3c9ebe0a15c9bebc, 0x431d67c49c100d4c,
            0x4cc5d4becb3e42b6, 0x597f299cfc657e2a, 0x5fcb6fab3ad6faec, 0x6c44198c4a475817
        };

        public static byte[] ShaNext_512(string input)
        {
            ulong[] H = (ulong[])H0.Clone();
            byte[] paddedMessage = PadMessage(Encoding.UTF8.GetBytes(input));
            int blockCount = paddedMessage.Length / BlockSize;

            for (int i = 0; i < blockCount; i++)
            {
                ulong[] W = new ulong[MessageScheduleSize];
                for (int j = 0; j < 16; j++)
                {
                    W[j] = BitConverter.ToUInt64(paddedMessage, i * BlockSize + j * 8);
                    W[j] = ReverseBytes(W[j]);
                }

                for (int t = 16; t < MessageScheduleSize; t++)
                {
                    ulong s0 = RightRotate(W[t - 15], 1) ^ RightRotate(W[t - 15], 8) ^ (W[t - 15] >> 7);
                    ulong s1 = RightRotate(W[t - 2], 19) ^ RightRotate(W[t - 2], 61) ^ (W[t - 2] >> 6);
                    W[t] = W[t - 16] + s0 + W[t - 7] + s1;
                }

                ulong a = H[0];
                ulong b = H[1];
                ulong c = H[2];
                ulong d = H[3];
                ulong e = H[4];
                ulong f = H[5];
                ulong g = H[6];
                ulong h = H[7];

                for (int t = 0; t < MessageScheduleSize; t++)
                {
                    ulong Σ1 = RightRotate(e, 14) ^ RightRotate(e, 18) ^ RightRotate(e, 41);
                    ulong Ch = (e & f) ^ ((~e) & g);
                    ulong temp1 = h + Σ1 + Ch + K[t] + W[t];
                    ulong Σ0 = RightRotate(a, 28) ^ RightRotate(a, 34) ^ RightRotate(a, 39);
                    ulong Maj = (a & b) ^ (a & c) ^ (b & c);
                    ulong temp2 = Σ0 + Maj;

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
                Buffer.BlockCopy(BitConverter.GetBytes(ReverseBytes(H[i])), 0, hash, i * 8, 8);
            }

            return hash;
        }

        public static byte[] PadMessage(byte[] message)
        {
            ulong bitLength = (ulong)message.Length * 8;
            int padLength = (message.Length % BlockSize < 112) ? (112 - message.Length % BlockSize) : (240 - message.Length % BlockSize);
            byte[] paddedMessage = new byte[message.Length + padLength + 16];
            Buffer.BlockCopy(message, 0, paddedMessage, 0, message.Length);
            paddedMessage[message.Length] = 0x80;
            for (int i = paddedMessage.Length - 16; i < paddedMessage.Length - 8; i++)
            {
                paddedMessage[i] = 0x00;
            }
            Buffer.BlockCopy(BitConverter.GetBytes(ReverseBytes((ulong)0)), 0, paddedMessage, paddedMessage.Length - 16, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(ReverseBytes(bitLength)), 0, paddedMessage, paddedMessage.Length - 8, 8);
            return paddedMessage;
        }

        public static ulong RightRotate(ulong x, int n)
        {
            return (x >> n) | (x << (64 - n));
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

        public byte[] ComputeHash(string input)
        {
            return ShaNext_512(input);
        }
    }
}
