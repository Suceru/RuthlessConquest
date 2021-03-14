using System;

namespace Game
{
    internal static class Crc32
    {
        static Crc32()
        {
            uint num = 3988292384U;
            Table = new uint[256];
            uint num2 = 0U;
            while (num2 < (ulong)Table.Length)
            {
                uint num3 = num2;
                for (int i = 8; i > 0; i--)
                {
                    if ((num3 & 1U) == 1U)
                    {
                        num3 = (num3 >> 1 ^ num);
                    }
                    else
                    {
                        num3 >>= 1;
                    }
                }
                Table[(int)num2] = num3;
                num2 += 1U;
            }
        }

        public static uint ComputeChecksum(byte[] bytes)
        {
            uint num = uint.MaxValue;
            for (int i = 0; i < bytes.Length; i++)
            {
                byte b = (byte)((num & 255U) ^ bytes[i]);
                num = (num >> 8 ^ Table[b]);
            }
            return ~num;
        }

        private static uint[] Table;
    }
}
