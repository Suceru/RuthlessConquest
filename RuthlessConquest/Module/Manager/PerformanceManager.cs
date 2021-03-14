using System;
using Engine;
using Engine.Graphics;
using Engine.Media;

namespace Game
{
    public static class PerformanceManager
    {
        public static float AverageFrameTime
        {
            get
            {
                return m_averageFrameTime.Value;
            }
        }

        public static float AverageCpuFrameTime
        {
            get
            {
                return m_averageCpuFrameTime.Value;
            }
        }

        public static long TotalMemoryUsed { get; private set; }

        public static long TotalGpuMemoryUsed { get; private set; }

        public static void Update()
        {
            m_averageFrameTime.AddSample(Program.LastFrameTime);
            m_averageCpuFrameTime.AddSample(Program.LastCpuFrameTime);
            if (Time.PeriodicEvent(1.0, 0.0))
            {
                TotalMemoryUsed = GC.GetTotalMemory(false);
                TotalGpuMemoryUsed = Display.GetGpuMemoryUsage();
            }
        }

        public static void Draw()
        {
            Vector2 vector = new Vector2(MathUtils.Round(MathUtils.Clamp(WidgetsManager.Scale, 1f, 4f)));
            Viewport viewport = Display.Viewport;
            if (SettingsManager.DisplayFpsCounter)
            {
                if (Time.PeriodicEvent(1.0, 0.0))
                {
                    m_statsString = string.Format("CPUMEM {0:0}MB, GPUMEM {1:0}MB, CPU {2:0}%, FPS {3:0.0}", new object[]
                    {
                        TotalMemoryUsed / 1024f / 1024f,
                        TotalGpuMemoryUsed / 1024f / 1024f,
                        AverageCpuFrameTime / AverageFrameTime * 100f,
                        1f / AverageFrameTime
                    });
                }
                m_primitivesRenderer.FontBatch(BitmapFont.DebugFont, 0, null, null, null, SamplerState.PointClamp).QueueText(m_statsString, new Vector2(viewport.Width, 0f), 0f, Color.White, TextAnchor.Right, vector, Vector2.Zero, 0f);
            }
            if (SettingsManager.DisplayFpsRibbon)
            {
                float num = (viewport.Width / vector.X > 480f) ? (vector.X * 2f) : vector.X;
                float num2 = viewport.Height / -0.1f;
                float num3 = viewport.Height - 1;
                float s = 0.5f;
                int num4 = (int)(viewport.Width / num);
                if (m_frameData == null || m_frameData.Length != num4)
                {
                    m_frameData = new PerformanceManager.FrameData[num4];
                    m_frameDataIndex = 0;
                }
                m_frameData[m_frameDataIndex] = new PerformanceManager.FrameData
                {
                    CpuTime = Program.LastCpuFrameTime,
                    TotalTime = Program.LastFrameTime
                };
                m_frameDataIndex = (m_frameDataIndex + 1) % m_frameData.Length;
                FlatBatch2D flatBatch2D = m_primitivesRenderer.FlatBatch(0, null, null, null);
                Color color = Color.Orange * s;
                Color color2 = Color.Red * s;
                for (int i = m_frameData.Length - 1; i >= 0; i--)
                {
                    int num5 = (i - m_frameData.Length + 1 + m_frameDataIndex + m_frameData.Length) % m_frameData.Length;
                    PerformanceManager.FrameData frameData = m_frameData[num5];
                    float x = i * num;
                    float x2 = (i + 1) * num;
                    flatBatch2D.QueueQuad(new Vector2(x, num3), new Vector2(x2, num3 + frameData.CpuTime * num2), 0f, color);
                    flatBatch2D.QueueQuad(new Vector2(x, num3 + frameData.CpuTime * num2), new Vector2(x2, num3 + frameData.TotalTime * num2), 0f, color2);
                }
                flatBatch2D.QueueLine(new Vector2(0f, num3 + 0.0166666675f * num2), new Vector2(viewport.Width, num3 + 0.0166666675f * num2), 0f, Color.Green);
            }
            else
            {
                m_frameData = null;
            }
            m_primitivesRenderer.Flush(true, int.MaxValue);
        }

        private static PrimitivesRenderer2D m_primitivesRenderer = new PrimitivesRenderer2D();

        private static RunningAverage m_averageFrameTime = new RunningAverage(1f);

        private static RunningAverage m_averageCpuFrameTime = new RunningAverage(1f);

        private static string m_statsString = string.Empty;

        private static PerformanceManager.FrameData[] m_frameData;

        private static int m_frameDataIndex;

        private struct FrameData
        {
            public float CpuTime;

            public float TotalTime;
        }
    }
}
