using System;
using System.Collections.Generic;
using System.Linq;

namespace Afg1Stromrallye.API
{
    public class Battery
    {
        public Vector2Int Position { get; }
        public Dictionary<Battery, IReadOnlyList<Vector2Int>> ShortestPaths { get; }
        public Dictionary<Battery, (Vector2Int ExitSpot, Vector2Int IntermediateSpot)> Roundabouts { get; }

        public Battery(Vector2Int position)
        {
            Position = position;
            ShortestPaths = new Dictionary<Battery, IReadOnlyList<Vector2Int>>();
            Roundabouts = new Dictionary<Battery, (Vector2Int ExitSpot, Vector2Int IntermediateSpot)>();
        }

        public void BuildShortestPaths(Board board)
        {
            //TODO: Use Dijkstra
            foreach (var battery in board.Batteries)
            {
                var path = new List<Vector2Int>();

                foreach (int x in Range(Position.X, battery.Position.X)) path.Add(new Vector2Int(x, Position.X));
                foreach (int y in Range(Position.Y, battery.Position.Y)) path.Add(new Vector2Int(battery.Position.X, y));
                path.Add(battery.Position);

                ShortestPaths[battery] = path;
                //TODO: Properly calculate this
                Roundabouts[battery] = (path[1], path[2]);
            }
        }

        private static IEnumerable<int> Range(int start, int end)
        {
            if (end < start)
            {
                for (int i = start; i > end; i--)
                {
                    yield return i;
                }

                yield break;
            }

            for (int i = start; i < end; i++)
            {
                yield return i;
            }
        }
    }
}
