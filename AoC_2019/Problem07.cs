using AoCHelper;
using FileParser;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace AoC_2019
{
    public class Problem07 : BaseProblem
    {
        public override string Solve_1()
        {
            var intCode = ParseInput().ToList();

            List<int> phaseList = new List<int> { 0, 1, 2, 3, 4 };
            int ampNumber = phaseList.Count;

            HashSet<string> permutations = GeneratePermutations(phaseList);

            permutations = permutations.OrderBy(_ => _).ToHashSet();

            int maxThusterSignal = int.MinValue;
            foreach (var permutation in permutations)
            {
                var previousOutput = 0;
                for (int ampIndex = 0; ampIndex < ampNumber; ++ampIndex)
                {
                    var outputList = RunIntCodeProgram(intCode, input: new[] { int.Parse(permutation[ampIndex].ToString()), previousOutput }).Result;

                    previousOutput = outputList.Single();
                }

                maxThusterSignal = Math.Max(previousOutput, maxThusterSignal);
            }

            return maxThusterSignal.ToString();
        }

        public override string Solve_2()
        {
            var input = ParseInput().ToList();

            return "";
        }

        private static HashSet<string> GeneratePermutations(List<int> phaseList)
        {
            int permutationNumber = CalculateNumberOfPermutations(phaseList);

            var permutations = new HashSet<string>();

            while (permutations.Count != permutationNumber)
            {
                permutations.Add(string.Join("", phaseList.OrderBy(_ => Guid.NewGuid())));
            }

            return permutations;
        }

        /// <summary>
        /// P(n,r) = n! / (n - r)!
        /// n -> number of total objects
        /// r -> number of objects to be ordered
        /// In our case:
        /// P = phases! / (phases - phases)! = phases!
        /// </summary>
        /// <param name="phaseList"></param>
        /// <returns></returns>
        private static int CalculateNumberOfPermutations(List<int> phaseList)
        {
            return Factorial(phaseList.Count);
        }

        private static int Factorial(int n)
        {
            return n > 0
                ? n * Factorial(n - 1)
                : 1;
        }

        private static async Task<ICollection<int>> RunIntCodeProgram(List<int> intCode, IEnumerable<int> input)
        {
            Channel<int> channel = Channel.CreateUnbounded<int>();
            IntCodeComputer computer = new IntCodeComputer(channel);

            foreach (int n in input)
            {
                await channel.Writer.WriteAsync(n).ConfigureAwait(false);
            }

            ICollection<int> result = new List<int>();
            await foreach (var item in computer.RunIntCodeProgram(intCode))
            {
                result.Add(item);
            }

            return result;
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
