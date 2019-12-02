using AoCHelper;
using FileParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC_2019
{
    public class Problem01 : BaseProblem
    {
        public override void Solve_1()
        {
            var input = ParseInput();

            var result = input.Select(CalculateFuel()).Sum();

            Console.Write($"Day 1, part 1: {result}");
        }

        public override void Solve_2()
        {
            var input = ParseInput();

            var initialFuel = input.Select(CalculateFuel());

            var result = initialFuel.Sum();
            for (int i = 0; i < initialFuel.Count(); ++i)
            {
                var fuelPerModule = initialFuel.ElementAt(i);
                while (true)
                {
                    var extraFuel = CalculateFuel().Invoke(fuelPerModule);
                    if (extraFuel <= 0)
                    {
                        break;
                    }

                    fuelPerModule = extraFuel;
                    result += extraFuel;
                }
            }

            Console.Write($"Day 1, part 2: {result}");
        }

        private static Func<int, int> CalculateFuel()
        {
            return item => (int)Math.Floor(item / 3.0) - 2;
        }

        public ICollection<int> ParseInput()
        {
            ParsedFile file = new ParsedFile(FilePath);

            return file.ToList<int>();
        }
    }
}
