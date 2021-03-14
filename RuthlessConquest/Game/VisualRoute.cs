using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
    internal class VisualRoute : Entity
    {
        public float TimeToLive { get; private set; }

        public VisualRoute(IEnumerable<Planet> planets, Color color)
        {
            this.Planets = planets.ToDynamicArray<Planet>();
            this.Color = color;
            this.TimeToLive = this.Duration + this.SegmentDelay * (this.Planets.Count - 2);
        }

        public void Draw(Color colorTransform)
        {
            FlatBatch2D batch = CameraModule.PrimitivesRenderer.FlatBatch(2, null, null, null);
            for (int i = 0; i < this.Planets.Count - 1; i++)
            {
                float num = this.TimeToLive + this.SegmentDelay * (i - (this.Planets.Count - 2));
                float s = 0.3f * MathUtils.Sigmoid(MathUtils.Saturate(0.3f * num), 4f) * MathUtils.Saturate(8f * (this.Duration - num));
                Color ci = this.Color * colorTransform * s;
                ci.A = 0;
                Vector2 vector = new Vector2(this.Planets[i].Position);
                Vector2 vector2 = new Vector2(this.Planets[i + 1].Position);
                Vector2.Perpendicular(Vector2.Normalize(vector2 - vector));
                DrawingUtils.QueueThickLine(batch, vector, vector2, 220f, 240f, 0f, 20f, ci, Color.Transparent);
            }
            this.TimeToLive -= Time.FrameDuration;
        }

        private DynamicArray<Planet> Planets = new DynamicArray<Planet>();

        private Color Color;

        private float Duration = 4f;

        private float SegmentDelay = 0.1f;
    }
}
