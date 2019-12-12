using AoC_2019.Extensions;
using AoC_2019.Model;
using AoCHelper;
using FileParser;
using MoreLinq.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AoC_2019
{
    public class Problem12 : BaseProblem
    {
        private static readonly Dictionary<int, string> _moonNames = new Dictionary<int, string>
        {
            [0] = "Io",
            [1] = "Europa",
            [2] = "Ganymede",
            [3] = "Callisto"
        };

        public override string Solve_1()
        {
            var moons = ParseInput().ToList();

            const int simulationTime = 1000;

            SimulateMovement(moons, simulationTime);

            long result = moons.Sum(moon => moon.TotalEnergy);

            return result.ToString();
        }

        public override string Solve_2()
        {
            var input = ParseInput().ToList();

            return "";
        }

        private static void SimulateMovement(IEnumerable<Moon> moons, int time)
        {
            if (moons.Subsets(2).Count() != CalculateNumberOfCombinations(moons.Count(), 2))
            {
                throw new SolvingException("You seem to be using moons.Subsets() method incorrectly to calculate the possible moon combinations");
            }

            for (int t = 0; t < time; ++t)
            {
                foreach (var pair in moons.Subsets(2))
                {
                    pair.First().ApplyGravity(pair.Last());
                    pair.Last().ApplyGravity(pair.First());
                }

                foreach (Moon m in moons)
                {
                    m.Move();
                }
            }
        }

        /// <summary>
        /// C(n,r) = n! / r!(n - r)!
        /// n -> number of total objects
        /// r -> size of each group
        /// In our case:
        /// C = moons! / 2!(moons - 2!)!
        /// </summary>
        /// <param name="items"></param>
        /// <param name="combinationSize"></param>
        /// <returns></returns>
        private static int CalculateNumberOfCombinations(int items, int combinationSize)
        {
            return IntHelpers.Factorial(items) / (IntHelpers.Factorial(combinationSize) * IntHelpers.Factorial(items - combinationSize));
        }

        private IEnumerable<Moon> ParseInput()
        {
            var file = new ParsedFile(FilePath);

            for (int i = 0; i < 4; ++i)
            {
                var rawLine = file.NextLine().ToSingleString();
                var enhancedLine = rawLine.Substring(1, rawLine.Length - 2).Replace(" ", string.Empty);

                var coordinates = enhancedLine
                    .Split(",")
                    .Select(str => int.Parse(str.Substring(2)));

                if (coordinates.Count() != 3)
                {
                    throw new SolvingException($"Only 3 coordinates are expected in this line:\n{rawLine}");
                }

                yield return new Moon(_moonNames[i],
                    new Point(coordinates.ElementAt(0), coordinates.ElementAt(1), coordinates.ElementAt(2)));
            }

            if (!file.Empty)
            {
                throw new SolvingException("Only 4 lines are expected in the input file");
            }
        }
    }

    public class Moon
    {
        public string Id { get; }

        public Point Position { get; private set; }

        public Point Velocity { get; private set; }

        public int PotentialEnergy => Math.Abs(Position.X) + Math.Abs(Position.Y) + Math.Abs(Position.Z);

        public int KineticEnergy => Math.Abs(Velocity.X) + Math.Abs(Velocity.Y) + Math.Abs(Velocity.Z);

        public long TotalEnergy => PotentialEnergy * KineticEnergy;

        public Moon(string id, Point initialPosition)
        {
            Id = id;
            Position = initialPosition;
            Velocity = new Point(0, 0, 0);
        }

        public void ApplyGravity(Moon otherMoon)
        {
            Velocity.X -= Math.Sign(Position.X - otherMoon.Position.X);
            Velocity.Y -= Math.Sign(Position.Y - otherMoon.Position.Y);
            Velocity.Z -= Math.Sign(Position.Z - otherMoon.Position.Z);
        }

        public void Move(int t = 1)
        {
            Position.X += t * Velocity.X;
            Position.Y += t * Velocity.Y;
            Position.Z += t * Velocity.Z;
        }
    }
}
