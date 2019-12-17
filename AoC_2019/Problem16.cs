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
                input = ApplyPhaseImproved(input).ToList();
            }

            return string.Join(string.Empty, input.Take(8).Select(n => n.ToString()));
        }

        /// <summary>
        /// Naive, bruteforce implementation using improved applyphase method
        /// </summary>
        /// <returns></returns>
        //public override string Solve_2()
        //{
        //    ICollection<int> input = ParseInput().ToList();

        //    int offset = int.Parse(string.Join(string.Empty, input.Take(7).Select(n => n.ToString())));

        //    List<int> realInput = new List<int>(10_000 * input.Count);
        //    for (int i = 0; i < 10_000; ++i)
        //    {
        //        realInput.AddRange(input);
        //    }

        //    const int phasesToApply = 100;
        //    for (int i = 0; i < phasesToApply; ++i)
        //    {
        //        realInput = ApplyPhaseImproved(realInput).ToList();
        //        Console.Write($"Phase {i + 1} completed!!");
        //    }

        //    return string.Join(string.Empty, realInput.Skip(offset).Take(8).Select(n => n.ToString()));
        //}

        public override string Solve_2()
        {
            return "";
        }

        private IEnumerable<int> ApplyPhase(IEnumerable<int> inputList)
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

                    ++patternRepetition;

                    int input = inputList.ElementAt(inputItemIndex);
                    int multiplier = _basePattern.ElementAt(patternIndex);
                    sum += input * multiplier;
                }

                patternIndex = 0;
                patternRepetition = 1;
                yield return Math.Abs(sum) % 10;
            }
        }

        private IEnumerable<int> ApplyPhaseImproved(IEnumerable<int> inputList)
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

                    // Avoid multiplying and adding elements when multiplier is going to be 0
                    if (_basePattern.ElementAt(patternIndex) == 0)
                    {
                        inputItemIndex += inputIndex + 1 - (inputItemIndex == 0 ? 1 : 0);
                        if (inputItemIndex >= inputLength)
                        {
                            break;
                        }
                        patternIndex = ++patternIndex % _basePattern.Count;
                        patternRepetition = 0;
                    }

                    ++patternRepetition;

                    int input = inputList.ElementAt(inputItemIndex);
                    if (input != 0)
                    {
                        int multiplier = _basePattern.ElementAt(patternIndex);
                        sum += input * multiplier;
                    }
                }

                patternIndex = 0;
                patternRepetition = 1;
                yield return Math.Abs(sum) % 10;
            }
        }

        /// <summary>
        /// [minRange, maxRange]
        /// </summary>
        /// <param name="inputList"></param>
        /// <param name="minRange"></param>
        /// <returns></returns>
        private IEnumerable<int> ApplyPhaseForSignalRange(IEnumerable<int> inputList, int minRange)
        {
            int inputLength = inputList.Count();

            // Index of pattern
            int patternIndex = 0;

            // repetitions of an pattern[patternIndex], starts in one to skip first item in the pattern for each 'row'
            int patternRepetition = 1;
            for (int inputIndex = minRange; inputIndex <= inputLength; ++inputIndex)    //  'row' index
            {
                int sum = 0;

                for (int inputItemIndex = minRange; inputItemIndex < inputLength; ++inputItemIndex)   // 'column' index
                {
                    if (patternRepetition % (inputIndex + 1) == 0)  // Note that min. number of repetition is 1, for row index 0
                    {
                        patternIndex = ++patternIndex % _basePattern.Count;
                        patternRepetition = 0;

                        if (_basePattern.ElementAt(patternIndex) == 0)
                        {
                            inputItemIndex += inputIndex;
                            patternIndex = ++patternIndex % _basePattern.Count;
                        }
                    }

                    ++patternRepetition;

                    int input = inputList.ElementAt(inputItemIndex);
                    int multiplier = _basePattern.ElementAt(patternIndex);
                    sum += input * multiplier;
                }

                patternIndex = 0;
                patternRepetition = 1;
                yield return Math.Abs(sum) % 10;
            }
        }

        private IEnumerable<int> ParseInput()
        {
            return new ParsedFile(FilePath)
                .ToSingleString()
                .Select(ch => int.Parse(ch.ToString()));
        }
    }
}
