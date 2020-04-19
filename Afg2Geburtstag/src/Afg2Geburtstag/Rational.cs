namespace Afg2Geburtstag
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Numerics;

    [DebuggerDisplay("{Numerator}/{Denominator}")]
    public readonly struct Rational : IEquatable<Rational>, ITerm
    {
        public readonly long Numerator;
        public readonly long Denominator;

        Rational ITerm.Value => this;

        public Rational(long numerator, long denominator, bool validate = true)
        {
            if (!validate || denominator == 1)
            {
                Numerator = numerator;
                Denominator = denominator;
                return;
            }

            if (denominator == 0) throw new DivideByZeroException("Denominator may not be zero");

            if (numerator == 0)
            {
                Numerator = 0;
                Denominator = 1;

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
        }

        private static long GCD(long a, long b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b) a %= b;
                else b %= a;
            }

            return a == 0 ? b : a;
        }

        public Rational(long numerator)
        {
            Numerator = numerator;
            Denominator = 1;
        }

        public static readonly Rational Zero = 0;
        public static readonly Rational One = 1;

        public static implicit operator Rational(long @int) => new Rational(@int);

        public static implicit operator Rational(int @int) => new Rational(@int);

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
            0,
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

        public static Rational? Factorial(Rational rational, long limit = 20)
        {
            if (!rational.IsInteger) return null;
            if (rational.Numerator < 0) return null;
            if (rational.Numerator > limit || rational.Numerator > 20) return null;

            return new Rational(_factorials[rational.Numerator]);
        }

        public static Rational ToFraction(double value)
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
                var denominator = fractionalComponent.Length;

                return new Rational(numerator, denominator);
            }
        }

        public static Rational Parse(string text) => ToFraction(double.Parse(text));

        public override string ToString() => Numerator.ToString() + (IsInteger ? string.Empty : "/" + Denominator.ToString());
        public string ToLaTeX() => IsInteger
            ? Numerator.ToString()
            : $"\\frac{{{Numerator}}}{{{Denominator}}}";

        public bool Equals(Rational other) => other.Numerator == Numerator && other.Denominator == Denominator;

        public override bool Equals(object obj) => obj is Rational rational && Equals(rational);

        public override int GetHashCode()
        {
            var hashCode = 1503752452;
            hashCode = (hashCode * -1521134295) + Numerator.GetHashCode();
            hashCode = (hashCode * -1521134295) + Denominator.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Rational left, Rational right) => left.Equals(right);

        public static bool operator !=(Rational left, Rational right) => !(left == right);
    }
}
