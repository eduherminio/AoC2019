﻿using AoCHelper;
using FileParser;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using AoC_2019.IntCode;
using AoC_2019.Extensions;

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

            long maxThrusterSignal = long.MinValue;
            foreach (var permutation in permutations)
            {
                long previousOutput = 0;
                for (int ampIndex = 0; ampIndex < ampNumber; ++ampIndex)
                {
                    var outputList = IntCodeHelpers.RunIntCodeProgram(intCode, input: new[] { long.Parse(permutation[ampIndex].ToString()), previousOutput }).Result;

                    previousOutput = outputList.Single();
                }

                maxThrusterSignal = Math.Max(previousOutput, maxThrusterSignal);
            }

            return maxThrusterSignal.ToString();
        }

        public override string Solve_2()
        {
            var intCode = ParseInput().ToList();

            List<int> phaseList = new List<int> { 5, 6, 7, 8, 9 };
            int ampNumber = phaseList.Count;

            HashSet<string> permutations = GeneratePermutations(phaseList);

            permutations = permutations.OrderBy(_ => _).ToHashSet();

            long maxThusterSignal = long.MinValue;
            foreach (var permutation in permutations)
            {
                ICollection<Amplifier> amplifiers = SetupAmplifiers(ampNumber, permutation).Result;

                long result = CalculateFeedbackLoopOutput(amplifiers, intCode).Result;

                maxThusterSignal = Math.Max(result, maxThusterSignal);
            }

            return maxThusterSignal.ToString();
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
            return IntHelpers.Factorial(phaseList.Count);
        }

        private static async Task<ICollection<Amplifier>> SetupAmplifiers(int ampNumber, string permutation)
        {
            List<Amplifier> amplifiers = new List<Amplifier>() { new Amplifier(ampNumber.ToString()) };
            for (int ampIndex = 0; ampIndex < ampNumber - 1; ++ampIndex)
            {
                amplifiers.Add(new Amplifier((ampNumber - ampIndex - 1).ToString(), amplifiers.Last()));
            }

            amplifiers.Reverse();

            amplifiers.Last().ConnectOutputTo(amplifiers.First());

            for (int ampIndex = 0; ampIndex < ampNumber; ++ampIndex)
            {
                await amplifiers[ampIndex].AddInput(int.Parse(permutation[ampIndex].ToString())).ConfigureAwait(false);
            }

            await amplifiers.First().AddInput(0).ConfigureAwait(false);

            return amplifiers;
        }

        private static async Task<long> CalculateFeedbackLoopOutput(ICollection<Amplifier> amplifiers, List<long> signal)
        {
            IEnumerable<Task<long>> tasks = amplifiers.Select(ampl => ampl.Run(signal));

            var result = await Task.WhenAll(tasks.ToArray()).ConfigureAwait(false);

            return result.Max();
        }

        private IEnumerable<long> ParseInput()
        {
            return new ParsedFile(FilePath)
                .ToSingleString()
                .Split(',')
                .Select(long.Parse);
        }
    }

    public class Amplifier
    {
#pragma warning disable S1450, IDE0052 // Private fields only used as local variables in methods should become local variables
        private readonly string _id;
#pragma warning restore S1450, IDE0052 // Private fields only used as local variables in methods should become local variables
        private readonly Channel<long> _channel;
        private Amplifier _nextAmplifier;
        public bool Active { get; set; }

        public Amplifier(string id)
        {
            _id = id;
            _channel = Channel.CreateUnbounded<long>();
            Active = true;
        }

        public Amplifier(string id, Amplifier nextAmplifier) : this(id)
        {
            _nextAmplifier = nextAmplifier;
        }

        public async Task<long> Run(List<long> signal)
        {
            List<long> outputs = new List<long>();
            await foreach (var item in new IntCodeComputer(_channel).RunIntCodeProgram(signal))
            {
                outputs.Add(item);
                await _nextAmplifier.AddInput(item).ConfigureAwait(false);
            }

            return outputs.Last();
        }

        public async Task AddInput(long input)
        {
            await _channel.Writer.WriteAsync(input).ConfigureAwait(false);
        }

        public void ConnectOutputTo(Amplifier amplifier)
        {
            _nextAmplifier = amplifier;
        }
    }
}
