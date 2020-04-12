using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Afg3Abbiegen
{
    public class Map
    {
        public IReadOnlyList<Street> Streets { get; }

        public IReadOnlyDictionary<Vector2Int, IReadOnlyCollection<Street>> Intersections { get; }

        public Map(IReadOnlyList<Street> streets)
        {
            Streets = streets;

            var intersections = new Dictionary<Vector2Int, HashSet<Street>>();

            // TBD
            foreach (var street in streets)
            {
                foreach (var intersection in street.Intersections)
                {
                    if (!intersections.TryGetValue(intersection.Position, out var value))
                    {
                        value = intersections[intersection.Position] = new HashSet<Street>();
                    }

                    value.Add(street);
                }
            }

            //Intersections = intersections;
        }
    }
}
