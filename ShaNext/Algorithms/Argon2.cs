using System.Text;

namespace ShaNext.ShaNext
{
    public class Argon2 : IHashAlgorithm
    {
        private const int DefaultMemoryCost = 4096;
        private const int DefaultTimeCost = 3;     
        private const int DefaultParallelism = 1;  
        private const int SaltLength = 16;         
        private const int HashLength = 32;         

        public byte[] ComputeHash(string input)
        {
            byte[] salt = GenerateSalt(SaltLength);
            return ComputeHash(input, salt, DefaultMemoryCost, DefaultTimeCost, DefaultParallelism, HashLength);
        }

        private byte[] ComputeHash(string input, byte[] salt, int memoryCost, int timeCost, int parallelism, int hashLength)
        {
            int memoryBlocks = memoryCost / (1024 * 4); 
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = new byte[hashLength];

            byte[][] memory = new byte[memoryBlocks][];
            for (int i = 0; i < memoryBlocks; i++)
            {
                memory[i] = new byte[1024 * 4];
            }

            FillBlock(inputBytes, salt, memory[0]);
            FillBlock(salt, inputBytes, memory[memoryBlocks - 1]);

            for (int i = 1; i < memoryBlocks - 1; i++)
            {
                FillBlock(memory[i - 1], memory[i], memory[i]);
            }

            for (int t = 0; t < timeCost; t++)
            {
                for (int i = 0; i < memoryBlocks; i++)
                {
                    int j = BitConverter.ToInt32(memory[i], 0) % memoryBlocks;
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

        private void FillBlock(byte[] input1, byte[] input2, byte[] output)
        {
            byte[] buffer = new byte[input1.Length + input2.Length];
            Buffer.BlockCopy(input1, 0, buffer, 0, input1.Length);
            Buffer.BlockCopy(input2, 0, buffer, input1.Length, input2.Length);

            for (int i = 0; i < output.Length; i++)
            {
                output[i] = (byte)(buffer[i % buffer.Length] ^ buffer[(i + 1) % buffer.Length]);
            }
        }

        private byte[] GenerateSalt(int length)
        {
            byte[] salt = new byte[length];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public string ComputeHashString(string input)
        {
            byte[] hashBytes = ComputeHash(input);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
