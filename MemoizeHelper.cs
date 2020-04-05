using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ConcurrencyWizardLibrary
{
    class MemoizeHelper
    {
        public static Func<T, R> Memoize<T, R>(Func<T, R> func)
            where T : IComparable
        {
            ConcurrentDictionary<T, R> cache = new ConcurrentDictionary<T, R>();

            return arg =>
            {
                if (!cache.ContainsKey(arg))
                    cache.TryAdd(arg, func(arg));

                return cache[arg];
            };
        }


    }
}
