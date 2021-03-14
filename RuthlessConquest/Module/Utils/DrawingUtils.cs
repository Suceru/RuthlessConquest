using System;
using Engine;
using Engine.Graphics;

namespace Game
{
    public static class DrawingUtils
    {
        public static void QueueRangeDisc(FlatBatch2D batch, Vector2 position, float range, float depth, Color color1, Color color2, bool outline)
        {
            float num = 80f;
            if (outline)
            {
                batch.QueueDisc(position, new Vector2(range - num / 2f), new Vector2(range - num), depth, color2, color1, 100, 0f, 6.28318548f);
                batch.QueueDisc(position, new Vector2(range), new Vector2(range - num / 2f), depth, Color.Transparent, color2, 100, 0f, 6.28318548f);
                return;
            }
            batch.QueueDisc(position, new Vector2(range - num * 0.8f), depth, color1, 100, 0f, 6.28318548f);
        }

        public static void QueueThickLine(FlatBatch2D batch, Vector2 p1, Vector2 p2, float ti1, float to1, float ti2, float to2, Color ci, Color co)
        {
            QueueThickLine(batch, p1, p2, ti1, to1, ti2, to2, ci, co, ci, co);
        }

        public static void QueueThickLine(FlatBatch2D batch, Vector2 p1, Vector2 p2, float ti1, float to1, float ti2, float to2, Color ci1, Color co1, Color ci2, Color co2)
        {
            Vector2 v = Vector2.Perpendicular(Vector2.Normalize(p2 - p1));
            batch.QueueQuad(p1 - ti1 * v, p1 - to1 * v, p2 - to2 * v, p2 - ti2 * v, 0f, ci1, co1, co2, ci2);
            batch.QueueQuad(p1 + ti1 * v, p1 + to1 * v, p2 + to2 * v, p2 + ti2 * v, 0f, ci1, co1, co2, ci2);
            batch.QueueQuad(p1 - ti1 * v, p1 + ti1 * v, p2 + ti2 * v, p2 - ti2 * v, 0f, ci1, ci1, ci2, ci2);
        }
    }
}
