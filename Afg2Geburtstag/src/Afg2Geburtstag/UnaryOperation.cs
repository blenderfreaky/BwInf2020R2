namespace Afg2Geburtstag
{
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("{ToString()}")]
    public sealed class UnaryOperation : ITerm
    {
        public UnaryOperator Operator { get; }

        public ITerm Operand { get; }

        /// <inheritdoc/>
        public Rational Value { get; }

        /// <summary>
        /// The hash code of the object.
        /// </summary>
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

        /// <summary>
        /// Creates a <see cref="UnaryOperation"/> from a <see cref="UnaryOperator"/> operator and an <see cref="ITerm"/> operand.
        /// </summary>
        /// <param name="operator">The operator.</param>
        /// <param name="operand">The operand.</param>
        /// <returns>The operation, or null if the operation was invalid (like (-1)!).</returns>
        public static UnaryOperation? Create(UnaryOperator @operator, ITerm operand)
        {
            var value = @operator.Evaluate(operand);

            return value.HasValue
                ? new UnaryOperation(@operator, operand, value.Value)
                : null;
        }

        /// <inheritdoc/>
        public override string ToString() => Operator.OperationToString(Operand);

        /// <inheritdoc/>
        public string ToLaTeX() => Operator.OperationToLaTeX(Operand);

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is UnaryOperation operation
            && HashCode == operation.HashCode
            && Value == operation.Value
            && EqualityComparer<UnaryOperator>.Default.Equals(Operator, operation.Operator)
            && EqualityComparer<ITerm>.Default.Equals(Operand, operation.Operand);

        /// <inheritdoc/>
        public override int GetHashCode() => (int)(HashCode % int.MaxValue);
    }
}
