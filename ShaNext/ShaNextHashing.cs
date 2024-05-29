using System;
using System.Security.Cryptography;
using System.Text;

namespace ShaNext.ShaNext
{
    public class ShaNextHashing
    {
        private const int HashSize = 32; 
        private const int Iterations = 10000; 

        public static string Hash(string input)
        {
            byte[] hash = SHA_Next(input);
            string convertedHash = BitConverter.ToString(hash).Replace("-", "").ToLower();
            return convertedHash;
        }

        public static byte[] SHA_Next(string input)
        {
            ulong[] H = {
                0x8c3d37c819544da2, 0x73e1996689dcd4d6, 0x1dfab7ae32ff9c82, 0x679dd514582f9fcf,
                0x0f6d2b697bd44da8, 0x77e36f7304c48942, 0x3f9d85a86a1d36c8, 0x1112e6ad91d692a1
            };

            ulong[] K = {
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

            byte[] paddedMessage = ShaNextUtilities.PadMessage(Encoding.UTF8.GetBytes(input));
            int blockCount = paddedMessage.Length / 128;

            for (int i = 0; i < blockCount; i++)
            {
                ulong[] W = new ulong[80];
                for (int j = 0; j < 16; j++)
                {
                    W[j] = BitConverter.ToUInt64(paddedMessage, i * 128 + j * 8);
                }

                for (int t = 16; t < 80; t++)
                {
                    ulong s0 = (ShaNextUtilities.RightRotate(W[t - 15], 1) ^ ShaNextUtilities.RightRotate(W[t - 15], 8) ^ (W[t - 15] >> 7));
                    ulong s1 = (ShaNextUtilities.RightRotate(W[t - 2], 19) ^ ShaNextUtilities.RightRotate(W[t - 2], 61) ^ (W[t - 2] >> 6));
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

                for (int t = 0; t < 80; t++)
                {
                    ulong Σ1 = (ShaNextUtilities.RightRotate(e, 14) ^ ShaNextUtilities.RightRotate(e, 18) ^ ShaNextUtilities.RightRotate(e, 41));
                    ulong Ch = (e & f) ^ ((~e) & g);
                    ulong temp1 = h + Σ1 + Ch + K[t] + W[t];
                    ulong Σ0 = (ShaNextUtilities.RightRotate(a, 28) ^ ShaNextUtilities.RightRotate(a, 34) ^ ShaNextUtilities.RightRotate(a, 39));
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

            byte[] hash = new byte[64];
            Buffer.BlockCopy(H, 0, hash, 0, 64);
            return hash;
        }

        public static string HashWithSalt(string input, string salt)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);
            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(input, saltBytes, Iterations))
            {
                byte[] hashBytes = rfc2898DeriveBytes.GetBytes(HashSize);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
