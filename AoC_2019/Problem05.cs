using AoC_2019.IntCode;
using AoCHelper;
using FileParser;
using System.Collections.Generic;
using System.Linq;

namespace AoC_2019
{
    public class Problem05 : BaseProblem
    {
        public override string Solve_1()
        {
            var intCode = ParseInput().ToList();

            var outputSequence = IntCodeHelpers.RunIntCodeProgram(intCode, input: 1).Result;

            return string.Join(string.Empty, outputSequence.SkipWhile(n => n == 0));
        }

        public override string Solve_2()
        {
            var intCode = ParseInput().ToList();

            var outputSequence = IntCodeHelpers.RunIntCodeProgram(intCode, input: 5).Result;

            return string.Join(string.Empty, outputSequence.SkipWhile(n => n == 0));
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
