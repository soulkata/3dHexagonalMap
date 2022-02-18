using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets
{
    public static class EnumerableHelper
    {
        public static IOrderedEnumerable<T> OrderByConstant<T, A1, A2, TKey>(this IEnumerable<T> items, A1 a1, A2 a2, Func<T, A1, A2, TKey> func) => items.OrderBy(new Helper<T, A1, A2, TKey>() { a1 = a1, a2 = a2, func = func }.Func);
        public static IEnumerable<T> WhereConstant<T, A1>(this IEnumerable<T> items, A1 a1, Func<T, A1, bool> func) => items.Where(new Helper<T, A1, bool>() { a1 = a1, func = func }.Func);
        public static IEnumerable<R> SelectConstant<T, A1, R>(this IEnumerable<T> items, A1 a1, Func<T, A1, R> func) => items.Select(new Helper<T, A1, R>() { a1 = a1, func = func }.Func);
        public static string Join(string separator, string lastSeparator, IEnumerable<string> items)
        {
            IEnumerator<string> enumerator = items.GetEnumerator();
            if (!enumerator.MoveNext())
                return string.Empty;
            string current = enumerator.Current;
            if (!enumerator.MoveNext())
                return current;
            string previous = current;
            current = enumerator.Current;
            while (enumerator.MoveNext())
            {
                previous = previous + separator + current;
                current = enumerator.Current;
            }
            return previous + lastSeparator + current;
        }

        class Helper<T, A1, A2, TKey>
        {
            public A1 a1;
            public A2 a2;
            public Func<T, A1, A2, TKey> func;

            public TKey Func(T item) => func(item, this.a1, this.a2);
        }

        class Helper<T, A1, TKey>
        {
            public A1 a1;
            public Func<T, A1, TKey> func;

            public TKey Func(T item) => func(item, this.a1);
        }

        public static float StdDev(this IEnumerable<float> values)
        {
            int count = values.Count();
            if (count > 1)
            {
                //Compute the Average
                float avg = values.Average();

                //Perform the Sum of (value-avg)^2
                float sum = values.SelectConstant(avg, (d, a) => (d - a) * (d - a)).Sum();

                //Put it all together
                return Mathf.Sqrt(sum / count);
            }
            return 0;
        }
    }
}