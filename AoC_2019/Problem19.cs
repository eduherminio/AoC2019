using System;
using AoC_2019.IntCode;
using AoC_2019.Model;
using Point = AoCHelper.Model.Point;
using AoCHelper;
using FileParser;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace AoC_2019
{
    public class Problem19 : BaseProblem
    {
        public override string Solve_1()
        {
            const int xMax = 50;
            const int yMax = 50;

            var map = ScanUniverse(xMax, yMax).ToList();

            //PrintMap(map.ToHashSet());

            int result = map.Count(p => p.Id == "1");

            return result.ToString();
        }

        public override string Solve_2()
        {
            const int squareSideSize = 100;

            var point = ScanSeekingSquare(squareSideSize);

            long result = 10_000 * point.X + point.Y;

            return result.ToString();
        }

        private IEnumerable<PointWithId> ScanUniverse(int xMax, int yMax)
        {
            for (int y = 0; y < yMax; ++y)
            {
                for (int x = 0; x < xMax; ++x)
                {
                    yield return new PointWithId(ScanPoint(x, y).Result.ToString(), x, y);
                }
            }
        }

        private async Task<long> ScanPoint(int x, int y)
        {
            var inputChannel = Channel.CreateUnbounded<long>();
            var computer = new IntCodeComputer(inputChannel);

            inputChannel.Writer.TryWrite(x);
            inputChannel.Writer.TryWrite(y);

            var output = await computer.RunIntCodeProgram(ParseInput().ToList()).ToListAsync();

            return output.Single();
        }

        private static void PrintMap(HashSet<PointWithId> map)
        {
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < map.Max(p => p.Y); ++y)
            {
                for (int x = 0; x < map.Max(p => p.X); ++x)
                {
                    if (map.TryGetValue(new PointWithId("", x, y), out var actualPoint))
                    {
                        sb.Append(actualPoint.Id);
                    }
                }

                sb.Append(Environment.NewLine);
            }

            Console.WriteLine(sb.ToString());
        }

        private Point ScanSeekingSquare(int side)
        {
            --side;
            int lastOnePosition = 0;
            for (int y = 10; ; ++y)
            {
                //if (y % 10 == 0) Console.WriteLine(y);

                bool firstOneDetected = false;
                long previousHorizontalScanResult = 0;
                for (int x = lastOnePosition; ; ++x)
                {
                    var bottomLeft = ScanPoint(x, y).Result;

                    if (bottomLeft == 1)
                    {
                        previousHorizontalScanResult = bottomLeft;

                        if (!firstOneDetected)
                        {
                            firstOneDetected = true;
                            lastOnePosition = x;
                        }

                        var bottomRight = ScanPoint(x + side, y).Result;
                        if (bottomRight != 1)
                            continue;

                        var topLeft = ScanPoint(x, y - side).Result;
                        if (topLeft != 1)
                            continue;

                        var topRight = ScanPoint(x + side, y - side).Result;

                        if (y >= side && x >= side
                            && bottomRight == 1
                            && topLeft == 1
                            && topRight == 1)
                        {
                            for (int _x = x; _x <= x + side; ++_x)
                            {
                                for (int _y = y; _y >= y - side; --_y)
                                {
                                    if (ScanPoint(_x, _y).Result == 0)
                                    {
                                        goto error;
                                    }
                                }
                            }

                            return new Point(x, y - side);
                            error:
                            ;
                        }
                    }
                    else
                    {
                        if (previousHorizontalScanResult == 1)
                        {
                            break;
                        }
                    }

                    previousHorizontalScanResult = bottomLeft;
                }
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
