using System;
using AoCHelper;
using FileParser;
using System.Collections.Generic;
using System.Linq;
using MersenneTwister;

namespace AoC_2019
{
    public class Problem18 : BaseProblem
    {
        public override string Solve_1()
        {
            var input = ParseInput().ToList();

            int result = DepthFirstAlgorithm(input.Where(p => p.ContentType != ContentType.Wall).ToList());

            return result.ToString();
        }

        public override string Solve_2()
        {
            var input = ParseInput();

            return "";
        }

        private static int DepthFirstAlgorithm(List<LocationPoint> emptyLocations)
        {
            int keysToCollect = emptyLocations.Count(p => p.ContentType == ContentType.Key);

            List<Path> solutions = new List<Path>();
            Queue<Path> paths = new Queue<Path>();

            var startPoint = emptyLocations.Single(p => p.ContentType == ContentType.StartPoint);
            paths.Enqueue(new Path(new Moment(startPoint, new HashSet<string>(), new HashSet<string>())));

            int stepCounter = 0;
            while (paths.Any())
            {
                ++stepCounter;
                if (stepCounter % 100 == 0)
                {
                    Console.WriteLine($"{paths.Max(p => p.Moments.Last().Keys.Count)}/{keysToCollect}");
                }

                Path currentPath = paths.First();
                Moment currentMoment = currentPath.Moments.Last();

                if (currentMoment.Keys.Count == keysToCollect)
                {
                    solutions.Add(currentPath);
                    paths.Dequeue();
                    continue;
                }

                if (currentMoment.Point.ContentType == ContentType.Key)
                {
                    currentMoment.Keys.Add(currentMoment.Point.Content);
                }

                var nextPointCandidates = emptyLocations
                    .Where(MovementCandidateCondition(currentMoment, currentPath))
                    .ToList();

                if (nextPointCandidates.Any())
                {
                    foreach (var candidate in nextPointCandidates.Skip(1))
                    {
                        paths.Enqueue(new Path(currentPath, new Moment(candidate, currentMoment.Keys, currentMoment.Doors)));
                    }

                    currentPath.Moments.Add(new Moment(nextPointCandidates.First(), currentMoment.Keys, currentMoment.Doors));
                }
                else
                {
                    paths.Dequeue();
                }
            }

            return solutions.Min(p => p.Moments.Count) - 2;
        }

        private static Func<LocationPoint, bool> MovementCandidateCondition(Moment currentMoment, Path currentPath)
        {
            return p =>
                currentMoment.Point.ManhattanDistance(p) == 1
                && (p.ContentType != ContentType.Door || currentMoment.Keys.Contains(p.Content.ToLower()))
                && (currentPath.Moments.Count < 2
                    || !currentPath.Moments[currentPath.Moments.Count - 2].Equals(new Moment(p, currentMoment.Keys, currentMoment.Doors)));
        }

        private IEnumerable<LocationPoint> ParseInput()
        {
            var file = new ParsedFile(FilePath);

            int counter = 0;
            int y = 0;
            while (!file.Empty)
            {
                var line = file.NextLine();
                int x = 0;
                while (!line.Empty)
                {
                    char ch = line.NextElement<char>();

                    ContentType contentType = ch switch
                    {
                        '@' => ContentType.StartPoint,
                        '#' => ContentType.Wall,
                        '.' => ContentType.Empty,
                        _ => ContentType.Door
                    };

                    if (contentType == ContentType.Door)
                    {
                        contentType = char.IsUpper(ch)
                            ? ContentType.Door
                            : ContentType.Key;
                    }

                    yield return new LocationPoint(counter.ToString(), x, y, ch.ToString(), contentType);
                    ++counter;
                    ++x;
                }

                ++y;
            }
        }
    }

    public enum ContentType
    {
        StartPoint,
        Wall,
        Empty,
        Key,
        Door
    }

    public class LocationPoint : Model.PointWithId
    {
        public string Content { get; }

        public ContentType ContentType { get; }

        public LocationPoint(string id, int x, int y, string content, ContentType contentType) : base(id, x, y)
        {
            Content = content;
            ContentType = contentType;
        }
    }

    public class Moment : IEquatable<Moment>
    {
        public LocationPoint Point { get; }

        public HashSet<string> Keys { get; }

        public HashSet<string> Doors { get; }

        public Moment(LocationPoint point, HashSet<string> keys, HashSet<string> doors)
        {
            Point = point;
            Keys = keys.ToHashSet();
            Doors = doors.ToHashSet();
        }

        #region Equals override

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is Moment))
            {
                return false;
            }

            return Equals((Moment)obj);
        }

        public bool Equals(Moment other)
        {
            if (other == null)
            {
                return false;
            }

            return Point.Equals(other.Point)
                && Doors.SequenceEqual(other.Doors)
                && Keys.SequenceEqual(other.Keys);
        }

        public static bool operator ==(Moment moment1, Moment moment2)
        {
            if (moment1 is null)
            {
                return moment2 is null;
            }

            return moment1.Equals(moment2);
        }

        public static bool operator !=(Moment moment1, Moment moment2)
        {
            if (moment1 is null)
            {
                return moment2 is object;
            }

            return !moment1.Equals(moment2);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Point, Keys, Doors);
        }

        #endregion
    }

    public class Path
    {
        public List<Moment> Moments { get; set; }

        public bool Visited { get; set; }

        /// <summary>
        /// 'Clone' constructor
        /// </summary>
        /// <param name="path"></param>
        /// <param name="moment"></param>
        public Path(Path path, Moment moment)
        {
            //Moments = new List<Moment>();

            //foreach (Moment m in path.Moments)
            //{
            //    Moments.Add(new Moment(
            //        m.Point,
            //        ((string[])m.Keys.ToArray().Clone()).ToHashSet(),
            //        ((string[])m.Doors.ToArray().Clone()).ToHashSet()));
            //}

            //Moments.Add(moment);

            Moments = new List<Moment>(path.Moments)
            {
                moment
            };
            Visited = false;
        }

        public Path(Moment moment)
        {
            Moments = new List<Moment> { moment };
            Visited = false;
        }
    }
}
