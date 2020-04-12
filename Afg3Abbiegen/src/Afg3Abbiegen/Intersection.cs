namespace Afg3Abbiegen
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    public struct Intersection
    {
        public Street Street;
        public Vector2Int Position;

        public Intersection(Street street, Vector2Int position)
        {
            Street = street;
            Position = position;
        }

        public override bool Equals(object? obj) => obj is Intersection other && EqualityComparer<Street>.Default.Equals(Street, other.Street) && Position.Equals(other.Position);
        public override int GetHashCode() => HashCode.Combine(Street, Position);
}
}
