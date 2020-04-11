namespace Afg2Geburtstag
{
    using System;
    using System.Diagnostics;
    using System.Numerics;

    [DebuggerDisplay("{Numerator}/{Denominator}")]
    public readonly struct BigRational : IEquatable<BigRational>, ITerm
    {
        public readonly BigInteger Numerator;
        public readonly BigInteger Denominator;

        BigRational ITerm.Value => this;

        public BigRational(BigInteger numerator, BigInteger denominator, bool validate = true)
        {
            if (validate)
            {
                var gcd = BigInteger.GreatestCommonDivisor(numerator, denominator);
                if (gcd > 0 && denominator < 0) gcd *= -1;
                Numerator = numerator / gcd;
                Denominator = denominator / gcd;
            }
            else
            {
                Numerator = numerator;
                Denominator = denominator;
            }
        }

        public BigRational(BigInteger numerator)
        {
            Numerator = numerator;
            Denominator = 1;
        }

        public static readonly BigRational Zero = 0;
        public static readonly BigRational One = 1;

        public static implicit operator BigRational(BigInteger integer) => new BigRational(integer);

        public static implicit operator BigRational(long @int) => new BigRational(@int);

        public static implicit operator BigRational(int @int) => new BigRational(@int);

        public static BigRational operator +(BigRational lhs, BigRational rhs) =>
            new BigRational((lhs.Numerator * rhs.Denominator) + (rhs.Numerator * lhs.Denominator), lhs.Denominator * rhs.Denominator);

        public static BigRational operator -(BigRational lhs, BigRational rhs) =>
            new BigRational((lhs.Numerator * rhs.Denominator) - (rhs.Numerator * lhs.Denominator), lhs.Denominator * rhs.Denominator);

        public static BigRational operator *(BigRational lhs, BigRational rhs) =>
            new BigRational(lhs.Numerator * rhs.Numerator, lhs.Denominator * rhs.Denominator);

        public static BigRational operator /(BigRational lhs, BigRational rhs) =>
            new BigRational(lhs.Numerator * rhs.Denominator, lhs.Denominator * rhs.Numerator);

        public static bool operator >(BigRational lhs, BigRational rhs) =>
            lhs.Numerator * rhs.Denominator > rhs.Numerator * lhs.Denominator;

        public static bool operator <(BigRational lhs, BigRational rhs) =>
            lhs.Numerator * rhs.Denominator < rhs.Numerator * lhs.Denominator;

        public BigRational Absolute => new BigRational((IsNegative ? -1 : 1) * Numerator, Denominator, false);

        public bool IsZero => Numerator == BigInteger.Zero;
        public bool IsInteger => Denominator == BigInteger.One;
        public bool IsPositive => Numerator > BigInteger.Zero;
        public bool IsNegative => Numerator < BigInteger.Zero;

        public static BigRational? Factorial(BigRational rational, long limit)
        {
            if (!rational.IsInteger) return null;
            if (rational.Numerator < 0) return null;
            if (rational.Numerator > limit) return null;

            return new BigRational(BigIntegerFactorial.Factorial((long)rational.Numerator));
        }

        public override string ToString() => Numerator.ToString() + (IsInteger ? string.Empty : "/" + Denominator.ToString());

        public bool Equals(BigRational other) => other.Numerator == Numerator && other.Denominator == Denominator;

        public override bool Equals(object obj) => obj is BigRational rational && Equals(rational);

        public override int GetHashCode()
        {
            var hashCode = 1503752452;
            hashCode = (hashCode * -1521134295) + Numerator.GetHashCode();
            hashCode = (hashCode * -1521134295) + Denominator.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(BigRational left, BigRational right) => left.Equals(right);

        public static bool operator !=(BigRational left, BigRational right) => !(left == right);
    }
}
