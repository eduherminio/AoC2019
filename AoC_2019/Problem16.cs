using AoCHelper;
using FileParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC_2019
{
    public class Problem16 : BaseProblem
    {
        private static readonly IReadOnlyCollection<int> _basePattern = new[] { 0, 1, 0, -1 };

        public override string Solve_1()
        {
            ICollection<int> input = ParseInput().ToList();
            const int phasesToApply = 100;

            for (int i = 0; i < phasesToApply; ++i)
            {
                input = ApplyPhase(input).ToList();
            }

            string solution = string.Join(string.Empty, input.Take(8).Select(n => n.ToString()));

            return solution;
        }

        IEnumerable<int> ApplyPhase(IEnumerable<int> inputList)
        {
            int inputLength = inputList.Count();

            // Index of pattern
            int patternIndex = 0;

            // repetitions of an pattern[patternIndex], starts in one to skip first item in the pattern for each 'row'
            int patternRepetition = 1;
            for (int inputIndex = 0; inputIndex < inputLength; ++inputIndex)    //  'row' index
            {
                int sum = 0;

                for (int inputItemIndex = 0; inputItemIndex < inputLength; ++inputItemIndex)   // 'column' index
                {
                    if (patternRepetition % (inputIndex + 1) == 0)  // Note that min. number of repetition is 1, for row index 0
                    {
                        patternIndex = ++patternIndex % _basePattern.Count;
                        patternRepetition = 0;
                    }
                    //else
                    //{
                    ++patternRepetition;
                    //}

                    int input = inputList.ElementAt(inputItemIndex);
                    int multiplier = _basePattern.ElementAt(patternIndex);
                    sum += input * multiplier;

                }

                patternIndex = 0;
                patternRepetition = 1;
                yield return Math.Abs(sum) % 10;
            }
        }

        public override string Solve_2()
        {
            return "";
        }

        private IEnumerable<int> ParseInput()
        {
            return new ParsedFile(FilePath)
                .ToSingleString()
                .Select(ch => int.Parse(ch.ToString()));
        }
    }
}
