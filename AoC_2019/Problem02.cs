using AoCHelper;
using AoCHelper.Helpers;
using FileParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC_2019
{
    public class Problem02 : BaseProblem
    {
        private const int Part2Output = 19690720;

        public override void Solve_1()
        {
            var input = ParseInput().ToList();

            var result = CalculateOutput(12, 2, input);

            Console.Write($"Day 2, part 1: {result}");
        }

        public override void Solve_2()
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

            Console.Write($"Day 2, part 2: {result}");
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
                        throw new Exception("Something went wront");
                }

                i += 4;
            }

            return input;
        }

        public IEnumerable<int> ParseInput()
        {
            return new ParsedFile(FilePath)
                .ToSingleString()
                .Split(',')
                .Select(int.Parse);
        }
    }
}
