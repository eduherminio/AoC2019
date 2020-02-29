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

            ICollection<List<Point>> vaporizationLapGroups = GroupAsteroidsByVaporizationLap(laserLocation, asteroidLocations);

            List<Point> orderedPoints = OrderAndAppendVaporizationLapGroups(laserLocation, vaporizationLapGroups);

            Point twoHundredth = orderedPoints[199];

            return ((100 * twoHundredth.X) + twoHundredth.Y).ToString();
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

        /// <summary>
        /// Extracts the asteroids grouped by the lap when they're going to be vaporizated
        /// </summary>
        /// <param name="laserLocation"></param>
        /// <param name="asteroidLocations"></param>
        /// <returns></returns>
        private static ICollection<List<Point>> GroupAsteroidsByVaporizationLap(Point laserLocation, IEnumerable<Point> asteroidLocations)
        {
            var lineGroups = asteroidLocations
                .Except(new[] { laserLocation })
                .GroupBy(location => new Line(laserLocation, location))
                .Select(group => group.OrderBy(p => p.DistanceTo(laserLocation)));

            ICollection<List<Point>> result = new List<List<Point>>();

            for (int lapIndex = 0; lapIndex < lineGroups.Max(group => group.Count()); ++lapIndex)
            {
                List<Point> list = new List<Point>();

                // Points located on the right side of the laser location
                bool rightSidePredicate(Point point) => point.X > laserLocation.X;
                list.AddRange(
                    lineGroups
                        .Where(group => group.Count(rightSidePredicate) > lapIndex)
                        .Select(group => group.Where(rightSidePredicate).ElementAt(lapIndex)));

                // Points located on the left side of the laser location
                bool leftSidePredicate(Point point) => point.X < laserLocation.X;
                list.AddRange(
                    lineGroups
                        .Where(group => group.Count(leftSidePredicate) > lapIndex)
                        .Select(group => group.Where(leftSidePredicate)
                                                .ElementAt(lapIndex)));

                bool infiniteMPredicate(IEnumerable<Point> group) => double.IsInfinity(new Line(group.First(), laserLocation).M);

                // Points located above the laser location
                bool abovePredicate(Point point) => point.Y > laserLocation.Y;
                list.AddRange(
                    lineGroups
                        .Where(group => group.Count(abovePredicate) > lapIndex && infiniteMPredicate(group))
                        .Select(group => group.Where(point => abovePredicate(point) && infiniteMPredicate(group))
                                                .ElementAt(lapIndex)));

                // Points located below the laser location
                bool belowPredicate(Point point) => point.Y < laserLocation.Y;
                list.AddRange(
                    lineGroups
                    .Where(group => group.Count(belowPredicate) > lapIndex && infiniteMPredicate(group))
                        .Select(group => group.Where(point => belowPredicate(point) && infiniteMPredicate(group))
                                                .ElementAt(lapIndex)));

                result.Add(list);
            }

            return result;
        }

        /// <summary>
        /// Orders the asteroids within each one of the laps and returns all of them in vaporization order
        /// </summary>
        /// <param name="laserLocation"></param>
        /// <param name="vaporizationLapGroups">Asteroids grouped by the lap when they're going to be vaporizated, but unordered</param>
        /// <returns></returns>
        private static List<Point> OrderAndAppendVaporizationLapGroups(Point laserLocation, IEnumerable<IEnumerable<Point>> vaporizationLapGroups)
        {
            List<Point> orderedPoints = new List<Point>();

            foreach (var lapGroup in vaporizationLapGroups)
            {
                var firstHalf = lapGroup.Where(point => point.X >= laserLocation.X);
                var firstQuadrant = firstHalf.Where(point => point.Y <= laserLocation.Y);
                var secondQuadrant = firstHalf.Except(firstQuadrant);
                var secondHalf = lapGroup.Except(firstHalf);
                var forthQuadrant = secondHalf.Where(point => laserLocation.Y >= point.Y);
                var thirdQuadrant = secondHalf.Except(forthQuadrant);

                var orderedFirstQuadrant = firstQuadrant.OrderBy(point => CalculateAlphaAngle(point, laserLocation));
                orderedPoints.AddRange(orderedFirstQuadrant.ToList());

                var orderedSecondQuadrant = secondQuadrant.OrderByDescending(point => CalculateAlphaAngle(point, laserLocation));
                orderedPoints.AddRange(orderedSecondQuadrant.ToList());

                var orderedThirdQuadrant = thirdQuadrant.OrderBy(point => CalculateAlphaAngle(point, laserLocation));
                orderedPoints.AddRange(orderedThirdQuadrant.ToList());

                var orderedForthQuadrant = forthQuadrant.OrderByDescending(point => CalculateAlphaAngle(point, laserLocation));
                orderedPoints.AddRange(orderedForthQuadrant.ToList());
            }

            return orderedPoints;
        }

        private static double CalculateAlphaAngle(Point point, Point laserLocation)
        {
            return Math.Atan(Math.Abs((double)(point.X - laserLocation.X) / (point.Y - laserLocation.Y)));
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
