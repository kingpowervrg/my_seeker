/********************************************************************
	created:  2018-4-18 14:51:17
	filename: MemoryPoolSafety.cs
	author:	  songguangze@outlook.com
	
	purpose:  线程安全的内存池
*********************************************************************/

namespace EngineCore
{
    public class MemoryPoolSafety<T> where T : class
    {
        public MemoryPoolSafety(int maxSize = 10)
        {
            _maxSize = maxSize;
        }
        public T Alloc()
        {
            T t = null;
            if (_objs.Dequeue(out t))
                return t;
            else
                return null;
        }
        public void Free(T t)
        {
            if (_objs.Count < _maxSize)
                _objs.Enqueue(t);
        }
        public void Dispose()
        {
            _objs.Clear();
        }
        public int Count { get { return _objs.Count; } }
        private ThreadSafeQueue<T> _objs = new ThreadSafeQueue<T>();
        private int _maxSize = 10;
    }
}