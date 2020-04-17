namespace Afg3Abbiegen
{
    using Priority_Queue;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public readonly struct DirectedVector2Int : IEquatable<DirectedVector2Int>
    {
        public readonly Vector2Int Position;
        public readonly Vector2Int Direction;

        public DirectedVector2Int(Vector2Int position, Vector2Int direction)
        {
            Position = position;
            Direction = direction;
        }

        public readonly override bool Equals(object? obj) => obj is DirectedVector2Int other && Equals(other);
        public readonly bool Equals(DirectedVector2Int other) => Position.Equals(other.Position) && Direction.Equals(other.Direction);
        public readonly override int GetHashCode() => HashCode.Combine(Position, Direction);

        public readonly override string ToString() => $"{Position} {Direction}";
    }
}
