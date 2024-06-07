using System.Text;

namespace ShaNext.ShaNext
{
    public class RIPEMD_160 : IHashAlgorithm
    {
        private const int BlockSize = 64;
        private uint[] h;

        public RIPEMD_160()
        {
            InitializeState();
        }

        private void InitializeState()
        {
            h =
            [
                0x67452301, 0xEFCDAB89, 0x98BADCFE, 0x10325476, 0xC3D2E1F0
            ];
        }

        private static readonly uint[] R1 = [
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,
            7, 4, 13, 1, 10, 6, 15, 3, 12, 0, 9, 5, 2, 14, 11, 8,
            3, 10, 14, 4, 9, 15, 8, 1, 2, 7, 0, 6, 13, 11, 5, 12,
            1, 9, 11, 10, 0, 8, 12, 4, 13, 3, 7, 15, 14, 5, 6, 2,
            4, 0, 5, 9, 7, 12, 2, 10, 14, 1, 3, 8, 11, 6, 15, 13
        ];

        private static readonly uint[] R2 = [
            5, 14, 7, 0, 9, 2, 11, 4, 13, 6, 15, 8, 1, 10, 3, 12,
            6, 11, 3, 7, 0, 13, 5, 10, 14, 15, 8, 12, 4, 9, 1, 2,
            15, 5, 1, 3, 7, 14, 6, 9, 11, 8, 12, 2, 10, 0, 4, 13,
            8, 6, 4, 1, 3, 11, 15, 0, 5, 12, 2, 13, 9, 7, 10, 14,
            12, 15, 10, 4, 1, 5, 8, 7, 6, 2, 13, 14, 0, 3, 9, 11
        ];

        private static readonly uint[] S1 = [
            11, 14, 15, 12, 5, 8, 7, 9, 11, 13, 14, 15, 6, 7, 9, 8,
            7, 6, 8, 13, 11, 9, 7, 15, 7, 12, 15, 9, 11, 7, 13, 12,
            11, 13, 6, 7, 14, 9, 13, 15, 14, 8, 13, 6, 5, 12, 7, 5,
            11, 12, 14, 15, 14, 15, 9, 8, 9, 14, 5, 6, 8, 6, 5, 12,
            9, 15, 5, 11, 6, 8, 13, 12, 5, 12, 13, 14, 11, 8, 5, 6
        ];

        private static readonly uint[] S2 = [
            8, 9, 9, 11, 13, 15, 15, 5, 7, 7, 8, 11, 14, 14, 12, 6,
            9, 13, 15, 7, 12, 8, 9, 11, 7, 7, 12, 7, 6, 15, 13, 11,
            9, 7, 15, 11, 8, 6, 6, 14, 12, 13, 5, 14, 13, 13, 7, 5,
            15, 5, 8, 11, 14, 14, 6, 14, 6, 9, 12, 9, 12, 5, 15, 8,
            8, 5, 12, 9, 12, 5, 14, 6, 8, 13, 6, 5, 15, 13, 11, 11
        ];

        private static uint F(uint x, uint y, uint z)
        {
            return x ^ y ^ z;
        }

        private static uint G(uint x, uint y, uint z)
        {
            return (x & y) | (~x & z);
        }

        private static uint H_func(uint x, uint y, uint z)
        {
            return (x | ~y) ^ z;
        }

        private static uint I(uint x, uint y, uint z)
        {
            return (x & z) | (y & ~z);
        }

        private static uint J(uint x, uint y, uint z)
        {
            return x ^ (y | ~z);
        }

        private static uint RotateLeft(uint x, int n)
        {
            return (x << n) | (x >> (32 - n));
        }

        public byte[] ComputeHash(byte[] input)
        {
            InitializeState(); 

            uint[] X = new uint[16];
            byte[] paddedInput = PadMessage(input);
            int blockCount = paddedInput.Length / BlockSize;

            for (int i = 0; i < blockCount; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    X[j] = BitConverter.ToUInt32(paddedInput, (i * BlockSize) + (j * 4));
                }

                uint aa = h[0];
                uint bb = h[1];
                uint cc = h[2];
                uint dd = h[3];
                uint ee = h[4];
                uint aaa = h[0];
                uint bbb = h[1];
                uint ccc = h[2];
                uint ddd = h[3];
                uint eee = h[4];

                for (int j = 0; j < 80; j++)
                {
                    int r1 = (int)R1[j];
                    int r2 = (int)R2[j];
                    int s1 = (int)S1[j];
                    int s2 = (int)S2[j];

                    uint t = RotateLeft(aa + F(bb, cc, dd) + X[r1], s1) + ee;
                    aa = ee;
                    ee = dd;
                    dd = RotateLeft(cc, 10);
                    cc = bb;
                    bb = t;

                    t = RotateLeft(aaa + J(bbb, ccc, ddd) + X[r2] + 0x50A28BE6, s2) + eee;
                    aaa = eee;
                    eee = ddd;
                    ddd = RotateLeft(ccc, 10);
                    ccc = bbb;
                    bbb = t;
                }

                for (int j = 0; j < 80; j++)
                {
                    int r1 = (int)R1[j];
                    int r2 = (int)R2[j];
                    int s1 = (int)S1[j];
                    int s2 = (int)S2[j];

                    uint t = RotateLeft(aa + G(bb, cc, dd) + X[r1] + 0x5A827999, s1) + ee;
                    aa = ee;
                    ee = dd;
                    dd = RotateLeft(cc, 10);
                    cc = bb;
                    bb = t;

                    t = RotateLeft(aaa + I(bbb, ccc, ddd) + X[r2] + 0x5C4DD124, s2) + eee;
                    aaa = eee;
                    eee = ddd;
                    ddd = RotateLeft(ccc, 10);
                    ccc = bbb;
                    bbb = t;
                }

                for (int j = 0; j < 80; j++)
                {
                    int r1 = (int)R1[j];
                    int r2 = (int)R2[j];
                    int s1 = (int)S1[j];
                    int s2 = (int)S2[j];

                    uint t = RotateLeft(aa + H_func(bb, cc, dd) + X[r1] + 0x6ED9EBA1, s1) + ee;
                    aa = ee;
                    ee = dd;
                    dd = RotateLeft(cc, 10);
                    cc = bb;
                    bb = t;

                    t = RotateLeft(aaa + G(bbb, ccc, ddd) + X[r2] + 0x6D703EF3, s2) + eee;
                    aaa = eee;
                    eee = ddd;
                    ddd = RotateLeft(ccc, 10);
                    ccc = bbb;
                    bbb = t;
                }

                for (int j = 0; j < 80; j++)
                {
                    int r1 = (int)R1[j];
                    int r2 = (int)R2[j];
                    int s1 = (int)S1[j];
                    int s2 = (int)S2[j];

                    uint t = RotateLeft(aa + I(bb, cc, dd) + X[r1] + 0x8F1BBCDC, s1) + ee;
                    aa = ee;
                    ee = dd;
                    dd = RotateLeft(cc, 10);
                    cc = bb;
                    bb = t;

                    t = RotateLeft(aaa + F(bbb, ccc, ddd) + X[r2], s2) + eee;
                    aaa = eee;
                    eee = ddd;
                    ddd = RotateLeft(ccc, 10);
                    ccc = bbb;
                    bbb = t;
                }

                for (int j = 0; j < 80; j++)
                {
                    int r1 = (int)R1[j];
                    int r2 = (int)R2[j];
                    int s1 = (int)S1[j];
                    int s2 = (int)S2[j];

                    uint t = RotateLeft(aa + J(bb, cc, dd) + X[r1] + 0xA953FD4E, s1) + ee;
                    aa = ee;
                    ee = dd;
                    dd = RotateLeft(cc, 10);
                    cc = bb;
                    bb = t;

                    t = RotateLeft(aaa + H_func(bbb, ccc, ddd) + X[r2] + 0x7A6D76E9, s2) + eee;
                    aaa = eee;
                    eee = ddd;
                    ddd = RotateLeft(ccc, 10);
                    ccc = bbb;
                    bbb = t;
                }

                uint t1 = h[1] + cc + ddd;
                h[1] = h[2] + dd + eee;
                h[2] = h[3] + ee + aaa;
                h[3] = h[4] + aa + bbb;
                h[4] = h[0] + bb + ccc;
                h[0] = t1;
            }

            byte[] hash = new byte[20];
            for (int i = 0; i < h.Length; i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(h[i]), 0, hash, i * 4, 4);
            }
            return hash;
        }

        private static byte[] PadMessage(byte[] input)
        {
            ulong bitLength = (ulong)input.Length * 8;
            int paddingSize = (input.Length % BlockSize < 56) ? (56 - input.Length % BlockSize) : (120 - input.Length % BlockSize);
            byte[] paddedMessage = new byte[input.Length + paddingSize + 8];
            Buffer.BlockCopy(input, 0, paddedMessage, 0, input.Length);
            paddedMessage[input.Length] = 0x80;
            for (int i = 0; i < 8; i++)
            {
                paddedMessage[paddedMessage.Length - 8 + i] = (byte)((bitLength >> (8 * i)) & 0xFF);
            }
            return paddedMessage;
        }

        public byte[] ComputeHash(string input)
        {
            return ComputeHash(Encoding.UTF8.GetBytes(input));
        }
    }
}
