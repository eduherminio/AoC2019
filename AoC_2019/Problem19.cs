using AoC_2019.IntCode;
using AoC_2019.Model;
using AoCHelper;
using FileParser;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
            var intCode = ParseInput().ToList();

            var output = IntCodeHelpers.RunIntCodeProgram(intCode, 2).Result;

            return output.Single().ToString();
        }

        private IEnumerable<PointWithId> ScanUniverse(int xMax, int yMax)
        {
            for (int y = 0; y < yMax; ++y)
            {
                for (int x = 0; x < xMax; ++x)
                {
                    yield return ScanPoint(x, y).Result;
                }
            }
        }

        private async Task<PointWithId> ScanPoint(int x, int y)
        {
            var inputChannel = Channel.CreateUnbounded<long>();
            var computer = new IntCodeComputer(inputChannel);

            inputChannel.Writer.TryWrite(x);
            inputChannel.Writer.TryWrite(y);

            var output = await computer.RunIntCodeProgram(ParseInput().ToList()).ToListAsync();

            return new PointWithId(output.Single().ToString(), x, y);
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
