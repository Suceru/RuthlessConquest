using System;
using Engine;
using Engine.Graphics;

namespace Game
{
    public class BusyBarWidget : Widget
    {
        public BusyBarWidget()
        {
            this.IsHitTestVisible = false;
            this.LitBarColor = Colors.Fore;
            this.UnlitBarColor = Colors.ForeDark;
        }

        public Color LitBarColor { get; set; }

        public Color UnlitBarColor { get; set; }

        public override void MeasureOverride(Vector2 parentAvailableSize)
        {
            IsDrawRequired = true;
            DesiredSize = new Vector2(120f, 14f);
        }

        public override void Draw()
        {
            if (Time.FrameStartTime - this.m_lastBoxesStepTime > 0.25)
            {
                this.m_boxIndex++;
                this.m_lastBoxesStepTime = Time.FrameStartTime;
            }
            float num = 2f * (float)MathUtils.Remainder(Time.FrameStartTime, 10000.0);
            int count = this.m_batch.TriangleVertices.Count;
            for (int i = 0; i < 5; i++)
            {
                Vector2 center = new Vector2((i + 0.5f) * 24f, 7f);
                Color c = (i == this.m_boxIndex % 5) ? this.LitBarColor : this.UnlitBarColor;
                float num2 = (i == this.m_boxIndex % 5) ? 14f : 10f;
                this.m_batch.QueueDisc(center, new Vector2(num2 / 2f), 0f, c * GlobalColorTransform, 5, num, num + 6.28318548f);
                this.m_batch.QueueDisc(center, new Vector2(num2 / 2f + 1f), new Vector2(num2 / 2f), 0f, Color.Transparent, c * GlobalColorTransform, 5, num, num + 6.28318548f);
            }
            this.m_batch.TransformTriangles(GlobalTransform, count, -1);
        }

        private const int m_barsCount = 5;

        private const float m_barSize = 10f;

        private const float m_barsSpacing = 24f;

        private int m_boxIndex;

        private double m_lastBoxesStepTime;

        private FlatBatch2D m_batch = WidgetsManager.PrimitivesRenderer2D.FlatBatch(0, DepthStencilState.None, null, null);
    }
}
