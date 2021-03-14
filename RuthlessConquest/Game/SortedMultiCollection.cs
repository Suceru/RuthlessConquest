using System;
using System.Collections;
using System.Collections.Generic;

namespace Game
{
    public class SortedMultiCollection<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
    {
        public SortedMultiCollection()
        {
            this.m_array = new KeyValuePair<TKey, TValue>[4];
            this.m_comparer = Comparer<TKey>.Default;
        }

        public SortedMultiCollection(IComparer<TKey> comparer)
        {
            this.m_array = new KeyValuePair<TKey, TValue>[4];
            this.m_comparer = comparer;
        }

        public SortedMultiCollection(int capacity) : this(capacity, null)
        {
            capacity = Math.Max(capacity, 4);
            this.m_array = new KeyValuePair<TKey, TValue>[capacity];
            this.m_comparer = Comparer<TKey>.Default;
        }

        public SortedMultiCollection(int capacity, IComparer<TKey> comparer)
        {
            capacity = Math.Max(capacity, 4);
            this.m_array = new KeyValuePair<TKey, TValue>[capacity];
            this.m_comparer = comparer;
        }

        public int Count
        {
            get
            {
                return this.m_count;
            }
        }

        public int Capacity
        {
            get
            {
                return this.m_array.Length;
            }
            set
            {
                value = Math.Max(Math.Max(4, this.m_count), value);
                if (value != this.m_array.Length)
                {
                    KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[value];
                    Array.Copy(this.m_array, array, this.m_count);
                    this.m_array = array;
                }
            }
        }

        public KeyValuePair<TKey, TValue> this[int i]
        {
            get
            {
                if (i < this.m_count)
                {
                    return this.m_array[i];
                }
                throw new ArgumentOutOfRangeException();
            }
        }

        public void Add(TKey key, TValue value)
        {
            int num = this.Find(key);
            if (num < 0)
            {
                num = ~num;
            }
            this.EnsureCapacity(this.m_count + 1);
            Array.Copy(this.m_array, num, this.m_array, num + 1, this.m_count - num);
            this.m_array[num] = new KeyValuePair<TKey, TValue>(key, value);
            this.m_count++;
            this.m_version++;
        }

        public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            foreach (KeyValuePair<TKey, TValue> keyValuePair in items)
            {
                this.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public bool Remove(TKey key)
        {
            int num = this.Find(key);
            if (num >= 0)
            {
                Array.Copy(this.m_array, num + 1, this.m_array, num, this.m_count - num - 1);
                this.m_array[this.m_count - 1] = default(KeyValuePair<TKey, TValue>);
                this.m_count--;
                this.m_version++;
                return true;
            }
            return false;
        }

        public void Clear()
        {
            for (int i = 0; i < this.m_count; i++)
            {
                this.m_array[i] = default(KeyValuePair<TKey, TValue>);
            }
            this.m_count = 0;
            this.m_version++;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            int num = this.Find(key);
            if (num >= 0)
            {
                value = this.m_array[num].Value;
                return true;
            }
            value = default(TValue);
            return false;
        }

        public bool ContainsKey(TKey key)
        {
            return this.Find(key) >= 0;
        }

        public SortedMultiCollection<TKey, TValue>.Enumerator GetEnumerator()
        {
            return new SortedMultiCollection<TKey, TValue>.Enumerator(this);
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()

        {
            return new SortedMultiCollection<TKey, TValue>.Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new SortedMultiCollection<TKey, TValue>.Enumerator(this);
        }

        private void EnsureCapacity(int capacity)
        {
            if (capacity > this.Capacity)
            {
                this.Capacity = Math.Max(capacity, 2 * this.Capacity);
            }
        }

        private int Find(TKey key)
        {
            if (this.m_count > 0)
            {
                int i = 0;
                int num = this.m_count - 1;
                while (i <= num)
                {
                    int num2 = i + num >> 1;
                    int num3 = this.m_comparer.Compare(this.m_array[num2].Key, key);
                    if (num3 == 0)
                    {
                        return num2;
                    }
                    if (num3 < 0)
                    {
                        i = num2 + 1;
                    }
                    else
                    {
                        num = num2 - 1;
                    }
                }
                return ~i;
            }
            return -1;
        }

        private const int MinCapacity = 4;

        private KeyValuePair<TKey, TValue>[] m_array;

        private int m_count;

        private int m_version;

        private IComparer<TKey> m_comparer;

        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDisposable, IEnumerator
        {
            internal Enumerator(SortedMultiCollection<TKey, TValue> collection)
            {
                this.m_collection = collection;
                this.m_current = default(KeyValuePair<TKey, TValue>);
                this.m_index = 0;
                this.m_version = collection.m_version;
            }

            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    return this.m_current;
                }
            }

            public void Dispose()
            {
            }

            object IEnumerator.Current
            {
                get
                {
                    return this.m_current;
                }
            }

            public bool MoveNext()
            {
                if (this.m_collection.m_version != this.m_version)
                {
                    throw new InvalidOperationException("SortedMultiCollection was modified, enumeration cannot continue.");
                }
                if (this.m_index < this.m_collection.m_count)
                {
                    this.m_current = this.m_collection.m_array[this.m_index];
                    this.m_index++;
                    return true;
                }
                this.m_current = default(KeyValuePair<TKey, TValue>);
                return false;
            }

            public void Reset()
            {
                if (this.m_collection.m_version != this.m_version)
                {
                    throw new InvalidOperationException("SortedMultiCollection was modified, enumeration cannot continue.");
                }
                this.m_index = 0;
                this.m_current = default(KeyValuePair<TKey, TValue>);
            }

            private SortedMultiCollection<TKey, TValue> m_collection;

            private KeyValuePair<TKey, TValue> m_current;

            private int m_index;

            private int m_version;
        }
    }
}
