using System.Collections.Generic;

namespace SqlBuilder
{
    /// <summary>
    /// A two-way dictionary.<para/>
    /// From https://stackoverflow.com/a/10966684 / https://stackoverflow.com/users/259769/enigmativity
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    internal class Map<T1, T2>
    {
        private readonly Dictionary<T1, T2> _forward = new Dictionary<T1, T2>();
        private readonly Dictionary<T2, T1> _reverse = new Dictionary<T2, T1>();

        public Dictionary<T1, T2> ForwardD => _forward;
        public Dictionary<T2, T1> ReverseD => _reverse;

        public Map()
        {
            Forward = new Indexer<T1, T2>(_forward);
            Reverse = new Indexer<T2, T1>(_reverse);
        }

        public static Map<T1, T2> FromDictionary(Dictionary<T1, T2> dict)
        {
            var map = new Map<T1, T2>();
            foreach (var kvp in dict)
                map.Add(kvp.Key, kvp.Value);
            return map;
        }

        public static Map<T1, T2> FromDictionaryReverse(Dictionary<T2, T1> dict)
        {
            var map = new Map<T1, T2>();
            foreach (var kvp in dict)
                map.Add(kvp.Value, kvp.Key);
            return map;
        }

        internal class Indexer<T3, T4>
        {
            private readonly Dictionary<T3, T4> _dictionary;

            public Indexer(Dictionary<T3, T4> dictionary)
            {
                _dictionary = dictionary;
            }

            public T4 this[T3 index]
            {
                get => _dictionary[index];
                set => _dictionary[index] = value;
            }
        }

        public void Add(T1 t1, T2 t2)
        {
            _forward.Add(t1, t2);
            _reverse.Add(t2, t1);
        }

        public Indexer<T1, T2> Forward { get; }
        public Indexer<T2, T1> Reverse { get; }
    }
}