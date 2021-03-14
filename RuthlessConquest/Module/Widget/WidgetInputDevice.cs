using System;

namespace Game
{
    [Flags]
    public enum WidgetInputDevice
    {
        None = 0,
        Keyboard = 1,
        Mouse = 2,
        Touch = 4,
        GamePad1 = 8,
        GamePad2 = 16,
        GamePad3 = 32,
        GamePad4 = 64,
        Gamepads = 120,
        All = 127
    }
}
