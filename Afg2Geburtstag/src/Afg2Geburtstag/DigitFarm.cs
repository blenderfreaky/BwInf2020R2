namespace Afg2Geburtstag
{
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;

    using RationalSet = System.Collections.Concurrent.ConcurrentDictionary<BigRational, byte>;
    using TermSet = System.Collections.Concurrent.ConcurrentDictionary<ITerm, byte>;

    public class DigitFarm
    {
        public List<TermSet> TermsOfSize { get; }
        public List<BinaryOperator> BinaryOperators { get; }
        public RationalSet AllValues { get; }

        public long Digit { get; }
        public long Base { get; }
        public long UnaryDepth { get; }

        public DigitFarm(List<BinaryOperator> binaryOperators, long digit, long @base = 10)
        {
            TermsOfSize = new List<TermSet>() { new TermSet() };
            AllValues = new RationalSet();
            BinaryOperators = binaryOperators;
            Digit = digit;
            Base = @base;
            UnaryDepth = 0;
        }

        public IEnumerable<ITerm> GetAllOfSize(int size)
        {
            if (TermsOfSize.Count < size) GetAllOfSize(size - 1);
            else if (TermsOfSize.Count > size) return TermsOfSize[size].Keys;
            var currentTerms = new TermSet();

            var element = new BigInteger(Digit);
            for (int i = 1; i < size; i++) element = (element * Base) + Digit;

            RegisterTerm(currentTerms, new ValueTerm(new BigRational(element)));

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

                            RegisterTerm(currentTerms, term);
                        }
                    }
                }
            });

            TermsOfSize.Add(currentTerms);

            return currentTerms.Keys;
        }

        private static readonly UnaryOperator Factorial = new UnaryOperator(x => BigRational.Factorial(x.Value, 1000), x => $"({x})!");

        private void RegisterTerm(TermSet terms, ITerm? term)
        {
            if (term == null) return;
            AddTermIfNew(terms, term);

            for (int i = 1; i <= UnaryDepth; i++)
            {
                if (term == null) break;

                term = UnaryOperation.Create(Factorial, term);
                AddTermIfNew(terms, term);
            }
        }

        private void AddTermIfNew(TermSet terms, ITerm? term)
        {
            if (term == null || AllValues.ContainsKey(term.Value)) return;

            if (term.Value.Absolute > int.MaxValue) return;

            AllValues.TryAdd(term.Value, 0);
            terms.TryAdd(term, 0);
        }
    }
}
