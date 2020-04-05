using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ConcurrencyWizardLibrary
{
    class MemoizeHelper
    {
        public static Func<T, R> Memoize<T, R>(Func<T, R> func)
            where T : IComparable
        {
            Dictionary<T, R> cache = new Dictionary<T, R>();

            return arg =>
            {
                if (cache.ContainsKey(arg))
                    return cache[arg];
                return cache[arg] = func(arg);
            };
        }

        public static Func<T, R> MemoizeConcurrent<T, R>(Func<T, R> func)
            where T : IComparable
        {
            ConcurrentDictionary<T, Lazy<R>> cache = new ConcurrentDictionary<T, Lazy<R>>();

            return arg => cache.GetOrAdd(arg, arg => new Lazy<R>(func(arg))).Value;
        }
        
        public static Func<T, R> MemoizeWeakWithTtl<T, R>(Func<T, R> func, TimeSpan ttl)
            where T : class, IEquatable<T>
            where R : class
        {
            var keyStore = new ConcurrentDictionary<int, T>();

            T ReduceKey(T obj)
            {
                var oldObj = keyStore.GetOrAdd(obj.GetHashCode(), obj);
                return obj.Equals(oldObj) ? oldObj : obj;
            }

            var cache = new ConditionalWeakTable<T, Tuple<R, DateTime>>();

            Tuple<R, DateTime> FactoryFunc(T key) =>
                new Tuple<R, DateTime>(func(key), DateTime.Now + ttl);

            return arg =>
            {
                var key = ReduceKey(arg);
                var value = cache.GetValue(key, FactoryFunc);

                if (value.Item2 >= DateTime.Now)
                    return value.Item1;
                value = FactoryFunc(key);                
                cache.Remove(key);
                cache.Add(key, value);
                return value.Item1;
            };
        }
    }
}
