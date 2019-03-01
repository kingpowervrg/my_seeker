using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace System.Linq
{
    public static class EnumerableExtension
    {
        /// <summary>
        /// 扩展IEnumerable的ForEach 方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            foreach (var item in source)
                action(item);

            return source;
        }
    }

}