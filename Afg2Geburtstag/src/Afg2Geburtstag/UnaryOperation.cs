namespace Afg2Geburtstag
{
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("{Term}")]
    public sealed class UnaryOperation : ITerm
    {
        public UnaryOperator Operator { get; }

        public ITerm Operand { get; }

        public BigRational Value { get; }

        public string Term => ToString();

        private UnaryOperation(UnaryOperator @operator, ITerm operand, BigRational value)
        {
            Operator = @operator;
            Operand = operand;
            Value = value;
        }

        public static UnaryOperation? Create(UnaryOperator @operator, ITerm operand)
        {
            var value = @operator.Evaluate(operand);

            return value.HasValue
                ? new UnaryOperation(@operator, operand, value.Value)
                : null;
        }


        public override string ToString() => Operator.OperationToString(Operand);

        public override bool Equals(object? obj) => obj is UnaryOperation operation
            && EqualityComparer<UnaryOperator>.Default.Equals(Operator, operation.Operator)
            && EqualityComparer<ITerm>.Default.Equals(Operand, operation.Operand)
            && Value.Equals(operation.Value);

        public override int GetHashCode()
        {
            var hashCode = -1180296392;
            hashCode = (hashCode * -1521134295) + EqualityComparer<UnaryOperator>.Default.GetHashCode(Operator);
            hashCode = (hashCode * -1521134295) + EqualityComparer<ITerm>.Default.GetHashCode(Operand);
            return hashCode;
        }
    }
}
