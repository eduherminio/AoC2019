using System;
using AoCHelper;
using FileParser;
using System.Collections.Generic;
using System.Linq;
using Priority_Queue;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using SheepTools.Model;

namespace AoC_2019
{
    public class Problem18 : BaseProblem
    {
        public override string Solve_1()
        {
            return "";

            var input = ParseInput().ToList();
            var pathOptions = input.Where(p => p.ContentType != ContentType.Wall).ToList();

            //EvaluateAlgorithmPerformance(BreathFirstAlgorithmPureStoringPaths, pathOptions);
            //EvaluateAlgorithmPerformance(BreathFirstAlgorithmOptimizedStoringPaths, pathOptions);
            //EvaluateAlgorithmPerformance(BreathFirstAlgorithmOptimizedStoringNodes, pathOptions);
            //EvaluateAlgorithmPerformance(DepthFirstAlgorithmPureStoringPaths, pathOptions);
            //EvaluateAlgorithmPerformance(DepthFirstAlgorithmOptimizedStoringPaths, pathOptions);
            //EvaluateAlgorithmPerformance(DepthFirstAlgorithmOptimizedStoringNodes, pathOptions);
            //EvaluateAlgorithmPerformance(AStarAlgorithmStoringPaths, pathOptions);
            EvaluateAlgorithmPerformance(AStarAlgorithmStoringNodes, pathOptions);

            int result = AStarAlgorithmStoringNodes(pathOptions);

            return result.ToString();
        }

        public override string Solve_2()
        {
            return "";
        }

        private static int DepthFirstAlgorithmPureStoringPaths(List<LocationPoint> emptyLocations)
        {
            int nKeysToCollect = emptyLocations.Count(p => p.ContentType == ContentType.Key);
            int existingDoors = emptyLocations.Count(p => p.ContentType == ContentType.Door);

            var startPoint = emptyLocations.Single(p => p.ContentType == ContentType.StartPoint);

            Path solution = null;

            Stack<Path> paths = new Stack<Path>();
            paths.Push(new Path(new Moment(startPoint, new HashSet<char>(nKeysToCollect), new HashSet<char>(existingDoors))));

            int stepCounter = 0;
            while (paths.Any())
            {
                TrackProgress(nKeysToCollect, paths, ref stepCounter);

                Path currentPath = paths.Pop();
                Moment currentMoment = currentPath.Moments.Last();

                if (currentMoment.Keys.Count == nKeysToCollect)
                {
                    if (!(currentPath.Moments.Count > solution?.Moments?.Count))
                    {
                        solution = currentPath;
                    }
                    continue;
                }

                if (currentMoment.Point.ContentType == ContentType.Key)
                {
                    currentMoment.Keys.Add(currentMoment.Point.Content);
                }

                var nextPointCandidates = emptyLocations
                    .Where(MovementCandidateCondition(currentMoment, currentPath))
                    .ToList();

                foreach (var candidate in nextPointCandidates)
                {
                    paths.Push(new Path(currentPath, new Moment(candidate, currentMoment.Keys, currentMoment.Doors)));
                }
            }

            if (solution == null)
            {
                throw new SolvingException($"{GetCurrentMethod()} wasn't able to find a solution");
            }

            Console.WriteLine($"{GetCurrentMethod()}: {solution.Moments.Count - 2} in {stepCounter}");

            return solution.Moments.Count - 2;
        }

        private static int DepthFirstAlgorithmOptimizedStoringPaths(List<LocationPoint> emptyLocations)
        {
            int nKeysToCollect = emptyLocations.Count(p => p.ContentType == ContentType.Key);
            int existingDoors = emptyLocations.Count(p => p.ContentType == ContentType.Door);

            var startPoint = emptyLocations.Single(p => p.ContentType == ContentType.StartPoint);

            Path solution = null;

            Stack<Path> paths = new Stack<Path>();
            paths.Push(new Path(new Moment(startPoint, new HashSet<char>(nKeysToCollect), new HashSet<char>(existingDoors))));

            int stepCounter = 0;
            while (paths.Any())
            {
                TrackProgress(nKeysToCollect, paths, ref stepCounter);

                Path currentPath = paths.Pop();

                if (currentPath.Moments.Count > solution?.Moments?.Count)
                {
                    continue;
                }

                Moment currentMoment = currentPath.Moments.Last();

                if (currentMoment.Keys.Count == nKeysToCollect)
                {
                    if (!(currentPath.Moments.Count > solution?.Moments?.Count))
                    {
                        solution = currentPath;
                    }
                    continue;
                }

                if (currentMoment.Point.ContentType == ContentType.Key)
                {
                    currentMoment.Keys.Add(currentMoment.Point.Content);
                }

                var nextPointCandidates = emptyLocations
                    .Where(MovementCandidateCondition(currentMoment, currentPath))
                    .ToList();

                foreach (var candidate in nextPointCandidates)
                {
                    paths.Push(new Path(currentPath, new Moment(candidate, currentMoment.Keys, currentMoment.Doors)));
                }
            }

            if (solution == null)
            {
                throw new SolvingException($"{GetCurrentMethod()} wasn't able to find a solution");
            }

            Console.WriteLine($"{GetCurrentMethod()}: {solution.Moments.Count - 2} in {stepCounter}");

            return solution.Moments.Count - 2;
        }

        private static int DepthFirstAlgorithmOptimizedStoringNodes(List<LocationPoint> emptyLocations)
        {
            var contiguousPoints = GenerateContiguousPointsDictionary(emptyLocations);

            int nKeysToCollect = emptyLocations.Count(p => p.ContentType == ContentType.Key);
            int existingDoors = emptyLocations.Count(p => p.ContentType == ContentType.Door);

            var startPoint = emptyLocations.Single(p => p.ContentType == ContentType.StartPoint);

            Moment solution = null;

            Stack<Moment> path = new Stack<Moment>();
            path.Push(new Moment(startPoint, new HashSet<char>(nKeysToCollect), new HashSet<char>(existingDoors)));

            int stepCounter = 0;
            while (path.Any())
            {
                Moment currentMoment = path.Pop();

                TrackProgress(nKeysToCollect, currentMoment, ref stepCounter);

                if (currentMoment.StepsFromOrigin > solution?.StepsFromOrigin)
                {
                    continue;
                }

                if (currentMoment.Keys.Count == nKeysToCollect)
                {
                    if (!(currentMoment.StepsFromOrigin > solution?.StepsFromOrigin))
                    {
                        solution = currentMoment;
                    }
                    continue;
                }

                if (currentMoment.Point.ContentType == ContentType.Key)
                {
                    currentMoment.Keys.Add(currentMoment.Point.Content);
                }

                var nextPointCandidates = contiguousPoints[currentMoment.Point]
                    .Where(MovementCandidateCondition(currentMoment))
                    .ToList();

                foreach (var candidate in nextPointCandidates)
                {
                    Moment newMoment = new Moment(candidate, currentMoment.Keys, currentMoment.Doors, currentMoment)
                    {
                        StepsFromOrigin = currentMoment.StepsFromOrigin + 1
                    };

                    path.Push(newMoment);
                }
            }

            List<Moment> solutionPath = new List<Moment>();
            while (solution != null)
            {
                solutionPath.Add(solution);
                solution = solution.Parent;
            }

            Console.WriteLine($"{GetCurrentMethod()}: {solutionPath.Count - 2} in {stepCounter}");

            return solutionPath.Count - 2;
        }

        private static int BreathFirstAlgorithmPureStoringPaths(List<LocationPoint> emptyLocations)
        {
            int nKeysToCollect = emptyLocations.Count(p => p.ContentType == ContentType.Key);
            int existingDoors = emptyLocations.Count(p => p.ContentType == ContentType.Door);

            var startPoint = emptyLocations.Single(p => p.ContentType == ContentType.StartPoint);

            Path solution = null;

            Queue<Path> paths = new Queue<Path>();
            paths.Enqueue(new Path(new Moment(startPoint, new HashSet<char>(nKeysToCollect), new HashSet<char>(existingDoors))));

            int stepCounter = 0;
            while (paths.Any())
            {
                TrackProgress(nKeysToCollect, paths, ref stepCounter);

                Path currentPath = paths.Dequeue();
                Moment currentMoment = currentPath.Moments.Last();

                if (currentMoment.Keys.Count == nKeysToCollect)
                {
                    if (!(currentPath.Moments.Count > solution?.Moments?.Count))
                    {
                        solution = currentPath;
                    }
                    continue;
                }

                if (currentMoment.Point.ContentType == ContentType.Key)
                {
                    currentMoment.Keys.Add(currentMoment.Point.Content);
                }

                var nextPointCandidates = emptyLocations
                    .Where(MovementCandidateCondition(currentMoment, currentPath))
                    .ToList();

                foreach (var candidate in nextPointCandidates)
                {
                    paths.Enqueue(new Path(currentPath, new Moment(candidate, currentMoment.Keys, currentMoment.Doors)));
                }
            }

            if (solution == null)
            {
                throw new SolvingException($"{GetCurrentMethod()} wasn't able to find a solution");
            }

            Console.WriteLine($"{GetCurrentMethod()}: {solution.Moments.Count - 2} in {stepCounter}");

            return solution.Moments.Count - 2;
        }

        private static int BreathFirstAlgorithmOptimizedStoringPaths(List<LocationPoint> emptyLocations)
        {
            int nKeysToCollect = emptyLocations.Count(p => p.ContentType == ContentType.Key);
            int existingDoors = emptyLocations.Count(p => p.ContentType == ContentType.Door);

            var startPoint = emptyLocations.Single(p => p.ContentType == ContentType.StartPoint);

            Path solution = null;

            Queue<Path> paths = new Queue<Path>();
            paths.Enqueue(new Path(new Moment(startPoint, new HashSet<char>(nKeysToCollect), new HashSet<char>(existingDoors))));

            int stepCounter = 0;
            while (paths.Any())
            {
                TrackProgress(nKeysToCollect, paths, ref stepCounter);

                Path currentPath = paths.Dequeue();

                if (currentPath.Moments.Count > solution?.Moments?.Count)
                {
                    continue;
                }

                Moment currentMoment = currentPath.Moments.Last();

                if (currentMoment.Keys.Count == nKeysToCollect)
                {
                    if (!(currentPath.Moments.Count > solution?.Moments?.Count))
                    {
                        solution = currentPath;
                    }
                    continue;
                }

                if (currentMoment.Point.ContentType == ContentType.Key)
                {
                    currentMoment.Keys.Add(currentMoment.Point.Content);
                }

                var nextPointCandidates = emptyLocations
                    .Where(MovementCandidateCondition(currentMoment, currentPath))
                    .ToList();

                foreach (var candidate in nextPointCandidates)
                {
                    paths.Enqueue(new Path(currentPath, new Moment(candidate, currentMoment.Keys, currentMoment.Doors)));
                }
            }

            if (solution == null)
            {
                throw new SolvingException($"{GetCurrentMethod()} wasn't able to find a solution");
            }

            Console.WriteLine($"{GetCurrentMethod()}: {solution.Moments.Count - 2} in {stepCounter}");

            return solution.Moments.Count - 2;
        }

        private static int BreathFirstAlgorithmOptimizedStoringNodes(List<LocationPoint> emptyLocations)
        {
            var contiguousPoints = GenerateContiguousPointsDictionary(emptyLocations);

            int nKeysToCollect = emptyLocations.Count(p => p.ContentType == ContentType.Key);
            int existingDoors = emptyLocations.Count(p => p.ContentType == ContentType.Door);

            var startPoint = emptyLocations.Single(p => p.ContentType == ContentType.StartPoint);

            Moment solution = null;

            Queue<Moment> path = new Queue<Moment>();
            path.Enqueue(new Moment(startPoint, new HashSet<char>(nKeysToCollect), new HashSet<char>(existingDoors)));

            int stepCounter = 0;
            while (path.Any())
            {
                Moment currentMoment = path.Dequeue();

                if (currentMoment.StepsFromOrigin >= solution?.StepsFromOrigin)
                {
                    continue;
                }

                TrackProgress(nKeysToCollect, currentMoment, ref stepCounter);

                if (currentMoment.Keys.Count == nKeysToCollect)
                {
                    if (!(currentMoment.StepsFromOrigin > solution?.StepsFromOrigin))
                    {
                        solution = currentMoment;
                    }
                    continue;
                }

                if (currentMoment.Point.ContentType == ContentType.Key)
                {
                    currentMoment.Keys.Add(currentMoment.Point.Content);
                }

                var nextPointCandidates = contiguousPoints[currentMoment.Point]
                    .Where(MovementCandidateCondition(currentMoment))
                    .ToList();

                foreach (var candidate in nextPointCandidates)
                {
                    Moment newMoment = new Moment(candidate, currentMoment.Keys, currentMoment.Doors, currentMoment)
                    {
                        StepsFromOrigin = currentMoment.StepsFromOrigin + 1
                    };

                    path.Enqueue(newMoment);
                }
            }

            if (solution == null)
            {
                throw new SolvingException($"{GetCurrentMethod()} wasn't able to find a solution");
            }

            List<Moment> solutionPath = new List<Moment>();
            while (solution != null)
            {
                solutionPath.Add(solution);
                solution = solution.Parent;
            }

            Console.WriteLine($"{GetCurrentMethod()}: {solutionPath.Count - 2} in {stepCounter}");

            return solutionPath.Count - 2;
        }

        private static int AStarAlgorithmStoringPaths(List<LocationPoint> emptyLocations)
        {
            int nKeysToCollect = emptyLocations.Count(p => p.ContentType == ContentType.Key);
            int existingDoors = emptyLocations.Count(p => p.ContentType == ContentType.Door);

            var startPoint = emptyLocations.Single(p => p.ContentType == ContentType.StartPoint);
            Path initialPath = new Path(new Moment(startPoint, new HashSet<char>(nKeysToCollect), new HashSet<char>(existingDoors)));

            SimplePriorityQueue<Path, double> paths = new SimplePriorityQueue<Path, double>();
            paths.Enqueue(initialPath, priority: nKeysToCollect);

            Dictionary<Path, int> costFromStart = new Dictionary<Path, int>
            {
                [initialPath] = 0
            };

            HashSet<Path> pathsToExpand = new HashSet<Path>()
            {
                initialPath
            };

            int stepCounter = 0;
            while (pathsToExpand.Any())
            {
                TrackProgress(nKeysToCollect, paths, ref stepCounter);

                Path currentPath = paths.First(p => pathsToExpand.Contains(p));

                pathsToExpand.Remove(currentPath);

                Moment currentMoment = currentPath.Moments.Last();

                if (currentMoment.Keys.Count == nKeysToCollect)
                {
                    int numberOfSteps = currentPath.Moments.Count - 2;

                    Console.WriteLine($"{GetCurrentMethod()}: {numberOfSteps} in {stepCounter}");

                    return numberOfSteps;
                }

                if (currentMoment.Point.ContentType == ContentType.Key)
                {
                    currentMoment.Keys.Add(currentMoment.Point.Content);
                }

                var nextPointCandidates = emptyLocations
                    .Where(MovementCandidateCondition(currentMoment, currentPath))
                    .ToList();

                foreach (var candidate in nextPointCandidates)
                {
                    var pathIncludingCandidate = new Path(currentPath, new Moment(candidate, currentMoment.Keys, currentMoment.Doors));
                    int newScoreForCandidate = costFromStart[currentPath] + 1;

                    if (!costFromStart.TryGetValue(pathIncludingCandidate, out int cost) || newScoreForCandidate < cost)
                    {
                        costFromStart[pathIncludingCandidate] = newScoreForCandidate;

                        var totalCost = newScoreForCandidate + EstimateCost(pathIncludingCandidate, nKeysToCollect);

                        paths.AddOrUpdatePriority(pathIncludingCandidate, totalCost);

                        pathsToExpand.Add(pathIncludingCandidate);
                    }
                }
            }

            throw new SolvingException($"{GetCurrentMethod()} wasn't able to find a solution");
        }

        private static int AStarAlgorithmStoringNodes(List<LocationPoint> emptyLocations)
        {
            var contiguousPoints = GenerateContiguousPointsDictionary(emptyLocations);

            int nKeysToCollect = emptyLocations.Count(p => p.ContentType == ContentType.Key);
            int existingDoors = emptyLocations.Count(p => p.ContentType == ContentType.Door);

            var startPoint = emptyLocations.Single(p => p.ContentType == ContentType.StartPoint);
            Moment initialNode = new Moment(startPoint, new HashSet<char>(nKeysToCollect), new HashSet<char>(existingDoors));

            SimplePriorityQueue<Moment, double> paths = new SimplePriorityQueue<Moment, double>();
            paths.Enqueue(initialNode, priority: nKeysToCollect);

            Dictionary<Moment, int> costFromStart = new Dictionary<Moment, int>
            {
                [initialNode] = 0
            };

            HashSet<Moment> momentsToExpand = new HashSet<Moment>()
            {
                initialNode
            };

            int stepCounter = 0;
            while (momentsToExpand.Any())
            {
                Moment currentMoment = paths.Dequeue();

                TrackProgress(nKeysToCollect, currentMoment, ref stepCounter);

                momentsToExpand.Remove(currentMoment);

                if (currentMoment.Keys.Count == nKeysToCollect)
                {
                    Moment solution = currentMoment;
                    List<Moment> solutionPath = new List<Moment>();

                    while (solution != null)
                    {
                        solutionPath.Add(solution);
                        solution = solution.Parent;
                    }

                    Console.WriteLine($"{GetCurrentMethod()}: {solutionPath.Count - 2} in {stepCounter}");

                    return solutionPath.Count - 2;
                }

                if (currentMoment.Point.ContentType == ContentType.Key)
                {
                    currentMoment.Keys.Add(currentMoment.Point.Content);
                }

                var nextPointCandidates = contiguousPoints[currentMoment.Point]
                    .Where(MovementCandidateCondition(currentMoment))
                    .ToList();

                foreach (var candidate in nextPointCandidates)
                {
                    var newMoment = new Moment(candidate, currentMoment.Keys, currentMoment.Doors, currentMoment);
                    int newScoreForCandidate = costFromStart[currentMoment] + 1;

                    if (!costFromStart.TryGetValue(newMoment, out int cost) || newScoreForCandidate < cost)
                    {
                        costFromStart[newMoment] = newScoreForCandidate;

                        var totalCost = newScoreForCandidate + EstimateCost(newMoment, nKeysToCollect);

                        paths.AddOrUpdatePriority(newMoment, totalCost);

                        momentsToExpand.Add(newMoment);
                    }
                }
            }

            throw new SolvingException($"{GetCurrentMethod()} wasn't able to find a solution");
        }

        private static Dictionary<LocationPoint, ICollection<LocationPoint>> GenerateContiguousPointsDictionary(List<LocationPoint> emptyLocations)
        {
            Dictionary<LocationPoint, ICollection<LocationPoint>> contiguousPoints = new Dictionary<LocationPoint, ICollection<LocationPoint>>();

            foreach (var point in emptyLocations)
            {
                contiguousPoints.Add(point, new List<LocationPoint>(4));
                foreach (var contiguousPoint in emptyLocations.Where(p => point.ManhattanDistance(p) == 1))
                {
                    contiguousPoints[point].Add(contiguousPoint);
                }
            }

            return contiguousPoints;
        }

        private static int EstimateCost(Path pathIncludingCandidate, int nKeysToCollect)
        {
            return EstimateCost(pathIncludingCandidate.Moments.Last(), nKeysToCollect);
        }

        private static int EstimateCost(Moment moment, int nKeysToCollect)
        {
            int newKeysFound = 0;
            if (moment.Point.ContentType == ContentType.Key && !moment.Keys.Contains(moment.Point.Content))
            {
                newKeysFound = 1;
            }

            return nKeysToCollect - moment.Keys.Count - newKeysFound;
        }

        private static Func<LocationPoint, bool> MovementCandidateCondition(Moment currentMoment, Path currentPath)
        {
            return p =>
                currentMoment.Point.ManhattanDistance(p) == 1
                && (p.ContentType != ContentType.Door || currentMoment.Keys.Contains(Char.ToLower(p.Content)))
                && (currentPath.Moments.Count < 2
                    || !currentPath.Moments[^2].Equals(new Moment(p, currentMoment.Keys, currentMoment.Doors)));
        }

        private static Func<LocationPoint, bool> MovementCandidateCondition(Moment currentMoment)
        {
            return p =>
            (p.ContentType != ContentType.Door || currentMoment.Keys.Contains(Char.ToLower(p.Content)))
                && (currentMoment.Parent?.Equals(new Moment(p, currentMoment.Keys, currentMoment.Doors)) != true);
        }

        #region Performance measurement

        private static void TrackProgress(int nKeysToCollect, IEnumerable<Path> paths, ref int stepCounter)
        {
            ++stepCounter;
            if (stepCounter % 100000 == 0)
            {
                Console.WriteLine($"{paths.Max(p => p.Moments.Last().Keys.Count)}/{nKeysToCollect} at step {stepCounter}");
            }
        }

        private static void TrackProgress(int nKeysToCollect, Moment moment, ref int stepCounter)
        {
            ++stepCounter;
            if (stepCounter % 10000 == 0)
            {
                Console.WriteLine($"{moment.Keys.Count}/{nKeysToCollect} at step {stepCounter}");
            }
        }

        private static void EvaluateAlgorithmPerformance(Func<List<LocationPoint>, int> algorithm, List<LocationPoint> input)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            algorithm.Invoke(input);
            watch.Stop();

            string elapsedTime = watch.ElapsedMilliseconds < 1000
                ? $"{watch.ElapsedMilliseconds} ms"
                : $"{0.001 * watch.ElapsedMilliseconds:F} s";

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{algorithm.Method.Name}: {elapsedTime}");
            Console.ResetColor();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static string GetCurrentMethod()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }

        #endregion

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

                    yield return new LocationPoint(counter.ToString(), x, y, ch, contentType);
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

    public record LocationPoint : Point
    {
        public char Content { get; }

        public ContentType ContentType { get; }

        public LocationPoint(string id, int x, int y, char content, ContentType contentType) : base(id, x, y)
        {
            Content = content;
            ContentType = contentType;
        }
    }

    public class Moment : IEquatable<Moment>
    {
        public LocationPoint Point { get; }

        public HashSet<char> Keys { get; }

        public HashSet<char> Doors { get; }

        public Moment Parent { get; set; }

        public int StepsFromOrigin { get; set; }

        public Moment(LocationPoint point, HashSet<char> keys, HashSet<char> doors)
            : this(point, keys, doors, null)
        {
        }

        public Moment(LocationPoint point, HashSet<char> keys, HashSet<char> doors, Moment parent)
        {
            Point = point;
            Keys = keys.ToHashSet();
            Doors = doors.ToHashSet();
            Parent = parent;
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

    public class Path : IEquatable<Path>
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

        #region Equals override

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is Path))
            {
                return false;
            }

            return Equals((Path)obj);
        }

        public bool Equals(Path other)
        {
            if (other == null)
            {
                return false;
            }

            return Moments.SequenceEqual(other.Moments);
        }

        public static bool operator ==(Path path1, Path path2)
        {
            if (path1 is null)
            {
                return path2 is null;
            }

            return path1.Equals(path2);
        }

        public static bool operator !=(Path path1, Path path2)
        {
            if (path1 is null)
            {
                return path2 is object;
            }

            return !path1.Equals(path2);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Moments);
        }

        #endregion
    }

    public static class PriorityQueueExtensions
    {
        public static void AddOrUpdatePriority<TItem, TPriority>(this SimplePriorityQueue<TItem, TPriority> queue, TItem item, TPriority priority)
            where TPriority : IComparable<TPriority>
        {
            if (!queue.TryUpdatePriority(item, priority))
            {
                queue.Enqueue(item, priority);
            }
        }
    }
}
