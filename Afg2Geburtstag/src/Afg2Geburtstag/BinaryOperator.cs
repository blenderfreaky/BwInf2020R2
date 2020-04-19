namespace Afg2Geburtstag
{
    using System;
    using System.Collections.Generic;

    public class BinaryOperator
    {
        public Func<ITerm, ITerm, Rational?> Evaluate { get; }
        public Func<ITerm, ITerm, string> OperationToString { get; }
        public Func<ITerm, ITerm, string> OperationToLaTeX { get; }

        public BinaryOperator(Func<ITerm, ITerm, Rational?> evaluate, Func<ITerm, ITerm, string> operationToString, Func<ITerm, ITerm, string> operationToLaTeX)
        {
            Evaluate = evaluate;
            OperationToString = operationToString;
            OperationToLaTeX = operationToLaTeX;
        }

        public override bool Equals(object? obj) => obj is BinaryOperator @operator && EqualityComparer<Func<ITerm, ITerm, Rational?>>.Default.Equals(Evaluate, @operator.Evaluate) && EqualityComparer<Func<ITerm, ITerm, string>>.Default.Equals(OperationToString, @operator.OperationToString) && EqualityComparer<Func<ITerm, ITerm, string>>.Default.Equals(OperationToLaTeX, @operator.OperationToLaTeX);

        public override int GetHashCode()
        {
            int hashCode = -912930555;
            hashCode = (hashCode * -1521134295) + EqualityComparer<Func<ITerm, ITerm, Rational?>>.Default.GetHashCode(Evaluate);
            hashCode = (hashCode * -1521134295) + EqualityComparer<Func<ITerm, ITerm, string>>.Default.GetHashCode(OperationToString);
            hashCode = (hashCode * -1521134295) + EqualityComparer<Func<ITerm, ITerm, string>>.Default.GetHashCode(OperationToLaTeX);
            return hashCode;
        }

        public static bool operator ==(BinaryOperator? left, BinaryOperator? right) => EqualityComparer<BinaryOperator>.Default.Equals(left, right);
        public static bool operator !=(BinaryOperator? left, BinaryOperator? right) => !(left == right);
    }
}
