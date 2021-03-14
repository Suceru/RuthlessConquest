using System;
using Engine;

namespace Game
{
    public static class Colors
    {
        public static void InitializeColorScheme()
        {
            int num = (SettingsManager.LastColorScheme + 1) % 4;
            SettingsManager.LastColorScheme = num;
            if (num == 0)
            {
                //new ValueTuple<byte, byte, byte>new Color
                High = new ValueTuple<byte, byte, byte>(100, 150, byte.MaxValue);
                HighDim = Color.MultiplyColorOnly(High, 0.7f);
                HighDark = Color.MultiplyColorOnly(High, 0.4f);
                Fore = Color.White;
                ForeDim = Color.MultiplyColorOnly(Fore, 0.5f);
                ForeDisabled = Color.MultiplyColorOnly(Fore, 0.31f);
                ForeDark = Fore * 0.2f;
                Back = Color.Black;
                Panel = new Color(232, 232, 255) * 0.7f;
                return;
            }
            if (num == 1)
            {
                High = new ValueTuple<byte, byte, byte>(byte.MaxValue, 90, 160);
                HighDim = Color.MultiplyColorOnly(High, 0.5f);
                HighDark = Color.MultiplyColorOnly(High, 0.3f);
                Fore = Color.White;
                ForeDim = Color.MultiplyColorOnly(Fore, 0.5f);
                ForeDisabled = Color.MultiplyColorOnly(Fore, 0.31f);
                ForeDark = Fore * 0.2f;
                Back = Color.Black;
                Panel = new Color(255, 244, 255) * 0.7f;
                return;
            }
            if (num == 2)
            {
                High = new ValueTuple<byte, byte, byte>(60, 242, 92);
                HighDim = Color.MultiplyColorOnly(High, 0.5f);
                HighDark = Color.MultiplyColorOnly(High, 0.3f);
                Fore = Color.White;
                ForeDim = Color.MultiplyColorOnly(Fore, 0.5f);
                ForeDisabled = Color.MultiplyColorOnly(Fore, 0.31f);
                ForeDark = Fore * 0.2f;
                Back = Color.Black;
                Panel = new Color(236, 255, 255) * 0.7f;
                return;
            }
            High = new ValueTuple<byte, byte, byte>(byte.MaxValue, 185, 40);
            HighDim = Color.MultiplyColorOnly(High, 0.5f);
            HighDark = Color.MultiplyColorOnly(High, 0.3f);
            Fore = Color.White;
            ForeDim = Color.MultiplyColorOnly(Fore, 0.5f);
            ForeDisabled = Color.MultiplyColorOnly(Fore, 0.31f);
            ForeDark = Fore * 0.2f;
            Back = Color.Black;
            Panel = new Color(255, 252, 236) * 0.7f;
        }

        public static Color High;

        public static Color HighDim;

        public static Color HighDark;

        public static Color Fore;

        public static Color ForeDim;

        public static Color ForeDisabled;

        public static Color ForeDark;

        public static Color Back;

        public static Color Panel;
    }
}
