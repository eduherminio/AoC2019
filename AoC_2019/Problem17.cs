using AoC_2019.IntCode;
using AoC_2019.Model;
using AoCHelper;
using FileParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC_2019
{
    public class Problem17 : BaseProblem
    {
        private const string Scaffold = "#";
        private readonly IReadOnlyCollection<string> Robot = new[] { "<", "^", ">", "v" };
        private const string OpenSpace = ".";
        private const string NewLine = "\n";
        private const string TumblingRobot = "X";

        public override string Solve_1()
        {
            var intCode = ParseInput().ToList();

            var intCodeOutput = IntCodeHelpers.RunIntCodeProgram(intCode).Result;

            ICollection<PointWithId> points = new List<PointWithId>();

            int x = 0, y = 0;
            foreach (long output in intCodeOutput)
            {
                string ch = Convert.ToChar(output).ToString();

                if (ch == NewLine)
                {
                    ++y;
                    x = 0;
                    continue;
                }

                points.Add(new PointWithId(ch, x, y));
                ++x;
            }

            var intersections = points.Where(p =>
                    p.Id == Scaffold
                    && points.Count(otherP =>
                        p.ManhattanDistance(otherP) == 1 && otherP.Id == Scaffold) == 4);

            static int CalculateAlignmentParameter(AoCHelper.Model.Point p) => p.X * p.Y;

            int result = intersections
                .Sum(CalculateAlignmentParameter);

            return result.ToString();
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
