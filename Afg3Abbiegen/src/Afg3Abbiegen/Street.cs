using System;

namespace Afg3Abbiegen
{
    public readonly struct Street : IEquatable<Street>
    {
        public readonly Vector2Int Start;
        public readonly Vector2Int End;

        public Street(Vector2Int start, Vector2Int end)
        {
            Start = start;
            End = end;
        }

        public readonly Street Flipped => new Street(End, Start);
        public readonly Vector2Int Path => End - Start;

        public readonly override string ToString() => $"{Start} - {End}";

        public override bool Equals(object obj) => obj is Street street && Equals(street);
        public readonly bool Equals(Street other) => (other.Start, other.End) == (Start, End) || (other.End, other.Start) == (Start, End);
        public override int GetHashCode() => HashCode.Combine(Start, End) * HashCode.Combine(End, Start);

        public static bool operator ==(Street left, Street right) => left.Equals(right);
        public static bool operator !=(Street left, Street right) => !(left == right);
    }
}
