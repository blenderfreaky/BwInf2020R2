using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Afg1Stromrallye.API
{
    public class Board
    {
        public readonly int Width, Height;
        public readonly List<Battery> Batteries;

        public Board(int width, int height, HashSet<Vector2Int> batteryPositions)
        {
            Width = width;
            Height = height;
            Batteries = batteryPositions;
            Paths = new Dictionary<(Vector2Int First, Vector2Int Second), (int? Distance, IEnumerable<Vector2Int>? Path)>();
        }

        private void BuildGraph()
        {
            foreach (var battery in Batteries) BuildGraphFor(battery.Position);
        }

        private void BuildGraphFor(Vector2Int position)
        {
            var paths = new Dictionary<Vector2Int, (int Distance, Vector2Int Origin)> { [position] = (0, position) };

            var notReached = Batteries.ToList();

            var dHead = position;

            while (notReached.Count > 0)
            {
                int distance = paths[dHead].Distance + 1;
                Vector2Int? nHead = null;

                foreach (Vector2Int next in GetVisitable(dHead))
                {
                    if (paths.TryGetValue(next, out var path))
                    {
                        if (path.Distance > distance)
                        {
                            paths[next] = (distance, dHead);
                        }
                    }
                    else
                    {
                        paths[next] = (distance, dHead);
                    }
                    nHead = next;
                }

                if (nHead == null) throw null;

                dHead = nHead.Value;
            }

            foreach (var battery in Batteries)
            {
                var val = paths[battery.Position];
                var path = new List<Vector2Int>();
                for (var head = position; head != battery.Position; head = paths[head].Origin) path.Add(head);
                path.Add(battery.Position);
                SetPath(position, battery.Position, val.Distance, path);
            }
        }

        private IEnumerable<Vector2Int> GetVisitable(Vector2Int position) =>
            position
            .GetNeighbours()
            .Where(x => Contains(x) && !Batteries.Contains(x));

        public bool Contains(Vector2Int position) =>
            position.X >= 0 && position.X < Width
            && position.Y >= 0 && position.Y < Height;

        private readonly Dictionary<(Vector2Int First, Vector2Int Second), (int? Distance, IEnumerable<Vector2Int>? Path)> Paths;

        public (int? Distance, IEnumerable<Vector2Int>? Path) GetPath(Vector2Int lhs, Vector2Int rhs)
        {
            int lhsScore = lhs.X + (lhs.Y * Width);
            int rhsScore = rhs.X + (rhs.Y * Width);

            if (lhsScore < rhsScore)
            {
                return Paths[(lhs, rhs)];
            }
            else
            {
                return Paths[(rhs, lhs)];
            }
        }

        private void SetPath(Vector2Int lhs, Vector2Int rhs, int? distance, IEnumerable<Vector2Int>? path)
        {
            int lhsScore = lhs.X + (lhs.Y * Width);
            int rhsScore = rhs.X + (rhs.Y * Width);

            if (lhsScore < rhsScore)
            {
                Paths[(lhs, rhs)] = (distance, path);
            }
            else
            {
                Paths[(rhs, lhs)] = (distance, path);
            }
        }
    }
}
