namespace Afg2Geburtstag
{
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("{Term} = {Value}")]
    public sealed class BinaryOperation : ITerm
    {
        public BinaryOperator Operator { get; }

        public ITerm Lhs { get; }
        public ITerm Rhs { get; }

        public BigRational Value { get; }

        public string Term => ToString();

        private BinaryOperation(BinaryOperator @operator, ITerm lhs, ITerm rhs, BigRational value)
        {
            Operator = @operator;
            Lhs = lhs;
            Rhs = rhs;
            Value = value;
        }

        public static BinaryOperation? Create(BinaryOperator @operator, ITerm lhs, ITerm rhs)
        {
            var value = @operator.Evaluate(lhs, rhs);

            return value.HasValue
                ? new BinaryOperation(@operator, lhs, rhs, value.Value)
                : null;
        }


        public override string ToString() => Operator.OperationToString(Lhs, Rhs);

        public override bool Equals(object? obj) => obj is BinaryOperation operation
            && EqualityComparer<BinaryOperator>.Default.Equals(Operator, operation.Operator)
            && EqualityComparer<ITerm>.Default.Equals(Lhs, operation.Lhs)
            && EqualityComparer<ITerm>.Default.Equals(Rhs, operation.Rhs)
            && Value.Equals(operation.Value);

        public override int GetHashCode()
        {
            var hashCode = -1180296392;
            hashCode = (hashCode * -1521134295) + EqualityComparer<BinaryOperator>.Default.GetHashCode(Operator);
            hashCode = (hashCode * -1521134295) + EqualityComparer<ITerm>.Default.GetHashCode(Lhs);
            hashCode = (hashCode * -1521134295) + EqualityComparer<ITerm>.Default.GetHashCode(Rhs);
            return hashCode;
        }
    }
}
