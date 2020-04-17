namespace Afg3Abbiegen
{
    using Priority_Queue;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

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
        /// The reachable intersections are specified by their position (Target), the direction they're in (Bidirection) and their distance form the other intersection (Distance).
        /// </summary>
        public Dictionary<Vector2Int, List<(Vector2Int Target, Vector2Int Bidirection, float Distance)>> ReachableFromIntersection { get; }

        /// <summary>
        /// Positions of all intersections.
        /// </summary>
        public HashSet<Vector2Int> Intersections { get; }

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
                var reachableIntersections = ReachableFromIntersection.GetOrCreateValue(street.Start);
                reachableIntersections.Add((street.End, street.Path.Bidirection, street.Path.Length));

                Intersections.Add(street.Start);
            }

            // Add all streets to all intersections
            foreach (var street in streets)
            {
                registerStreet(street);
                // Both ends are intersections => flip to register the other
                registerStreet(street.Flipped);
            }
        }

        /// <summary>
        /// Gets the shortest path as computed by dijkstras.
        /// </summary>
        /// <param name="fullDistance">The length of the computed path.</param>
        /// <returns>The path from starting point to ending point, including those points.</returns>
        public IEnumerable<Vector2Int> ShortestPath(out float fullDistance)
        {
            var paths = new Dictionary<Vector2Int, Vector2Int>();
            var distances = new Dictionary<Vector2Int, float>();

            // TODO: Try FastPriorityQueue with a Dict for distance
            var priorityQueue = new SimplePriorityQueue<Vector2Int, float>();

            foreach (var intersection in Intersections)
            {
                var distance = intersection == Start ? 0 : float.PositiveInfinity;
                priorityQueue.Enqueue(intersection, distance);
                distances[intersection] = distance;
            }

            while (true)
            {
                var head = priorityQueue.Dequeue();
                var headDistance = distances[head];

                if (head == End)
                {
                    fullDistance = headDistance;
                    break;
                }

                var reachableStreets = ReachableFromIntersection[head];

                foreach (var (target, bidirection, distance) in reachableStreets)
                {
                    var oldDistance = distances[target];

                    var newDistance = headDistance + distance;

                    if (newDistance > oldDistance) continue;

                    if (!priorityQueue.TryUpdatePriority(target, newDistance)) priorityQueue.Enqueue(target, newDistance);

                    paths[target] = head;
                    distances[target] = newDistance;
                }
            }

            var path = new List<Vector2Int>();
            for (var current = End; current != Start; current = paths[current]) path.Add(current);
            path.Add(Start);
            path.Reverse();

            return path;
        }

        public static List<(Vector2Int, float)> Debug = new List<(Vector2Int, float)>();
        public static List<(Vector2Int,Vector2Int, float)> Debug2 = new List<(Vector2Int, Vector2Int, float)>();

        /// <summary>
        /// Gets bilals path, meaning the path with least turns shorter in length then <paramref name="maxLength"/>.
        /// </summary>
        /// <param name="maxLength">The longest the path can be.</param>
        /// <param name="fullTurns">The amount of turns in the computed path.</param>
        /// <param name="fullDistance">The length of the computed path.</param>
        /// <returns>The path from starting point to ending point, including those points.</returns>
        public IEnumerable<Vector2Int> BilalsPath(float maxLength, out int fullTurns, out float fullDistance)
        {
            var paths = new Dictionary<DirectedVector2Int, DirectedVector2Int>();
            var distances = new Dictionary<DirectedVector2Int, float>();
            var costs = new Dictionary<DirectedVector2Int, int>();

            // TODO: Try FastPriorityQueue
            var priorityQueue = new SimplePriorityQueue<DirectedVector2Int, int>();

            var starts = new HashSet<DirectedVector2Int>();
            var ends = new HashSet<DirectedVector2Int>();

            foreach (var intersection in ReachableFromIntersection)
            {
                var streetStart = intersection.Key;

                foreach (var (_, bidirection, _) in intersection.Value)
                {
                    var priority = int.MaxValue;
                    var street = new DirectedVector2Int(streetStart, bidirection);

                    if (streetStart == Start)
                    {
                        priority = 0;
                        distances[street] = 0;
                        starts.Add(street);
                    }
                    else
                    {
                        distances[street] = float.PositiveInfinity;
                    }

                    if (streetStart == End)
                    {
                        ends.Add(street);
                    }

                    priorityQueue.EnqueueWithoutDuplicates(street, priority);
                    costs[street] = priority;
                }
            }

            var maxCost = int.MaxValue;
            var bestEnd = ends.First(); // Arbitrarily choose the first one just to have some value

            while (priorityQueue.Any() &&!ends.All(paths.ContainsKey)) // While there are ends that have not gotten any path to them discovered
            {
                var head = priorityQueue.Dequeue();
                var headCost = costs[head];

                if (headCost > maxCost) break;

                var headDistance = distances[head];
                Debug.Add((head.Position, headDistance));

                if (ends.Contains(head))
                {
                    maxCost = headCost;

                    if (headDistance < distances[bestEnd]) bestEnd = head;
                }

                var reachableStreets = ReachableFromIntersection[head.Position];

                foreach (var (target, bidirection, distance) in reachableStreets)
                {
                    var newDistance = headDistance + distance;
                    //if (newDistance > maxLength + 1E-5) continue;

                    var directedTarget = new DirectedVector2Int(target, bidirection);
                    var oldCost = costs[directedTarget];

                    var newCost = headCost + (bidirection == head.Direction ? 0 : 1);

                    var oldDistance = distances[directedTarget];

                    if (newCost == oldCost && newDistance > oldDistance)
                        continue;
                    if (newCost > oldCost)
                        continue;

                    if (!priorityQueue.TryUpdatePriority(directedTarget, newCost)) priorityQueue.Enqueue(directedTarget, newCost);

                    paths[directedTarget] = head;
                    costs[directedTarget] = newCost;
                    distances[directedTarget] = newDistance;
                }
            }
            foreach(var p in paths)
            {
                //Debug2.Add((p.Key.Position, p.Value.Position, 3));
            }

            var path = new List<Vector2Int>();

            //var current = bestEnd;
            //for (; current.Position != Start; current = paths[current]) path.Add(current.Position);
            //path.Add(Start);
            //path.Reverse();

            fullDistance = distances[bestEnd];
            fullTurns = maxCost;

            return path;
        }

        /// <summary>
        /// Gets bilals path, meaning the path with least turns shorter in length then <paramref name="distanceFactor"/> times the length of the shortest path.
        /// </summary>
        /// <param name="distanceFactor">The factor to apply to the length of the shortest path.</param>
        /// <param name="shortestPath">The shortest path. Same as output of <see cref="ShortestPath(out float)"/>.</param>
        /// <param name="shortestPathLength">The length of the shortest path. Same as out parameter of <see cref="ShortestPath(out float)"/>.</param>
        /// <param name="fullTurns">The amount of turns in the computed path.</param>
        /// <param name="fullDistance">The length of the computed path.</param>
        /// <returns>The path from starting point to ending point, including those points.</returns>
        public IEnumerable<Vector2Int> BilalsPath(float distanceFactor, out IEnumerable<Vector2Int> shortestPath, out float shortestPathLength, out int fullTurns, out float fullDistance)
        {
            shortestPath = ShortestPath(out shortestPathLength);
            //fullTurns = 0;
            //fullDistance = 0;
            //return Array.Empty<Vector2Int>();
            return BilalsPath(distanceFactor * shortestPathLength, out fullTurns, out fullDistance);
        }
    }
}
