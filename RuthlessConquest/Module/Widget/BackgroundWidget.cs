using System;
using Engine;
using Engine.Graphics;

namespace Game
{
    public class BackgroundWidget : Widget
    {
        public BackgroundWidget()
        {
            IsDrawRequired = true;
            this.IsHitTestVisible = false;
        }

        public override void Draw()
        {
            Display.Clear(new Color?(Color.Black), new float?(1f), null);
            float num = 1.25f * GlobalTransform.M11;
            Random.Reset(0);
            if (Time.FrameIndex > LastAnimateFrame)
            {
                LastAnimateFrame = Time.FrameIndex;
                float num2 = (float)MathUtils.Remainder(Time.FrameStartTime + Seed, 10000.0);
                float x = 6.28318548f * SimplexNoise.OctavedNoise(num2, 0.05f, 1, 2f, 0.5f);
                float num3 = this.ShiftSpeed * num * MathUtils.Lerp(0.5f, 1.5f, SimplexNoise.OctavedNoise(num2 + 1000f, 0.1f, 1, 2f, 0.5f));
                Shift += num3 * Time.FrameDuration * new Vector2(MathUtils.Sin(x), -MathUtils.Cos(x));
            }
            TexturedBatch2D texturedBatch2D = WidgetsManager.PrimitivesRenderer2D.TexturedBatch(Textures.Star, false, 0, null, null, BlendState.AlphaBlend, null);
            for (int i = 0; i < (int)(this.Density * 800f); i++)
            {
                float x2 = MathUtils.Remainder(Random.Float(0f, Window.Size.X) + Shift.X, Window.Size.X);
                float y = MathUtils.Remainder(Random.Float(0f, Window.Size.X) + Shift.Y, Window.Size.Y);
                Vector2 v = new Vector2(x2, y);
                Vector2 v2 = new Vector2(num * Random.Float(1f, 2.5f));
                float r = Random.Float(0.8f, 1f);
                float g = Random.Float(0.7f, 0.8f);
                int num4 = 1;
                float s = MathUtils.Sqr(Random.Float(0.1f, this.Brightness));
                texturedBatch2D.QueueQuad(v - v2, v + v2, 0f, Vector2.Zero, Vector2.One, s * new Color(r, g, num4));
            }
            TexturedBatch2D texturedBatch2D2 = WidgetsManager.PrimitivesRenderer2D.TexturedBatch(Textures.StarTwinkle, false, 0, null, null, BlendState.AlphaBlend, null);
            for (int j = 0; j < (int)(this.Density * 100f); j++)
            {
                float x3 = MathUtils.Remainder(Random.Float(0f, Window.Size.X) + Shift.X, Window.Size.X);
                float y2 = MathUtils.Remainder(Random.Float(0f, Window.Size.Y) + Shift.Y, Window.Size.Y);
                Vector2 v3 = new Vector2(x3, y2);
                Vector2 v4 = new Vector2(num * Random.Float(2f, 7f));
                float r2 = Random.Float(0.8f, 1f);
                float g2 = Random.Float(0.7f, 0.8f);
                int num5 = 1;
                int count = texturedBatch2D2.TriangleVertices.Count;
                texturedBatch2D2.QueueQuad(v3 - v4, v3 + v4, 0f, Vector2.Zero, Vector2.One, 0.8f * this.Brightness * new Color(r2, g2, num5));
                texturedBatch2D2.TransformTriangles(Matrix.CreateRotationZ(Random.Float(0f, 6.28f)), count, -1);
            }
        }

        private static float Seed = new Engine.Random().Float(0f, 1000f);

        private static Vector2 Shift = new Engine.Random().Vector2(10000f);

        private static int LastAnimateFrame = -1;

        private static Engine.Random Random = new Engine.Random(0);

        public float ShiftSpeed = 2f;

        public float Density = 1f;

        public float Brightness = 0.75f;
    }
}
