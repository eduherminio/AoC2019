using AoCHelper;
using SheepTools.Model;
using FileParser;
using System;
using System.Collections.Generic;
using System.Linq;
using SheepTools;

namespace AoC_2019
{
    public class Problem03 : BaseProblem
    {
        public override string Solve_1()
        {
            var input = ParseInput().ToList();

            HashSet<Point> crossPoints = new HashSet<Point>();
            IDictionary<Point, int> allLinesPoints = new Dictionary<Point, int>();

            int lineIndex = 0;
            foreach (IEnumerable<string> line in input)
            {
                Point current = new Point(0, 0);
                foreach (string ins in line)
                {
                    int dis = int.Parse(ins.Substring(1));
                    char c = ins[0];

                    void handlePoint(int x, int y, int lineIndex)
                    {
                        var newPoint = new Point(x, y);
                        if (allLinesPoints.TryGetValue(newPoint, out int existingLineIndex))
                        {
                            if (existingLineIndex != lineIndex)
                            {
                                crossPoints.Add(newPoint);
                            }
                        }
                        else
                        {
                            allLinesPoints.Add(newPoint, lineIndex);
                        }
                    }

                    switch (c)
                    {
                        case 'U':
                            RangeHelpers.GenerateRange((int)current.Y + 1, (int)current.Y + dis).ToList()
                                .ForEach(y => handlePoint((int)current.X, y, lineIndex));
                            current.Y += dis;
                            break;

                        case 'D':
                            RangeHelpers.GenerateRange((int)current.Y - dis, (int)current.Y - 1).ToList()
                                .ForEach(y => handlePoint((int)current.X, y, lineIndex));
                            current.Y -= dis;
                            break;

                        case 'R':
                            RangeHelpers.GenerateRange((int)current.X + 1, (int)current.X + dis).ToList()
                                .ForEach(x => handlePoint(x, (int)current.Y, lineIndex));
                            current.X += dis;
                            break;

                        case 'L':
                            RangeHelpers.GenerateRange((int)current.X - dis, (int)current.X - 1).ToList()
                                .ForEach(x => handlePoint(x, (int)current.Y, lineIndex));
                            current.X -= dis;
                            break;
                    }
                }
                lineIndex++;
            }

            var origin = new Point(0, 0);
            crossPoints.Remove(origin);

            var closesPoint = origin.CalculateClosestManhattanPoint(crossPoints);

            return origin.ManhattanDistance(closesPoint).ToString();
        }

        public override string Solve_2()
        {
            var input = ParseInput().ToList();

            IDictionary<Point, int> crossPoints = new Dictionary<Point, int>();
            IDictionary<Point, Tuple<int, int>> allLinesPoints = new Dictionary<Point, Tuple<int, int>>();

            int lineIndex = 0;
            foreach (IEnumerable<string> line in input)
            {
                Point current = new Point(0, 0);
                int lineLength = 0;
                foreach (string ins in line)
                {
                    int dis = int.Parse(ins.Substring(1));
                    char c = ins[0];

                    void handlePoint(int x, int y, int lineIndex, ref int lineLength)
                    {
                        lineLength++;
                        var newPoint = new Point(x, y);
                        if (allLinesPoints.TryGetValue(newPoint, out Tuple<int, int> existingLineIndexAndValue))
                        {
                            if (existingLineIndexAndValue.Item1 != lineIndex)
                            {
                                crossPoints.Add(newPoint, existingLineIndexAndValue.Item2 + lineLength);
                            }
                        }
                        else
                        {
                            allLinesPoints.Add(newPoint, Tuple.Create(lineIndex, lineLength));
                        }
                    }

                    switch (c)
                    {
                        case 'U':
                            RangeHelpers.GenerateRange((int)current.Y + 1, (int)current.Y + dis).ToList()
                                .ForEach(y => handlePoint((int)current.X, y, lineIndex, ref lineLength));
                            current.Y += dis;
                            break;

                        case 'D':
                            RangeHelpers.GenerateRange((int)current.Y - dis, (int)current.Y - 1).ToList()
                                .ForEach(y => handlePoint((int)current.X, y, lineIndex, ref lineLength));
                            current.Y -= dis;
                            break;

                        case 'R':
                            RangeHelpers.GenerateRange((int)current.X + 1, (int)current.X + dis).ToList()
                                .ForEach(x => handlePoint(x, (int)current.Y, lineIndex, ref lineLength));
                            current.X += dis;
                            break;

                        case 'L':
                            RangeHelpers.GenerateRange((int)current.X - dis, (int)current.X - 1).ToList()
                                .ForEach(x => handlePoint(x, (int)current.Y, lineIndex, ref lineLength));
                            current.X -= dis;
                            break;
                    }
                }
                lineIndex++;
            }

            var origin = new Point(0, 0);
            crossPoints.Remove(origin);

            var closestPoint = crossPoints.OrderBy(item => item.Value).First();

            return closestPoint.Value.ToString();
        }

        private IEnumerable<IEnumerable<string>> ParseInput()
        {
            var input = new List<List<string>>();
            var file = new ParsedFile(FilePath);

            while (!file.Empty)
            {
                var line = file.NextLine();

                input.Add(line.ToSingleString()
                    .Split(',')
                    .Select(s => s.ToUpperInvariant()).ToList());
            }

            return input;
        }
    }
}
