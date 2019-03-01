using System;
using System.Collections.Generic;


namespace EngineCore
{
    public class ThreadSafeQueue<T>
    {
        private readonly object syncLock = new object();
        private Queue<T> queue;

        public int Count
        {
            get
            {
                lock (syncLock)
                {
                    return queue.Count;
                }
            }
        }

        public ThreadSafeQueue()
        {
            this.queue = new Queue<T>();
        }

        public T Peek()
        {
            lock (syncLock)
            {
                return queue.Peek();
            }
        }

        public void Enqueue(T obj)
        {
            lock (syncLock)
            {
                queue.Enqueue(obj);
            }
        }

        public bool Dequeue(out T t)
        {
            lock (syncLock)
            {
                if (queue.Count > 0)
                {
                    t = queue.Dequeue();
                    return true;
                }
                else
                {
                    t = default(T);
                    return false;
                }
            }
        }

        public void Clear()
        {
            lock (syncLock)
            {
                queue.Clear();
            }
        }

        public T[] CopyToArray()
        {
            lock (syncLock)
            {
                if (queue.Count == 0)
                {
                    return new T[0];
                }

                T[] values = new T[queue.Count];
                queue.CopyTo(values, 0);
                return values;
            }
        }

        public static ThreadSafeQueue<T> InitFromArray(IEnumerable<T> initValues)
        {
            var queue = new ThreadSafeQueue<T>();

            if (initValues == null)
            {
                return queue;
            }

            foreach (T val in initValues)
            {
                queue.Enqueue(val);
            }

            return queue;
        }
    }
}