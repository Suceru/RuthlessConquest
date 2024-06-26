﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Engine;

namespace Game
{
    public struct Profiler : IDisposable
    {
        public Profiler(string name)
        {
            if (Enabled)
            {
                if (!m_metrics.TryGetValue(name, out this.m_metric))
                {
                    this.m_metric = new Profiler.Metric();
                    this.m_metric.Name = name;
                    m_maxNameLength = MathUtils.Max(m_maxNameLength, name.Length);
                    m_metrics.Add(name, this.m_metric);
                    m_sortedMetrics.Add(this.m_metric);
                    m_sortNeeded = true;
                }
                this.m_startTicks = Stopwatch.GetTimestamp();
                return;
            }
            this.m_startTicks = 0L;
            this.m_metric = null;
        }

        public void Dispose()
        {
            if (this.m_metric != null)
            {
                long num = Stopwatch.GetTimestamp() - this.m_startTicks;
                this.m_metric.TotalTicks += num;
                this.m_metric.MaxTicks = MathUtils.Max(this.m_metric.MaxTicks, num);
                this.m_metric.HitCount++;
                this.m_metric = null;
                return;
            }
            throw new InvalidOperationException("Profiler.Dispose called without a matching constructor.");
        }

        public static int MaxNameLength
        {
            get
            {
                return m_maxNameLength;
            }
        }

        public static void Sample()
        {
            foreach (Profiler.Metric metric in Metrics)
            {
                float sample = metric.TotalTicks / (float)Stopwatch.Frequency;
                metric.AverageHitCount.AddSample(metric.HitCount);
                metric.AverageTime.AddSample(sample);
                metric.HitCount = 0;
                metric.TotalTicks = 0L;
                metric.MaxTicks = 0L;
            }
        }

        public static ReadOnlyList<Profiler.Metric> Metrics
        {
            get
            {
                if (m_sortNeeded)
                {
                    m_sortedMetrics.Sort((Profiler.Metric x, Profiler.Metric y) => string.CompareOrdinal(x.Name, y.Name));
                    m_sortNeeded = false;
                }
                return new ReadOnlyList<Profiler.Metric>(m_sortedMetrics);
            }
        }

        public static void ReportAverage(Profiler.Metric metric, StringBuilder text)
        {
            int num = m_maxNameLength + 2;
            int length = text.Length;
            text.Append(metric.Name);
            text.Append('.', Math.Max(1, num - text.Length + length));
            text.AppendNumber(metric.AverageHitCount.Value, 2);
            text.Append("x");
            text.Append('.', Math.Max(1, num + 9 - text.Length + length));
            FormatTimeSimple(text, metric.AverageTime.Value);
        }

        public static void ReportFrame(Profiler.Metric metric, StringBuilder text)
        {
            int num = m_maxNameLength + 2;
            int length = text.Length;
            text.Append(metric.Name);
            text.Append('.', Math.Max(1, num - text.Length + length));
            FormatTimeSimple(text, metric.TotalTicks / (float)Stopwatch.Frequency);
        }

        public static void ReportAverage(StringBuilder text)
        {
            foreach (Profiler.Metric metric in Metrics)
            {
                ReportAverage(metric, text);
                text.Append("\n");
            }
        }

        public static void ReportFrame(StringBuilder text)
        {
            foreach (Profiler.Metric metric in Metrics)
            {
                ReportFrame(metric, text);
                text.Append("\n");
            }
        }

        public static void FormatTimeSimple(StringBuilder text, float time)
        {
            text.AppendNumber(time * 1000f, 3);
            text.Append("ms");
        }

        public static void FormatTime(StringBuilder text, float time)
        {
            if (time >= 1f)
            {
                text.AppendNumber(time, 2);
                text.Append("s");
                return;
            }
            if (time >= 0.1f)
            {
                text.AppendNumber(time * 1000f, 0);
                text.Append("ms");
                return;
            }
            if (time >= 0.01f)
            {
                text.AppendNumber(time * 1000f, 1);
                text.Append("ms");
                return;
            }
            if (time >= 0.001f)
            {
                text.AppendNumber(time * 1000f, 2);
                text.Append("ms");
                return;
            }
            if (time >= 0.0001f)
            {
                text.AppendNumber(time * 1000000f, 0);
                text.Append("us");
                return;
            }
            if (time >= 1E-05f)
            {
                text.AppendNumber(time * 1000000f, 1);
                text.Append("us");
                return;
            }
            if (time >= 1E-06f)
            {
                text.AppendNumber(time * 1000000f, 2);
                text.Append("us");
                return;
            }
            if (time >= 1E-07f)
            {
                text.AppendNumber(time * 1E+09f, 0);
                text.Append("ns");
                return;
            }
            if (time >= 1E-08f)
            {
                text.AppendNumber(time * 1E+09f, 1);
                text.Append("ns");
                return;
            }
            text.AppendNumber(time * 1E+09f, 2);
            text.Append("ns");
        }

        private static Dictionary<string, Profiler.Metric> m_metrics = new Dictionary<string, Profiler.Metric>();

        private static List<Profiler.Metric> m_sortedMetrics = new List<Profiler.Metric>();

        private static int m_maxNameLength;

        private static bool m_sortNeeded;

        private long m_startTicks;

        private Profiler.Metric m_metric;

        public static bool Enabled = true;

        public class Metric
        {
            public string Name;

            public int HitCount;

            public long TotalTicks;

            public long MaxTicks;

            public readonly RunningAverage AverageHitCount = new RunningAverage(5f);

            public readonly RunningAverage AverageTime = new RunningAverage(5f);
        }
    }
}
