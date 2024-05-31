using System;
using System.Text;

namespace ShaNext.ShaNext
{
    public class SHA_512_224 : IHashAlgorithm
    {
        private const int HashSize = 28;
        private const int BlockSize = 128; 
        private const int MessageScheduleSize = 80; 

        public static byte[] ShaNext_512_224(string input)
        {
            ulong[] H = {
                0x8C3D37C819544DA2, 0x73E1996689DCD4D6,
                0x1DFAB7AE32FF9C82, 0x679DD514582F9FCF,
                0x0F6D2B697BD44DA8, 0x77E36F7304C48942,
                0x3F9D85A86A1D36C8, 0x1112E6AD91D692A1
            };

            ulong[] K = {
                0x428A2F98D728AE22, 0x7137449123EF65CD, 0xB5C0FBCFEC4D3B2F, 0xE9B5DBA58189DBBC,
                0x3956C25BF348B538, 0x59F111F1B605D019, 0x923F82A4AF194F9B, 0xAB1C5ED5DA6D8118,
                0xD807AA98A3030242, 0x12835B0145706FBE, 0x243185BE4EE4B28C, 0x550C7DC3D5FFB4E2,
                0x72BE5D74F27B896F, 0x80DEB1FE3B1696B1, 0x9BDC06A725C71235, 0xC19BF174CF692694,
                0xE49B69C19EF14AD2, 0xEFBE4786384F25E3, 0x0FC19DC68B8CD5B5, 0x240CA1CC77AC9C65,
                0x2DE92C6F592B0275, 0x4A7484AA6EA6E483, 0x5CB0A9DCBD41FBD4, 0x76F988DA831153B5,
                0x983E5152EE66DFAB, 0xA831C66D2DB43210, 0xB00327C898FB213F, 0xBF597FC7BEEF0EE4,
                0xC6E00BF33DA88FC2, 0xD5A79147930AA725, 0x06CA6351E003826F, 0x142929670A0E6E70,
                0x27B70A8546D22FFC, 0x2E1B21385C26C926, 0x4D2C6DFC5AC42AED, 0x53380D139D95B3DF,
                0x650A73548BAF63DE, 0x766A0ABB3C77B2A8, 0x81C2C92E47EDAEE6, 0x92722C851482353B,
                0xA2BFE8A14CF10364, 0xA81A664BBC423001, 0xC24B8B70D0F89791, 0xC76C51A30654BE30,
                0xD192E819D6EF5218, 0xD69906245565A910, 0xF40E35855771202A, 0x106AA07032BBD1B8,
                0x19A4C116B8D2D0C8, 0x1E376C085141AB53, 0x2748774CDF8EEB99, 0x34B0BCB5E19B48A8,
                0x391C0CB3C5C95A63, 0x4ED8AA4AE3418ACB, 0x5B9CCA4F7763E373, 0x682E6FF3D6B2B8A3,
                0x748F82EE5DEFB2FC, 0x78A5636F43172F60, 0x84C87814A1F0AB72, 0x8CC702081A6439EC,
                0x90BEFFFA23631E28, 0xA4506CEBDE82BDE9, 0xBEF9A3F7B2C67915, 0xC67178F2E372532B,
                0xCA273ECEEA26619C, 0xD186B8C721C0C207, 0xEADA7DD6CDE0EB1E, 0xF57D4F7FEE6ED178,
                0x06F067AA72176FBA, 0x0A637DC5A2C898A6, 0x113F9804BEF90DAE, 0x1B710B35131C471B,
                0x28DB77F523047D84, 0x32CAAB7B40C72493, 0x3C9EBE0A15C9BEBC, 0x431D67C49C100D4C,
                0x4CC5D4BECB3E42B6, 0x597F299CFC657E2A, 0x5FCB6FAB3AD6FAEC, 0x6C44198C4A475817
            };

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
            for (int i = 0; i < 4; i++)
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
            return ShaNext_512_224(input);
        }
    }
}
