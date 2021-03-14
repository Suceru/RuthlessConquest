using System;
using Engine;
using Engine.Content;
using Engine.Graphics;
using Engine.Input;

namespace Game
{
    public class WidgetInput
    {
        public bool Any { get; private set; }

        public bool Ok { get; private set; }

        public bool Cancel { get; private set; }

        public bool Back { get; private set; }

        public bool Left { get; private set; }

        public bool Right { get; private set; }

        public bool Up { get; private set; }

        public bool Down { get; private set; }

        public Vector2? Press { get; private set; }

        public Vector2? Tap { get; private set; }

        public Segment2? Click { get; private set; }

        public Segment2? SecondaryClick { get; private set; }

        public Vector2? Drag { get; private set; }

        public Vector2? Hold { get; private set; }

        public float HoldTime { get; private set; }

        public Vector3? Scroll { get; private set; }

        public Key? LastKey
        {
            get
            {
                if (this.m_isCleared || (this.Devices & WidgetInputDevice.Keyboard) == WidgetInputDevice.None)
                {
                    return null;
                }
                return Keyboard.LastKey;
            }
        }

        public char? LastChar
        {
            get
            {
                if (this.m_isCleared || (this.Devices & WidgetInputDevice.Keyboard) == WidgetInputDevice.None)
                {
                    return null;
                }
                return Keyboard.LastChar;
            }
        }

        public bool IsKeyDown(Key key)
        {
            return !this.m_isCleared && (this.Devices & WidgetInputDevice.Keyboard) != WidgetInputDevice.None && Keyboard.IsKeyDown(key);
        }

        public bool IsKeyDownOnce(Key key)
        {
            return !this.m_isCleared && (this.Devices & WidgetInputDevice.Keyboard) != WidgetInputDevice.None && Keyboard.IsKeyDownOnce(key);
        }

        public bool IsKeyDownRepeat(Key key)
        {
            return !this.m_isCleared && (this.Devices & WidgetInputDevice.Keyboard) != WidgetInputDevice.None && Keyboard.IsKeyDownRepeat(key);
        }

        public void EnterText(string title, string text, int maxLength, Action<string> handler)
        {
            DialogsManager.ShowDialog(null, new TextBoxDialog(title, text, maxLength, handler), true);
        }

        public bool IsMouseCursorVisible
        {
            get
            {
                return (this.Devices & WidgetInputDevice.Mouse) != WidgetInputDevice.None && this.m_isMouseVisible;
            }
            set
            {
                this.m_isMouseVisible = value;
            }
        }

        public void SetMouseCursorPosition(int x, int y)
        {
            if ((this.Devices & WidgetInputDevice.Mouse) != WidgetInputDevice.None)
            {
                Mouse.SetMousePosition(x, y);
            }
        }

        public Point2? MousePosition
        {
            get
            {
                if (!this.m_isCleared && (this.Devices & WidgetInputDevice.Mouse) != WidgetInputDevice.None)
                {
                    return Mouse.MousePosition;
                }
                return null;
            }
        }

        public Point2 MouseMovement
        {
            get
            {
                if (!this.m_isCleared && (this.Devices & WidgetInputDevice.Mouse) != WidgetInputDevice.None)
                {
                    return Mouse.MouseMovement;
                }
                return Point2.Zero;
            }
        }

        public int MouseWheelMovement
        {
            get
            {
                if (!this.m_isCleared && (this.Devices & WidgetInputDevice.Mouse) != WidgetInputDevice.None)
                {
                    return Mouse.MouseWheelMovement;
                }
                return 0;
            }
        }

        public bool IsMouseButtonDown(MouseButton button)
        {
            return !this.m_isCleared && (this.Devices & WidgetInputDevice.Mouse) != WidgetInputDevice.None && Mouse.IsMouseButtonDown(button);
        }

        public bool IsMouseButtonDownOnce(MouseButton button)
        {
            return !this.m_isCleared && (this.Devices & WidgetInputDevice.Mouse) != WidgetInputDevice.None && Mouse.IsMouseButtonDownOnce(button);
        }

        public bool IsPadCursorVisible
        {
            get
            {
                return this.m_isPadVisible && (((this.Devices & WidgetInputDevice.GamePad1) != WidgetInputDevice.None && GamePad.IsConnected(0)) || ((this.Devices & WidgetInputDevice.GamePad2) != WidgetInputDevice.None && GamePad.IsConnected(1)) || ((this.Devices & WidgetInputDevice.GamePad3) != WidgetInputDevice.None && GamePad.IsConnected(2)) || ((this.Devices & WidgetInputDevice.GamePad4) != WidgetInputDevice.None && GamePad.IsConnected(3)));
            }
            set
            {
                this.m_isPadVisible = value;
            }
        }

        public Vector2 PadCursorPosition
        {
            get
            {
                return this.m_padCursorPosition;
            }
            set
            {
                Vector2 vector;
                Vector2 max;
                if (this.Widget != null)
                {
                    vector = this.Widget.GlobalBounds.Min;
                    max = this.Widget.GlobalBounds.Max;
                }
                else
                {
                    vector = Vector2.Zero;
                    max = new Vector2(Window.Size);
                }
                value.X = MathUtils.Clamp(value.X, vector.X, max.X - 1f);
                value.Y = MathUtils.Clamp(value.Y, vector.Y, max.Y - 1f);
                this.m_padCursorPosition = value;
            }
        }

        public Vector2 GetPadStickPosition(GamePadStick stick, float deadZone = 0f)
        {
            if (this.m_isCleared)
            {
                return Vector2.Zero;
            }
            Vector2 vector = Vector2.Zero;
            for (int i = 0; i < 4; i++)
            {
                if ((this.Devices & (WidgetInputDevice)(8 << i)) != WidgetInputDevice.None)
                {
                    vector += GamePad.GetStickPosition(i, stick, deadZone);
                }
            }
            if (vector.LengthSquared() <= 1f)
            {
                return vector;
            }
            return Vector2.Normalize(vector);
        }

        public float GetPadTriggerPosition(GamePadTrigger trigger, float deadZone = 0f)
        {
            if (this.m_isCleared)
            {
                return 0f;
            }
            float num = 0f;
            for (int i = 0; i < 4; i++)
            {
                if ((this.Devices & (WidgetInputDevice)(8 << i)) != WidgetInputDevice.None)
                {
                    num += GamePad.GetTriggerPosition(i, trigger, deadZone);
                }
            }
            return MathUtils.Min(num, 1f);
        }

        public bool IsPadButtonDown(GamePadButton button)
        {
            if (this.m_isCleared)
            {
                return false;
            }
            for (int i = 0; i < 4; i++)
            {
                if ((this.Devices & (WidgetInputDevice)(8 << i)) != WidgetInputDevice.None && GamePad.IsButtonDown(i, button))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsPadButtonDownOnce(GamePadButton button)
        {
            if (this.m_isCleared)
            {
                return false;
            }
            for (int i = 0; i < 4; i++)
            {
                if ((this.Devices & (WidgetInputDevice)(8 << i)) != WidgetInputDevice.None && GamePad.IsButtonDownOnce(i, button))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsPadButtonDownRepeat(GamePadButton button)
        {
            if (this.m_isCleared)
            {
                return false;
            }
            for (int i = 0; i < 4; i++)
            {
                if ((this.Devices & (WidgetInputDevice)(8 << i)) != WidgetInputDevice.None && GamePad.IsButtonDownRepeat(i, button))
                {
                    return true;
                }
            }
            return false;
        }

        public ReadOnlyList<TouchLocation> TouchLocations
        {
            get
            {
                if (!this.m_isCleared && (this.Devices & WidgetInputDevice.Touch) != WidgetInputDevice.None)
                {
                    return Touch.TouchLocations;
                }
                return ReadOnlyList<TouchLocation>.Empty;
            }
        }

        public static WidgetInput EmptyInput { get; } = new WidgetInput(WidgetInputDevice.None);

        public Widget Widget
        {
            get
            {
                return this.m_widget;
            }
        }

        public WidgetInputDevice Devices { get; private set; }

        public WidgetInput(WidgetInputDevice devices = WidgetInputDevice.All)
        {
            this.Devices = devices;
        }

        public void Clear()
        {
            this.m_isCleared = true;
            this.m_mouseDownPoint = null;
            this.m_mouseDragInProgress = false;
            this.m_touchCleared = true;
            this.m_padDownPoint = null;
            this.ClearInput();
        }

        public void Update()
        {
            this.m_isCleared = false;
            this.ClearInput();
            if (!Window.IsActive)
            {
                return;
            }
            if ((this.Devices & WidgetInputDevice.Keyboard) != WidgetInputDevice.None)
            {
                this.UpdateInputFromKeyboard();
            }
            if ((this.Devices & WidgetInputDevice.Mouse) != WidgetInputDevice.None)
            {
                this.UpdateInputFromMouse();
            }
            if ((this.Devices & WidgetInputDevice.Gamepads) != WidgetInputDevice.None)
            {
                this.UpdateInputFromGamepads();
            }
            if (this.IsPadCursorVisible)
            {
                Vector2 vector = Vector2.Transform(this.PadCursorPosition, this.Widget.InvertedGlobalTransform);
                Vector2 padStickPosition = this.GetPadStickPosition(GamePadStick.Left, SettingsManager.GamepadDeadZone);
                Vector2 vector2 = new Vector2(padStickPosition.X, -padStickPosition.Y);
                vector2 = 1200f * SettingsManager.GamepadCursorSpeed * vector2.LengthSquared() * Vector2.Normalize(vector2) * Time.FrameDuration;
                vector += vector2;
                this.PadCursorPosition = Vector2.Transform(vector, this.Widget.GlobalTransform);
                Texture2D texture2D;
                if (this.m_padDragInProgress)
                {
                    texture2D = ContentCache.Get<Texture2D>("Textures/Gui/PadCursorDrag", true);
                }
                else if (this.m_padDownPoint != null)
                {
                    texture2D = ContentCache.Get<Texture2D>("Textures/Gui/PadCursorDown", true);
                }
                else
                {
                    texture2D = ContentCache.Get<Texture2D>("Textures/Gui/PadCursor", true);
                }
                TexturedBatch2D texturedBatch2D = WidgetsManager.CursorPrimitivesRenderer2D.TexturedBatch(texture2D, false, 0, null, null, null, null);
                vector = Vector2.Transform(this.PadCursorPosition, this.Widget.InvertedGlobalTransform);
                Vector2 corner = vector;
                Vector2 corner2 = vector + new Vector2(texture2D.Width, texture2D.Height) * 0.8f;
                int count = texturedBatch2D.TriangleVertices.Count;
                texturedBatch2D.QueueQuad(corner, corner2, 0f, Vector2.Zero, Vector2.One, Color.White);
                texturedBatch2D.TransformTriangles(this.Widget.GlobalTransform, count, -1);
            }
            if ((this.Devices & WidgetInputDevice.Touch) != WidgetInputDevice.None)
            {
                this.UpdateInputFromTouch();
            }
        }

        private void ClearInput()
        {
            this.Any = false;
            this.Ok = false;
            this.Cancel = false;
            this.Back = false;
            this.Left = false;
            this.Right = false;
            this.Up = false;
            this.Down = false;
            this.Press = null;
            this.Tap = null;
            this.Click = null;
            this.SecondaryClick = null;
            this.Drag = null;
            this.Hold = null;
            this.HoldTime = 0f;
            this.Scroll = null;
        }

        private void UpdateInputFromKeyboard()
        {
            if (this.LastKey != null)
            {
                Key? lastKey = this.LastKey;
                Key key = Key.Escape;
                if (!(lastKey.GetValueOrDefault() == key & lastKey != null))
                {
                    this.Any = true;
                }
            }
            if (this.IsKeyDownOnce(Key.Escape))
            {
                this.Back = true;
                this.Cancel = true;
            }
            if (this.IsKeyDownOnce(Key.Enter))
            {
                this.Ok = true;
            }
            if (this.IsKeyDownRepeat(Key.LeftArrow))
            {
                this.Left = true;
            }
            if (this.IsKeyDownRepeat(Key.RightArrow))
            {
                this.Right = true;
            }
            if (this.IsKeyDownRepeat(Key.UpArrow))
            {
                this.Up = true;
            }
            if (this.IsKeyDownRepeat(Key.DownArrow))
            {
                this.Down = true;
            }
            this.Back |= Keyboard.IsKeyDownOnce(Key.Back);
        }

        private void UpdateInputFromMouse()
        {
            if (this.IsMouseButtonDownOnce(MouseButton.Left))
            {
                this.Any = true;
            }
            if (this.IsMouseCursorVisible && this.MousePosition != null)
            {
                Vector2 vector = new Vector2(this.MousePosition.Value);
                if (this.IsMouseButtonDown(MouseButton.Left) || this.IsMouseButtonDown(MouseButton.Right))
                {
                    this.Press = new Vector2?(vector);
                }
                if (this.IsMouseButtonDownOnce(MouseButton.Left) || this.IsMouseButtonDownOnce(MouseButton.Right))
                {
                    this.Tap = new Vector2?(vector);
                    this.m_mouseDownPoint = new Vector2?(vector);
                    this.m_mouseDownButton = (this.IsMouseButtonDownOnce(MouseButton.Left) ? MouseButton.Left : MouseButton.Right);
                    this.m_mouseDragTime = Time.FrameStartTime;
                }
                if (!this.IsMouseButtonDown(MouseButton.Left) && this.m_mouseDownPoint != null && this.m_mouseDownButton == MouseButton.Left)
                {
                    this.Click = new Segment2?(new Segment2(this.m_mouseDownPoint.Value, vector));
                }
                if (!this.IsMouseButtonDown(MouseButton.Right) && this.m_mouseDownPoint != null && this.m_mouseDownButton == MouseButton.Right)
                {
                    this.SecondaryClick = new Segment2?(new Segment2(this.m_mouseDownPoint.Value, vector));
                }
                if (this.MouseWheelMovement != 0)
                {
                    this.Scroll = new Vector3?(new Vector3(vector, MouseWheelMovement / 120f));
                }
                if (this.m_mouseHoldInProgress && this.m_mouseDownPoint != null)
                {
                    this.Hold = new Vector2?(this.m_mouseDownPoint.Value);
                    this.HoldTime = (float)(Time.FrameStartTime - this.m_mouseDragTime);
                }
                if (this.m_mouseDragInProgress)
                {
                    this.Drag = new Vector2?(vector);
                }
                else if ((this.IsMouseButtonDown(MouseButton.Left) || this.IsMouseButtonDown(MouseButton.Right)) && this.m_mouseDownPoint != null)
                {
                    if (Vector2.Distance(this.m_mouseDownPoint.Value, vector) > SettingsManager.MinimumDragDistance * WidgetsManager.Scale)
                    {
                        this.m_mouseDragInProgress = true;
                        this.Drag = new Vector2?(this.m_mouseDownPoint.Value);
                    }
                    else if (Time.FrameStartTime - this.m_mouseDragTime > SettingsManager.MinimumHoldDuration)
                    {
                        this.m_mouseHoldInProgress = true;
                    }
                }
            }
            if (!this.IsMouseButtonDown(MouseButton.Left) && !this.IsMouseButtonDown(MouseButton.Right))
            {
                this.m_mouseDragInProgress = false;
                this.m_mouseHoldInProgress = false;
                this.m_mouseDownPoint = null;
            }
        }

        private void UpdateInputFromGamepads()
        {
            if (this.IsPadButtonDownRepeat(GamePadButton.DPadLeft))
            {
                this.Left = true;
            }
            if (this.IsPadButtonDownRepeat(GamePadButton.DPadRight))
            {
                this.Right = true;
            }
            if (this.IsPadButtonDownRepeat(GamePadButton.DPadUp))
            {
                this.Up = true;
            }
            if (this.IsPadButtonDownRepeat(GamePadButton.DPadDown))
            {
                this.Down = true;
            }
            if (this.IsPadCursorVisible)
            {
                if (this.IsPadButtonDownRepeat(GamePadButton.DPadUp))
                {
                    this.Scroll = new Vector3?(new Vector3(this.PadCursorPosition, 1f));
                }
                if (this.IsPadButtonDownRepeat(GamePadButton.DPadDown))
                {
                    this.Scroll = new Vector3?(new Vector3(this.PadCursorPosition, -1f));
                }
                if (this.IsPadButtonDown(GamePadButton.A))
                {
                    this.Press = new Vector2?(this.PadCursorPosition);
                }
                if (this.IsPadButtonDownOnce(GamePadButton.A))
                {
                    this.Ok = true;
                    this.Tap = new Vector2?(this.PadCursorPosition);
                    this.m_padDownPoint = new Vector2?(this.PadCursorPosition);
                    this.m_padDragTime = Time.FrameStartTime;
                }
                if (!this.IsPadButtonDown(GamePadButton.A) && this.m_padDownPoint != null)
                {
                    if (this.GetPadTriggerPosition(GamePadTrigger.Left, 0f) > 0.5f)
                    {
                        this.SecondaryClick = new Segment2?(new Segment2(this.m_padDownPoint.Value, this.PadCursorPosition));
                    }
                    else
                    {
                        this.Click = new Segment2?(new Segment2(this.m_padDownPoint.Value, this.PadCursorPosition));
                    }
                }
            }
            if (this.IsPadButtonDownOnce(GamePadButton.A) || this.IsPadButtonDownOnce(GamePadButton.B) || this.IsPadButtonDownOnce(GamePadButton.X) || this.IsPadButtonDownOnce(GamePadButton.Y))
            {
                this.Any = true;
            }
            if (!this.IsPadButtonDown(GamePadButton.A))
            {
                this.m_padDragInProgress = false;
                this.m_padDownPoint = null;
            }
            if (this.IsPadButtonDownOnce(GamePadButton.B))
            {
                this.Cancel = true;
            }
            if (this.IsPadButtonDownOnce(GamePadButton.Back))
            {
                this.Back = true;
            }
            if (this.m_padDragInProgress)
            {
                this.Drag = new Vector2?(this.PadCursorPosition);
                return;
            }
            if (this.IsPadButtonDown(GamePadButton.A) && this.m_padDownPoint != null)
            {
                if (Vector2.Distance(this.m_padDownPoint.Value, this.PadCursorPosition) > SettingsManager.MinimumDragDistance * WidgetsManager.Scale)
                {
                    this.m_padDragInProgress = true;
                    this.Drag = new Vector2?(this.m_padDownPoint.Value);
                    return;
                }
                if (Time.FrameStartTime - this.m_padDragTime > SettingsManager.MinimumHoldDuration)
                {
                    this.Hold = new Vector2?(this.m_padDownPoint.Value);
                    this.HoldTime = (float)(Time.FrameStartTime - this.m_padDragTime);
                }
            }
        }

        private void UpdateInputFromTouch()
        {
            foreach (TouchLocation touchLocation in this.TouchLocations)
            {
                if (touchLocation.State == TouchLocationState.Pressed)
                {
                    if (this.Widget.HitTest(touchLocation.Position))
                    {
                        this.Any = true;
                        this.Tap = new Vector2?(touchLocation.Position);
                        this.Press = new Vector2?(touchLocation.Position);
                        this.m_touchStartPoint = touchLocation.Position;
                        this.m_touchId = new int?(touchLocation.Id);
                        this.m_touchCleared = false;
                        this.m_touchStartTime = Time.FrameStartTime;
                        this.m_touchDragInProgress = false;
                        this.m_touchHoldInProgress = false;
                    }
                }
                else if (touchLocation.State == TouchLocationState.Moved)
                {
                    int? touchId = this.m_touchId;
                    int id = touchLocation.Id;
                    if (touchId.GetValueOrDefault() == id & touchId != null)
                    {
                        this.Press = new Vector2?(touchLocation.Position);
                        if (!this.m_touchCleared)
                        {
                            if (this.m_touchDragInProgress)
                            {
                                this.Drag = new Vector2?(touchLocation.Position);
                            }
                            else if (Vector2.Distance(touchLocation.Position, this.m_touchStartPoint) > SettingsManager.MinimumDragDistance * WidgetsManager.Scale)
                            {
                                this.m_touchDragInProgress = true;
                                this.Drag = new Vector2?(this.m_touchStartPoint);
                            }
                            if (!this.m_touchDragInProgress)
                            {
                                if (this.m_touchHoldInProgress)
                                {
                                    this.Hold = new Vector2?(this.m_touchStartPoint);
                                    this.HoldTime = (float)(Time.FrameStartTime - this.m_touchStartTime);
                                }
                                else if (Time.FrameStartTime - this.m_touchStartTime > SettingsManager.MinimumHoldDuration)
                                {
                                    this.m_touchHoldInProgress = true;
                                }
                            }
                        }
                    }
                }
                else if (touchLocation.State == TouchLocationState.Released)
                {
                    int? touchId = this.m_touchId;
                    int id = touchLocation.Id;
                    if (touchId.GetValueOrDefault() == id & touchId != null)
                    {
                        if (!this.m_touchCleared)
                        {
                            if (this.m_touchDragInProgress)
                            {
                                this.Drag = new Vector2?(touchLocation.Position);
                            }
                            this.Click = new Segment2?(new Segment2(this.m_touchStartPoint, touchLocation.Position));
                        }
                        this.m_touchId = null;
                        this.m_touchCleared = false;
                        this.m_touchDragInProgress = false;
                        this.m_touchHoldInProgress = false;
                    }
                }
            }
        }

        private bool m_isCleared;

        internal Widget m_widget;

        private Vector2? m_mouseDownPoint;

        private MouseButton m_mouseDownButton;

        private double m_mouseDragTime;

        private bool m_mouseDragInProgress;

        private bool m_mouseHoldInProgress;

        private bool m_isMouseVisible = true;

        private int? m_touchId;

        private bool m_touchCleared;

        private Vector2 m_touchStartPoint;

        private double m_touchStartTime;

        private bool m_touchDragInProgress;

        private bool m_touchHoldInProgress;

        private Vector2 m_padCursorPosition;

        private Vector2? m_padDownPoint;

        private double m_padDragTime;

        private bool m_padDragInProgress;

        private bool m_isPadVisible = true;
    }
}
