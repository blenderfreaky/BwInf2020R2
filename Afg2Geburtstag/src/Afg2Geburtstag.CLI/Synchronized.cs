namespace Afg2Geburtstag.CLI
{
    using System;

    internal static class Synchronized
    {
        public static void For(int inclusiveStart, int exclusiveEnd, Action<int> action)
        {
            for (int i = inclusiveStart; i < exclusiveEnd; i++) action(i);
        }
    }
}
