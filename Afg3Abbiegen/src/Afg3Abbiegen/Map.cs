using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Afg3Abbiegen
{
    public class Map
    {
        /// <summary>
        /// All streets.
        /// </summary>
        public List<Street> Streets { get; }

        /// <summary>
        /// The starting point.
        /// </summary>
        public Vector2Int Start { get; }

        /// <summary>
        /// The target.
        /// </summary>
        public Vector2Int End { get; }

        /// <summary>
        /// Maps the positions of intersections to all intersections directly reachable from it.
        /// </summary>
        public Dictionary<Vector2Int, List<Vector2Int>> ReachableFromIntersection { get; }

        /// <summary>
        /// Maps the positions of intersections to all streets that are part of it.
        /// Both ends of each street are at an intersection.
        /// The start of any street is the intersection itself if there is no straight path forward.
        /// </summary>
        public Dictionary<Vector2Int, List<Street>> Intersections { get; }

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
            ReachableFromIntersection = new Dictionary<Vector2Int, List<Vector2Int>>();
            Intersections = new Dictionary<Vector2Int, List<Street>>();

            // Adds a street to the intersection at its starting point
            void RegisterStreet(Street street)
            {
                var reachableIntersections = ReachableFromIntersection.GetOrCreateValue(street.Start);
                reachableIntersections.Add(street.End);

                var streets = Intersections.GetOrCreateValue(street.Start);
                streets.Add(street);
            }

            // Add all streets to all intersections
            foreach (var street in streets)
            {
                RegisterStreet(street);
                // Both ends are intersections => flip to register the other
                RegisterStreet(street.Flipped);
            }

            foreach (var intersection in Intersections)
            {
                var center = intersection.Key;
                var streetsAtIntersection = intersection.Value;

                var streetInDirection = new Dictionary<Vector2Int, int>();

                for (int i = 0; i < streetsAtIntersection.Count; i++)
                {
                    var street = intersection.Value[i];

                    var direction = street.Path.Bidirection;

                    if (!streetInDirection.TryGetValue(direction, out var otherIndex))
                    {
                        streetInDirection[direction] = i;
                        continue;
                    }

                    // -- Streets are parallel => Merge --

                    // Remove current street and appropriately move index
                    streetsAtIntersection.RemoveAt(i);
                    i--;

                    // Edit other street to contain both
                    var otherStreet = streetsAtIntersection[otherIndex];
                    // Start is always at center, if it is not, that's because it has been previously merged => 3 parallel streets => error
                    if (otherStreet.Start != center) throw new InvalidOperationException("Too many parallel streets at " + center);

                    streetsAtIntersection[otherIndex] = new Street(street.End, otherStreet.End);
                }
            }
        }

        public IReadOnlyCollection<Vector2Int> ShortestPath()
        {
            var paths = new Dictionary<Vector2Int, (Vector2Int From, int TotalDistance)>();

            var unvisitedNodes = Intersections.Keys.ToList();
            

            paths[Start] = (Start, 0);
            foreach (var pos in Intersections.Keys)
            {
                paths[pos] = (pos, int.MaxValue);
            }

            while (true)
            {
                var head = priorityQueue[0];
            }
            throw null;
        }
    }
}
