using System;
using System.Diagnostics;

namespace Game
{
    public class RunningAverage
    {
        public RunningAverage(float period)
        {
            this.m_period = (long)(period * Stopwatch.Frequency);
        }

        public float Value
        {
            get
            {
                return this.m_value;
            }
        }

        public void AddSample(float sample)
        {
            this.m_sumValues += sample;
            this.m_countValues++;
            long timestamp = Stopwatch.GetTimestamp();
            if (timestamp >= this.m_startTicks + this.m_period)
            {
                this.m_value = this.m_sumValues / m_countValues;
                this.m_sumValues = 0f;
                this.m_countValues = 0;
                this.m_startTicks = timestamp;
            }
        }

        private long m_startTicks;

        private long m_period;

        private float m_sumValues;

        private int m_countValues;

        private float m_value;
    }
}
