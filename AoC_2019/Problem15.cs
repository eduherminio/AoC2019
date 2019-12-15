using AoC_2019.Arcade;
using AoC_2019.Arcade.Pieces;
using AoC_2019.IntCode;
using AoCHelper;
using FileParser;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using MersenneTwister;
using System;

namespace AoC_2019
{
    public class Problem15 : BaseProblem
    {
        private static readonly Random _rnd = Randoms.Create(RandomType.FastestInt32);

        public override string Solve_1()
        {
            var input = ParseInput().ToList();

            int minSteps = DosSuff(input).Result;

            return minSteps.ToString();
        }

        private static int GenerateRandomInput()
        {
            return _rnd.Next(0, 5);
        }

        private static async Task<int> DosSuff(List<long> input)
        {
            Channel<long> ch = Channel.CreateUnbounded<long>();

            IntCodeComputer computer = new IntCodeComputer(ch);
            int minSteps = int.MaxValue;


            List<int> winningSequence = new List<int>();
            int previousSteps = 0;
            while (true)
            {
                List<int> usedSequence = new List<int>();

                //for (int i = usedSequence.Count; i < 100; ++i)
                //{
                //    int randomInput = GenerateRandomInput();
                //    if (ch.Writer.TryWrite(randomInput))
                //    {
                //        usedSequence.Add(randomInput);
                //    }
                //}

                int randomInput = GenerateRandomInput();
                ch.Writer.TryWrite(randomInput);
                usedSequence.Add(randomInput);

                var intCode = (long[])input.ToArray().Clone();
                int steps = 0;
                await foreach (var output in computer.RunIntCodeProgram(intCode.ToList().ToList()))
                {
                    ++steps;
                    if (output == 2)
                    {
                        if (steps < minSteps)
                        {
                            Console.WriteLine(minSteps);
                            minSteps = steps;
                        }
                        break;
                    }
                    else if (output == 1)
                    {
                        ++steps;
                        randomInput = GenerateRandomInput();
                        ch.Writer.TryWrite(randomInput);
                        usedSequence.Add(randomInput);
                    }
                    else if (output == 0)
                    {
                        while (randomInput == usedSequence.Last())
                        {
                            randomInput = GenerateRandomInput();
                        }
                        ch.Writer.TryWrite(randomInput);
                        usedSequence.RemoveAt(usedSequence.Count - 1);
                        usedSequence.Add(randomInput);
                        steps--;
                    }
                }

                int unusedInputs = 0;
                while (ch.Reader.TryRead(out var _))
                {
                    ++unusedInputs;
                }
                int processedInputs = 100 - unusedInputs;

                //            if ( > topScore
                //|| (processedInputs > winningSequence.Count && processedInputs > 4))
                //            {

                //                if ()
            }

            return minSteps;
        }

        public override string Solve_2()
        {
            return "";
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
