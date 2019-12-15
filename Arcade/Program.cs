using AoC_2019.Arcade;
using FileParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Arcade
{
    public static class Program
    {
        private const string BlockDestroyerInputFilePath = "Inputs/BlockDestroyer.in";
        private const string HumanInputFilePath = "Inputs/13535_without_latest_moves.txt";

        public static void Main()
        {
            PlayCheatingWithPreSelectedInput();
        }

        private static void Play()
        {
            while (true)
            {
                ArcadeMachine arcadeMachine = new ArcadeMachine(ParseIntCode().ToList());

                long score = arcadeMachine.Play(DifficultyLevel.Easy).Result;

                Console.WriteLine($"Score: {score}");
                Console.WriteLine("Press any key to play again");
                Console.ReadKey();
                Console.Clear();
            }
        }

        private static void PlayWithPreselectedInput()
        {
            while (true)
            {
                Channel<long> input = Channel.CreateUnbounded<long>();
                ArcadeMachine arcadeMachine = new ArcadeMachine(ParseIntCode().ToList(), input);

                Task.Run(() => UserInputThread(input));

                long score = arcadeMachine.Play(DifficultyLevel.Easy).Result;

                Console.WriteLine($"Score: {score}");
                Console.WriteLine("Press any key to play again");
                Console.ReadKey();
                Console.Clear();
            }
        }

        private static void PlayCheatingWithPreSelectedInput()
        {
            var intCode = ParseIntCode().ToList();
            var preselectedInput = ParseHumanInput().ToList();

            Channel<long> input = Channel.CreateUnbounded<long>();
            ArcadeMachine arcadeMachine = new ArcadeMachine(intCode, input);

            var score = arcadeMachine.PlayCheatingWithPreSelectedInput(preselectedInput).Result;
            Console.WriteLine($"Score: {score}");
        }

        private static async Task UserInputThread(Channel<long> inputChannel)
        {
            while (true)
            {
                int userInput = await GetInputAsync().ConfigureAwait(false);
                await inputChannel.Writer.WriteAsync(userInput).ConfigureAwait(false);
            }
        }

        private static int InputCounter = 0;

        private static List<int> RecordedUserInput = ParseHumanInput().ToList();

        private static async Task<int> GetInputAsync()
        {
            if (InputCounter < RecordedUserInput.Count)
            {
                int inp = RecordedUserInput[InputCounter];
                ++InputCounter;
                return inp;
            }

            var input = await Task.Run(Console.ReadKey);

            int chosenOne = input.Key switch
            {
                ConsoleKey.LeftArrow => -1,
                ConsoleKey.A => -1,
                ConsoleKey.RightArrow => +1,
                ConsoleKey.D => +1,
                _ => RecordedUserInput[InputCounter]
            };

            ++InputCounter;

            return chosenOne;
        }

        private static IEnumerable<long> ParseIntCode()
        {
            return new ParsedFile(BlockDestroyerInputFilePath)
                .ToSingleString()
                .Split(',')
                .Select(long.Parse);
        }

        private static IEnumerable<int> ParseHumanInput()
        {
            return new ParsedFile(HumanInputFilePath)
                .ToList<int>();
        }
    }
}
