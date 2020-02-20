namespace Afg2Geburtstag
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;

    public class DigitFarm
    {
        public List<List<ITerm>> TermsOfSize { get; }
        public List<BinaryOperator> BinaryOperators { get; }
        public HashSet<BigRational> AllValues { get; }

        public long Digit { get; }
        public long Base { get; }
        public long UnaryDepth { get; }

        public DigitFarm(List<BinaryOperator> binaryOperators, long digit, long @base = 10)
        {
            TermsOfSize = new List<List<ITerm>>() { new List<ITerm>() };
            AllValues = new HashSet<BigRational>();
            BinaryOperators = binaryOperators;
            Digit = digit;
            Base = @base;
            UnaryDepth = 5;
        }

        public List<ITerm> GetAllOfSize(int size)
        {
            if (TermsOfSize.Count < size) GetAllOfSize(size - 1);
            else if (TermsOfSize.Count > size) return TermsOfSize[size];
            List<ITerm> currentTerms = new List<ITerm>();

            var element = new BigInteger(Digit);
            for (int i = 1; i < size; i++) element = (element * Base) + Digit;

            RegisterTerm(currentTerms, new ValueTerm(new BigRational(element)));

            for (int i = 1; i < size; i++)
            {
                var allLhs = TermsOfSize[i];
                var allRhs = TermsOfSize[size - i];

                foreach (var lhs in allLhs)
                {
                    foreach (var rhs in allRhs)
                    {
                        foreach (var @operator in BinaryOperators)
                        {
                            var term = BinaryOperation.Create(@operator, lhs, rhs);

                            RegisterTerm(currentTerms, term);
                        }
                    }
                }
            }

            TermsOfSize.Add(currentTerms);

            return currentTerms;
        }

        private static readonly UnaryOperator Factorial = new UnaryOperator(x => BigRational.Factorial(x.Value, 1000), x => $"({x})!");

        private void RegisterTerm(List<ITerm> terms, ITerm? term)
        {
            AddTermIfNew(terms, term);

            for (int i = 1; i <= UnaryDepth; i++)
            {
                if (term == null) break;

                term = UnaryOperation.Create(Factorial, term);
                AddTermIfNew(terms, term);
            }
        }

        private void AddTermIfNew(List<ITerm> terms, ITerm? term)
        {
            if (term == null || AllValues.Contains(term.Value)) return;

            if (term.Value.Absolute > int.MaxValue) return;

            AllValues.Add(term.Value);
            terms.Add(term);
        }
    }
}
