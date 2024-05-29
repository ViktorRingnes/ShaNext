using System;
using System.Security.Cryptography;
using System.Text;

namespace ShaNext.ShaNext
{
    public class ShaNextUtilities
    {

        public static byte[] PadMessage(byte[] message)
        {
            ulong bitLength = (ulong)message.Length * 8;
            int padLength = (message.Length % 128 < 112) ? (112 - message.Length % 128) : (240 - message.Length % 128);
            byte[] paddedMessage = new byte[message.Length + padLength + 16];
            Buffer.BlockCopy(message, 0, paddedMessage, 0, message.Length);
            paddedMessage[message.Length] = 0x80;
            for (int i = paddedMessage.Length - 16; i < paddedMessage.Length; i++)
            {
                paddedMessage[i] = 0x00;
            }
            Buffer.BlockCopy(BitConverter.GetBytes(bitLength).Reverse().ToArray(), 0, paddedMessage, paddedMessage.Length - 8, 8);
            return paddedMessage;
        }

        public static ulong RightRotate(ulong x, int n)
        {
            return (x >> n) | (x << (64 - n));
        }
    }
}
