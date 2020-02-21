namespace Afg2Geburtstag
{
    using System;
    using System.Collections.Generic;

    public class UnaryOperator
    {
        public Func<ITerm, BigRational?> Evaluate { get; }
        public Func<ITerm, string> OperationToString { get; }

        public UnaryOperator(Func<ITerm, BigRational?> evaluate, Func<ITerm, string> operationToString)
        {
            Evaluate = evaluate;
            OperationToString = operationToString;
        }

        public override bool Equals(object? obj) => obj is UnaryOperator @operator && EqualityComparer<Func<ITerm, BigRational?>>.Default.Equals(Evaluate, @operator.Evaluate) && EqualityComparer<Func<ITerm, string>>.Default.Equals(OperationToString, @operator.OperationToString);

        public override int GetHashCode()
        {
            var hashCode = 2118633752;
            hashCode = hashCode * -1521134295 + EqualityComparer<Func<ITerm, BigRational?>>.Default.GetHashCode(Evaluate);
            hashCode = hashCode * -1521134295 + EqualityComparer<Func<ITerm, string>>.Default.GetHashCode(OperationToString);
            return hashCode;
        }
    }
}
