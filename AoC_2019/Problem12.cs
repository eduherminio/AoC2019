using AoCHelper;
using SheepTools.Extensions;
using SheepTools.Model;
using FileParser;
using MoreLinq.Extensions;
using System;
using System.Collections.Generic;
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

            double result = moons.Sum(moon => moon.TotalEnergy);

            return result.ToString();
        }

        public override string Solve_2()
        {
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
            return items.Factorial() / (combinationSize.Factorial() * (items - combinationSize).Factorial());
        }

        private IEnumerable<Moon> ParseInput()
        {
            var file = new ParsedFile(FilePath);

            for (int i = 0; i < 4; ++i)
            {
                var rawLine = file.NextLine().ToSingleString();
                var enhancedLine = rawLine[1..^1].Replace(" ", string.Empty);

                var coordinates = enhancedLine
                    .Split(",")
                    .Select(str => int.Parse(str.Substring(2)));

                if (coordinates.Count() != 3)
                {
                    throw new SolvingException($"Only 3 coordinates are expected in this line:\n{rawLine}");
                }

                yield return new Moon(_moonNames[i],
                    new Point3D(coordinates.ElementAt(0), coordinates.ElementAt(1), coordinates.ElementAt(2)));
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

        public Point3D Position { get; }

        public Point3D Velocity { get; }

        public double PotentialEnergy => Math.Abs(Position.X) + Math.Abs(Position.Y) + Math.Abs(Position.Z);

        public double KineticEnergy => Math.Abs(Velocity.X) + Math.Abs(Velocity.Y) + Math.Abs(Velocity.Z);

        public double TotalEnergy => PotentialEnergy * KineticEnergy;

        public Moon(string id, Point3D initialPosition)
        {
            Id = id;
            Position = initialPosition;
            Velocity = new Point3D(0, 0, 0);
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
