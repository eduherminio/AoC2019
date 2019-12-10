using AoCHelper;
using FileParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace AoC_2019
{
    public class Problem09 : BaseProblem
    {
        public override string Solve_1()
        {
            var intCode = ParseInput().ToList();

            var output = RunIntCodeProgram(intCode, 1).Result;

            return output.Single().ToString();
        }

        public override string Solve_2()
        {
            var intCode = ParseInput().ToList();

            var output = RunIntCodeProgram(intCode, 2).Result;

            return output.Single().ToString();
        }

        private static async Task<ICollection<long>> RunIntCodeProgram(List<long> intCode, long input)
        {
            Channel<long> channel = Channel.CreateUnbounded<long>();
            IntCodeComputer computer = new IntCodeComputer(channel);

            await channel.Writer.WriteAsync(input).ConfigureAwait(false);

            ICollection<long> result = new List<long>();
            await foreach (var item in computer.RunIntCodeProgram(intCode))
            {
                result.Add(item);
            }

            return result;
        }

        private IEnumerable<long> ParseInput()
        {
            return new ParsedFile(FilePath)
                .ToSingleString()
                .Split(',')
                .Select(long.Parse);
        }
    }
}
