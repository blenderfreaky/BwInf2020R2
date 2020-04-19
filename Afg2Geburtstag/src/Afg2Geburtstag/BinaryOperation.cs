namespace Afg2Geburtstag
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("{ToString()} = {Value}")]
    public sealed class BinaryOperation : ITerm
    {
        public BinaryOperator Operator { get; }

        public ITerm Lhs { get; }
        public ITerm Rhs { get; }

        /// <inheritdoc/>
        public Rational Value { get; }

        /// <summary>
        /// The hash code of the object.
        /// </summary>
        public long HashCode { get; }

        private BinaryOperation(BinaryOperator @operator, ITerm lhs, ITerm rhs, Rational value)
        {
            Operator = @operator;
            Lhs = lhs;
            Rhs = rhs;
            Value = value;

            var hashCode = -1180296392;
            hashCode = (hashCode * -1521134295) + EqualityComparer<BinaryOperator>.Default.GetHashCode(Operator);
            hashCode = (hashCode * -1521134295) + EqualityComparer<ITerm>.Default.GetHashCode(Lhs);
            hashCode = (hashCode * -1521134295) + EqualityComparer<ITerm>.Default.GetHashCode(Rhs);
            HashCode = hashCode;
        }

        /// <summary>
        /// Creates a <see cref="BinaryOperation"/> from a <see cref="BinaryOperator"/> operator and two <see cref="ITerm"/> operands.
        /// </summary>
        /// <param name="operator">The operator.</param>
        /// <param name="lhs">The left operand.</param>
        /// <param name="rhs">The right operand.</param>
        /// <returns>The operation, or null if the operation was invalid (like x/0).</returns>
        public static BinaryOperation? Create(BinaryOperator @operator, ITerm lhs, ITerm rhs)
        {
            try
            {
                var value = @operator.Evaluate(lhs, rhs);

                return value.HasValue
                    ? new BinaryOperation(@operator, lhs, rhs, value.Value)
                    : null;
            }
            catch (OverflowException)
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public override string ToString() => Operator.OperationToString(Lhs, Rhs);

        /// <inheritdoc/>
        public string ToLaTeX() => Operator.OperationToLaTeX(Lhs, Rhs);

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is BinaryOperation operation
            && HashCode == operation.HashCode
            && Value == operation.Value
            && EqualityComparer<BinaryOperator>.Default.Equals(Operator, operation.Operator)
            && EqualityComparer<ITerm>.Default.Equals(Lhs, operation.Lhs)
            && EqualityComparer<ITerm>.Default.Equals(Rhs, operation.Rhs);

        /// <inheritdoc/>
        public override int GetHashCode() => (int)(HashCode % int.MaxValue);
    }
}
