namespace Afg3Abbiegen.CLI
{
    using CommandLine;
    using System;
    using System.Drawing;
    using System.IO;
    using Console = Colorful.Console;

    public class Options
    {
        [Option('p', "path", HelpText = "The path to the file.", Required = true)]
        public string? Path { get; set; }
    }

    public static class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunWithOptions);
        }

        private static void RunWithOptions(Options options)
        {
            if (options.Path == null) throw new InvalidOperationException("Internal error");

            if (!File.Exists(options.Path))
            {
                WriteError("File does not exist: " + Path.GetFullPath(options.Path));
                return;
            }

            if (!StreetParser.TryParse(File.ReadAllLines(options.Path), out var start, out var end, out var streets))
            {
                WriteError("Parsing file failed.");
                return;
            }


        }

        private static void WriteError(string error) => Console.WriteLine("ERROR: " + error, Color.Red);
    }
}
