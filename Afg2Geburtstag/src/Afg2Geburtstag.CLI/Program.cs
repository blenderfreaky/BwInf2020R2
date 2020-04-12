namespace Afg2Geburtstag.CLI
{
    using Colorful;
    using CommandLine;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;
    using Console = Colorful.Console;

    public class Options
    {
        [Option('t', "targets", Separator = ',', HelpText = "The numbers to represent", Required = true)]
        public IEnumerable<string>? Targets { get; set; }

        [Option('d', "digits", Separator = ',', HelpText = "The digits to use", Required = true)]
        public IEnumerable<string>? Digits { get; set; }

        [Option('b', "base", HelpText = "The base to use", Default = 10)]
        public int Base { get; set; }

        [Option('f', "factorial", HelpText = "Allow factorial", Default = false)]
        public bool AllowFactorial { get; set; }

        [Option('e', "exponentiation", HelpText = "Allow exponents", Default = false)]
        public bool AllowExponentiation { get; set; }
    }

    public static class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunWithArguments);
        }

        public static readonly BinaryOperator Addition =
            new BinaryOperator((l, r) => l.Value + r.Value, (l, r) => $"({l} + {r})");

        public static readonly BinaryOperator Subtraction =
            new BinaryOperator((l, r) => l.Value - r.Value, (l, r) => $"({l} - {r})");

        public static readonly BinaryOperator Multiplication =
            new BinaryOperator((l, r) => l.Value * r.Value, (l, r) => $"({l} * {r})");

        public static readonly BinaryOperator Division =
            new BinaryOperator((l, r) => !r.Value.IsZero ? l.Value / r.Value : (BigRational?)null, (l, r) => $"({l} / {r})");

        public static readonly BinaryOperator Exponentiation =
            new BinaryOperator(
                (l, r) =>
                {
                    if (!r.Value.IsInteger) return null;
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
                (l, r) => $"({l} ^ {r})");

        public static readonly UnaryOperator Factorial =
            new UnaryOperator(
                x => BigRational.Factorial(x.Value, 80),
                x => $"({x})!");

        private static void RunWithArguments(Options options)
        {
            var targets = options.Targets.Select(x => (BigRational)int.Parse(x.Trim().Replace(",", ""))).ToList();

            Parallel.ForEach(options.Digits.Where(x => !string.IsNullOrWhiteSpace(x)),
                digit =>
                    Farm(targets,
                        options.AllowExponentiation,
                        options.AllowFactorial,
                        int.Parse(digit.Trim().Replace(",", "")),
                        options.Base));
        }

        public static void Farm(
            IEnumerable<BigRational> targetsSource,
            bool useExponentiation,
            bool useFactorial,
            long digit,
            long @base = 10)
        {
            if (!useFactorial && !useExponentiation && digit == 0) return;

            var binaryOperators = new List<BinaryOperator>
            {
                Addition,
                Subtraction,
                Multiplication,
                Division,
            };

            if (useExponentiation) binaryOperators.Add(Exponentiation);

            var targets = new ConcurrentDictionary<BigRational, ITerm?>();
            foreach (var target in targetsSource) targets[target] = null;

            var stopwatch = Stopwatch.StartNew();

            var farm = new DigitFarm(binaryOperators, useFactorial ? Factorial : null, targets,
                (term, digits) => Console.WriteLine($"Found solution ({digits} digits) [{stopwatch.Elapsed}]\n  {term.Value} = {term}", Color.Green),
                digit, @base);

            for (int i = 1; ; i++)
            {
                farm.GetAllOfSize(i);

                if (farm.UnfoundTargets == 0)
                {
                    stopwatch.Stop();
                    farm = null!;
                    GC.Collect();
                    break;
                }
            }
        }
    }
}
