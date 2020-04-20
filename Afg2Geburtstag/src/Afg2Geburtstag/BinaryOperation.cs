namespace Afg2Geburtstag
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Represents the application of a binary operator on two <see cref="ITerm"/> operands.
    /// </summary>
    [DebuggerDisplay("{ToString()} = {Value}")]
    public sealed class BinaryOperation : ITerm, IEquatable<BinaryOperation>
    {
        public BinaryOperator Operator { get; }

        public ITerm Lhs { get; }
        public ITerm Rhs { get; }

        /// <inheritdoc/>
        public Rational Value { get; }

        /// <summary>
        /// The hash code of the object.
        /// </summary>
        public int HashCode { get; }

        private BinaryOperation(BinaryOperator @operator, ITerm lhs, ITerm rhs, Rational value)
        {
            Operator = @operator;
            Lhs = lhs;
            Rhs = rhs;
            Value = value;

            var hashCode = -1180296392;
            hashCode = (hashCode * -1521134295) + Operator.GetHashCode();
            hashCode = (hashCode * -1521134295) + Lhs.GetHashCode();
            hashCode = (hashCode * -1521134295) + Rhs.GetHashCode();
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
            catch (OverflowException) { return null; }
            catch (DivideByZeroException) { return null; }
        }

        /// <inheritdoc/>
        public override string ToString() => Operator.OperationToString(Lhs, Rhs);

        /// <inheritdoc/>
        public string ToLaTeX() => Operator.OperationToLaTeX(Lhs, Rhs);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object? obj) => obj is BinaryOperation operation && Equals(operation);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ITerm obj) => obj is BinaryOperation operation && Equals(operation);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(BinaryOperation operation) => HashCode == operation.HashCode
                    && Value == operation.Value
                    && Operator == operation.Operator
                    && Lhs.Equals(operation.Lhs)
                    && Rhs.Equals(operation.Rhs);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => HashCode;
    }
}