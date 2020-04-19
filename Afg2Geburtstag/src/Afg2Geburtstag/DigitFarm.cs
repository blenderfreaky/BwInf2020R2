namespace Afg2Geburtstag
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using RationalSet = System.Collections.Concurrent.ConcurrentDictionary<Rational, byte>;
    using TermSet = System.Collections.Concurrent.ConcurrentDictionary<ITerm, byte>;

    public class DigitFarm
    {
        public List<BinaryOperator> BinaryOperators { get; }
        public UnaryOperator? UnaryOperator { get; }

        public List<TermSet> TermsOfSize { get; }
        public RationalSet AllValues { get; }

        public long Digit { get; }
        public long Base { get; }

        public ConcurrentDictionary<Rational, ITerm?> HitTargets { get; }
        public int UnfoundTargets { get; private set; }
        public Action<ITerm, int> OnFound { get; }

        public DigitFarm(
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

            HitTargets = hitTargets;
            UnfoundTargets = hitTargets.Count;
            OnFound = onFound;
        }

        public void GetAllOfSize(int size)
        {
            if (TermsOfSize.Count < size) GetAllOfSize(size - 1);
            else if (TermsOfSize.Count > size) return;
            var currentTerms = new TermSet();

            var element = Digit;
            for (int i = 1; i < size; i++) element = (element * Base) + Digit;

            RegisterTerm(currentTerms, new Rational(element), size);

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
                        }
                    }

                    if (UnfoundTargets == 0) return;
                }
            });

            TermsOfSize.Add(currentTerms);
        }

        /// <summary>
        /// Adds the
        /// </summary>
        /// <param name="terms"></param>
        /// <param name="term"></param>
        /// <param name="digits"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RegisterTerm(TermSet terms, ITerm? term, int digits)
        {
            if (term == null) return;
            if (AddTermIfNew(terms, term, digits)) return;

            if (UnaryOperator == null) return;

            for (int i = 1; ; i++)
            {
                var newTerm = UnaryOperation.Create(UnaryOperator, term);
                if (newTerm == null || newTerm.Value == term.Value) break;
                term = newTerm;
                if (!AddTermIfNew(terms, term, digits)) break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool AddTermIfNew(TermSet terms, ITerm? term, int digits)
        {
            if (term == null
                || AllValues.ContainsKey(term.Value)
                || term.Value.Absolute > int.MaxValue)
            {
                return false;
            }

            if (HitTargets.TryGetValue(term.Value, out var otherTerm) && otherTerm == null)
            {
                HitTargets[term.Value] = term;
                UnfoundTargets--;
                OnFound(term, digits);
            }

            AllValues.TryAdd(term.Value, 0);
            terms.TryAdd(term, 0);

            return true;
        }
    }
}