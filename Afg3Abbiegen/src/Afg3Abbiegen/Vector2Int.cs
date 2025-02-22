﻿namespace Afg3Abbiegen
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Represents a 2-dimensional vector of <see cref="int"/>
    /// </summary>
    [DebuggerStepThrough]
    [DebuggerDisplay("({X}, {Y})")]
    public readonly struct Vector2Int : IEquatable<Vector2Int>
    {
        /// <summary>
        /// Represents the X-component of the <see cref="Vector2Int"/>.
        /// </summary>
        public readonly int X;

        /// <summary>
        /// Represents the Y-component of the <see cref="Vector2Int"/>.
        /// </summary>
        public readonly int Y;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector2Int"/> structure.
        /// </summary>
        /// <param name="x">The X-component of the <see cref="Vector2Int"/>.</param>
        /// <param name="y">The Y-component of the <see cref="Vector2Int"/>.</param>
        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Returns a <see cref="Vector2Int"/> uniquely describing the direction of the vector in a way that is invariant to scale.
        /// Generated by dividing X and Y components by their GCD.
        /// </summary>
        /// <example>
        /// The two <see cref="Vector2Int"/> instances <c>(1, 2)</c> and <c>(2, 4)</c> have the same <see cref="Direction"/> value.
        /// <c>(1, 2)</c> and <c>(1, 3)</c> do not.
        /// Note: <c>(1, 0)</c> and <c>(-1, 0)</c> also do not share the same <see cref="Direction"/> value, as they differ by flipping (scaling by -1).
        /// </example>
        public readonly Vector2Int Direction
        {
            get
            {
                static int gcd(int a, int b)
                {
                    while (a != 0 && b != 0)
                    {
                        if (a > b) a %= b;
                        else b %= a;
                    }

                    return a == 0 ? b : a;
                }

                var xyGcd = gcd(Math.Abs(X), Math.Abs(Y));

                return new Vector2Int(X / xyGcd, Y / xyGcd);
            }
        }

        /// <summary>
        /// The length of the vector.
        /// </summary>
        public readonly float Length => MathF.Sqrt((X * X) + (Y * Y));

        /// <inheritdoc/>
        public readonly bool Equals(Vector2Int vector) => X == vector.X && Y == vector.Y;

        /// <inheritdoc/>
        public override readonly bool Equals(object obj) => obj is Vector2Int vector && Equals(vector);

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(X, Y);

        public static bool operator ==(Vector2Int left, Vector2Int right) => left.Equals(right);

        public static bool operator !=(Vector2Int left, Vector2Int right) => !(left == right);

        public static Vector2Int operator +(Vector2Int lhs, Vector2Int rhs)
            => new Vector2Int(lhs.X + rhs.X, lhs.Y + rhs.Y);

        public static Vector2Int operator -(Vector2Int lhs, Vector2Int rhs)
            => new Vector2Int(lhs.X - rhs.X, lhs.Y - rhs.Y);

        public static Vector2Int operator *(Vector2Int lhs, int rhs)
            => new Vector2Int(lhs.X * rhs, lhs.Y * rhs);

        public static Vector2Int operator -(Vector2Int vec)
            => new Vector2Int(-vec.X, -vec.Y);

        /// <inheritdoc/>
        public override readonly string ToString() => $"({X}, {Y})";
    }
}