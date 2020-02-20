using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Afg1Stromrallye.API
{
    public class Board
    {
        public int Width { get; }
        public int Height { get; }
        public IReadOnlyCollection<Battery> Batteries { get; }

        public Board(int width, int height, IReadOnlyCollection<Vector2Int> batteryPositions)
        {
            Width = width;
            Height = height;
            Batteries = batteryPositions.Select(x => new Battery(x)).ToList();
            foreach (var battery in Batteries) battery.BuildShortestPaths(this);
        }

        private static IEnumerable<Vector2Int> GetDirectNeighbours(Vector2Int position)
        {
            yield return position + new Vector2Int(0, -1);
            yield return position + new Vector2Int(0, +1);
            yield return position + new Vector2Int(-1, 0);
            yield return position + new Vector2Int(+1, 0);
        }

        public IEnumerable<Vector2Int> GetVisitable(Vector2Int position) =>
            GetDirectNeighbours(position)
            .Where(x => Contains(x) && !Batteries.Any(y => y.Position == x));

        public bool Contains(Vector2Int position) =>
            position.X >= 0 && position.X < Width
            && position.Y >= 0 && position.Y < Height;
    }
}
