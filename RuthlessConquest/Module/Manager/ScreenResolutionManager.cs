using System;
using Engine;

namespace Game
{
    public static class ScreenResolutionManager
    {
        static ScreenResolutionManager()
        {
            ApproximateScreenDpi = MathUtils.Clamp(ApproximateScreenDpi, 96f, 800f);
        }

        public static float ApproximateScreenDpi { get; private set; } = 100f;

        public static float ApproximateWindowInches
        {
            get
            {
                return MathUtils.Sqrt(Window.Size.X * Window.Size.X + Window.Size.Y * Window.Size.Y) / ApproximateScreenDpi;
            }
        }
    }
}
