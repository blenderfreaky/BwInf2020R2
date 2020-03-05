namespace Afg3Abbiegen
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    public class DirectedStreet
    {
        public Vector2Int Start { get; }
        public Vector2Int End { get; }

        public DirectedStreet? Forward { get; }

        public IReadOnlyList<DirectedStreet> Exits { get; }

        public DirectedStreet(Vector2Int start, Vector2Int end, DirectedStreet? forward, IReadOnlyList<DirectedStreet> exits)
        {
            Start = start;
            End = end;
            Forward = forward;
            Exits = exits;
        }

        public static IEnumerable<DirectedStreet> CreateStreets(
            [DisallowNull] Vector2Int start, [DisallowNull] Vector2Int end,
            [DisallowNull] IEnumerable<(Vector2Int Start, Vector2Int End)> streetsSource)
        {
            var streets = streetsSource.ToList();

            var intersections = new Dictionary<Vector2Int, List<(Vector2Int Start, Vector2Int End)>>();

            List<(Vector2Int Start, Vector2Int End)> Intersections(Vector2Int vec)
            {
                if (!intersections.TryGetValue(vec, out var list))
                {
                    return intersections[vec] = new List<(Vector2Int Start, Vector2Int End)>();
                }

                return list;
            }

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
                inter
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
