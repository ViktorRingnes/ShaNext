using ShaNext.ShaNext;
using System;
using System.Text;

public class SHA_1 : IHashAlgorithm
{
    private static readonly uint[] H = {
        0x67452301, 0xEFCDAB89, 0x98BADCFE, 0x10325476, 0xC3D2E1F0
    };

    private static uint RotateLeft(uint x, int n)
    {
        return (x << n) | (x >> (32 - n));
    }

    private static byte[] PadMessage(byte[] message)
    {
        uint bitLen = (uint)(message.Length * 8);
        int padLen = (message.Length % 64 < 56) ? (56 - message.Length % 64) : (120 - message.Length % 64);

        byte[] padded = new byte[message.Length + padLen + 8];
        Buffer.BlockCopy(message, 0, padded, 0, message.Length);
        padded[message.Length] = 0x80;
        for (int i = 0; i < 8; i++)
        {
            padded[padded.Length - 1 - i] = (byte)(bitLen >> (8 * i));
        }

        return padded;
    }

    public static byte[] ComputeHash(byte[] message)
    {
        byte[] paddedMessage = PadMessage(message);
        uint[] words = new uint[80];
        uint h0 = H[0], h1 = H[1], h2 = H[2], h3 = H[3], h4 = H[4];

        for (int chunk = 0; chunk < paddedMessage.Length / 64; chunk++)
        {
            for (int i = 0; i < 16; i++)
            {
                words[i] = BitConverter.ToUInt32(paddedMessage, chunk * 64 + i * 4);
                words[i] = ((words[i] << 24) & 0xFF000000) | ((words[i] << 8) & 0x00FF0000) |
                           ((words[i] >> 8) & 0x0000FF00) | ((words[i] >> 24) & 0x000000FF);
            }

            for (int i = 16; i < 80; i++)
            {
                words[i] = RotateLeft(words[i - 3] ^ words[i - 8] ^ words[i - 14] ^ words[i - 16], 1);
            }

            uint a = h0, b = h1, c = h2, d = h3, e = h4;

            for (int i = 0; i < 80; i++)
            {
                uint f, k;

                if (i < 20)
                {
                    f = (b & c) | (~b & d);
                    k = 0x5A827999;
                }
                else if (i < 40)
                {
                    f = b ^ c ^ d;
                    k = 0x6ED9EBA1;
                }
                else if (i < 60)
                {
                    f = (b & c) | (b & d) | (c & d);
                    k = 0x8F1BBCDC;
                }
                else
                {
                    f = b ^ c ^ d;
                    k = 0xCA62C1D6;
                }

                uint temp = RotateLeft(a, 5) + f + e + k + words[i];
                e = d;
                d = c;
                c = RotateLeft(b, 30);
                b = a;
                a = temp;
            }

            h0 += a;
            h1 += b;
            h2 += c;
            h3 += d;
            h4 += e;
        }

        byte[] hash = new byte[20];
        Buffer.BlockCopy(BitConverter.GetBytes(h0), 0, hash, 0, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(h1), 0, hash, 4, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(h2), 0, hash, 8, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(h3), 0, hash, 12, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(h4), 0, hash, 16, 4);

        for (int i = 0; i < 20; i += 4)
        {
            Array.Reverse(hash, i, 4);
        }

        return hash;
    }

    public static string ComputeHashString(string message)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(message);
        byte[] hashBytes = ComputeHash(bytes);
        StringBuilder hash = new StringBuilder();
        foreach (byte b in hashBytes)
        {
            hash.AppendFormat("{0:x2}", b);
        }
        return hash.ToString();
    }

    public byte[] ComputeHash(string input)
    {
        return ComputeHash(Encoding.UTF8.GetBytes(input));
    }
}
