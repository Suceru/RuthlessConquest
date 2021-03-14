using System;
using Engine;
using Engine.Graphics;

namespace Game
{
    public class BevelledRectangleWidget : Widget
    {
        public BevelledRectangleWidget()
        {
            this.Size = new Vector2(float.PositiveInfinity);
            this.BevelSize = 2f;
            this.AmbientLight = 0.6f;
            this.DirectionalLight = 0.4f;
            this.TextureScale = 1f;
            this.TextureLinearFilter = false;
            this.CenterColor = Colors.Panel;
            this.BevelColor = Colors.Panel;
            this.ShadowColor = new Color(0, 0, 0, 80);
            this.IsHitTestVisible = false;
        }

        public Vector2 Size { get; set; }

        public float BevelSize { get; set; }

        public float DirectionalLight { get; set; }

        public float AmbientLight { get; set; }

        public Texture2D Texture
        {
            get
            {
                return this.m_texture;
            }
            set
            {
                if (value != this.m_texture)
                {
                    this.m_texture = value;
                    this.m_texturedBatch = null;
                }
            }
        }

        public float TextureScale { get; set; }

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

        public Color CenterColor { get; set; }

        public Color BevelColor { get; set; }

        public Color ShadowColor { get; set; }

        public override void Draw()
        {
            if (this.Texture != null)
            {
                if (this.m_texturedBatch == null)
                {
                    SamplerState samplerState = this.TextureLinearFilter ? SamplerState.LinearWrap : SamplerState.PointWrap;
                    this.m_texturedBatch = WidgetsManager.PrimitivesRenderer2D.TexturedBatch(this.Texture, false, 0, DepthStencilState.None, null, null, samplerState);
                }
                int count = this.m_flatBatch.TriangleVertices.Count;
                int count2 = this.m_texturedBatch.TriangleVertices.Count;
                QueueBevelledRectangle(this.m_texturedBatch, this.m_flatBatch, Vector2.Zero, ActualSize, 0f, this.BevelSize, this.CenterColor * GlobalColorTransform, this.BevelColor * GlobalColorTransform, this.ShadowColor * GlobalColorTransform, this.AmbientLight, this.DirectionalLight, this.TextureScale);
                this.m_flatBatch.TransformTriangles(GlobalTransform, count, -1);
                this.m_texturedBatch.TransformTriangles(GlobalTransform, count2, -1);
                return;
            }
            int count3 = this.m_flatBatch.TriangleVertices.Count;
            QueueBevelledRectangle(null, this.m_flatBatch, Vector2.Zero, ActualSize, 0f, this.BevelSize, this.CenterColor * GlobalColorTransform, this.BevelColor * GlobalColorTransform, this.ShadowColor * GlobalColorTransform, this.AmbientLight, this.DirectionalLight, 0f);
            this.m_flatBatch.TransformTriangles(GlobalTransform, count3, -1);
        }

        public override void MeasureOverride(Vector2 parentAvailableSize)
        {
            IsDrawRequired = (this.BevelColor.A != 0 || this.CenterColor.A > 0);
            DesiredSize = this.Size;
        }

        public static void QueueBevelledRectangle(TexturedBatch2D texturedBatch, FlatBatch2D flatBatch, Vector2 c1, Vector2 c2, float depth, float bevelSize, Color color, Color bevelColor, Color shadowColor, float ambientLight, float directionalLight, float textureScale)
        {
            float num = MathUtils.Abs(bevelSize);
            Vector2 vector = c1 + new Vector2(num);
            Vector2 vector2 = c2 - new Vector2(num);
            Vector2 vector3 = c2 + new Vector2(1.5f * num);
            float x = c1.X;
            float x2 = vector.X;
            float x3 = vector2.X;
            float x4 = c2.X;
            float x5 = vector3.X;
            float y = c1.Y;
            float y2 = vector.Y;
            float y3 = vector2.Y;
            float y4 = c2.Y;
            float y5 = vector3.Y;
            float num2 = MathUtils.Saturate(((bevelSize > 0f) ? 1f : -0.75f) * directionalLight + ambientLight);
            float num3 = MathUtils.Saturate(((bevelSize > 0f) ? -0.75f : 1f) * directionalLight + ambientLight);
            float num4 = MathUtils.Saturate(((bevelSize > 0f) ? -0.375f : 0.5f) * directionalLight + ambientLight);
            float num5 = MathUtils.Saturate(((bevelSize > 0f) ? 0.5f : -0.375f) * directionalLight + ambientLight);
            float num6 = MathUtils.Saturate(0f * directionalLight + ambientLight);
            Color color2 = new Color((byte)(num4 * bevelColor.R), (byte)(num4 * bevelColor.G), (byte)(num4 * bevelColor.B), bevelColor.A);
            Color color3 = new Color((byte)(num5 * bevelColor.R), (byte)(num5 * bevelColor.G), (byte)(num5 * bevelColor.B), bevelColor.A);
            Color color4 = new Color((byte)(num2 * bevelColor.R), (byte)(num2 * bevelColor.G), (byte)(num2 * bevelColor.B), bevelColor.A);
            Color color5 = new Color((byte)(num3 * bevelColor.R), (byte)(num3 * bevelColor.G), (byte)(num3 * bevelColor.B), bevelColor.A);
            Color color6 = new Color((byte)(num6 * color.R), (byte)(num6 * color.G), (byte)(num6 * color.B), color.A);
            if (texturedBatch != null)
            {
                float num7 = textureScale / texturedBatch.Texture.Width;
                float num8 = textureScale / texturedBatch.Texture.Height;
                float num9 = x * num7;
                float num10 = y * num8;
                float x6 = num9;
                float x7 = (x2 - x) * num7 + num9;
                float x8 = (x3 - x) * num7 + num9;
                float x9 = (x4 - x) * num7 + num9;
                float y6 = num10;
                float y7 = (y2 - y) * num8 + num10;
                float y8 = (y3 - y) * num8 + num10;
                float y9 = (y4 - y) * num8 + num10;
                if (bevelColor.A > 0)
                {
                    texturedBatch.QueueQuad(new Vector2(x, y), new Vector2(x2, y2), new Vector2(x3, y2), new Vector2(x4, y), depth, new Vector2(x6, y6), new Vector2(x7, y7), new Vector2(x8, y7), new Vector2(x9, y6), color4);
                    texturedBatch.QueueQuad(new Vector2(x3, y2), new Vector2(x3, y3), new Vector2(x4, y4), new Vector2(x4, y), depth, new Vector2(x8, y7), new Vector2(x8, y8), new Vector2(x9, y9), new Vector2(x9, y6), color3);
                    texturedBatch.QueueQuad(new Vector2(x, y4), new Vector2(x4, y4), new Vector2(x3, y3), new Vector2(x2, y3), depth, new Vector2(x6, y9), new Vector2(x9, y9), new Vector2(x8, y8), new Vector2(x7, y8), color5);
                    texturedBatch.QueueQuad(new Vector2(x, y), new Vector2(x, y4), new Vector2(x2, y3), new Vector2(x2, y2), depth, new Vector2(x6, y6), new Vector2(x6, y9), new Vector2(x7, y8), new Vector2(x7, y7), color2);
                }
                if (color6.A > 0)
                {
                    texturedBatch.QueueQuad(new Vector2(x2, y2), new Vector2(x3, y3), depth, new Vector2(x7, y7), new Vector2(x8, y8), color6);
                }
            }
            else if (flatBatch != null)
            {
                if (bevelColor.A > 0)
                {
                    flatBatch.QueueQuad(new Vector2(x, y), new Vector2(x2, y2), new Vector2(x3, y2), new Vector2(x4, y), depth, color4);
                    flatBatch.QueueQuad(new Vector2(x3, y2), new Vector2(x3, y3), new Vector2(x4, y4), new Vector2(x4, y), depth, color3);
                    flatBatch.QueueQuad(new Vector2(x, y4), new Vector2(x4, y4), new Vector2(x3, y3), new Vector2(x2, y3), depth, color5);
                    flatBatch.QueueQuad(new Vector2(x, y), new Vector2(x, y4), new Vector2(x2, y3), new Vector2(x2, y2), depth, color2);
                }
                if (color6.A > 0)
                {
                    flatBatch.QueueQuad(new Vector2(x2, y2), new Vector2(x3, y3), depth, color6);
                }
            }
            if (bevelSize > 0f && flatBatch != null && shadowColor.A > 0)
            {
                Color color7 = shadowColor;
                Color color8 = new Color(0, 0, 0, 0);
                flatBatch.QueueTriangle(new Vector2(x, y4), new Vector2(x2, y5), new Vector2(x2, y4), depth, color8, color8, color7);
                flatBatch.QueueTriangle(new Vector2(x4, y), new Vector2(x4, y2), new Vector2(x5, y2), depth, color8, color7, color8);
                flatBatch.QueueTriangle(new Vector2(x4, y4), new Vector2(x4, y5), new Vector2(x5, y4), depth, color7, color8, color8);
                flatBatch.QueueQuad(new Vector2(x2, y4), new Vector2(x2, y5), new Vector2(x4, y5), new Vector2(x4, y4), depth, color7, color8, color8, color7);
                flatBatch.QueueQuad(new Vector2(x4, y2), new Vector2(x4, y4), new Vector2(x5, y4), new Vector2(x5, y2), depth, color7, color7, color8, color8);
            }
        }

        private FlatBatch2D m_flatBatch = WidgetsManager.PrimitivesRenderer2D.FlatBatch(0, DepthStencilState.None, null, null);

        private TexturedBatch2D m_texturedBatch;

        private Texture2D m_texture;

        private bool m_textureLinearFilter;
    }
}
