using System;

namespace Game
{
    public struct AnalyticsParameter
    {
        public AnalyticsParameter(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name;

        public string Value;
    }
}
