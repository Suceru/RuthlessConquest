using System;
using System.Diagnostics;

namespace Game
{
    [Conditional("DEBUG")]
    public class DebugMenuItemAttribute : DebugItemAttribute
    {
        public DebugMenuItemAttribute()
        {
            this.Step = 1.0;
        }

        public DebugMenuItemAttribute(double step)
        {
            this.Step = step;
        }

        public double Step;
    }
}
