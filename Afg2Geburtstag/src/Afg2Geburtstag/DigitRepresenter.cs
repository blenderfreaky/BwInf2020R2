namespace Afg2Geburtstag
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using RationalSet = System.Collections.Concurrent.ConcurrentDictionary<Rational, byte>;
    using TermSet = System.Collections.Concurrent.ConcurrentDictionary<ITerm, byte>;

    /// <summary>
    /// Contains the main algorithm.
    /// </summary>
    public class DigitRepresenter
    {
        /// <summary>
        /// All binary operators available.
        /// </summary>
        public List<BinaryOperator> BinaryOperators { get; }

        /// <summary>
        /// Contains a unary operator. <c>null</c> if no unary operator is to be used.
        /// Multiple unary operators are no supported.
        /// </summary>
        public UnaryOperator? UnaryOperator { get; }

        /// <summary>
        /// All calculated terms, where the set at index i contains all the terms which use the digit i times.
        /// </summary>
        public List<TermSet> TermsOfSize { get; }

        /// <summary>
        /// Contains all values of all terms.
        /// </summary>
        public RationalSet AllValues { get; }

        public long Digit { get; }
        public long Base { get; }

        /// <summary>
        /// All targets to find representations for, along with their optimal representation, if one was found.
        /// </summary>
        public ConcurrentDictionary<Rational, ITerm?> Targets { get; }

        /// <summary>
        /// The number of targets for which no optimal representation has been found yet.
        /// </summary>
        public int UnfoundTargets;

        /// <summary>
        /// An action to execute once an optimal representation for any given target was found.
        /// </summary>
        public Action<ITerm, int> OnFound { get; }

        public DigitRepresenter(
            List<BinaryOperator> binaryOperators,
            UnaryOperator? unaryOperator,
            ConcurrentDictionary<Rational, ITerm?> hitTargets,
            Action<ITerm, int> onFound,
            long digit,
            long @base = 10)
        {
            TermsOfSize = new List<TermSet>() { new TermSet() };
            AllValues = new RationalSet();
            BinaryOperators = binaryOperators;
            UnaryOperator = unaryOperator;
            Digit = digit;
            Base = @base;

            Targets = hitTargets;
            UnfoundTargets = hitTargets.Count;
            OnFound = onFound;
        }

        /// <summary>
        /// Calculates all terms of size.
        /// </summary>
        /// <param name="size">The size of the terms to calculate.</param>
        public void CalculateAllOfSize(int size)
        {
            if (TermsOfSize.Count < size) CalculateAllOfSize(size - 1);
            else if (TermsOfSize.Count > size) return;
            var currentTerms = new TermSet();

            // The digit concatenated with itself <size> times
            var repeatedDigit = Digit;
            for (int i = 1; i < size; i++) repeatedDigit = (repeatedDigit * Base) + Digit;
            RegisterTerm(currentTerms, new Rational(repeatedDigit), size);

            // Execute all different ways of combining the smaller terms to terms of size <size> in parallel
            Parallel.For(1, size, i =>
            {
                var allLhs = TermsOfSize[i];
                var allRhs = TermsOfSize[size - i];

                foreach (var lhs in allLhs)
                {
                    foreach (var rhs in allRhs)
                    {
                        foreach (var @operator in BinaryOperators)
                        {
                            var term = BinaryOperation.Create(@operator, lhs.Key, rhs.Key);

                            RegisterTerm(currentTerms, term, size);

                            if (UnfoundTargets == 0) return;
                        }
                    }
                }
            });

            TermsOfSize.Add(currentTerms);
        }

        /// <summary>
        /// Adds <paramref name="term"/> and repeated applications of <see cref="UnaryOperator"/> upon it
        /// to <paramref name="termSet"/>if the terms are optimal representations of their values.
        /// </summary>
        /// <param name="termSet">The set of terms to add the terms to.</param>
        /// <param name="term">The term.</param>
        /// <param name="digitCount">The amount of times the digit is used in <paramref name="term"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RegisterTerm(TermSet termSet, ITerm? term, int digitCount)
        {
            if (term == null) return;
            if (AddTermIfNew(termSet, term, digitCount)) return;

            if (UnaryOperator == null) return;

            // Apply the unary operator repeatedly until either
            //  - The value stops changing
            //  - The unary operator cannot be applied anymore (values too big, trying to compute (-1)! etc.)
            do
            {
                var newTerm = UnaryOperation.Create(UnaryOperator, term);
                if (newTerm == null || newTerm.Value == term.Value) break;
                term = newTerm;
            }
            while (AddTermIfNew(termSet, term, digitCount));
        }

        /// <summary>
        /// Adds <paramref name="term"/> to <paramref name="termSet"/> if the term are optimal representations of
        /// their values and their values are not yet represented by any other term.
        /// If the term is the value of a target <see cref="OnFound"/> is called
        /// and the term is stored as its optimal representation.
        /// </summary>
        /// <param name="termSet">The set of terms to add the term to.</param>
        /// <param name="term">The term.</param>
        /// <param name="digitCount">The amount of times the digit is used in <paramref name="term"/>.</param>
        /// <returns>Whether the value was added.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool AddTermIfNew(TermSet termSet, ITerm? term, int digitCount)
        {
            // Don't add null or exisitng values
            if (term == null || AllValues.ContainsKey(term.Value))
            {
                return false;
            }

            // Check if the terms value is a target
            if (Targets.TryGetValue(term.Value, out var otherTerm) && otherTerm == null)
            {
                Targets[term.Value] = term;
                Interlocked.Decrement(ref UnfoundTargets);
                OnFound(term, digitCount);
            }

            // Add the term as key with value 0, as the value is ignored
            AllValues.TryAdd(term.Value, 0);
            termSet.TryAdd(term, 0);

            return true;
        }
    }
}