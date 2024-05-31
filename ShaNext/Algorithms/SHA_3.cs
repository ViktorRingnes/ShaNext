using System;
using System.Text;

namespace ShaNext.ShaNext
{
    public class SHA_3 : IHashAlgorithm
    {
        private const int Rate = 1088;
        private const int Capacity = 512;
        private const int OutputLength = 256;
        private const int StateSize = 1600;
        private const int BlockSize = 200 - 2 * (OutputLength / 8);
        private static readonly ulong[] RoundConstants = new ulong[24]
        {
            0x0000000000000001, 0x0000000000008082, 0x800000000000808A, 0x8000000080008000,
            0x000000000000808B, 0x0000000080000001, 0x8000000080008081, 0x8000000000008009,
            0x000000000000008A, 0x0000000000000088, 0x0000000080008009, 0x000000008000000A,
            0x000000008000808B, 0x800000000000008B, 0x8000000000008089, 0x8000000000008003,
            0x8000000000008002, 0x8000000000000080, 0x000000000000800A, 0x800000008000000A,
            0x8000000080008081, 0x8000000000008080, 0x0000000080000001, 0x8000000080008008
        };

        public static byte[] ShaNext_3_256(string input)
        {
            byte[] paddedMessage = PadMessage(Encoding.UTF8.GetBytes(input));
            ulong[] state = new ulong[StateSize / 64];

            for (int i = 0; i < paddedMessage.Length; i += BlockSize)
            {
                byte[] block = new byte[BlockSize];
                Array.Copy(paddedMessage, i, block, 0, BlockSize);
                KeccakF(state, block);
            }

            byte[] hash = new byte[OutputLength / 8];
            for (int i = 0; i < hash.Length; i++)
            {
                hash[i] = (byte)(state[i / 8] >> (8 * (i % 8)) & 0xFF);
            }

            return hash;
        }

        private static byte[] PadMessage(byte[] message)
        {
            int padLength = BlockSize - (message.Length % BlockSize);
            byte[] paddedMessage = new byte[message.Length + padLength];
            Array.Copy(message, paddedMessage, message.Length);
            paddedMessage[message.Length] = 0x06;
            paddedMessage[paddedMessage.Length - 1] |= 0x80;
            return paddedMessage;
        }

        private static void KeccakF(ulong[] state, byte[] block)
        {
            for (int i = 0; i < BlockSize; i++)
            {
                state[i / 8] ^= (ulong)block[i] << (8 * (i % 8));
            }

            for (int round = 0; round < 24; round++)
            {
                Theta(state);
                Rho(state);
                Pi(state);
                Chi(state);
                Iota(state, round);
            }
        }

        private static void Theta(ulong[] state)
        {
            ulong[] C = new ulong[5];
            for (int i = 0; i < 5; i++)
            {
                C[i] = state[i] ^ state[i + 5] ^ state[i + 10] ^ state[i + 15] ^ state[i + 20];
            }

            for (int i = 0; i < 5; i++)
            {
                ulong d = C[(i + 4) % 5] ^ ((C[(i + 1) % 5] << 1) | (C[(i + 1) % 5] >> 63));
                for (int j = 0; j < 25; j += 5)
                {
                    state[j + i] ^= d;
                }
            }
        }

        private static void Rho(ulong[] state)
        {
            int[] rhoOffsets = {
                0,  1, 62, 28, 27,
                36, 44,  6, 55, 20,
                3, 10, 43, 25, 39,
                41, 45, 15, 21,  8,
                18, 2, 61, 56, 14
            };

            for (int i = 0; i < state.Length; i++)
            {
                state[i] = (state[i] << rhoOffsets[i]) | (state[i] >> (64 - rhoOffsets[i]));
            }
        }

        private static void Pi(ulong[] state)
        {
            ulong[] tempState = new ulong[state.Length];
            Array.Copy(state, tempState, state.Length);

            int[] piOffsets = {
                0,  1,  6,  9, 22,
                14, 20,  2, 12, 13,
                23, 15,  4, 24, 21,
                8,  5, 16, 3, 18,
                17, 11,  7, 10, 19
            };

            for (int i = 0; i < state.Length; i++)
            {
                state[i] = tempState[piOffsets[i]];
            }
        }

        private static void Chi(ulong[] state)
        {
            for (int j = 0; j < 25; j += 5)
            {
                ulong[] tempState = new ulong[5];
                for (int i = 0; i < 5; i++)
                {
                    tempState[i] = state[j + i];
                }

                for (int i = 0; i < 5; i++)
                {
                    state[j + i] ^= (~tempState[(i + 1) % 5]) & tempState[(i + 2) % 5];
                }
            }
        }

        private static void Iota(ulong[] state, int round)
        {
            state[0] ^= RoundConstants[round];
        }

        public byte[] ComputeHash(string input)
        {
            return ShaNext_3_256(input);
        }
    }
}
