using System;
using System.Text;

namespace ShaNext.ShaNext
{
    public class Whirlpool : IHashAlgorithm
    {
        private static readonly ulong[][] C =
        [
            [0x18186018c07830d8, 0x23238c2305af4626, 0xc6c63fc67ef991b8, 0xe8e887e8136fcdfb, 0x87872687a9f5d8d0, 0xb8b8dab8a9626d11, 0x0101040108050209, 0x4f4f214f426e9e0d, 0x3636d836adee6c9b, 0xa6a6a2a6590451ff, 0xd2d26fd2debdb90c, 0xf5f5f3f5fb06f70e, 0x7979f979ef80f296, 0x6f6fa16f5fcede30, 0x91917e91fcef3f6d, 0x52525552aa07a4f8],
            [0x60609d6027fdc047, 0xbcbccabc89766535, 0x9b9b569baccd2b37, 0x8e8e028e8cbf2354, 0xa3a3b6a371155bd2, 0x0c0c300c603c186c, 0x7b7bf17bff8af684, 0x3535d435b5e16a80, 0x1d1d741de8693af5, 0xe0e0a7e05347ddb3, 0xd7d77bd7f6acb321, 0xc2c22fc25eed999c, 0x2e2eb82e6d965c43, 0x4b4b314b627a9629, 0xfefedffea321e15d, 0x575741578216aed5],
            [0x15155415a8412abd, 0x7777c1779fb6eee8, 0x3737dc37a5eb6e92, 0xe5e5b3e57b56d79e, 0x9f9f469f8cd92313, 0xf0f0e7f0d317fd23, 0x4a4a354a6a7f9420, 0xdada4fda9e95a944, 0x58587d58fa25b0a2, 0xc9c903c906ca8fcf, 0x2929a429558d527c, 0x0a0a280a5022145a, 0xb1b1feb1e14f7f50, 0xa0a0baa0691a5dc9, 0x6b6bb16b7fdad614, 0x85852e855cabd09e],
            [0xbdbdcebd8173673c, 0x5d5d695dd234ba8f, 0x1010401080502090, 0xf4f4f7f4f303f507, 0xcbcb0bcb16c08bdd, 0x3e3ef83eedc67cd3, 0x0505140528110a2d, 0x676781671fe6ce78, 0xe4e4b7e47353d597, 0x27279c2725bb4e02, 0x4141194132588273, 0x8b8b168b2c9d0ba7, 0xa7a7a6a7510153f6, 0x7d7de97dcf94fab2, 0x95956e95dcfb3749, 0xd8d847d88e9fad56],
            [0xfbfbcbfb8b30eb70, 0xeeee9fee2371c1cd, 0x7c7ced7cc791f8bb, 0x6666856617e3cc71, 0xdddd53dda68ea77b, 0x17175c17b84b2eaf, 0x4747014702468e45, 0x9e9e429e84dc211a, 0xcaca0fca1ec589d4, 0x2d2db42d75995a58, 0xbfbfc6bf9179632e, 0x07071c07381b0e3f, 0xadad8ead012347ac, 0x5a5a755aea2fb4b0, 0x838336836cb51bef, 0x3333cc3385ff66b6],
            [0x636391633ff2c65c, 0x02020802100a0412, 0xaaaa92aa39384993, 0x7171d971afa8e2de, 0xc8c807c80ecf8dc6, 0x19196419c87d32d1, 0x494939497270923b, 0xd9d943d9869aaf5f, 0xf2f2eff2c31df931, 0xe3e3abe34b48dba8, 0x5b5b715be22ab6b9, 0x88881a8834920dbc, 0x9a9a529aa4c8293e, 0x262698262dbe4c0b, 0x3232c8328dfa64bf, 0xb0b0fab0e94a7d59],
            [0xe9e983e91b6acff2, 0x0f0f3c0f78331e77, 0xd5d573d5e6a6b733, 0x80803a8074ba1df4, 0xbebec2be997c6127, 0xcdcd13cd26de87eb, 0x3434d034bde46889, 0x48483d487a759032, 0xffffdbffab24e354, 0x7a7af57af78ff48d, 0x90907a90f4ea3d64, 0x5f5f615fc23ebe9d, 0x202080201da0403d, 0x6868bd6867d5d00f, 0x1a1a681ad07234ca, 0xaeae82ae192c41b7],
            [0xb4b4eab4c95e757d, 0x54544d549a19a8ce, 0x93937693ece53b7f, 0x222288220daa442f, 0x64648d6407e9c863, 0xf1f1e3f1db12ff2a, 0x7373d173bfa2e6cc, 0x12124812905a2482, 0x40401d403a5d807a, 0x0808200840281048, 0xc3c32bc356e89b95, 0xecec97ec337bc5df, 0xdbdb4bdb96a0a2ba, 0xa1a1bea1611f5fc0, 0x8d8d0e8d1c830791, 0x3d3df43df5c97ac8],
        ];

        private static readonly ulong[] RC =
        [
            0x1823c6e887b8014f, 0x36a6d2f5796f9152, 0x60bc9b8ea30c7b35, 0x1de0d7c22e4bfe57,
            0x157737e59ff04ada, 0x58c9290ab1a06b85, 0xbd5d10f4cb3e0567, 0xe427418ba77d95d8,
            0xfbee7c66dd17479e, 0xca2dbf07ad5a8333
        ];

        private ulong[] K = new ulong[8];
        private ulong[] L = new ulong[8];
        private ulong[] block = new ulong[8];
        private ulong[] state = new ulong[8];

        public byte[] ComputeHash(byte[] input)
        {
            int paddedLength = input.Length + 1 + 31;
            paddedLength -= paddedLength % 32;
            byte[] paddedInput = new byte[paddedLength];
            Buffer.BlockCopy(input, 0, paddedInput, 0, input.Length);
            paddedInput[input.Length] = 0x80;
            ulong bitLength = (ulong)input.Length * 8;
            for (int i = 0; i < 8; i++)
            {
                paddedInput[paddedInput.Length - 1 - i] = (byte)(bitLength >> (8 * i));
            }

            for (int i = 0; i < 8; i++)
            {
                state[i] = 0;
            }

            for (int i = 0; i < paddedInput.Length; i += 32)
            {
                for (int j = 0; j < 8; j++)
                {
                    block[j] = BitConverter.ToUInt64(paddedInput, i + j * 8);
                }
                ProcessBlock();
            }

            byte[] hash = new byte[64];
            for (int i = 0; i < 8; i++)
            {
                byte[] temp = BitConverter.GetBytes(state[i]);
                Array.Reverse(temp);
                Buffer.BlockCopy(temp, 0, hash, i * 8, 8);
            }
            return hash;
        }

        private void ProcessBlock()
        {
            for (int i = 0; i < 8; i++)
            {
                state[i] ^= block[i];
                K[i] = state[i];
            }

            for (int r = 0; r < 10; r++)
            {
                for (int i = 0; i < 8; i++)
                {
                    L[i] = 0;
                    for (int j = 0; j < 8; j++)
                    {
                        L[i] ^= C[j][(K[(i - j) & 7] >> ((7 - j) * 8)) & 0xFF];
                    }
                }

                Array.Copy(L, K, 8);
                K[0] ^= RC[r];

                for (int i = 0; i < 8; i++)
                {
                    L[i] = 0;
                    for (int j = 0; j < 8; j++)
                    {
                        L[i] ^= C[j][(state[(i - j) & 7] >> ((7 - j) * 8)) & 0xFF];
                    }
                }

                Array.Copy(L, state, 8);
            }

            for (int i = 0; i < 8; i++)
            {
                state[i] ^= block[i];
            }
        }

        public byte[] ComputeHash(string input)
        {
            return ComputeHash(Encoding.UTF8.GetBytes(input));
        }
    }
}
