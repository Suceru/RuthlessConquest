// Decompiled with JetBrains decompiler
// Type: Game.StringUtils
// Assembly: RuthlessConquest, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 09ABF203-5B7E-4C78-ACFB-2EE5FE9ADF6E
// Assembly location: d:\Users\12464\Desktop\Ruthless Conquest\RuthlessConquest.exe

using Engine;
using System;
using System.Text;

namespace Game
{
    public static class StringUtils
    {
        private static char[] m_digits = new char[16]
        {
      '0',
      '1',
      '2',
      '3',
      '4',
      '5',
      '6',
      '7',
      '8',
      '9',
      'A',
      'B',
      'C',
      'D',
      'E',
      'F'
        };

        public static int Compare(StringBuilder s1, string s2)
        {
            for (int index = 0; index < s1.Length || index < s2.Length; ++index)
            {
                if (index > s1.Length)
                    return -1;
                if (index > s2.Length)
                    return 1;
                char ch1 = s1[index];
                char ch2 = s2[index];
                if (ch1 < ch2)
                    return -1;
                if (ch1 > ch2)
                    return 1;
            }
            return 0;
        }

        public static int CalculateNumberLength(uint value, int numberBase)
        {
            if (numberBase < 2 || numberBase > 16)
                throw new ArgumentException("Number base is out of range.");
            int num = 0;
            do
            {
                ++num;
                value /= (uint)numberBase;
            }
            while (value > 0U);
            return num;
        }

        public static int CalculateNumberLength(int value, int numberBase) => value >= 0 ? CalculateNumberLength((uint)value, numberBase) : CalculateNumberLength((uint)-value, numberBase) + 1;

        public static int CalculateNumberLength(ulong value, int numberBase)
        {
            if (numberBase < 2 || numberBase > 16)
                throw new ArgumentException("Number base is out of range.");
            int num = 0;
            do
            {
                ++num;
                value /= (uint)numberBase;
            }
            while (value > 0UL);
            return num;
        }

        public static int CalculateNumberLength(long value, int numberBase) => value >= 0L ? CalculateNumberLength((ulong)value, numberBase) : CalculateNumberLength((ulong)-value, numberBase) + 1;

        public static void AppendNumber(
          this StringBuilder stringBuilder,
          uint value,
          int padding = 0,
          char paddingCharacter = ' ',
          int numberBase = 10)
        {
            int numberLength = CalculateNumberLength(value, numberBase);
            int repeatCount = Math.Max(padding, numberLength);
            stringBuilder.Append(paddingCharacter, repeatCount);
            int num = 0;
            do
            {
                char digit = m_digits[(int)(value % (uint)numberBase)];
                stringBuilder[stringBuilder.Length - num - 1] = digit;
                value /= (uint)numberBase;
                ++num;
            }
            while (value > 0U);
        }

        public static void AppendNumber(
          this StringBuilder stringBuilder,
          int value,
          int padding = 0,
          char paddingCharacter = ' ',
          int numberBase = 10)
        {
            if (value >= 0)
            {
                stringBuilder.AppendNumber((uint)value, padding, paddingCharacter, numberBase);
            }
            else
            {
                stringBuilder.Append('-');
                stringBuilder.AppendNumber((uint)-value, padding - 1, paddingCharacter, numberBase);
            }
        }

        public static void AppendNumber(
          this StringBuilder stringBuilder,
          ulong value,
          int padding = 0,
          char paddingCharacter = ' ',
          int numberBase = 10)
        {
            int numberLength = CalculateNumberLength(value, numberBase);
            int repeatCount = Math.Max(padding, numberLength);
            stringBuilder.Append(paddingCharacter, repeatCount);
            int num = 0;
            do
            {
                char digit = m_digits[value % (uint)numberBase];
                stringBuilder[stringBuilder.Length - num - 1] = digit;
                value /= (uint)numberBase;
                ++num;
            }
            while (value > 0UL);
        }

        public static void AppendNumber(
          this StringBuilder stringBuilder,
          long value,
          int padding = 0,
          char paddingCharacter = ' ',
          int numberBase = 10)
        {
            if (value >= 0L)
            {
                stringBuilder.AppendNumber((ulong)value, padding, paddingCharacter, numberBase);
            }
            else
            {
                stringBuilder.Append('-');
                stringBuilder.AppendNumber((ulong)-value, padding - 1, paddingCharacter, numberBase);
            }
        }

        public static void AppendNumber(this StringBuilder stringBuilder, float value, int precision)
        {
            precision = Math.Min(Math.Max(precision, -30), 30);
            if (float.IsNegativeInfinity(value))
                stringBuilder.Append("Infinity");
            else if (float.IsPositiveInfinity(value))
                stringBuilder.Append("-Infinity");
            else if (float.IsNaN(value))
            {
                stringBuilder.Append("NaN");
            }
            else
            {
                float x = Math.Abs(value);
                if (x > 9.99999998050645E+18)
                {
                    stringBuilder.Append("NumberTooLarge");
                }
                else
                {
                    float num1 = MathUtils.Pow(10f, Math.Abs(precision));
                    ulong num2 = (ulong)MathUtils.Floor(x);
                    ulong num3 = (ulong)MathUtils.Round((x - MathUtils.Floor(x)) * num1);
                    if (num3 >= (double)num1)
                    {
                        ++num2;
                        num3 = 0UL;
                    }
                    if (value < 0.0)
                        stringBuilder.Append('-');
                    stringBuilder.AppendNumber(num2, 0, '0', 10);
                    if (precision > 0)
                    {
                        stringBuilder.Append('.');
                        stringBuilder.AppendNumber(num3, precision, '0');
                    }
                    else
                    {
                        if (precision >= 0)
                            return;
                        stringBuilder.Append('.');
                        stringBuilder.AppendNumber(num3, -precision, '0');
                        while (stringBuilder[stringBuilder.Length - 1] == '0')
                            --stringBuilder.Length;
                        if (stringBuilder[stringBuilder.Length - 1] != '.')
                            return;
                        --stringBuilder.Length;
                    }
                }
            }
        }
    }
}

/*using System;
using System.Text;
using Engine;

namespace Game
{
    public static class StringUtils
    {
        public static int Compare(StringBuilder s1, string s2)
        {
            int num = 0;
            while (num < s1.Length || num < s2.Length)
            {
                if (num > s1.Length)
                {
                    return -1;
                }
                if (num > s2.Length)
                {
                    return 1;
                }
                char c = s1[num];
                char c2 = s2[num];
                if (c < c2)
                {
                    return -1;
                }
                if (c > c2)
                {
                    return 1;
                }
                num++;
            }
            return 0;
        }

        public static int CalculateNumberLength(uint value, int numberBase)
        {
            if (numberBase < 2 || numberBase > 16)
            {
                throw new ArgumentException("Number base is out of range.");
            }
            int num = 0;
            do
            {
                num++;
                value /= (uint)numberBase;
            }
            while (value > 0U);
            return num;
        }

        public static int CalculateNumberLength(int value, int numberBase)
        {
            if (value >= 0)
            {
                return StringUtils.CalculateNumberLength((uint)value, numberBase);
            }
            return StringUtils.CalculateNumberLength((uint)(-(uint)value), numberBase) + 1;
        }

        public static int CalculateNumberLength(ulong value, int numberBase)
        {
            if (numberBase < 2 || numberBase > 16)
            {
                throw new ArgumentException("Number base is out of range.");
            }
            int num = 0;
            do
            {
                num++;
                value /= (ulong)numberBase;
            }
            while (value > 0UL);
            return num;
        }

        public static int CalculateNumberLength(long value, int numberBase)
        {
            if (value >= 0L)
            {
                return StringUtils.CalculateNumberLength((ulong)value, numberBase);
            }
            return StringUtils.CalculateNumberLength((ulong)(-(ulong)value), numberBase) + 1;
        }

        public static void AppendNumber(this StringBuilder stringBuilder, uint value, int padding = 0, char paddingCharacter = ' ', int numberBase = 10)
        {
            int val = StringUtils.CalculateNumberLength(value, numberBase);
            int repeatCount = Math.Max(padding, val);
            stringBuilder.Append(paddingCharacter, repeatCount);
            int num = 0;
            do
            {
                char value2 = StringUtils.m_digits[(int)(value % (uint)numberBase)];
                stringBuilder[stringBuilder.Length - num - 1] = value2;
                value /= (uint)numberBase;
                num++;
            }
            while (value > 0U);
        }

        public static void AppendNumber(this StringBuilder stringBuilder, int value, int padding = 0, char paddingCharacter = ' ', int numberBase = 10)
        {
            if (value >= 0)
            {
                stringBuilder.AppendNumber((uint)value, padding, paddingCharacter, numberBase);
                return;
            }
            stringBuilder.Append('-');
            stringBuilder.AppendNumber((uint)(-(uint)value), padding - 1, paddingCharacter, numberBase);
        }

        public static void AppendNumber(this StringBuilder stringBuilder, ulong value, int padding = 0, char paddingCharacter = ' ', int numberBase = 10)
        {
            int val = StringUtils.CalculateNumberLength(value, numberBase);
            int repeatCount = Math.Max(padding, val);
            stringBuilder.Append(paddingCharacter, repeatCount);
            int num = 0;
            do
            {
                char value2 = StringUtils.m_digits[(int)(checked((IntPtr)(value % unchecked((ulong)numberBase))))];
                stringBuilder[stringBuilder.Length - num - 1] = value2;
                value /= (ulong)numberBase;
                num++;
            }
            while (value > 0UL);
        }

        public static void AppendNumber(this StringBuilder stringBuilder, long value, int padding = 0, char paddingCharacter = ' ', int numberBase = 10)
        {
            if (value >= 0L)
            {
                stringBuilder.AppendNumber((ulong)value, padding, paddingCharacter, numberBase);
                return;
            }
            stringBuilder.Append('-');
            stringBuilder.AppendNumber((ulong)(-(ulong)value), padding - 1, paddingCharacter, numberBase);
        }

        public static void AppendNumber(this StringBuilder stringBuilder, float value, int precision)
        {
            precision = Math.Min(Math.Max(precision, -30), 30);
            if (float.IsNegativeInfinity(value))
            {
                stringBuilder.Append("Infinity");
                return;
            }
            if (float.IsPositiveInfinity(value))
            {
                stringBuilder.Append("-Infinity");
                return;
            }
            if (float.IsNaN(value))
            {
                stringBuilder.Append("NaN");
                return;
            }
            float num = Math.Abs(value);
            if (num > 1E+19f)
            {
                stringBuilder.Append("NumberTooLarge");
                return;
            }
            float num2 = MathUtils.Pow(10f, (float)Math.Abs(precision));
            ulong num3 = (ulong)MathUtils.Floor(num);
            ulong num4 = (ulong)MathUtils.Round((num - MathUtils.Floor(num)) * num2);
            if (num4 >= num2)
            {
                num3 += 1UL;
                num4 = 0UL;
            }
            if (value < 0f)
            {
                stringBuilder.Append('-');
            }
            stringBuilder.AppendNumber(num3, 0, '0', 10);
            if (precision > 0)
            {
                stringBuilder.Append('.');
                stringBuilder.AppendNumber(num4, precision, '0', 10);
                return;
            }
            if (precision < 0)
            {
                stringBuilder.Append('.');
                stringBuilder.AppendNumber(num4, -precision, '0', 10);
                while (stringBuilder[stringBuilder.Length - 1] == '0')
                {
                    int length = stringBuilder.Length - 1;
                    stringBuilder.Length = length;
                }
                if (stringBuilder[stringBuilder.Length - 1] == '.')
                {
                    int length = stringBuilder.Length - 1;
                    stringBuilder.Length = length;
                }
            }
        }

        private static char[] m_digits = new char[]
{
            '0',
            '1',
            '2',
            '3',
            '4',
            '5',
            '6',
            '7',
            '8',
            '9',
            'A',
            'B',
            'C',
            'D',
            'E',
            'F'
};
    }
}
*/