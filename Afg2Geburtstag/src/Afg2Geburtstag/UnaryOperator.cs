namespace Afg2Geburtstag
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a unary operator.
    /// </summary>
    public class UnaryOperator
    {
        /// <summary>
        /// Evaluates the value of applying the operator to the given <see cref="ITerm"/> operand.
        /// </summary>
        public Func<ITerm, Rational?> Evaluate { get; }

        /// <summary>
        /// Generates a string describing the application of the operator to the given <see cref="ITerm"/> operand.
        /// </summary>
        public Func<ITerm, string> OperationToString { get; }

        /// <summary>
        /// Generates a string as LaTeX code describing the application of the operator to the given <see cref="ITerm"/> operand.
        /// </summary>
        public Func<ITerm, string> OperationToLaTeX { get; }

        public UnaryOperator(Func<ITerm, Rational?> evaluate, Func<ITerm, string> operationToString, Func<ITerm, string> operationToLaTeX)
        {
            Evaluate = evaluate;
            OperationToString = operationToString;
            OperationToLaTeX = operationToLaTeX;
        }

        public override bool Equals(object? obj) => obj is UnaryOperator @operator && EqualityComparer<Func<ITerm, Rational?>>.Default.Equals(Evaluate, @operator.Evaluate) && EqualityComparer<Func<ITerm, string>>.Default.Equals(OperationToString, @operator.OperationToString) && EqualityComparer<Func<ITerm, string>>.Default.Equals(OperationToLaTeX, @operator.OperationToLaTeX);

        public override int GetHashCode()
        {
            int hashCode = -912930555;
            hashCode = (hashCode * -1521134295) + EqualityComparer<Func<ITerm, Rational?>>.Default.GetHashCode(Evaluate);
            hashCode = (hashCode * -1521134295) + EqualityComparer<Func<ITerm, string>>.Default.GetHashCode(OperationToString);
            hashCode = (hashCode * -1521134295) + EqualityComparer<Func<ITerm, string>>.Default.GetHashCode(OperationToLaTeX);
            return hashCode;
        }

        public static bool operator ==(UnaryOperator? left, UnaryOperator? right) => EqualityComparer<UnaryOperator?>.Default.Equals(left, right);

        public static bool operator !=(UnaryOperator? left, UnaryOperator? right) => !(left == right);
    }
}