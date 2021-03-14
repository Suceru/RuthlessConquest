using System;
using System.Collections.Generic;
using System.Linq;
using Engine;

namespace Game
{
    public static class EnumUtils
    {
        public static string GetEnumName(Type type, int value)
        {
            int num = GetEnumValues(type).IndexOf(value);
            if (num >= 0)
            {
                return GetEnumNames(type)[num];
            }
            return "<invalid enum>";
        }

        public static IList<string> GetEnumNames(Type type)
        {
            return Cache.Query(type).Names;
        }

        public static IList<int> GetEnumValues(Type type)
        {
            return Cache.Query(type).Values;
        }

        private struct NamesValues
        {
            public ReadOnlyList<string> Names;

            public ReadOnlyList<int> Values;
        }

        private static class Cache
        {
            public static EnumUtils.NamesValues Query(Type type)
            {
                Dictionary<Type, EnumUtils.NamesValues> namesValuesByType = m_namesValuesByType;
                EnumUtils.NamesValues namesValues2;
                lock (namesValuesByType)
                {
                    EnumUtils.NamesValues namesValues;
                    if (!m_namesValuesByType.TryGetValue(type, out namesValues))
                    {
                        namesValues2 = new EnumUtils.NamesValues
                        {
                            Names = new ReadOnlyList<string>(new List<string>(Enum.GetNames(type))),
                            Values = new ReadOnlyList<int>(new List<int>(Enum.GetValues(type).Cast<int>()))
                        };
                        namesValues = namesValues2;
                        m_namesValuesByType.Add(type, namesValues);
                    }
                    namesValues2 = namesValues;
                }
                return namesValues2;
            }

            private static Dictionary<Type, EnumUtils.NamesValues> m_namesValuesByType = new Dictionary<Type, EnumUtils.NamesValues>();
        }
    }
}
