namespace Afg2Geburtstag
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a unary operator.
    /// </summary>
    public class BinaryOperator
    {
        /// <summary>
        /// Evaluates the value of applying the operator to the given <see cref="ITerm"/> operands.
        /// </summary>
        public Func<ITerm, ITerm, Rational?> Evaluate { get; }

        /// <summary>
        /// Generates a string describing the application of the operator to the given <see cref="ITerm"/> operands.
        /// </summary>
        public Func<ITerm, ITerm, string> OperationToString { get; }

        /// <summary>
        /// Generates a string as LaTeX code describing the application of the operator to the given <see cref="ITerm"/> operands.
        /// </summary>
        public Func<ITerm, ITerm, string> OperationToLaTeX { get; }

        public BinaryOperator(Func<ITerm, ITerm, Rational?> evaluate, Func<ITerm, ITerm, string> operationToString, Func<ITerm, ITerm, string> operationToLaTeX)
        {
            Evaluate = evaluate;
            OperationToString = operationToString;
            OperationToLaTeX = operationToLaTeX;
        }

        public override bool Equals(object? obj) => obj is BinaryOperator @operator && EqualityComparer<Func<ITerm, ITerm, Rational?>>.Default.Equals(Evaluate, @operator.Evaluate) && EqualityComparer<Func<ITerm, ITerm, string>>.Default.Equals(OperationToString, @operator.OperationToString) && EqualityComparer<Func<ITerm, ITerm, string>>.Default.Equals(OperationToLaTeX, @operator.OperationToLaTeX);

        public override int GetHashCode()
        {
            int hashCode = -912930555;
            hashCode = (hashCode * -1521134295) + EqualityComparer<Func<ITerm, ITerm, Rational?>>.Default.GetHashCode(Evaluate);
            hashCode = (hashCode * -1521134295) + EqualityComparer<Func<ITerm, ITerm, string>>.Default.GetHashCode(OperationToString);
            hashCode = (hashCode * -1521134295) + EqualityComparer<Func<ITerm, ITerm, string>>.Default.GetHashCode(OperationToLaTeX);
            return hashCode;
        }

        public static bool operator ==(BinaryOperator? left, BinaryOperator? right) => EqualityComparer<BinaryOperator?>.Default.Equals(left, right);

        public static bool operator !=(BinaryOperator? left, BinaryOperator? right) => !(left == right);
    }
}