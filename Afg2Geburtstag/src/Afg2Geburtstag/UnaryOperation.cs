namespace Afg2Geburtstag
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Represents the application of a unary operator on an <see cref="ITerm"/> operand.
    /// </summary>
    [DebuggerDisplay("{ToString()} = {Value}")]
    public sealed class UnaryOperation : ITerm, IEquatable<UnaryOperation>
    {
        public UnaryOperator Operator { get; }

        public ITerm Operand { get; }

        /// <inheritdoc/>
        public Rational Value { get; }

        /// <summary>
        /// The hash code of the object.
        /// </summary>
        public int HashCode { get; }

        private UnaryOperation(UnaryOperator @operator, ITerm operand, Rational value)
        {
            Operator = @operator;
            Operand = operand;
            Value = value;

            var hashCode = -1180296392;
            hashCode = (hashCode * -1521134295) + Operator.GetHashCode();
            hashCode = (hashCode * -1521134295) + Operand.GetHashCode();
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
            try
            {
                var value = @operator.Evaluate(operand);

                return value.HasValue
                    ? new UnaryOperation(@operator, operand, value.Value)
                    : null;
            }
            catch (OverflowException) { return null; }
            catch (DivideByZeroException) { return null; }
        }

        /// <inheritdoc/>
        public override string ToString() => Operator.OperationToString(Operand);

        /// <inheritdoc/>
        public string ToLaTeX() => Operator.OperationToLaTeX(Operand);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object? obj) => obj is UnaryOperation operation && Equals(operation);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ITerm term) => term is UnaryOperation operation && Equals(operation);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(UnaryOperation operation) => HashCode == operation.HashCode
                    && Value == operation.Value
                    && Operator == operation.Operator
                    && Operand.Equals(operation.Operand);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => HashCode;
    }
}