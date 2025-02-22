﻿namespace Afg2Geburtstag
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Numerics;
    using System.Runtime.CompilerServices;

    [DebuggerDisplay("{Numerator}/{Denominator}")]
    public readonly struct Rational : IEquatable<Rational>, ITerm
    {
        public readonly long Numerator;
        public readonly long Denominator;

        public readonly int HashCode;

        public Rational(long numerator, long denominator, bool validate = true)
        {
            HashCode = 0;

            if (!validate || denominator == 1)
            {
                Numerator = numerator;
                Denominator = denominator;
                HashCode = CalculateHashCode();
                return;
            }

            if (denominator == 0) throw new DivideByZeroException("Denominator may not be zero");

            if (numerator == 0)
            {
                Numerator = 0;
                Denominator = 1;
                HashCode = CalculateHashCode();
                return;
            }

            checked
            {
                if (numerator < 0)
                {
                    numerator *= -1;
                    denominator *= -1;
                }
                var sign = 1;
                if (denominator < 0)
                {
                    denominator *= -1;
                    sign = -1;
                }
                var gcd = GCD(numerator, denominator);
                Numerator = sign * numerator / gcd;
                Denominator = denominator / gcd;
            }
            HashCode = CalculateHashCode();
        }

        private int CalculateHashCode()
        {
            var hashCode = 1503752452;
            hashCode = (hashCode * -1521134295) + Numerator.GetHashCode();
            hashCode = (hashCode * -1521134295) + Denominator.GetHashCode();
            return hashCode;
        }

        public Rational(long numerator)
        {
            Numerator = numerator;
            Denominator = 1;
            HashCode = 0;
            HashCode = CalculateHashCode();
        }

        /// <summary>
        /// Calculates the greatest common divisor between two positive <see cref="long"/>s.
        /// </summary>
        /// <param name="a">The first <see cref="long"/>.</param>
        /// <param name="b">The second <see cref="long"/>.</param>
        /// <returns>The greatest common divisior.</returns>
        private static long GCD(long a, long b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b) a %= b;
                else b %= a;
            }

            return a == 0 ? b : a;
        }

        public static readonly Rational Zero = 0;
        public static readonly Rational One = 1;

        public static implicit operator Rational(long @int) => new Rational(@int);

        public static implicit operator Rational(int @int) => new Rational(@int);

        public Rational Absolute => new Rational((IsNegative ? -1 : 1) * Numerator, Denominator, false);

        public bool IsZero => Numerator == BigInteger.Zero;
        public bool IsInteger => Denominator == BigInteger.One;
        public bool IsPositive => Numerator > BigInteger.Zero;
        public bool IsNegative => Numerator < BigInteger.Zero;

        /// <summary>
        /// All factorials from 0 to 20.
        /// </summary>
        private static readonly long[] _factorials = new long[]
        {
            1,
            1,
            2,
            6,
            24,
            120,
            720,
            5040,
            40320,
            362880,
            3628800,
            39916800,
            479001600,
            6227020800,
            87178291200,
            1307674368000,
            20922789888000,
            355687428096000,
            6402373705728000,
            121645100408832000,
            2432902008176640000,
        };

        /// <summary>
        /// Computes the fatorial of <paramref name="operand"/>.
        /// If <paramref name="operand"/> is negative, non-integer or bigger than <paramref name="limit"/> <c>null</c> is returned.
        /// </summary>
        /// <param name="operand">The operand to apply factorial to.</param>
        /// <param name="limit">The size of the biggest allowed operand.</param>
        /// <returns>The result of applying factorial or <c>null</c>.</returns>
        public static Rational? Factorial(Rational operand, long limit = 20)
        {
            if (!operand.IsInteger) return null;
            if (operand.Numerator < 0) return null;
            if (operand.Numerator > limit || operand.Numerator > 20) return null;

            return new Rational(_factorials[operand.Numerator]);
        }

        /// <summary>
        /// Converts a <see cref="double"/> to a <see cref="Rational"/>.
        /// </summary>
        /// <param name="value">The <see cref="double"/> to convert.</param>
        public static explicit operator Rational(double value)
        {
            if (value % 1 == 0) // Return whole numbers directly
            {
                return new Rational((long)value);
            }
            else
            {
                var asString = value.ToString("R", CultureInfo.InvariantCulture);
                var components = asString.Split('.');

                if (components.Length != 2) throw new InvalidOperationException("Invalid state");

                var (integerComponent, fractionalComponent) = (components[0], components[1]);

                var numerator = long.Parse(integerComponent + fractionalComponent);
                var denominator = 1;
                for (var i = 0; i < fractionalComponent.Length; i++) denominator *= 10;

                return new Rational(numerator, denominator);
            }
        }

        /// <summary>
        /// Converts the string representation of a number to its <see cref="Rational"/> equivalent.
        /// </summary>
        /// <param name="text">The string representation.</param>
        /// <returns>The <see cref="Rational"/> equivalent.</returns>
        public static Rational Parse(string text)
        {
            var components = text.Split('/');
            if (components.Length == 1) return (Rational)(double.Parse(text));
            if (components.Length != 2) throw new FormatException("Too many slashes");
            return new Rational(int.Parse(components[0]), int.Parse(components[1]));
        }

        /// <inheritdoc/>
        public override string ToString() => Numerator.ToString() + (IsInteger ? string.Empty : "/" + Denominator.ToString());

        /// <inheritdoc/>
        public string ToLaTeX() => IsInteger
            ? Numerator.ToString()
            : $"\\frac{{{Numerator}}}{{{Denominator}}}";

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Rational other) => other.Numerator == Numerator && other.Denominator == Denominator;

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object? obj) => obj is Rational rational && Equals(rational);

        /// <inheritdoc/>
        Rational ITerm.Value => this;

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IEquatable<ITerm>.Equals(ITerm other) => other is Rational rational && Equals(rational);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => HashCode;

        public static Rational operator +(Rational lhs, Rational rhs) =>
            new Rational((lhs.Numerator * rhs.Denominator) + (rhs.Numerator * lhs.Denominator), lhs.Denominator * rhs.Denominator);

        public static Rational operator -(Rational lhs, Rational rhs) =>
            new Rational((lhs.Numerator * rhs.Denominator) - (rhs.Numerator * lhs.Denominator), lhs.Denominator * rhs.Denominator);

        public static Rational operator *(Rational lhs, Rational rhs) =>
            new Rational(lhs.Numerator * rhs.Numerator, lhs.Denominator * rhs.Denominator);

        public static Rational operator /(Rational lhs, Rational rhs) =>
            new Rational(lhs.Numerator * rhs.Denominator, lhs.Denominator * rhs.Numerator);

        public static bool operator >(Rational lhs, Rational rhs) =>
            lhs.Numerator * rhs.Denominator > rhs.Numerator * lhs.Denominator;

        public static bool operator <(Rational lhs, Rational rhs) =>
            lhs.Numerator * rhs.Denominator < rhs.Numerator * lhs.Denominator;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Rational left, Rational right) => left.Equals(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Rational left, Rational right) => !(left == right);
    }
}