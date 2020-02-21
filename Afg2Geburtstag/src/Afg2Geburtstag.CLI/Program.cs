namespace Afg2Geburtstag.CLI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;

    public static class Program
    {
        public static void Main(string[] args)
        {
            //Farm(2019, 3);
            var targets = new BigRational[]
            { 
                2019,
                //2020, 2030, 2080, 2980,
                //202020, 69420 
            };
            Parallel.For(1, 10, x => Farm(targets, x));
        }

        public static void Farm(IEnumerable<BigRational> targetsSource, long digit, long @base = 10)
        {
            var binaryOperators = new List<BinaryOperator>
            {
                new BinaryOperator((l, r) => l.Value + r.Value, (l, r) => $"({l} + {r})"),
                new BinaryOperator((l, r) => l.Value - r.Value, (l, r) => $"({l} - {r})"),
                new BinaryOperator((l, r) => l.Value * r.Value, (l, r) => $"({l} * {r})"),
                new BinaryOperator((l, r) => !r.Value.IsZero ? l.Value / r.Value : (BigRational?)null, (l, r) => $"({l} / {r})"),
                new BinaryOperator((l, r) =>
                {
                    if(!r.Value.IsInteger) return null;
                    var exponentPositive = r.Value.IsPositive;
                    var exponentBig = r.Value.Absolute;
                    if (exponentBig > 100) return null;
                    var exponent = (int)exponentBig.Numerator;

                    if (exponent == 0) return BigRational.One;

                    var numerator = BigInteger.Pow(l.Value.Numerator, exponent);
                    var denominator = BigInteger.Pow(l.Value.Denominator, exponent);

                    return exponentPositive
                        ? new BigRational(numerator, denominator)
                        : new BigRational(denominator, numerator);
                },
                (l, r) => $"({l} ^ {r})"),
            };

            var targets = new HashSet<BigRational>(targetsSource);

            var farm = new DigitFarm(binaryOperators, digit, @base);
            var stopwatch = Stopwatch.StartNew();

            for (int i = 1; ; i++)
            {
                
                GC. farm.GetAllOfSize(i);

                //Console.WriteLine("Calculated optimal expressions with " + i + " digits (d: " + digit + ", b: " + @base + ")");
                var elapsed = stopwatch.Elapsed;

                foreach (var hitTarget in targets.Where(farm.AllValues.Contains).ToList())
                {
                    targets.Remove(hitTarget);
                    var val = farm.TermsOfSize[i].First(x => x.Value == hitTarget);
                    Console.WriteLine($"Found solution ({i} digits) [{elapsed}]\n  {hitTarget} = {val}");
                }

                if (targets.Count == 0)
                {
                    stopwatch.Stop();
                    farm = null;
                    GC.Collect();
                    break;
                }
            }
        }
    }
}
