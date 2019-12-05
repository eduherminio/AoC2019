using AoCHelper;
using FileParser;
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
            IEnumerable<List<int>> candidates = ExtractCandidates(input);

            var validCandidates = candidates
                .Where(candidate =>
                {
                    bool hasDuplicatedDigits = false;
                    for (int index = 1; index < candidate.Count; ++index)
                    {
                        var left = candidate[index - 1];
                        var right = candidate[index];

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

            IEnumerable<List<int>> candidates = ExtractCandidates(input);

            var validCandidates = candidates
                .Where(candidate =>
                {
                    List<int> weightedCandidate = new List<int>(candidate.Count) { 1 };

                    for (int index = 1; index < candidate.Count; ++index)
                    {
                        var left = candidate[index - 1];
                        var right = candidate[index];

                        if (left < right)
                        {
                            weightedCandidate.Add(1);
                        }
                        else if (left == right)
                        {
                            ++weightedCandidate[weightedCandidate.Count - 1];
                        }
                        else
                        {
                            return false;
                        }
                    }

                    return weightedCandidate.Any(n => n == 2);
                });

            return validCandidates.Count().ToString();
        }

        public string Solve_1_Alternative()
        {
            var input = ParseInput().ToList();

            IEnumerable<List<int>> candidates = ExtractCandidates(input);

            var validCandidates = candidates
                .Where(candidate =>
                {
                    List<int> weightedCandidate = new List<int>(candidate.Count) { 1 };

                    for (int index = 1; index < candidate.Count; ++index)
                    {
                        var left = candidate[index - 1];
                        var right = candidate[index];

                        if (left < right)
                        {
                            weightedCandidate.Add(1);
                        }
                        else if (left == right)
                        {
                            ++weightedCandidate[weightedCandidate.Count - 1];
                        }
                        else
                        {
                            return false;
                        }
                    }

                    return weightedCandidate.Any(n => n >= 2);
                });

            return validCandidates.Count().ToString();
        }

        private static IEnumerable<List<int>> ExtractCandidates(List<int> input)
        {
            return RangeHelpers.GenerateRange(input.First(), input.Last())
                .Select(n => n.ToString()
                    .Select(c => int.Parse(c.ToString())).ToList());
        }

        private IEnumerable<int> ParseInput()
        {
            return new ParsedFile(FilePath)
                .ToSingleString()
                .Split('-')
                .Select(int.Parse);
        }
    }
}
