namespace Afg3Abbiegen
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    public class Street
    {
        public Vector2Int Start { get; }
        public Vector2Int End { get; }

        public IReadOnlyList<Intersection> Intersections { get; }

        public Street(Vector2Int start, Vector2Int end)
        {
            Start = start;
            End = end;
            Intersections = new List<Intersection>();
        }

        public static IEnumerable<Street> CreateStreets(
            [DisallowNull] Vector2Int start, [DisallowNull] Vector2Int end,
            [DisallowNull] IEnumerable<(Vector2Int Start, Vector2Int End)> streetsSource)
        {
            var streets = streetsSource.ToDictionary(x => x, x => new Street(x.Start, x.End));


            for (int i = 0; i < streets.Count; i++)
            {
                var first = streets[i];

                for (int j = 0; j < i; j++)
                {
                    var second = streets[j];

                    if (!GetLineIntersection(first.Start, first.End, second.Start, second.End, out var intersection))
                    {
                        continue;
                    }

                    var list = Intersections(intersection);

                    list.Add(first);
                    list.Add(second);
                }
                var a = "";
                a.
                if (IsPointOnLine(start, first.Start, first.End))
                {
                    Intersections(start).Add(first);
                }

                if (IsPointOnLine(end, first.Start, first.End))
                {
                    Intersections(end).Add(first);
                }
            }

            foreach (var intersection in intersections)
            {
                intersection
            }
        }

        private static bool IsPointOnLine(
            [DisallowNull] Vector2Int point,
            [DisallowNull] Vector2Int start, [DisallowNull] Vector2Int end)
        {
            var endFromStart = end - start;
            var pointFromStart = point - start;

            var xRatio = Math.DivRem(endFromStart.X, pointFromStart.X, out var xRem);
            if (xRem != 0 || xRatio < 0) return false;

            var yRatio = Math.DivRem(endFromStart.Y, pointFromStart.Y, out var yRem);
            if (yRem != 0) return false;

            return xRatio == yRatio;
        }

        // Taken from https://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect
        private static bool GetLineIntersection(
            [DisallowNull] Vector2Int firstStart, [DisallowNull] Vector2Int firstEnd,
            [DisallowNull] Vector2Int secondStart, [DisallowNull] Vector2Int secondEnd,
            [NotNullWhen(true)][MaybeNullWhen(false)] out Vector2Int intersection)
        {
            var firstPath = new Vector2Int(firstEnd.X - firstStart.X, firstEnd.Y - firstStart.X);
            var secondPath = new Vector2Int(secondEnd.X - secondStart.X, secondEnd.Y - secondStart.Y);

            float s = ((-firstPath.Y * (firstStart.X - secondStart.X)) + (firstPath.X * (firstStart.Y - secondStart.Y)))
                / ((-secondPath.X * firstPath.Y) + (firstPath.X * secondPath.Y));

            float t = ((secondPath.X * (firstStart.Y - secondStart.Y)) - (secondPath.Y * (firstStart.X - secondStart.X)))
                / ((-secondPath.X * firstPath.Y) + (firstPath.X * secondPath.Y));

            if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
            {
                // Collision detected
                intersection = new Vector2Int(firstStart.X + (int)(t * firstPath.X), firstStart.Y + (int)(t * firstPath.Y));
                return true;
            }

            // No collision
            intersection = default;
            return false;
        }
    }
}
