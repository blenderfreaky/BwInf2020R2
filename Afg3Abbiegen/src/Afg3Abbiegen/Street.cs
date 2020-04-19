namespace Afg3Abbiegen
{
    using System;

    /// <summary>
    /// Represents a street between two points.
    /// </summary>
    public readonly struct Street : IEquatable<Street>
    {
        /// <summary>
        /// The start of the street.
        /// </summary>
        public readonly Vector2Int Start;
        /// <summary>
        /// The end of the street.
        /// </summary>
        public readonly Vector2Int End;

        public Street(Vector2Int start, Vector2Int end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Gets a new with <see cref="Start"/> being equal to <see cref="End"/> of <c>this</c> and vice versa.
        /// </summary>
        public readonly Street Flipped => new Street(End, Start);

        /// <summary>
        /// Returns the path from start to end.
        /// </summary>
        public readonly Vector2Int Path => End - Start;

        /// <inheritdoc/>
        public readonly override string ToString() => $"{Start} - {End}";

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is Street street && Equals(street);

        /// <inheritdoc/>
        public readonly bool Equals(Street other) => (other.Start, other.End) == (Start, End) || (other.End, other.Start) == (Start, End);

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(Start, End) * HashCode.Combine(End, Start);

        public static bool operator ==(Street left, Street right) => left.Equals(right);

        public static bool operator !=(Street left, Street right) => !(left == right);
    }
}