using AoCHelper;
using FileParser;
using System;
using System.Collections.Generic;
using System.Linq;
using AoCHelper.Helpers;

namespace AoC_2019
{
    public class Problem04 : BaseProblem
    {
        public override string Solve_1()
        {
            var input = ParseInput().ToList();

            IEnumerable<List<int>> candidates = RangeHelpers.GenerateRange(input.First(), input.Last())
                .Select(n => n.ToString()
                    .ToArray()
                    .Select(c => int.Parse(c.ToString())).ToList())
                .ToList();

            var validCandidates = candidates
                .Where(candidate =>
                {
                    bool hasDuplicatedDigits = false;
                    for (int index = 1; index < candidate.Count; ++index)
                    {
                        var left = candidate.ElementAt(index - 1);
                        var right = candidate.ElementAt(index);

                        if (left <= right)
                        {
                            hasDuplicatedDigits |= (left == right);
                            continue;
                        }

                        return false;
                    }

                    return hasDuplicatedDigits;
                });


            return validCandidates.Count().ToString();
        }

        public override string Solve_2()
        {
            var input = ParseInput().ToList();

            IEnumerable<List<int>> candidates = RangeHelpers.GenerateRange(input.First(), input.Last())
                .Select(n => n.ToString()
                    .ToArray()
                    .Select(c => int.Parse(c.ToString())).ToList())
                .ToList();

            var validCandidates = candidates
                .Where(candidate =>
                {
                    List<int> weightedCandidated = new List<int>(candidate.Count) { 1 };

                    for (int index = 1; index < candidate.Count; ++index)
                    {
                        var left = candidate.ElementAt(index - 1);
                        var right = candidate.ElementAt(index);

                        if (left <= right)
                        {
                            if (left == right)
                            {
                                weightedCandidated[weightedCandidated.Count - 1]++;
                            }
                            else
                            {
                                weightedCandidated.Add(1);
                            }
                            continue;
                        }

                        return false;
                    }

                    return weightedCandidated.Any(n => n == 2);
                });

            return validCandidates.Count().ToString();
        }

        public IEnumerable<int> ParseInput()
        {
            return new ParsedFile(FilePath)
                .ToSingleString()
                .Split('-')
                .Select(int.Parse);
        }
    }
}
