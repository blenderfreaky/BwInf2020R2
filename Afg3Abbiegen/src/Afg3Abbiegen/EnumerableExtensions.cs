using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Afg3Abbiegen
{
    public static class EnumerableExtensions
    {
        public static (int Index, TValue Value, TKey Key) MinValue<TKey, TValue>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelector)
            where TKey :IComparable<TKey>
        {
            var enumerator = source.GetEnumerator();

            if (!enumerator.MoveNext()) throw new ArgumentException("Sequence contains no elements", nameof(source));

            int maxIndex = 0;
            TValue maxValue = enumerator.Current;
            TKey maxKey = keySelector(maxValue);

            int index = 0;

            while(enumerator.MoveNext())
            {
                index++;
                var value = enumerator.Current;
                var key = keySelector(value);

                if (key.CompareTo(maxKey) < 0)
                {
                    maxIndex = index;
                    maxValue = value;
                    maxKey = key;
                }
            }

            return (maxIndex, maxValue, maxKey);
        }
    }
}
