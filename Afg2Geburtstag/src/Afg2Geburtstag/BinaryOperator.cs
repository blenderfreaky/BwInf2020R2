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

        public int HashCode { get; }

        public BinaryOperator(Func<ITerm, ITerm, Rational?> evaluate, Func<ITerm, ITerm, string> operationToString, Func<ITerm, ITerm, string> operationToLaTeX)
        {
            Evaluate = evaluate;
            OperationToString = operationToString;
            OperationToLaTeX = operationToLaTeX;

            var hashCode = -912930555;
            hashCode = (hashCode * -1521134295) + Evaluate.GetHashCode();
            hashCode = (hashCode * -1521134295) + OperationToString.GetHashCode();
            hashCode = (hashCode * -1521134295) + OperationToLaTeX.GetHashCode();
            HashCode= hashCode;
        }

        public override int GetHashCode() => HashCode;
    }
}