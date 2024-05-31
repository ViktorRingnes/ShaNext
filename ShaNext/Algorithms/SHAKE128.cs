using System;
using System.Linq;
using System.Text;

namespace ShaNext.ShaNext
{
    public class SHAKE128 : IHashAlgorithm
    {
        private const int Rate = 168; 
        private const int Capacity = 256; 
        private const int StateSize = 200;
        private byte[] state = new byte[StateSize];
        private int rateInBytes = Rate;
        private int absorbedBytes = 0;

        public SHAKE128()
        {
            Array.Clear(state, 0, StateSize);
        }

        public void Absorb(byte[] data)
        {
            int offset = 0;
            while (offset < data.Length)
            {
                int blockSize = Math.Min(rateInBytes - absorbedBytes, data.Length - offset);
                for (int i = 0; i < blockSize; i++)
                {
                    state[absorbedBytes + i] ^= data[offset + i];
                }

                absorbedBytes += blockSize;
                offset += blockSize;

                if (absorbedBytes == rateInBytes)
                {
                    KeccakF1600();
                    absorbedBytes = 0;
                }
            }
        }

        public byte[] Squeeze(int outputLength)
        {
            byte[] output = new byte[outputLength];
            int offset = 0;

            if (absorbedBytes > 0)
            {
                KeccakF1600();
                absorbedBytes = 0;
            }

            while (offset < outputLength)
            {
                int blockSize = Math.Min(rateInBytes, outputLength - offset);
                Buffer.BlockCopy(state, 0, output, offset, blockSize);
                offset += blockSize;

                if (offset < outputLength)
                {
                    KeccakF1600();
                }
            }

            return output;
        }

        public byte[] Shake128(byte[] input, int outputLength)
        {
            Absorb(input);
            byte[] padding = new byte[1] { 0x1F };
            Absorb(padding);
            state[rateInBytes - 1] ^= 0x80;
            return Squeeze(outputLength);
        }

        private void KeccakF1600()
        {
            ulong[] A = new ulong[25];
            for (int i = 0; i < 25; i++)
            {
                A[i] = BitConverter.ToUInt64(state, i * 8);
            }

            ulong[] C = new ulong[5];
            ulong[] D = new ulong[5];
            ulong[] B = new ulong[25];
            for (int round = 0; round < 24; round++)
            {
                for (int i = 0; i < 5; i++)
                {
                    C[i] = A[i] ^ A[i + 5] ^ A[i + 10] ^ A[i + 15] ^ A[i + 20];
                }

                for (int i = 0; i < 5; i++)
                {
                    D[i] = C[(i + 4) % 5] ^ ((C[(i + 1) % 5] << 1) | (C[(i + 1) % 5] >> (64 - 1)));
                }

                for (int i = 0; i < 25; i += 5)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        A[i + j] ^= D[j];
                    }
                }

                Array.Copy(A, B, 25);
                for (int x = 0; x < 5; x++)
                {
                    for (int y = 0; y < 5; y++)
                    {
                        A[y + 5 * ((2 * x + 3 * y) % 5)] = (B[x + 5 * y] << R[x, y]) | (B[x + 5 * y] >> (64 - R[x, y]));
                    }
                }

                Array.Copy(A, B, 25);
                for (int y = 0; y < 5; y++)
                {
                    for (int x = 0; x < 5; x++)
                    {
                        A[x + 5 * y] = B[x + 5 * y] ^ ((~B[(x + 1) % 5 + 5 * y]) & B[(x + 2) % 5 + 5 * y]);
                    }
                }

                A[0] ^= RoundConstants[round];
            }

            for (int i = 0; i < 25; i++)
            {
                byte[] bytes = BitConverter.GetBytes(A[i]);
                Buffer.BlockCopy(bytes, 0, state, i * 8, 8);
            }
        }

        private static readonly int[,] R = new int[,]
        {
            { 0, 36, 3, 41, 18 },
            { 1, 44, 10, 45, 2 },
            { 62, 6, 43, 15, 61 },
            { 28, 55, 25, 21, 56 },
            { 27, 20, 39, 8, 14 }
        };

        private static readonly ulong[] RoundConstants = new ulong[]
        {
            0x0000000000000001, 0x0000000000008082, 0x800000000000808A, 0x8000000080008000,
            0x000000000000808B, 0x0000000080000001, 0x8000000080008081, 0x8000000000008009,
            0x000000000000008A, 0x0000000000000088, 0x0000000080008009, 0x000000008000000A,
            0x000000008000808B, 0x800000000000008B, 0x8000000000008089, 0x8000000000008003,
            0x8000000000008002, 0x8000000000000080, 0x000000000000800A, 0x800000008000000A,
            0x8000000080008081, 0x8000000000008080, 0x0000000080000001, 0x8000000080008008
        };

        public byte[] ComputeHash(string input)
        {
            return Shake128(Encoding.UTF8.GetBytes(input), 32);
        }
    }
}
