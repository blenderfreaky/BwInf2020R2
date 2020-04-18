namespace Afg3Abbiegen
{
    using System;
    using System.Collections.Generic;

    public static class EnumerableExtensions
    {
        public static int CountTurns(this IEnumerable<Vector2Int> street)
        {
            using var en = street.GetEnumerator();

            if (!en.MoveNext()) return 0; // If the street has no intersections there are no turns => return 0
            var zerothIntersection = en.Current;

            if (!en.MoveNext()) return 0; // If the street has only one intersection there are no turns => return 0
            var lastIntersection = en.Current;

            var lastDirection = (lastIntersection - zerothIntersection).Bidirection;

            // Remember the last intersection and its direction and compare it against the current one
            // If the directions differ count a turn
            var turns = 0;

            while (en.MoveNext())
            {
                var currentIntersection = en.Current;
                var currentDirection = (currentIntersection - lastIntersection).Bidirection;

                if (currentDirection != lastDirection) turns++;

                lastIntersection = currentIntersection;
                lastDirection = currentDirection;
            }

            return turns;
        }
    }
}
