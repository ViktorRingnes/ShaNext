using System;
using System.Text;
using System.Security.Cryptography;

namespace ShaNext.ShaNext
{
    public class Argon2 : IHashAlgorithm
    {
        private const int DefaultMemoryCost = 4096;  
        private const int DefaultTimeCost = 3;       
        private const int DefaultParallelism = 1;    
        private const int HashLength = 32;           

        private static readonly byte[] FixedSalt = Encoding.UTF8.GetBytes("fixedSaltValue1234");

        public byte[] ComputeHash(string input)
        {
            return ComputeHash(input, FixedSalt, DefaultMemoryCost, DefaultTimeCost, DefaultParallelism, HashLength);
        }

        private byte[] ComputeHash(string input, byte[] salt, int memoryCost, int timeCost, int parallelism, int hashLength)
        {
            int memoryBlocks = memoryCost; 
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = new byte[hashLength];

            byte[][] memory = new byte[memoryBlocks][];
            for (int i = 0; i < memoryBlocks; i++)
            {
                memory[i] = new byte[1024];
            }

            FillBlock(inputBytes, salt, memory[0]);
            FillBlock(salt, inputBytes, memory[memoryBlocks - 1]);

            for (int t = 0; t < timeCost; t++)
            {
                for (int i = 0; i < memoryBlocks; i++)
                {
                    int j = BitConverter.ToInt32(memory[i], 0) % memoryBlocks;
                    if (j < 0) j += memoryBlocks;
                    FillBlock(memory[i], memory[j], memory[i]);
                }
            }

            Buffer.BlockCopy(memory[memoryBlocks - 1], 0, hashBytes, 0, hashLength);

            for (int i = 0; i < memoryBlocks; i++)
            {
                Array.Clear(memory[i], 0, memory[i].Length);
            }

            return hashBytes;
        }

        private static void FillBlock(byte[] input1, byte[] input2, byte[] output)
        {
            byte[] buffer = new byte[input1.Length + input2.Length];
            Buffer.BlockCopy(input1, 0, buffer, 0, input1.Length);
            Buffer.BlockCopy(input2, 0, buffer, input1.Length, input2.Length);
            byte[] hash = SHA256.HashData(buffer);
            Buffer.BlockCopy(hash, 0, output, 0, Math.Min(output.Length, hash.Length));
        }

        public string ComputeHashString(string input)
        {
            byte[] hashBytes = ComputeHash(input);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
