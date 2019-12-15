using AoC_2019.Arcade;
using FileParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcade
{
    public static class Program
    {
        private const string BlockDestroyerInputFilePath = "Inputs/BlockDestroyer.in";

        public static void Main()
        {
            bool success = false;

            while (!success)
            {
                ArcadeMachine arcadeMachine = new ArcadeMachine(ParseInput().ToList());

                long score = arcadeMachine.Play(DifficultyLevel.Easy).Result;

                Console.WriteLine($"Score: {score}");

                Console.WriteLine("Press any key to play again");

                Console.ReadKey();

                Console.Clear();
            }
        }

        private static IEnumerable<long> ParseInput()
        {
            return new ParsedFile(BlockDestroyerInputFilePath)
                .ToSingleString()
                .Split(',')
                .Select(long.Parse);
        }
    }
}
