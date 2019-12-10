using AoCHelper;
using AoCHelper.Helpers;
using FileParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AoC_2019
{
    public class Problem02 : BaseProblem
    {
        private const int Part2Output = 19690720;

        public override string Solve_1()
        {
            var input = ParseInput().ToList();

            var result = CalculateOutput(12, 2, input);

            return result.ToString();
        }

        public override string Solve_2()
        {
            var originalInput = ParseInput();
            var nounVerb = Tuple.Create(-1, -1);

            foreach (var one in RangeHelpers.GenerateRange(0, 99))
            {
                foreach (var two in RangeHelpers.GenerateRange(0, 99))
                {
                    var input = originalInput.ToList();
                    var output = CalculateOutput(one, two, input);

                    if (output == Part2Output)
                    {
                        nounVerb = Tuple.Create(one, two);
                        break;
                    }
                }
            }

            int result = (100 * nounVerb.Item1) + nounVerb.Item2;

            return result.ToString();
        }

        private static long CalculateOutput(long noun, long verb, List<long> input)
        {
            input[1] = noun;
            input[2] = verb;

            RunIntCodeProgram(input).Wait();

            return input.First();
        }

        private static async Task<ICollection<long>> RunIntCodeProgram(List<long> intCode)
        {
            IntCodeComputer computer = new IntCodeComputer();

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
