﻿using AoCHelper;
using FileParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace AoC_2019
{
    public class Problem05 : BaseProblem
    {
        public override string Solve_1()
        {
            List<long> intCode = ParseInput().ToList();

            ICollection<long> outputSequence = RunIntCodeProgram(intCode, input: 1).Result;

            return string.Join(string.Empty, outputSequence.SkipWhile(n => n == 0));
        }

        public override string Solve_2()
        {
            List<long> intCode = ParseInput().ToList();

            ICollection<long> outputSequence = RunIntCodeProgram(intCode, input: 5).Result;

            return string.Join(string.Empty, outputSequence.SkipWhile(n => n == 0));
        }

        private static async Task<ICollection<long>> RunIntCodeProgram(List<long> intCode, long input)
        {
            Channel<long> channel = Channel.CreateUnbounded<long>();
            IntCodeComputer computer = new IntCodeComputer(channel);

            await channel.Writer.WriteAsync(input).ConfigureAwait(false);

            ICollection<long> result = new List<long>();
            await foreach (var item in computer.RunIntCodeProgram(intCode))
            {
                result.Add(item);
            }

            return result;
        }

        private IEnumerable<long> ParseInput()
        {
            return new ParsedFile(FilePath)
                .ToSingleString()
                .Split(',')
                .Select(long.Parse);
        }
    }
}
