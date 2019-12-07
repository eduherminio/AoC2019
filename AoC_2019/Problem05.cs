using AoCHelper;
using FileParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace AoC_2019
{
    public class Problem05 : BaseProblem
    {
        public override string Solve_1()
        {
            List<int> intCode = ParseInput().ToList();

            ICollection<int> outputSequence = RunIntCodeProgram(intCode, input: 1).Result;

            return string.Join(string.Empty, outputSequence.SkipWhile(n => n == 0));
        }

        public override string Solve_2()
        {
            List<int> intCode = ParseInput().ToList();

            ICollection<int> outputSequence = RunIntCodeProgram(intCode, input: 5).Result;

            return string.Join(string.Empty, outputSequence.SkipWhile(n => n == 0));
        }

        private static async Task<ICollection<int>> RunIntCodeProgram(List<int> intCode, int input)
        {
            Channel<int> channel = Channel.CreateUnbounded<int>();
            IntCodeComputer computer = new IntCodeComputer(channel);

            await channel.Writer.WriteAsync(input).ConfigureAwait(false);

            ICollection<int> result = new List<int>();
            await foreach (var item in computer.RunIntCodeProgram(intCode))
            {
                result.Add(item);
            }

            return result;
        }

        private IEnumerable<int> ParseInput()
        {
            return new ParsedFile(FilePath)
                .ToSingleString()
                .Split(',')
                .Select(int.Parse);
        }
    }
}
