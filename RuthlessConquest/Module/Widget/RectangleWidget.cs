using System;
using Engine;
using Engine.Graphics;

namespace Game
{
    public class RectangleWidget : Widget
    {
        public RectangleWidget()
        {
            this.Size = new Vector2(float.PositiveInfinity);
            this.TextureLinearFilter = true;
            this.FillColor = Colors.Back;
            this.OutlineColor = Colors.Fore;
            this.OutlineThickness = 1f;
            this.IsHitTestVisible = false;
            this.Texcoord1 = Vector2.Zero;
            this.Texcoord2 = Vector2.One;
        }

        public Vector2 Size { get; set; }

        public float Depth { get; set; }

        public bool DepthWriteEnabled
        {
            get
            {
                return this.m_depthWriteEnabled;
            }
            set
            {
                if (value != this.m_depthWriteEnabled)
                {
                    this.m_depthWriteEnabled = value;
                    this.m_flatBatch = null;
                }
            }
        }

        public Subtexture Subtexture
        {
            get
            {
                return this.m_subtexture;
            }
            set
            {
                if (value != this.m_subtexture)
                {
                    this.m_subtexture = value;
                    this.m_texturedBatch = null;
                }
            }
        }

        public bool TextureWrap
        {
            get
            {
                return this.m_textureWrap;
            }
            set
            {
                if (value != this.m_textureWrap)
                {
                    this.m_textureWrap = value;
                    this.m_texturedBatch = null;
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
                    this.m_texturedBatch = null;
                }
            }
        }

        public bool FlipHorizontal { get; set; }

        public bool FlipVertical { get; set; }

        public Color FillColor { get; set; }

        public Color OutlineColor { get; set; }

        public float OutlineThickness { get; set; }

        public Vector2 Texcoord1 { get; set; }

        public Vector2 Texcoord2 { get; set; }

        public override void Draw()
        {
            if (this.FillColor.A == 0 && (this.OutlineColor.A == 0 || this.OutlineThickness <= 0f))
            {
                return;
            }
            DepthStencilState depthStencilState = this.DepthWriteEnabled ? DepthStencilState.DepthWrite : DepthStencilState.None;
            Matrix globalTransform = GlobalTransform;
            Vector2 zero = Vector2.Zero;
            Vector2 vector = new Vector2(ActualSize.X, 0f);
            Vector2 actualSize = ActualSize;
            Vector2 vector2 = new Vector2(0f, ActualSize.Y);
            Vector2 vector3;
            Vector2.Transform(ref zero, ref globalTransform, out vector3);
            Vector2 vector4;
            Vector2.Transform(ref vector, ref globalTransform, out vector4);
            Vector2 vector5;
            Vector2.Transform(ref actualSize, ref globalTransform, out vector5);
            Vector2 vector6;
            Vector2.Transform(ref vector2, ref globalTransform, out vector6);
            Color color = this.FillColor * GlobalColorTransform;
            if (color.A != 0)
            {
                if (this.Subtexture != null)
                {
                    if (this.m_texturedBatch == null)
                    {
                        SamplerState samplerState = this.TextureWrap ? (this.TextureLinearFilter ? SamplerState.LinearWrap : SamplerState.PointWrap) : (this.TextureLinearFilter ? SamplerState.LinearClamp : SamplerState.PointClamp);
                        this.m_texturedBatch = WidgetsManager.PrimitivesRenderer2D.TexturedBatch(this.Subtexture.Texture, false, 0, depthStencilState, null, null, samplerState);
                    }
                    Vector2 zero2;
                    Vector2 texCoord;
                    Vector2 vector7;
                    Vector2 texCoord2;
                    if (this.TextureWrap)
                    {
                        zero2 = Vector2.Zero;
                        texCoord = new Vector2(ActualSize.X / Subtexture.Texture.Width, 0f);
                        vector7 = new Vector2(ActualSize.X / Subtexture.Texture.Width, ActualSize.Y / Subtexture.Texture.Height);
                        texCoord2 = new Vector2(0f, ActualSize.Y / Subtexture.Texture.Height);
                    }
                    else
                    {
                        zero2.X = MathUtils.Lerp(this.Subtexture.TopLeft.X, this.Subtexture.BottomRight.X, this.Texcoord1.X);
                        zero2.Y = MathUtils.Lerp(this.Subtexture.TopLeft.Y, this.Subtexture.BottomRight.Y, this.Texcoord1.Y);
                        vector7.X = MathUtils.Lerp(this.Subtexture.TopLeft.X, this.Subtexture.BottomRight.X, this.Texcoord2.X);
                        vector7.Y = MathUtils.Lerp(this.Subtexture.TopLeft.Y, this.Subtexture.BottomRight.Y, this.Texcoord2.Y);
                        texCoord = new Vector2(vector7.X, zero2.Y);
                        texCoord2 = new Vector2(zero2.X, vector7.Y);
                    }
                    if (this.FlipHorizontal)
                    {
                        Utilities.Swap<float>(ref zero2.X, ref texCoord.X);
                        Utilities.Swap<float>(ref vector7.X, ref texCoord2.X);
                    }
                    if (this.FlipVertical)
                    {
                        Utilities.Swap<float>(ref zero2.Y, ref vector7.Y);
                        Utilities.Swap<float>(ref texCoord.Y, ref texCoord2.Y);
                    }
                    this.m_texturedBatch.QueueQuad(vector3, vector4, vector5, vector6, this.Depth, zero2, texCoord, vector7, texCoord2, color);
                }
                else
                {
                    if (this.m_flatBatch == null)
                    {
                        this.m_flatBatch = WidgetsManager.PrimitivesRenderer2D.FlatBatch(1, depthStencilState, null, null);
                    }
                    this.m_flatBatch.QueueQuad(vector3, vector4, vector5, vector6, this.Depth, color);
                }
            }
            Color color2 = this.OutlineColor * GlobalColorTransform;
            if (color2.A != 0 && this.OutlineThickness > 0f)
            {
                if (this.m_flatBatch == null)
                {
                    this.m_flatBatch = WidgetsManager.PrimitivesRenderer2D.FlatBatch(1, depthStencilState, null, null);
                }
                Vector2 vector8 = Vector2.Normalize(GlobalTransform.Right.XY);
                Vector2 v = -Vector2.Normalize(GlobalTransform.Up.XY);
                int num = (int)MathUtils.Max(MathUtils.Round(this.OutlineThickness * GlobalTransform.Right.Length()), 1f);
                for (int i = 0; i < num; i++)
                {
                    this.m_flatBatch.QueueLine(vector3, vector4, this.Depth, color2);
                    this.m_flatBatch.QueueLine(vector4, vector5, this.Depth, color2);
                    this.m_flatBatch.QueueLine(vector5, vector6, this.Depth, color2);
                    this.m_flatBatch.QueueLine(vector6, vector3, this.Depth, color2);
                    vector3 += vector8 - v;
                    vector4 += -vector8 - v;
                    vector5 += -vector8 + v;
                    vector6 += vector8 + v;
                }
            }
        }

        public override void MeasureOverride(Vector2 parentAvailableSize)
        {
            IsDrawRequired = (this.FillColor.A != 0 || (this.OutlineColor.A != 0 && this.OutlineThickness > 0f));
            DesiredSize = this.Size;
        }

        private FlatBatch2D m_flatBatch;

        private TexturedBatch2D m_texturedBatch;

        private Subtexture m_subtexture;

        private bool m_textureWrap;

        private bool m_textureLinearFilter;

        private bool m_depthWriteEnabled;
    }
}
