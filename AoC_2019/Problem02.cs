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

            input[1] = 12;
            input[2] = 2;

            //input = new[] { 1, 1, 1, 4, 99, 5, 6, 0, 99 }.ToList(); //  30,1,1,4,2,5,6,0,99

            //AlterInput(ref input);

            //input = new[] { 2, 4, 4, 5, 99, 0 }.ToList();   //   2,4,4,5,99,9801

            //AlterInput(ref input);

            AlterInput(ref input);

            Console.Write($"Day 2, part 1: {input.First()}");
        }

        private void AlterInput(ref List<int> input)
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
                        return;

                    default:
                        input = new List<int> { -1 };
                        return;
                }

                i += 4;
            }
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
                    input[1] = one;
                    input[2] = two;

                    AlterInput(ref input);

                    if (input.First() == Part2Output)
                    {
                        nounVerb = Tuple.Create(one, two);
                        break;
                    }
                }
            }

            int result = 100 * nounVerb.Item1 + nounVerb.Item2;

            Console.Write($"Day 2, part 2: {result}");
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
