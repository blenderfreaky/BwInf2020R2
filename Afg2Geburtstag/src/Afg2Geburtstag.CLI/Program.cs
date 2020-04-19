namespace Afg2Geburtstag.CLI
{
    using Afg2Geburtstag;
    using Colorful;
    using CommandLine;
    using System;
    using System.Buffers.Text;
    using System.Collections;
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
        [Option('t', "targets", Separator = ',', HelpText = "The numbers to find representations for", Required = true)]
        public IEnumerable<string>? Targets { get; set; }

        [Option('d', "digits", Separator = ',', HelpText = "The digits to use", Required = true)]
        public IEnumerable<string>? Digits { get; set; }

        [Option('b', "base", HelpText = "The base to use for concatenation", Default = 10)]
        public int Base { get; set; }

        [Option('f', "factorial", HelpText = "Allow usage of factorial", Default = false)]
        public bool AllowFactorial { get; set; }

        [Option('e', "exponentiation", HelpText = "Allow usage of exponentiation", Default = false)]
        public bool AllowExponentiation { get; set; }

        [Option('l', "latex", HelpText = "Output as source code for a latex table", Default = false)]
        public bool OutputAsLatex { get; set; }

        [Option('s', "sync", HelpText = "Forces the program to run the code for different digits synchronized rather than parallel. Use if too much RAM is being consumed", Default = false)]
        public bool Sync { get; set; }
    }

    public static class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunWithArguments);
        }

        public static readonly BinaryOperator Addition =
            new BinaryOperator((l, r) => checked(l.Value + r.Value), (l, r) => $"({l} + {r})", (l, r) => $"\\left({l.ToLaTeX()} + {r.ToLaTeX()}\\right)");

        public static readonly BinaryOperator Subtraction =
            new BinaryOperator((l, r) => checked(l.Value - r.Value), (l, r) => $"({l} - {r})", (l, r) => $"\\left({l.ToLaTeX()} - {r.ToLaTeX()}\\right)");

        public static readonly BinaryOperator Multiplication =
            new BinaryOperator((l, r) => checked(l.Value * r.Value), (l, r) => $"({l} * {r})", (l, r) => $"\\left({l.ToLaTeX()} \\cdot {r.ToLaTeX()}\\right)");

        public static readonly BinaryOperator Division =
            new BinaryOperator((l, r) => !r.Value.IsZero ? checked(l.Value / r.Value) : (Rational?)null, (l, r) => $"({l} / {r})", (l, r) => $"\\left(\\frac{{{l.ToLaTeX()}}}{{{r.ToLaTeX()}}}\\right)");

        public static readonly BinaryOperator Exponentiation =
            new BinaryOperator(
                (l, r) =>
                {
                    if (!r.Value.IsInteger) return null;

                    if (l.Value.IsZero) return r.Value.IsNegative ? (Rational?)null : Rational.Zero;
                    if (l.Value == Rational.One) return Rational.One;

                    var exponentPositive = r.Value.IsPositive;
                    var exponentBig = r.Value.Absolute;
                    if (exponentBig > 64) return null; // 2^64 can't be stored by int64, much less any bigger value ^64
                    var exponent = (int)exponentBig.Numerator;

                    if (exponent == 0) return Rational.One;

                    var numerator = BigInteger.Pow(l.Value.Numerator, exponent);
                    var denominator = BigInteger.Pow(l.Value.Denominator, exponent);

                    if (numerator > long.MaxValue || numerator < long.MinValue) return null;
                    if (denominator > long.MaxValue || denominator < long.MinValue) return null;

                    return exponentPositive
                        ? new Rational((long)numerator, (long)denominator)
                        : new Rational((long)denominator, (long)numerator);
                },
                (l, r) => $"({l} ^ {r})",
                (l, r) => $"\\left({{{l}}}^{{{r}}}\\right)");

        public static readonly UnaryOperator Factorial =
            new UnaryOperator(
                x => Rational.Factorial(x.Value),
                x => $"({x}!)",
                x => $"\\left({x}!\\right)");

        private static void RunWithArguments(Options options)
        {
            var targets = options.Targets!.CleanStrings().Select(Rational.Parse).ToList();
            var digits = options.Digits!.CleanStrings().Select(int.Parse).ToList();

            Console.WriteLine($"% Targets: {string.Join(", ", targets)}");
            Console.WriteLine($"% Digits: {string.Join(", ", digits)}");
            Console.WriteLine($"% Base: {options.Base}");
            Console.WriteLine($"% {(options.AllowExponentiation ? "Exponentiation " : "")}{(options.AllowFactorial ? "Factorial " : "")}");

            if (options.OutputAsLatex)
            {
                Console.WriteLine("\\begin{center}\n\\begin{tabular}{ | l | l | p{7cm} | l | l | }");
                Console.WriteLine("\\hline Digit & Value & Term & Digit Usages & Time \\\\\\hline");
            }

            void digitAction(int digit) =>
                Farm(targetsSource: targets,
                    useExponentiation: options.AllowExponentiation,
                    useFactorial: options.AllowFactorial,
                    digit: digit,
                    asLatex: options.OutputAsLatex,
                    @base: options.Base);

            if (options.Sync) Synchronized.ForEach(digits, digitAction);
            else Parallel.ForEach(digits, digitAction);

            if (options.OutputAsLatex)
            {
                Console.WriteLine("\\end{tabular}\n\\end{center}");
            }
        }
        /// <summary>
        /// CommandLineParser produces weird outputs for Option Lists sometimes. This cleans them up by removing empty/null strings and removing trailing commas.
        /// </summary>
        /// <param name="strings">The strings to clean up.</param>
        /// <returns>The cleaned up strings.</returns>
        private static IEnumerable<string> CleanStrings(this IEnumerable<string> strings) =>
            strings
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim().Replace(",", ""));

        public static void Farm(
            IEnumerable<Rational> targetsSource,
            bool useExponentiation,
            bool useFactorial,
            long digit,
            bool asLatex = false,
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

            var targets = new ConcurrentDictionary<Rational, ITerm?>();
            foreach (var target in targetsSource) targets[target] = null;

            var stopwatch = Stopwatch.StartNew();

            Action<ITerm, int> onFound = asLatex
                ? (term, digits) =>
                    Console.WriteLine($"\t{digit} & {term.Value} & \\( {term.ToLaTeX()} \\) & {digits} & {stopwatch.Elapsed.TotalSeconds:0.000}s \\\\\\hline")
                : (Action<ITerm, int>)((term, digits) =>
                    Console.WriteLine($"Found solution for {term.Value} with {digit} with {digits} digits [{stopwatch.Elapsed.TotalSeconds:0.000}s]:\n  {term.Value} = {term}\n"));

            var farm = new DigitFarm(binaryOperators, useFactorial ? Factorial : null, targets,
                onFound,
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
