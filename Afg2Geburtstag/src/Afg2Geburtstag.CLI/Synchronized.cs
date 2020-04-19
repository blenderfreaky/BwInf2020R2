namespace Afg2Geburtstag.CLI
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A class similar to <see cref="System.Threading.Tasks.Parallel"/>, but not parallel, so parallel methods can be quickly swapped out for synchronized methods for debug purposes.
    /// </summary>
    internal static class Synchronized
    {
        public static void For(int inclusiveStart, int exclusiveEnd, Action<int> action)
        {
            for (int i = inclusiveStart; i < exclusiveEnd; i++) action(i);
        }

        public static void ForEach<T>(IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var elem in enumerable) action(elem);
        }
    }
}