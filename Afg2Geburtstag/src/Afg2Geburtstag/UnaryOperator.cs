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

        public int HashCode { get; }

        public UnaryOperator(Func<ITerm, Rational?> evaluate, Func<ITerm, string> operationToString, Func<ITerm, string> operationToLaTeX)
        {
            Evaluate = evaluate;
            OperationToString = operationToString;
            OperationToLaTeX = operationToLaTeX;

            var hashCode = -912930555;
            hashCode = (hashCode * -1521134295) + Evaluate.GetHashCode();
            hashCode = (hashCode * -1521134295) + OperationToString.GetHashCode();
            hashCode = (hashCode * -1521134295) + OperationToLaTeX.GetHashCode();
            HashCode = hashCode;
        }

        public override int GetHashCode() => HashCode;
    }
}