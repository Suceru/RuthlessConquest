using System;
using Engine;
using Engine.Graphics;

namespace Game
{
    public class InterlaceWidget : CanvasWidget
    {
        public InterlaceWidget()
        {
            IsDrawRequired = true;
            this.IsHitTestVisible = false;
            Size = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        }

        public override void Draw()
        {
            FlatBatch2D flatBatch2D = WidgetsManager.PrimitivesRenderer2D.FlatBatch(0, null, null, null);
            int num = (int)GlobalBounds.Min.X;
            float num2 = (int)GlobalBounds.Min.Y;
            int num3 = (int)GlobalBounds.Max.X;
            int num4 = (int)GlobalBounds.Max.Y;
            for (float num5 = num2; num5 < num4; num5 += 1f)
            {
                if (num5 % 4f == 0f)
                {
                    flatBatch2D.QueueLine(new Vector2(num, num5), new Vector2(num3, num5), 0f, this.Color);
                }
            }
        }

        public Color Color = Color.White * 0.1f;
    }
}
