using System;
using System.Diagnostics;

namespace Game
{
    [Conditional("DEBUG")]
    public class FloatListDebugMenuItemAttribute : DebugMenuItemAttribute
    {
        public FloatListDebugMenuItemAttribute(float[] items, int precision, string unit) : base(0.0)
        {
            this.Items = items;
            this.Precision = precision;
            this.Unit = unit;
        }

        public float[] Items;
    }
}
