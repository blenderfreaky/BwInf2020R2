namespace Afg2Geburtstag
{
    using System.Collections.Generic;
    using System.Numerics;

    public class ValueTerm : ITerm
    {
        public BigRational Value { get; }

        public ValueTerm(BigRational value) => Value = value;

        public override string ToString() => Value.ToString();
        public override bool Equals(object? obj) => obj is ValueTerm term && Value.Equals(term.Value);
        public override int GetHashCode() => -1937169414 + Value.GetHashCode();
    }
}
