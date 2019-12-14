using AoC_2019;
using FileParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Arcade
{
    public static class Program
    {
        private static readonly string BlockDestroyerInputFilePath = "Inputs/BlockDestroyer.in";

        public static void Main()
        {
            bool success = false;

            while (!success)
            {
                //using var outputCapturer = new OutputCapture();

                ArcadeMachine arcadeMachine = new ArcadeMachine(ParseInput().ToList());

                long score = arcadeMachine.Play().Result;

                Console.WriteLine($"Score: {score}");

                //var stuff = outputCapturer.Captured.ToString();
            }
        }

        private static IEnumerable<long> ParseInput()
        {
            return new ParsedFile(BlockDestroyerInputFilePath)
                .ToSingleString()
                .Split(',')
                .Select(long.Parse);
        }
    }

    public class OutputCapture : TextWriter, IDisposable
    {
        private TextWriter stdOutWriter;
        public TextWriter Captured { get; private set; }
        public override Encoding Encoding { get { return Encoding.ASCII; } }

        public OutputCapture()
        {
            this.stdOutWriter = Console.Out;
            Console.SetOut(this);
            Captured = new StringWriter();
        }

        override public void Write(string output)
        {
            // Capture the output and also send it to StdOut
            Captured.Write(output);
            stdOutWriter.Write(output);
        }

        override public void WriteLine(string output)
        {
            // Capture the output and also send it to StdOut
            Captured.WriteLine(output);
            stdOutWriter.WriteLine(output);
        }
    }
}
