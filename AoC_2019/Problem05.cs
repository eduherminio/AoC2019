using AoCHelper;
using FileParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC_2019
{
    public class Problem05 : BaseProblem
    {
        public override string Solve_1()
        {
            var intCode = ParseInput().ToList();

            var outputSequence = IntCodeComputer.RunIntcodeProgram(intCode, input: new[] { 1 }).ToList();

            return string.Join(string.Empty, outputSequence.SkipWhile(n => n == 0));
        }

        public override string Solve_2()
        {
            var intCode = ParseInput().ToList();

            var outputSequence = IntCodeComputer.RunIntcodeProgram(intCode, input: new[] { 5 }).ToList();

            return string.Join(string.Empty, outputSequence.SkipWhile(n => n == 0));
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
