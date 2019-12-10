using AoC_2019.IntCode;
using AoCHelper;
using FileParser;
using System.Collections.Generic;
using System.Linq;

namespace AoC_2019
{
    public class Problem09 : BaseProblem
    {
        public override string Solve_1()
        {
            var intCode = ParseInput().ToList();

            var output = IntCodeHelpers.RunIntCodeProgram(intCode, 1).Result;

            return output.Single().ToString();
        }

        public override string Solve_2()
        {
            var intCode = ParseInput().ToList();

            var output = IntCodeHelpers.RunIntCodeProgram(intCode, 2).Result;

            return output.Single().ToString();
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
