﻿using AoC_2019.IntCode;
using AoCHelper;
using FileParser;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using MersenneTwister;
using System;
using AoCHelper.Model;
using System.Text;

namespace AoC_2019
{
    public class Problem15 : BaseProblem
    {
        private static readonly Random _rnd = Randoms.Create(RandomType.FastestInt32);

        public override string Solve_1()
        {
            int result = CalculateFewestCommandsToOxigen().Result;

            return result.ToString();
        }

        public override string Solve_2()
        {
            return "";
        }

        private static int GenerateRandomInput()
        {
            return _rnd.Next(0, 5);
        }

        private enum Status
        {
            Unknown = -1,
            Wall = 0,
            Empty = 1,
            Destination = 2,
            Origin = 3,
            Robot = 4,
            InPath = 5,
            NoPath = 6
        }

        private async Task<int> CalculateFewestCommandsToOxigen()
        {
            Channel<long> inputChannel = Channel.CreateUnbounded<long>();
            IntCodeComputer computer = new IntCodeComputer(inputChannel);

            IDictionary<Point, Status> map = new Dictionary<Point, Status>()
            {
                [new Point(0, 0)] = Status.Origin
            };

            Point currentPosition = new Point(0, 0);
            Point nextPosition = new Point(1, 0);
            inputChannel.Writer.TryWrite(1);

            while (map.Values.SingleOrDefault(v => v == Status.Destination) == default)
            {
                await foreach (var output in computer.RunIntCodeProgram(ParseInput().ToList()))
                {
                    map.TryAdd(nextPosition, (Status)output);

                    if ((Status)output == Status.Empty || ((Status)output) == Status.Destination)
                    {
                        currentPosition = nextPosition;
                    }

                    int command = ChooseNextPosition(map, currentPosition, out nextPosition);

                    if (map.Values.SingleOrDefault(v => v == Status.Destination) != default && map.Count > 1600)
                    {
                        break;
                    }

                    inputChannel.Writer.TryWrite(command);
                }
            }

            // PrintMap(map);

            return CalculateShortestDistance(map);
        }

        private static int ChooseNextPosition(IDictionary<Point, Status> map, Point currentPosition, out Point nextPosition)
        {
            const int maxAttempts = 25;
            int attempts = 0;

            while (true)
            {
                int command = _rnd.Next(1, 5);
                nextPosition = command switch
                {
                    1 => new Point(currentPosition.X, currentPosition.Y + 1),
                    2 => new Point(currentPosition.X, currentPosition.Y - 1),
                    3 => new Point(currentPosition.X - 1, currentPosition.Y),
                    4 => new Point(currentPosition.X + 1, currentPosition.Y),
                    _ => throw new SolvingException("Learn to use pseudo-random generator lib")
                };

                if (!map.ContainsKey(nextPosition)
                || (attempts > maxAttempts
                    && Math.Abs(nextPosition.X) < 100
                    && Math.Abs(nextPosition.Y) < 100))
                {
                    return command;
                }
                ++attempts;
            }
        }

        private static int CalculateShortestDistance(IDictionary<Point, Status> map)
        {
            Point origin = map.Single(p => p.Value == Status.Origin).Key;
            Point destination = map.Single(p => p.Value == Status.Destination).Key;

            ICollection<Point> pathCandidates = map.Where(pair => pair.Value == Status.Empty).Select(p => p.Key).ToList();

            int minPath = int.MaxValue;

            List<Point> optimalPath = new List<Point>();
            HashSet<Point> noPath = new HashSet<Point>();
            HashSet<Point> pointsThatLedToDestination = new HashSet<Point>();

            int index = 0;
            while (minPath == int.MaxValue || index < 250)
            {
                Point currentPoint = origin;
                List<Point> path = new List<Point>();

                bool success = true;
                while (currentPoint.ManhattanDistance(destination) != 1)
                {
                    var candidates = pathCandidates.Where(p =>
                        p.ManhattanDistance(currentPoint) == 1
                        && !path.Contains(p)
                        && !noPath.Contains(p))
                        .ToList();

                    int nCandidates = candidates.Count;

                    if (nCandidates == 0)
                    {
                        success = false;
                        noPath.Add(currentPoint);
                        break;
                    }
                    else if (nCandidates == 1)
                    {
                        path.Add(candidates.First());
                        currentPoint = candidates.First();
                    }
                    else
                    {
                        var promisingCandidate = candidates.FirstOrDefault(c => !pointsThatLedToDestination.Contains(c));

                        var chosenCandidate = promisingCandidate ?? candidates[_rnd.Next(0, nCandidates)];

                        path.Add(chosenCandidate);
                        currentPoint = chosenCandidate;
                    }

                    // ShowRobotAdventures(map, path, noPath, currentPoint);
                }

                if (success && path.Count < minPath)
                {
                    pointsThatLedToDestination = new HashSet<Point>(path);
                    optimalPath = path;
                    minPath = path.Count;
                    // Console.WriteLine($"New shortest path: {path.Count + 1}");
                }

                ++index;
            }

            return minPath + 1;  //   Include destination
        }

        private static void PrintMap(IDictionary<Point, Status> map)
        {
            int minX = map.Keys.Min(p => p.X);
            int minY = map.Keys.Min(p => p.Y);
            int maxX = map.Keys.Max(p => p.X);
            int maxY = map.Keys.Max(p => p.Y);

            StringBuilder sb = new StringBuilder();
            for (int x = minX; x < maxX; ++x)
            {
                for (int y = minY; y < maxY; ++y)
                {
                    if (map.TryGetValue(new Point(x, y), out var status))
                    {
                        sb.Append(status switch
                        {
                            Status.Unknown => '?',
                            Status.Empty => ' ',
                            Status.Destination => 'v',
                            Status.Origin => 'O',
                            Status.Wall => '#',
                            Status.Robot => '^',
                            Status.InPath => '.',
                            Status.NoPath => 'X',
                            _ => throw new SolvingException("Unhandled status")
                        });
                    }
                    else
                    {
                        sb.Append("?");
                    }
                }
                sb.Append(Environment.NewLine);
            }

            string printedMap = sb.ToString();

            Console.Clear();
            Console.WriteLine(printedMap);
        }

        private static void ShowRobotAdventures(IDictionary<Point, Status> map, IEnumerable<Point> path, IEnumerable<Point> noPath, Point currentPoint)
        {
            foreach (var p in path)
            {
                map.Remove(p);
                map.Add(p, Status.InPath);
            }

            foreach (var p in noPath)
            {
                map.Remove(p);
                map.Add(p, Status.NoPath);
            }

            map.Remove(currentPoint);
            map.Add(currentPoint, Status.Robot);

            PrintMap(map);
            System.Threading.Thread.Sleep(5);

            foreach (var key in map.Where(pair => pair.Value == Status.InPath).Select(p => p.Key).ToList())
            {
                map[key] = Status.Empty;
            }

            foreach (var key in map.Where(pair => pair.Key.X != 0 && pair.Key.Y != 0 && pair.Value == Status.Robot).Select(p => p.Key).ToList())
            {
                map[key] = Status.Empty;
            }
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
