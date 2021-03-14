using System;
using Engine;
using Engine.Content;
using Engine.Graphics;
using Engine.Input;
using Engine.Media;

namespace Game
{
    internal class TextBoxWidget : Widget
    {
        public TextBoxWidget()
        {
            ClampToBounds = true;
            this.Color = Color.White;
            this.Font = ContentCache.Get<BitmapFont>("Fonts/Normal", true);
            this.FontScale = 1f;
            this.Title = string.Empty;
            this.Description = string.Empty;
        }

        public event Action<TextBoxWidget> TextChanged;

        public event Action<TextBoxWidget> Enter;

        public event Action<TextBoxWidget> Escape;

        public event Action<TextBoxWidget> FocusLost;

        public Vector2 Size
        {
            get
            {
                if (this.m_size == null)
                {
                    return Vector2.Zero;
                }
                return this.m_size.Value;
            }
            set
            {
                this.m_size = new Vector2?(value);
            }
        }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Text
        {
            get
            {
                return this.m_text;
            }
            set
            {
                string text = (value.Length > this.MaximumLength) ? value.Substring(0, this.MaximumLength) : value;
                if (text != this.m_text)
                {
                    this.m_text = text;
                    this.CaretPosition = this.CaretPosition;
                    Action<TextBoxWidget> textChanged = this.TextChanged;
                    if (textChanged == null)
                    {
                        return;
                    }
                    textChanged(this);
                }
            }
        }

        public int MaximumLength
        {
            get
            {
                return this.m_maximumLength;
            }
            set
            {
                this.m_maximumLength = MathUtils.Max(value, 0);
                if (this.Text.Length > this.m_maximumLength)
                {
                    this.Text = this.Text.Substring(0, this.m_maximumLength);
                }
            }
        }

        public bool OverwriteMode { get; set; }

        public bool HasFocus
        {
            get
            {
                return this.m_hasFocus;
            }
            set
            {
                if (value != this.m_hasFocus)
                {
                    this.m_hasFocus = value;
                    if (value)
                    {
                        this.CaretPosition = this.m_text.Length;
                        return;
                    }
                    Action<TextBoxWidget> focusLost = this.FocusLost;
                    if (focusLost == null)
                    {
                        return;
                    }
                    focusLost(this);
                }
            }
        }

        public BitmapFont Font
        {
            get
            {
                return this.m_font;
            }
            set
            {
                if (value != this.m_font)
                {
                    this.m_font = value;
                    this.m_fontBatch = null;
                }
            }
        }

        public float FontScale { get; set; }

        public Vector2 FontSpacing { get; set; }

        public Color Color { get; set; }

        public int CaretPosition
        {
            get
            {
                return this.m_caretPosition;
            }
            set
            {
                this.m_caretPosition = MathUtils.Clamp(value, 0, this.Text.Length);
                this.m_focusStartTime = Time.RealTime;
            }
        }

        public override void Update()
        {
            if (this.m_hasFocus)
            {
                if (Input.LastChar != null && !Input.IsKeyDown(Key.Control) && !char.IsControl(Input.LastChar.Value))
                {
                    this.EnterText(new string(Input.LastChar.Value, 1));
                    Input.Clear();
                }
                if (Input.LastKey != null)
                {
                    bool flag = false;
                    Key value = Input.LastKey.Value;
                    if (value == Key.V && Input.IsKeyDown(Key.Control))
                    {
                        this.EnterText(ClipboardManager.ClipboardString);
                        flag = true;
                    }
                    else if (value == Key.BackSpace && this.CaretPosition > 0)
                    {
                        int caretPosition = this.CaretPosition;
                        this.CaretPosition = caretPosition - 1;
                        this.Text = this.Text.Remove(this.CaretPosition, 1);
                        flag = true;
                    }
                    else if (value == Key.Delete)
                    {
                        if (this.CaretPosition < this.m_text.Length)
                        {
                            this.Text = this.Text.Remove(this.CaretPosition, 1);
                            flag = true;
                        }
                    }
                    else if (value == Key.LeftArrow)
                    {
                        int caretPosition = this.CaretPosition;
                        this.CaretPosition = caretPosition - 1;
                        flag = true;
                    }
                    else if (value == Key.RightArrow)
                    {
                        int caretPosition = this.CaretPosition;
                        this.CaretPosition = caretPosition + 1;
                        flag = true;
                    }
                    else if (value == Key.Home)
                    {
                        this.CaretPosition = 0;
                        flag = true;
                    }
                    else if (value == Key.End)
                    {
                        this.CaretPosition = this.m_text.Length;
                        flag = true;
                    }
                    else if (value == Key.Enter)
                    {
                        flag = true;
                        this.HasFocus = false;
                        Action<TextBoxWidget> enter = this.Enter;
                        if (enter != null)
                        {
                            enter(this);
                        }
                    }
                    else if (value == Key.Escape)
                    {
                        flag = true;
                        this.HasFocus = false;
                        Action<TextBoxWidget> escape = this.Escape;
                        if (escape != null)
                        {
                            escape(this);
                        }
                    }
                    if (flag)
                    {
                        Input.Clear();
                    }
                }
            }
            if (Input.Click != null)
            {
                this.HasFocus = (WidgetsManager.HitTest(Input.Click.Value.Start) == this && WidgetsManager.HitTest(Input.Click.Value.End) == this);
            }
        }

        public override void MeasureOverride(Vector2 parentAvailableSize)
        {
            IsDrawRequired = true;
            if (this.m_size != null)
            {
                DesiredSize = this.m_size.Value;
                return;
            }
            if (this.Text.Length == 0)
            {
                DesiredSize = this.Font.MeasureText(" ", new Vector2(this.FontScale), this.FontSpacing);
            }
            else
            {
                DesiredSize = this.Font.MeasureText(this.Text, new Vector2(this.FontScale), this.FontSpacing);
            }
            DesiredSize += new Vector2(1f * this.FontScale * this.Font.Scale, 0f);
        }

        public override void ArrangeOverride()
        {
            base.ArrangeOverride();
        }

        public override void Draw()
        {
            Color color = this.Color * GlobalColorTransform;
            if (!string.IsNullOrEmpty(this.m_text))
            {
                Vector2 position = new Vector2(-this.m_scroll, ActualSize.Y / 2f);
                if (this.m_fontBatch == null)
                {
                    this.m_fontBatch = WidgetsManager.PrimitivesRenderer2D.FontBatch(this.Font, 1, DepthStencilState.None, null, null, null);
                }
                int count = this.m_fontBatch.TriangleVertices.Count;
                this.m_fontBatch.QueueText(this.Text, position, 0f, color, TextAnchor.VerticalCenter, new Vector2(this.FontScale), this.FontSpacing, 0f);
                this.m_fontBatch.TransformTriangles(GlobalTransform, count, -1);
            }
            if (this.m_hasFocus && MathUtils.Remainder(Time.RealTime - this.m_focusStartTime, 0.5) < 0.25)
            {
                float num = this.Font.CalculateCharacterPosition(this.Text, this.CaretPosition, new Vector2(this.FontScale), this.FontSpacing);
                Vector2 vector = new Vector2(0f, ActualSize.Y / 2f) + new Vector2(num - this.m_scroll, 0f);
                if (this.m_hasFocus)
                {
                    if (vector.X < 0f)
                    {
                        this.m_scroll = MathUtils.Max(this.m_scroll + vector.X, 0f);
                    }
                    if (vector.X > ActualSize.X)
                    {
                        this.m_scroll += vector.X - ActualSize.X + 1f;
                    }
                }
                int count2 = this.m_flatBatch.TriangleVertices.Count;
                this.m_flatBatch.QueueQuad(vector - new Vector2(0f, this.Font.GlyphHeight / 2f * this.FontScale * this.Font.Scale), vector + new Vector2(1f, this.Font.GlyphHeight / 2f * this.FontScale * this.Font.Scale), 0f, color);
                this.m_flatBatch.TransformTriangles(GlobalTransform, count2, -1);
            }
        }

        private void EnterText(string s)
        {
            if (this.OverwriteMode)
            {
                if (this.CaretPosition + s.Length <= this.MaximumLength)
                {
                    if (this.CaretPosition < this.m_text.Length)
                    {
                        string text = this.Text;
                        text = text.Remove(this.CaretPosition, s.Length);
                        text = text.Insert(this.CaretPosition, s);
                        this.Text = text;
                    }
                    else
                    {
                        this.Text = this.m_text + s;
                    }
                    this.CaretPosition += s.Length;
                    return;
                }
            }
            else if (this.m_text.Length + s.Length <= this.MaximumLength)
            {
                if (this.CaretPosition < this.m_text.Length)
                {
                    this.Text = this.Text.Insert(this.CaretPosition, s);
                }
                else
                {
                    this.Text = this.m_text + s;
                }
                this.CaretPosition += s.Length;
            }
        }

        private BitmapFont m_font;

        private string m_text = string.Empty;

        private int m_maximumLength = 32;

        private bool m_hasFocus;

        private int m_caretPosition;

        private double m_focusStartTime;

        private float m_scroll;

        private Vector2? m_size;

        private FontBatch2D m_fontBatch;

        private FlatBatch2D m_flatBatch = WidgetsManager.PrimitivesRenderer2D.FlatBatch(1, DepthStencilState.None, null, null);
    }
}
