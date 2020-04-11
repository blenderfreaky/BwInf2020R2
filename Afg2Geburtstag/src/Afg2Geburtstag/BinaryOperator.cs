namespace Afg2Geburtstag
{
    using System;
    using System.Collections.Generic;

    public class BinaryOperator
    {
        public Func<ITerm, ITerm, BigRational?> Evaluate { get; }
        public Func<ITerm, ITerm, string> OperationToString { get; }

        public BinaryOperator(Func<ITerm, ITerm, BigRational?> evaluate, Func<ITerm, ITerm, string> operationToString)
        {
            Evaluate = evaluate;
            OperationToString = operationToString;
        }

        public override bool Equals(object? obj) => obj is BinaryOperator @operator && EqualityComparer<Func<ITerm, ITerm, BigRational?>>.Default.Equals(Evaluate, @operator.Evaluate) && EqualityComparer<Func<ITerm, ITerm, string>>.Default.Equals(OperationToString, @operator.OperationToString);

        public override int GetHashCode()
        {
            var hashCode = 2118633752;
            hashCode = (hashCode * -1521134295) + EqualityComparer<Func<ITerm, ITerm, BigRational?>>.Default.GetHashCode(Evaluate);
            hashCode = (hashCode * -1521134295) + EqualityComparer<Func<ITerm, ITerm, string>>.Default.GetHashCode(OperationToString);
            return hashCode;
        }
    }
}
