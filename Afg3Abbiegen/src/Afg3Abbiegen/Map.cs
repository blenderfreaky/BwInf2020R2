namespace Afg3Abbiegen
{
    using Priority_Queue;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class Map
    {
        /// <summary>
        /// All streets.
        /// </summary>
        public List<Street> Streets { get; }

        /// <summary>
        /// The position of the starting point.
        /// </summary>
        public Vector2Int Start { get; }

        /// <summary>
        /// The position of the target.
        /// </summary>
        public Vector2Int End { get; }

        /// <summary>
        /// Maps the positions of intersections to all intersections directly reachable from it.
        /// The reachable intersections are specified by their position (Target), the direction they're in (Direction) and their distance form the other intersection (Distance).
        /// </summary>
        public Dictionary<Vector2Int, List<(Vector2Int Target, Vector2Int Direction, float Distance)>> ReachableFromIntersection { get; }

        /// <summary>
        /// Positions of all intersections.
        /// </summary>
        public HashSet<Vector2Int> Intersections { get; }

        /// <summary>
        /// The smallest x and y coordinates of any intersection.
        /// </summary>
        public Vector2Int Min { get; }

        /// <summary>
        /// The largest x and y coordinates of any intersection.
        /// </summary>
        public Vector2Int Max { get; }

        /// <summary>
        /// The difference between the largest and smallest x and y coordinates of any intersection.
        /// </summary>
        public Vector2Int Size { get; }

        public static Map FromText(string[] text)
        {
            if (!StreetParser.TryParse(text, out var start, out var end, out var streets)) throw new ArgumentException(nameof(text));

            return new Map(streets, start, end);
        }

        public Map(List<Street> streets, Vector2Int start, Vector2Int end)
        {
            Streets = streets;
            Start = start;
            End = end;
            ReachableFromIntersection = new Dictionary<Vector2Int, List<(Vector2Int Target, Vector2Int Bidirection, float Distance)>>();
            Intersections = new HashSet<Vector2Int>();

            // Adds a street to the intersection at its starting point
            void registerStreet(Street street)
            {
                // Get the existing list or add a new one for key street.Start
                var reachableIntersections = ReachableFromIntersection.GetOrCreateValue(street.Start);

                reachableIntersections.Add((street.End, street.Path.Direction, street.Path.Length));

                Intersections.Add(street.Start);
            }

            // Add all streets to all intersections
            foreach (var street in streets)
            {
                registerStreet(street);
                // Both ends are intersections => flip to register the other
                registerStreet(street.Flipped);
            }

            Min = new Vector2Int(
                Streets.Min(x => Math.Min(x.Start.X, x.End.X)),
                Streets.Min(x => Math.Min(x.Start.Y, x.End.Y)));
            Max = new Vector2Int(
                Streets.Max(x => Math.Max(x.Start.X, x.End.X)),
                Streets.Max(x => Math.Max(x.Start.Y, x.End.Y)));
            Size = Max - Min;
        }

        /// <summary>
        /// Gets the shortest path as computed by dijkstras.
        /// </summary>
        /// <param name="fullDistance">The length of the computed path.</param>
        /// <returns>The path from starting point to ending point, including those points.</returns>
        public IEnumerable<Vector2Int> ShortestPath(out float fullDistance)
        {
            // The position a point is reached from on the shortest currently known path leading to it.
            var paths = new Dictionary<Vector2Int, Vector2Int>();
            // The distance of the shortest currently known path to a given point.
            var distances = new Dictionary<Vector2Int, float>();

            var priorityQueue = new SimplePriorityQueue<Vector2Int, float>();

            foreach (var intersection in Intersections)
            {
                var distance = intersection == Start ? 0 : float.PositiveInfinity;
                priorityQueue.Enqueue(intersection, distance);
                distances[intersection] = distance;
            }

            while (true) // Loop forever. Stop condition is handeled by a break.
            {
                var head = priorityQueue.Dequeue();
                var headDistance = distances[head];

                // End is at the top of the priority queue => the path to End is the shortest unvisited path => finished, break
                if (head == End)
                {
                    fullDistance = headDistance;
                    break;
                }

                var reachableStreets = ReachableFromIntersection[head];

                foreach (var (target, _, distance) in reachableStreets)
                {
                    var oldDistance = distances[target];

                    var newDistance = headDistance + distance;

                    if (newDistance > oldDistance) continue;

                    // Update the priority only if the key already exists. Otherwise add it again.
                    if (!priorityQueue.TryUpdatePriority(target, newDistance)) priorityQueue.Enqueue(target, newDistance);

                    paths[target] = head;
                    distances[target] = newDistance;
                }
            }

            var path = new List<Vector2Int>();
            for (var current = End; current != Start; current = paths[current]) path.Add(current);
            path.Add(Start);
            // Reverse such that Start is actually at the start and End at the end.
            path.Reverse();

            return path;
        }

        /// <summary>
        /// For use in <see cref="BilalsPath(int, float, out int, out float)"/>.
        /// </summary>
        protected class Path : IComparable<Path>, IEquatable<Path>
        {
            public readonly int Turns;
            public readonly float Distance;
            public readonly Vector2Int End;
            public readonly Vector2Int Direction;
            public readonly Path? Previous;

            public Path(int turns, float distance, Vector2Int end, Vector2Int direction, Path previous)
            {
                Turns = turns;
                Distance = distance;
                End = end;
                Direction = direction;
                Previous = previous;
            }

            public Path(Vector2Int end)
            {
                Turns = 0;
                Distance = 0;
                End = end;
                Direction = default;
                Previous = null;
            }

            /// <summary>
            /// Returns a new <see cref="Path"/> instance with <see cref="Previous"/> set to this and <see cref="End"/> set to <paramref name="next"/>.
            /// </summary>
            /// <param name="next">The <see cref="End"/> of the new <see cref="Path"/> instance.</param>
            /// <param name="direction">The <see cref="Vector2Int.Direction"/> value of the path between <see cref="End"/> and <paramref cref="next"/>.</param>
            /// <param name="distance">The <see cref="Vector2Int.Length"/> value of the path between <see cref="End"/> and <paramref cref="next"/>.</param>
            /// <returns>The new <see cref="Path"/> instance.</returns>
            public Path ContinueTo(Vector2Int next, Vector2Int direction, float distance)
            {
                var isTurn = Direction != default && Direction != direction;

                return new Path(Turns + (isTurn ? 1 : 0), Distance + distance, next, direction, this);
            }

            /// <inheritdoc/>
            public int CompareTo(Path other)
            {
                var turnsComp = Turns.CompareTo(other.Turns);
                if (turnsComp != 0) return turnsComp;
                return Distance.CompareTo(other.Distance);
            }

            /// <inheritdoc/>
            public override bool Equals(object? obj) => obj is Path path && Equals(path);

            /// <inheritdoc/>
            public bool Equals(Path path) => End.Equals(path.End)
                && EqualityComparer<Path?>.Default.Equals(Previous, path.Previous);

            /// <inheritdoc/>
            public override int GetHashCode() => HashCode.Combine(End, Previous);

            public override string ToString() => (Previous == null ? string.Empty : Previous.ToString() + " -> ")
                + $"[{End}, T: {Turns}, D: {Distance}]";
        }

        /// <summary>
        /// Gets bilals path, meaning the path with least turns shorter in length then <paramref name="maxLength"/>.
        /// Returns <c>null</c> if no such path was found.
        /// </summary>
        /// <param name="maxLength">The longest the path can be.</param>
        /// <param name="fullTurns">The amount of turns in the computed path.</param>
        /// <param name="fullDistance">The length of the computed path.</param>
        /// <returns>The path from starting point to ending point, including those points.</returns>
        public IEnumerable<Vector2Int>? BilalsPath(int maxTurns, float maxLength, out int fullTurns, out float fullDistance)
        {
            // Path implements the priority comparisons itself => use it as key and priority type
            var priorityQueue = new SimplePriorityQueue<Path, Path>();
            var paths = new Dictionary<DirectedVector2Int, Dictionary<int, Path>>();

            var start = new Path(Start);
            priorityQueue.Enqueue(start, start);

            Path? endPath = null;

            while (priorityQueue.Any())
            {
                var head = priorityQueue.Dequeue();

                if (head.End == End)
                {
                    endPath = head;
                    break;
                }

                var reachableStreets = ReachableFromIntersection[head.End];

                foreach (var (target, direction, distance) in reachableStreets)
                {
                    var newPath = head.ContinueTo(target, direction, distance);

                    if (newPath.Distance > maxLength
                        || newPath.Turns > maxTurns)
                    {
                        continue;
                    }

                    var directedTarget = new DirectedVector2Int(target, direction);
                    var pathsToTarget = paths.GetOrCreateValue(directedTarget);

                    if (!pathsToTarget.TryGetValue(newPath.Turns, out var oldPath))
                    {
                        pathsToTarget[newPath.Turns] = newPath;
                        priorityQueue.EnqueueWithoutDuplicates(newPath, newPath);
                        continue;
                    }

                    if (oldPath.Turns < newPath.Turns) continue;
                    if (oldPath.Turns == newPath.Turns && oldPath.Distance < newPath.Distance) continue;

                    pathsToTarget[newPath.Turns] = newPath;
                    priorityQueue.EnqueueWithoutDuplicates(newPath, newPath);
                }
            }

            if (endPath == null)
            {
                fullTurns = int.MaxValue;
                fullDistance = float.PositiveInfinity;
                return null;
            }

            fullTurns = endPath.Turns;
            fullDistance = endPath.Distance;

            var path = new List<Vector2Int>();

            for (var current = endPath; current!.End != Start; current = current.Previous!) path.Add(current.End);
            path.Add(Start);
            path.Reverse();

            return path;
        }

        /// <summary>
        /// Gets bilals path, meaning the path with least turns shorter in length then <paramref name="distanceFactor"/> times the length of the shortest path.
        /// Returns <c>null</c> if no such path was found.
        /// </summary>
        /// <param name="distanceFactor">The factor to apply to the length of the shortest path.</param>
        /// <param name="shortestPath">The shortest path. Same as output of <see cref="ShortestPath(out float)"/>.</param>
        /// <param name="shortestPathLength">The length of the shortest path. Same as out parameter of <see cref="ShortestPath(out float)"/>.</param>
        /// <param name="fullTurns">The amount of turns in the computed path.</param>
        /// <param name="fullDistance">The length of the computed path.</param>
        /// <returns>The path from starting point to ending point, including those points.</returns>
        public IEnumerable<Vector2Int>? BilalsPath(float distanceFactor, out IEnumerable<Vector2Int> shortestPath, out int shorestPathTurns, out float shortestPathLength, out int fullTurns, out float fullDistance)
        {
            shortestPath = ShortestPath(out shortestPathLength);
            shorestPathTurns = shortestPath.CountTurns();
            return BilalsPath(shorestPathTurns, distanceFactor * shortestPathLength, out fullTurns, out fullDistance);
        }
    }
}
