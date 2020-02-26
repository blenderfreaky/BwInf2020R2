using System.Collections.Generic;

namespace Afg3Abbiegen
{
    public class Street
    {
        public Vector2Int Start { get; }
        public Vector2Int End { get; }

        public IReadOnlyList<Vector2Int> Intersections { get; }
    }
}
