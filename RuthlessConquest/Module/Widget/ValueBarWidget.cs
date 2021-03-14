using System;
using Engine;
using Engine.Graphics;

namespace Game
{
    public class ValueBarWidget : Widget
    {
        public ValueBarWidget()
        {
            this.IsHitTestVisible = false;
            this.BarSize = new Vector2(24f);
            this.BarBlending = true;
            this.TextureLinearFilter = true;
        }

        public float Value
        {
            get
            {
                return this.m_value;
            }
            set
            {
                this.m_value = MathUtils.Saturate(value);
            }
        }

        public int BarsCount
        {
            get
            {
                return this.m_barsCount;
            }
            set
            {
                this.m_barsCount = MathUtils.Clamp(value, 1, 1000);
            }
        }

        public bool FlipDirection { get; set; }

        public Vector2 BarSize { get; set; }

        public float Spacing { get; set; }

        public Color LitBarColor
        {
            get
            {
                return this.m_litBarColor;
            }
            set
            {
                this.m_litBarColor = value;
            }
        }

        public Color LitBarColor2
        {
            get
            {
                return this.m_litBarColor2;
            }
            set
            {
                this.m_litBarColor2 = value;
            }
        }

        public Color UnlitBarColor
        {
            get
            {
                return this.m_unlitBarColor;
            }
            set
            {
                this.m_unlitBarColor = value;
            }
        }

        public bool BarBlending { get; set; }

        public bool HalfBars { get; set; }

        public Subtexture BarSubtexture
        {
            get
            {
                return this.m_barSubtexture;
            }
            set
            {
                if (value != this.m_barSubtexture)
                {
                    this.m_barSubtexture = value;
                    this.m_batch = null;
                }
            }
        }

        public bool TextureLinearFilter
        {
            get
            {
                return this.m_textureLinearFilter;
            }
            set
            {
                if (value != this.m_textureLinearFilter)
                {
                    this.m_textureLinearFilter = value;
                    this.m_batch = null;
                }
            }
        }

        public LayoutDirection LayoutDirection
        {
            get
            {
                return this.m_layoutDirection;
            }
            set
            {
                this.m_layoutDirection = value;
            }
        }

        public void Flash(int count)
        {
            this.m_flashCount = MathUtils.Max(this.m_flashCount, count);
        }

        public override void Draw()
        {
            if (this.m_batch == null)
            {
                if (this.BarSubtexture != null)
                {
                    this.m_batch = WidgetsManager.PrimitivesRenderer2D.TexturedBatch(this.BarSubtexture.Texture, false, 0, DepthStencilState.None, null, null, this.TextureLinearFilter ? SamplerState.LinearClamp : SamplerState.PointClamp);
                }
                else
                {
                    this.m_batch = WidgetsManager.PrimitivesRenderer2D.FlatBatch(0, DepthStencilState.None, null, null);
                }
            }
            int start = 0;
            int count;
            if (this.m_batch is TexturedBatch2D)
            {
                count = ((TexturedBatch2D)this.m_batch).TriangleVertices.Count;
            }
            else
            {
                start = ((FlatBatch2D)this.m_batch).LineVertices.Count;
                count = ((FlatBatch2D)this.m_batch).TriangleVertices.Count;
            }
            Vector2 zero = Vector2.Zero;
            if (this.m_layoutDirection == LayoutDirection.Horizontal)
            {
                zero.X += this.Spacing / 2f;
            }
            else
            {
                zero.Y += this.Spacing / 2f;
            }
            int num = this.HalfBars ? 1 : 2;
            for (int i = 0; i < 2 * this.BarsCount; i += num)
            {
                bool flag = i % 2 == 0;
                float num2 = 0.5f * i;
                float num3;
                if (this.FlipDirection)
                {
                    num3 = MathUtils.Clamp((this.Value - (BarsCount - num2 - 1f) / BarsCount) * BarsCount, 0f, 1f);
                }
                else
                {
                    num3 = MathUtils.Clamp((this.Value - num2 / BarsCount) * BarsCount, 0f, 1f);
                }
                if (!this.BarBlending)
                {
                    num3 = MathUtils.Ceiling(num3);
                }
                float s = (this.m_flashCount > 0f) ? (1f - MathUtils.Abs(MathUtils.Sin(this.m_flashCount * 3.14159274f))) : 1f;
                Color c = this.LitBarColor;
                if (this.LitBarColor2 != Color.Transparent && this.BarsCount > 1)
                {
                    c = Color.Lerp(this.LitBarColor, this.LitBarColor2, num2 / (this.BarsCount - 1));
                }
                Color color = Color.Lerp(this.UnlitBarColor, c, num3) * s * GlobalColorTransform;
                if (this.HalfBars)
                {
                    if (flag)
                    {
                        Vector2 zero2 = Vector2.Zero;
                        Vector2 vector = (this.m_layoutDirection == LayoutDirection.Horizontal) ? new Vector2(0.5f, 1f) : new Vector2(1f, 0.5f);
                        if (this.m_batch is TexturedBatch2D)
                        {
                            Vector2 topLeft = this.BarSubtexture.TopLeft;
                            Vector2 texCoord = new Vector2(MathUtils.Lerp(this.BarSubtexture.TopLeft.X, this.BarSubtexture.BottomRight.X, vector.X), MathUtils.Lerp(this.BarSubtexture.TopLeft.Y, this.BarSubtexture.BottomRight.Y, vector.Y));
                            ((TexturedBatch2D)this.m_batch).QueueQuad(zero + zero2 * this.BarSize, zero + vector * this.BarSize, 0f, topLeft, texCoord, color);
                        }
                        else
                        {
                            ((FlatBatch2D)this.m_batch).QueueQuad(zero + zero2 * this.BarSize, zero + vector * this.BarSize, 0f, color);
                        }
                    }
                    else
                    {
                        Vector2 vector2 = (this.m_layoutDirection == LayoutDirection.Horizontal) ? new Vector2(0.5f, 0f) : new Vector2(0f, 0.5f);
                        Vector2 one = Vector2.One;
                        if (this.m_batch is TexturedBatch2D)
                        {
                            Vector2 texCoord2 = new Vector2(MathUtils.Lerp(this.BarSubtexture.TopLeft.X, this.BarSubtexture.BottomRight.X, vector2.X), MathUtils.Lerp(this.BarSubtexture.TopLeft.Y, this.BarSubtexture.BottomRight.Y, vector2.Y));
                            Vector2 bottomRight = this.BarSubtexture.BottomRight;
                            ((TexturedBatch2D)this.m_batch).QueueQuad(zero + vector2 * this.BarSize, zero + one * this.BarSize, 0f, texCoord2, bottomRight, color);
                        }
                        else
                        {
                            ((FlatBatch2D)this.m_batch).QueueQuad(zero + vector2 * this.BarSize, zero + one * this.BarSize, 0f, color);
                        }
                    }
                }
                else
                {
                    Vector2 zero3 = Vector2.Zero;
                    Vector2 one2 = Vector2.One;
                    if (this.m_batch is TexturedBatch2D)
                    {
                        Vector2 topLeft2 = this.BarSubtexture.TopLeft;
                        Vector2 bottomRight2 = this.BarSubtexture.BottomRight;
                        ((TexturedBatch2D)this.m_batch).QueueQuad(zero + zero3 * this.BarSize, zero + one2 * this.BarSize, 0f, topLeft2, bottomRight2, color);
                    }
                    else
                    {
                        ((FlatBatch2D)this.m_batch).QueueQuad(zero + zero3 * this.BarSize, zero + one2 * this.BarSize, 0f, color);
                        ((FlatBatch2D)this.m_batch).QueueRectangle(zero + zero3 * this.BarSize, zero + one2 * this.BarSize, 0f, Color.MultiplyColorOnly(color, 0.75f));
                    }
                }
                if (!flag || !this.HalfBars)
                {
                    if (this.m_layoutDirection == LayoutDirection.Horizontal)
                    {
                        zero.X += this.BarSize.X + this.Spacing;
                    }
                    else
                    {
                        zero.Y += this.BarSize.Y + this.Spacing;
                    }
                }
            }
            if (this.m_batch is TexturedBatch2D)
            {
                ((TexturedBatch2D)this.m_batch).TransformTriangles(GlobalTransform, count, -1);
            }
            else
            {
                ((FlatBatch2D)this.m_batch).TransformLines(GlobalTransform, start, -1);
                ((FlatBatch2D)this.m_batch).TransformTriangles(GlobalTransform, count, -1);
            }
            this.m_flashCount = MathUtils.Max(this.m_flashCount - 4f * Time.FrameDuration, 0f);
        }

        public override void MeasureOverride(Vector2 parentAvailableSize)
        {
            IsDrawRequired = true;
            if (this.m_layoutDirection == LayoutDirection.Horizontal)
            {
                DesiredSize = new Vector2((this.BarSize.X + this.Spacing) * BarsCount, this.BarSize.Y);
                return;
            }
            DesiredSize = new Vector2(this.BarSize.X, (this.BarSize.Y + this.Spacing) * BarsCount);
        }

        private float m_value;

        private int m_barsCount = 8;

        private Color m_litBarColor = Colors.High;

        private Color m_litBarColor2 = Color.Transparent;

        private Color m_unlitBarColor = Colors.HighDim;

        private Subtexture m_barSubtexture;

        private LayoutDirection m_layoutDirection;

        private float m_flashCount;

        private bool m_textureLinearFilter;

        private BaseBatch m_batch;
    }
}
