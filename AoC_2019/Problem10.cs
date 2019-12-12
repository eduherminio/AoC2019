using AoCHelper;
using FileParser;
using System.Collections.Generic;
using System.Linq;
using AoCHelper.Model;
using System;

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
            var asteroidLocations = ParseInput().ToList();

            Point laserLocation = ExtractOptimumLaserLocation(asteroidLocations);

            var unorderedLapGroups = ExtractOrderedListsOfUnorderedItems(laserLocation, asteroidLocations.Except(new[] { laserLocation }).ToList()).ToList();

            List<Point> orderedPoints = new List<Point>();

            foreach (var lapGroup in unorderedLapGroups)
            {
                var firstHalf = lapGroup.Where(point => point.X >= laserLocation.X).ToList();
                var firstQuadrant = firstHalf.Where(point => point.Y <= laserLocation.Y).ToList();
                var secondQuadrant = firstHalf.Except(firstQuadrant).ToList();
                var secondHalf = lapGroup.Except(firstHalf).ToList();
                var forthQuadrant = secondHalf.Where(point => laserLocation.Y >= point.Y).ToList();
                var thirdQuadrant = secondHalf.Except(forthQuadrant).ToList();

                var orderedFirstQuadrant = firstQuadrant
                    .OrderBy(point => CalculateAlphaAngle(point, laserLocation));

                orderedPoints.AddRange(orderedFirstQuadrant.ToList());

                var orderedSecondQuadrant = secondQuadrant.OrderByDescending(point => CalculateAlphaAngle(point, laserLocation));
                orderedPoints.AddRange(orderedSecondQuadrant.ToList());

                var orderedThirdQuadrant = thirdQuadrant.OrderBy(point => CalculateAlphaAngle(point, laserLocation));
                orderedPoints.AddRange(orderedThirdQuadrant.ToList());
                var orderedForthQuadrant = forthQuadrant.OrderByDescending(point => CalculateAlphaAngle(point, laserLocation));
                orderedPoints.AddRange(orderedForthQuadrant.ToList());
            }

            Point twoHundredth = orderedPoints[199];

            return (100 * twoHundredth.X + twoHundredth.Y).ToString();
        }

        private static double CalculateAlphaAngle(Point point, Point laserLocation)
        {
            return Math.Atan(Math.Abs((double)(point.X - laserLocation.X) / (point.Y - laserLocation.Y)));
        }

        private static Point ExtractOptimumLaserLocation(IEnumerable<Point> asteroidLocations)
        {
            var locations = asteroidLocations.Select(location =>
            {
                var lineGroups = asteroidLocations
                    .Except(new[] { location })
                    .GroupBy(asteroid => new Line(location, asteroid));

                var verticalLineGroup = lineGroups.SingleOrDefault(g => double.IsInfinity(g.Key.M));

                return
                     Tuple.Create(location,
                     lineGroups.Count(g => g.Any(p => p.X < location.X))
                     + lineGroups.Count(g => g.Any(p => p.X > location.X))
                     + (verticalLineGroup?.Any(p => p.Y > location.Y) == true ? 1 : 0)
                     + (verticalLineGroup?.Any(p => p.Y < location.Y) == true ? 1 : 0));
            });

            var tuple = locations
                .OrderByDescending(tuple => tuple.Item2)
                .First();

            return tuple.Item1;
        }

        private static ICollection<List<Point>> ExtractOrderedListsOfUnorderedItems(Point laserLocation, ICollection<Point> asteroidLocations)
        {
            List<List<Point>> lineGroups = asteroidLocations
                .Except(new[] { laserLocation })
                .GroupBy(location => new Line(laserLocation, location))
                .Select(group => group.OrderBy(p => p.DistanceTo(laserLocation)).ToList())
                .ToList();

            ICollection<List<Point>> result = new List<List<Point>>();

            for (int lapIndex = 0; lapIndex < lineGroups.Max(group => group.Count); ++lapIndex)
            {
                List<Point> list = new List<Point>();

                list.AddRange(
                    lineGroups
                        .Where(group => group.Count(point => point.X > laserLocation.X) > lapIndex)
                        .Select(group => group.Where(point => point.X > laserLocation.X)
                                                .ElementAt(lapIndex)));
                list.AddRange(
                    lineGroups
                        .Where(group => group.Count(point => point.X < laserLocation.X) > lapIndex)
                        .Select(group => group.Where(point => point.X < laserLocation.X)
                                                .ElementAt(lapIndex)));
                list.AddRange(
                    lineGroups
                        .Where(group => group.Count(point => point.Y > laserLocation.Y) > lapIndex && double.IsInfinity(new Line(group.First(), laserLocation).M))
                        .Select(group => group.Where(point => point.Y > laserLocation.Y && double.IsInfinity(new Line(group.First(), laserLocation).M))
                                                .ElementAt(lapIndex)));

                list.AddRange(
                    lineGroups
                    .Where(group => group.Count(point => point.Y < laserLocation.Y) > lapIndex && double.IsInfinity(new Line(group.First(), laserLocation).M))
                        .Select(group => group.Where(point => point.Y < laserLocation.Y && double.IsInfinity(new Line(group.First(), laserLocation).M))
                                                .ElementAt(lapIndex)));

                result.Add(list);
            }

            return result;
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

    public static class PointExtensions
    {
        public static double DistanceTo(this Point point, Point otherPoint)
        {
            return Math.Sqrt(
                Math.Pow(otherPoint.X - point.X, 2)
                + Math.Pow(otherPoint.Y - point.Y, 2));
        }
    }
}
