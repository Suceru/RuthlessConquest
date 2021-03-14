using System;
using System.Collections.Generic;

namespace Game
{
    public static class CollectionUtils
    {
        public static void RandomShuffle<T>(this IList<T> list, Func<int> randomIntGenerator)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int index = (int)(randomIntGenerator() * (long)(i + 1) / int.MaxValue);
                T value = list[index];
                list[index] = list[i];
                list[i] = value;
            }
        }

        public static int FirstIndex<T>(this IEnumerable<T> collection, T value)
        {
            int num = 0;
            using (IEnumerator<T> enumerator = collection.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (Equals(enumerator.Current, value))
                    {
                        return num;
                    }
                    num++;
                }
            }
            return -1;
        }

        public static int FirstIndex<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            int num = 0;
            foreach (T arg in collection)
            {
                if (predicate(arg))
                {
                    return num;
                }
                num++;
            }
            return -1;
        }
    }
}
