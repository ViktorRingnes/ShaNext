using System;
using System.Security.Cryptography;
using System.Text;

namespace ShaNext.ShaNext
{
    public class Scrypt : IHashAlgorithm
    {
        private const int DefaultMemoryCost = 16384; 
        private const int DefaultTimeCost = 8;      
        private const int DefaultParallelism = 1;  
        private const int SaltLength = 16;          
        private const int HashLength = 32;        

        public byte[] ComputeHash(string input)
        {
            byte[] salt = GenerateSalt(SaltLength);
            return ComputeHash(input, salt, DefaultMemoryCost, DefaultTimeCost, DefaultParallelism, HashLength);
        }

        public string ComputeHashString(string input)
        {
            byte[] hashBytes = ComputeHash(input);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        private byte[] ComputeHash(string input, byte[] salt, int memoryCost, int timeCost, int parallelism, int hashLength)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            return SCrypt.ComputeDerivedKey(inputBytes, salt, memoryCost, timeCost, parallelism, null, hashLength);
        }

        private byte[] GenerateSalt(int length)
        {
            byte[] salt = new byte[length];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
    }

    public static class SCrypt
    {
        public static byte[] ComputeDerivedKey(byte[] password, byte[] salt, int N, int r, int p, byte[]? derivedKey, int dkLen)
        {
            if (N <= 1 || (N & (N - 1)) != 0)
                throw new ArgumentException("N must be a power of 2 greater than 1");
            if (N > int.MaxValue / 128 / r)
                throw new ArgumentException("Parameter N is too large");
            if (r > int.MaxValue / 128 / p)
                throw new ArgumentException("Parameter r is too large");

            if (derivedKey == null)
                derivedKey = new byte[dkLen];

            using (var hmac = new HMACSHA256(password))
            {
                int blockSize = 128 * r;
                byte[] B = Pbkdf2(hmac, salt, 1, p * blockSize);
                byte[] XY = new byte[256 * r];
                byte[] V = new byte[blockSize * N];

                for (int i = 0; i < p; i++)
                {
                    SMix(B, i * blockSize, r, N, V, XY);
                }

                return Pbkdf2(hmac, B, 1, dkLen);
            }
        }

        private static void SMix(byte[] B, int Bi, int r, int N, byte[] V, byte[] XY)
        {
            int blockSize = 128 * r;
            int Xi = 0;
            int Yi = 128 * r;
            Buffer.BlockCopy(B, Bi, XY, Xi, blockSize);

            for (int i = 0; i < N; i++)
            {
                Buffer.BlockCopy(XY, Xi, V, i * blockSize, blockSize);
                BlockMix(XY, Xi, Yi, r);
            }

            for (int i = 0; i < N; i++)
            {
                int j = Integerify(XY, Xi, r) & (N - 1);
                Xor(XY, Xi, V, j * blockSize, blockSize);
                BlockMix(XY, Xi, Yi, r);
            }

            Buffer.BlockCopy(XY, Xi, B, Bi, blockSize);
        }

        private static void BlockMix(byte[] B, int Bi, int Yi, int r)
        {
            int blockSize = 128 * r;
            byte[] X = new byte[64];
            Buffer.BlockCopy(B, Bi + blockSize - 64, X, 0, 64);

            for (int i = 0; i < 2 * r; i++)
            {
                Xor(X, 0, B, Bi + i * 64, 64);
                Salsa20Core(X);
                Buffer.BlockCopy(X, 0, B, Yi + i * 64, 64);
            }

            for (int i = 0; i < r; i++)
            {
                Buffer.BlockCopy(B, Yi + i * 2 * 64, B, Bi + i * 64, 64);
            }

            for (int i = 0; i < r; i++)
            {
                Buffer.BlockCopy(B, Yi + (i * 2 + 1) * 64, B, Bi + (r + i) * 64, 64);
            }
        }

        private static void Salsa20Core(byte[] B)
        {
            int[] x = new int[16];
            for (int i = 0; i < 16; i++)
            {
                x[i] = BitConverter.ToInt32(B, i * 4);
            }

            for (int i = 0; i < 8; i += 2)
            {
                QuarterRound(x, 0, 4, 8, 12);
                QuarterRound(x, 1, 5, 9, 13);
                QuarterRound(x, 2, 6, 10, 14);
                QuarterRound(x, 3, 7, 11, 15);
                QuarterRound(x, 0, 5, 10, 15);
                QuarterRound(x, 1, 6, 11, 12);
                QuarterRound(x, 2, 7, 8, 13);
                QuarterRound(x, 3, 4, 9, 14);
            }

            for (int i = 0; i < 16; i++)
            {
                BitConverter.GetBytes(x[i] + BitConverter.ToInt32(B, i * 4)).CopyTo(B, i * 4);
            }
        }

        private static void QuarterRound(int[] x, int a, int b, int c, int d)
        {
            x[b] ^= Rotl(x[a] + x[d], 7);
            x[c] ^= Rotl(x[b] + x[a], 9);
            x[d] ^= Rotl(x[c] + x[b], 13);
            x[a] ^= Rotl(x[d] + x[c], 18);
        }

        private static int Rotl(int x, int y)
        {
            return (x << y) | (x >> (32 - y));
        }

        private static int Integerify(byte[] B, int bi, int r)
        {
            return BitConverter.ToInt32(B, bi + (2 * r - 1) * 64);
        }

        private static void Xor(byte[] a, int ai, byte[] b, int bi, int len)
        {
            for (int i = 0; i < len; i++)
            {
                a[ai + i] ^= b[bi + i];
            }
        }

        private static byte[] Pbkdf2(HMAC hmac, byte[] salt, int iterations, int dkLen)
        {
            int hLen = hmac.HashSize / 8;
            int l = (dkLen + hLen - 1) / hLen;
            int r = dkLen - (l - 1) * hLen;

            byte[] dk = new byte[l * hLen];
            byte[] block1 = new byte[salt.Length + 4];
            Buffer.BlockCopy(salt, 0, block1, 0, salt.Length);

            for (int i = 1; i <= l; i++)
            {
                block1[salt.Length + 0] = (byte)(i >> 24);
                block1[salt.Length + 1] = (byte)(i >> 16);
                block1[salt.Length + 2] = (byte)(i >> 8);
                block1[salt.Length + 3] = (byte)(i >> 0);

                byte[] t = hmac.ComputeHash(block1);
                Buffer.BlockCopy(t, 0, dk, (i - 1) * hLen, t.Length);

                for (int j = 1; j < iterations; j++)
                {
                    t = hmac.ComputeHash(t);
                    for (int k = 0; k < hLen; k++)
                    {
                        dk[(i - 1) * hLen + k] ^= t[k];
                    }
                }
            }

            if (dkLen != dk.Length)
            {
                byte[] dkTruncated = new byte[dkLen];
                Buffer.BlockCopy(dk, 0, dkTruncated, 0, dkLen);
                return dkTruncated;
            }
            else
            {
                return dk;
            }
        }
    }
}
