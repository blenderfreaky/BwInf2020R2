namespace Afg2Geburtstag
{
    using System;
    using System.Collections.Generic;

    public class UnaryOperator
    {
        public Func<ITerm, BigRational?> Evaluate { get; }
        public Func<ITerm, string> OperationToString { get; }
        public Func<ITerm, string> OperationToLaTeX { get; }

        public UnaryOperator(Func<ITerm, BigRational?> evaluate, Func<ITerm, string> operationToString, Func<ITerm, string> operationToLaTeX)
        {
            Evaluate = evaluate;
            OperationToString = operationToString;
            OperationToLaTeX = operationToLaTeX;
        }

        public override bool Equals(object? obj) => obj is UnaryOperator @operator && EqualityComparer<Func<ITerm, BigRational?>>.Default.Equals(Evaluate, @operator.Evaluate) && EqualityComparer<Func<ITerm, string>>.Default.Equals(OperationToString, @operator.OperationToString) && EqualityComparer<Func<ITerm, string>>.Default.Equals(OperationToLaTeX, @operator.OperationToLaTeX);

        public override int GetHashCode()
        {
            int hashCode = -912930555;
            hashCode = hashCode * -1521134295 + EqualityComparer<Func<ITerm, BigRational?>>.Default.GetHashCode(Evaluate);
            hashCode = hashCode * -1521134295 + EqualityComparer<Func<ITerm, string>>.Default.GetHashCode(OperationToString);
            hashCode = hashCode * -1521134295 + EqualityComparer<Func<ITerm, string>>.Default.GetHashCode(OperationToLaTeX);
            return hashCode;
        }

        public static bool operator ==(UnaryOperator? left, UnaryOperator? right) => EqualityComparer<UnaryOperator>.Default.Equals(left, right);
        public static bool operator !=(UnaryOperator? left, UnaryOperator? right) => !(left == right);
    }
}
