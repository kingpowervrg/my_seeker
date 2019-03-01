namespace fastJSON
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.InteropServices;

    public sealed class SafeDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _Dictionary;
        private readonly object _Padlock;

        public SafeDictionary()
        {
            this._Padlock = new object();
            this._Dictionary = new Dictionary<TKey, TValue>();
        }

        public SafeDictionary(int capacity)
        {
            this._Padlock = new object();
            this._Dictionary = new Dictionary<TKey, TValue>(capacity);
        }

        public void Add(TKey key, TValue value)
        {
            lock (this._Padlock)
            {
                if (!this._Dictionary.ContainsKey(key))
                {
                    this._Dictionary.Add(key, value);
                }
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (this._Padlock)
            {
                return this._Dictionary.TryGetValue(key, out value);
            }
        }

        public int Count
        {
            get
            {
                lock (this._Padlock)
                {
                    return this._Dictionary.Count;
                }
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                lock (this._Padlock)
                {
                    return this._Dictionary[key];
                }
            }
            set
            {
                lock (this._Padlock)
                {
                    this._Dictionary[key] = value;
                }
            }
        }

        public void Clear()
        {
            lock (this._Padlock)
            {
                this._Dictionary.Clear();
            }
        }
    }
}

