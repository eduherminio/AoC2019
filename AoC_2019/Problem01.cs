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

            var result = input.Select(CalculateFuel).Sum();

            Console.Write($"Day 1, part 1: {result}");
        }

        public override void Solve_2()
        {
            var input = ParseInput();

            var result = input.Select(CalculateFuelRecursive).Sum();

            Console.Write($"Day 1, part 2: {result}");
        }

        private static int CalculateFuel(int mass)
        {
            return (int)Math.Floor(mass / 3.0) - 2;
        }

        private static int CalculateFuelRecursive(int mass)
        {
            int initialFuel = CalculateFuel(mass);

            return initialFuel > 0
                ? initialFuel + CalculateFuelRecursive(initialFuel)
                : 0;
        }

        public ICollection<int> ParseInput()
        {
            ParsedFile file = new ParsedFile(FilePath);

            return file.ToList<int>();
        }
    }
}
