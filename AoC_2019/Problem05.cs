using AoCHelper;
using AoCHelper.Helpers;
using FileParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace AoC_2019
{
    public class Problem05 : BaseProblem
    {
        public override string Solve_1()
        {
            var input = ParseInput().ToList();

            var result = CalculateOutput(12, 2, input);

            return result.ToString();
        }

        public override string Solve_2()
        {
            var originalInput = ParseInput();

            var result = string.Empty;

            return result.ToString();
        }

        private static int CalculateOutput(int noun, int verb, List<int> input)
        {
            input[1] = noun;
            input[2] = verb;

            var outputSequence = RunIntcodeProgram(input);

            return outputSequence.First();
        }

        private static ICollection<int> RunIntcodeProgram(List<int> input)
        {
            for (int i = 0; i < input.Count;)
            {
                switch (input[i])
                {
                    case 1:
                        input[input[i + 3]] = input[input[i + 1]] + input[input[i + 2]];
                        break;

                    case 2:
                        input[input[i + 3]] = input[input[i + 1]] * input[input[i + 2]];
                        break;

                    case 99:
                        return input;

                    default:
                        throw new SolvingException("Something went wrong");
                }

                i += 4;
            }

            return input;
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
