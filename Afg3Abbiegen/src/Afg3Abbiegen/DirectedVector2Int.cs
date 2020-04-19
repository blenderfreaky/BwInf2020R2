namespace Afg3Abbiegen
{
    using System;

    /// <summary>
    /// Represents a <see cref="Vector2Int"/> with a direction.
    /// </summary>
    public readonly struct DirectedVector2Int : IEquatable<DirectedVector2Int>
    {
        /// <summary>
        /// The position.
        /// </summary>
        public readonly Vector2Int Position;

        /// <summary>
        /// The direction.
        /// </summary>
        public readonly Vector2Int Direction;

        public DirectedVector2Int(Vector2Int position, Vector2Int direction)
        {
            Position = position;
            Direction = direction;
        }

        /// <inheritdoc/>
        public readonly override bool Equals(object? obj) => obj is DirectedVector2Int other && Equals(other);

        /// <inheritdoc/>
        public readonly bool Equals(DirectedVector2Int other) => Position.Equals(other.Position) && Direction.Equals(other.Direction);

        /// <inheritdoc/>
        public readonly override int GetHashCode() => HashCode.Combine(Position, Direction);

        /// <inheritdoc/>
        public readonly override string ToString() => $"{Position} {Direction}";
    }
}