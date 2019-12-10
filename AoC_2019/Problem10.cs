using AoCHelper;
using FileParser;
using System.Collections.Generic;
using System.Linq;
using AoCHelper.Model;

namespace AoC_2019
{
    public class Problem10 : BaseProblem
    {
        public override string Solve_1()
        {
            var asteroidLocations = ParseInput().ToList();

            int maxNumberOfVisibleAsteroid = asteroidLocations.Max(location =>
            {
                var lineGroups = asteroidLocations
                    .Except(new[] { location })
                    .GroupBy(asteroid => new Line(location, asteroid));

                var verticalLineGroup = lineGroups.SingleOrDefault(g => double.IsInfinity(g.Key.M));

                return
                     lineGroups.Count(g => g.Any(p => p.X < location.X))
                     + lineGroups.Count(g => g.Any(p => p.X > location.X))
                     + (verticalLineGroup?.Any(p => p.Y > location.Y) == true ? 1 : 0)
                     + (verticalLineGroup?.Any(p => p.Y < location.Y) == true ? 1 : 0);
            });

            return maxNumberOfVisibleAsteroid.ToString();
        }

        public override string Solve_2()
        {
            var input = ParseInput().ToList();

            return "";
        }

        private IEnumerable<Point> ParseInput()
        {
            var file = new ParsedFile(FilePath);

            int y = 0;
            while (!file.Empty)
            {
                var line = file.NextLine().ToSingleString();
                for (int x = 0; x < line.Length; ++x)
                {
                    if (line[x] != '.')
                    {
                        yield return new Point(x, y);
                    }
                }

                ++y;
            }
        }
    }
}
