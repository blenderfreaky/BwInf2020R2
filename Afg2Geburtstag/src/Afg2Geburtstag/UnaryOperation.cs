namespace Afg2Geburtstag
{
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("{ToString()}")]
    public sealed class UnaryOperation : ITerm
    {
        public UnaryOperator Operator { get; }

        public ITerm Operand { get; }

        public Rational Value { get; }

        public long HashCode { get; }

        private UnaryOperation(UnaryOperator @operator, ITerm operand, Rational value)
        {
            Operator = @operator;
            Operand = operand;
            Value = value;

            var hashCode = -1180296392;
            hashCode = (hashCode * -1521134295) + EqualityComparer<UnaryOperator>.Default.GetHashCode(Operator);
            hashCode = (hashCode * -1521134295) + EqualityComparer<ITerm>.Default.GetHashCode(Operand);
            HashCode = hashCode;
        }

        public static UnaryOperation? Create(UnaryOperator @operator, ITerm operand)
        {
            var value = @operator.Evaluate(operand);

            return value.HasValue
                ? new UnaryOperation(@operator, operand, value.Value)
                : null;
        }

        public override string ToString() => Operator.OperationToString(Operand);
        public string ToLaTeX() => Operator.OperationToLaTeX(Operand);

        public override bool Equals(object? obj) => obj is UnaryOperation operation
            && HashCode == operation.HashCode
            && Value == operation.Value
            && EqualityComparer<UnaryOperator>.Default.Equals(Operator, operation.Operator)
            && EqualityComparer<ITerm>.Default.Equals(Operand, operation.Operand);

        public override int GetHashCode() => (int)(HashCode % int.MaxValue);
    }
}
