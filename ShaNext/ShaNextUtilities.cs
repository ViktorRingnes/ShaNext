namespace ShaNext.ShaNext
{
    public static class ShaNextUtilities
    {
        public static byte[] PadMessage(byte[] message)
        {
            ulong bitLength = (ulong)message.Length * 8;
            int padLength = (message.Length % 64 < 56) ? (56 - message.Length % 64) : (120 - message.Length % 64);
            byte[] paddedMessage = new byte[message.Length + padLength + 8];
            Buffer.BlockCopy(message, 0, paddedMessage, 0, message.Length);
            paddedMessage[message.Length] = 0x80;
            for (int i = paddedMessage.Length - 8; i < paddedMessage.Length - 4; i++)
            {
                paddedMessage[i] = 0x00;
            }
            Buffer.BlockCopy(BitConverter.GetBytes(ReverseBytes(bitLength)), 0, paddedMessage, paddedMessage.Length - 8, 8);
            return paddedMessage;
        }

        public static uint RightRotate(uint x, int n)
        {
            return (x >> n) | (x << (32 - n));
        }

        public static ulong RightRotate(ulong x, int n)
        {
            return (x >> n) | (x << (64 - n));
        }

        public static uint ReverseBytes(uint x)
        {
            return ((x & 0x000000FFU) << 24) |
                   ((x & 0x0000FF00U) << 8) |
                   ((x & 0x00FF0000U) >> 8) |
                   ((x & 0xFF000000U) >> 24);
        }

        public static ulong ReverseBytes(ulong x)
        {
            return ((x & 0x00000000000000FFUL) << 56) |
                   ((x & 0x000000000000FF00UL) << 40) |
                   ((x & 0x0000000000FF0000UL) << 24) |
                   ((x & 0x00000000FF000000UL) << 8) |
                   ((x & 0x000000FF00000000UL) >> 8) |
                   ((x & 0x0000FF0000000000UL) >> 24) |
                   ((x & 0x00FF000000000000UL) >> 40) |
                   ((x & 0xFF00000000000000UL) >> 56);
        }
    }
}
