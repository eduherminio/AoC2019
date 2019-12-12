using System;
using AoCHelper;
using FileParser;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using AoC_2019.IntCode;
using AoCHelper.Model;
using System.Text;

namespace AoC_2019
{
    public class Problem11 : BaseProblem
    {
        public override string Solve_1()
        {
            var input = ParseInput().ToList();

            var result = EstimatePanelsToPaint(Color.Black, input).Result;

            return result.ToString();
        }

        public override string Solve_2()
        {
            var input = ParseInput().ToList();

            var result = PrintPaintedPanels(Color.White, input).Result;

            return result;
        }

        private async Task<int> EstimatePanelsToPaint(Color initialColor, List<long> input)
        {
            var visitedPoints = await new Robot(initialColor).Run(input).ConfigureAwait(false);

            return visitedPoints.Count;
        }

        private async Task<string> PrintPaintedPanels(Color initialColor, List<long> input)
        {
            var visitedPoints = await new Robot(initialColor).Run(input).ConfigureAwait(false);

            StringBuilder sb = new StringBuilder(Environment.NewLine);
            for (int y = visitedPoints.Keys.Min(p => p.Y); y <= visitedPoints.Keys.Max(p => p.Y); ++y)
            {
                sb.Append(Environment.NewLine);
                for (int x = visitedPoints.Keys.Min(p => p.X); x <= visitedPoints.Keys.Max(p => p.X); ++x)
                {
                    sb.Append(visitedPoints.TryGetValue(new Point(x, y), out Color color) && color == Color.White
                        ? "o"
                        : " ");
                }
            }

            return sb.ToString();
        }

        private IEnumerable<long> ParseInput()
        {
            return new ParsedFile(FilePath)
                .ToSingleString()
                .Split(',')
                .Select(long.Parse);
        }
    }

    public class Robot
    {
        private readonly StateMachine _stateMachine;
        private readonly IntCodeComputer _computer;
        private readonly Channel<long> _inputReader;
        private readonly Dictionary<Point, Color> _visitedPoints;

        public Color InitialColor { get; }

        public Robot(Color initialColor)
        {
            InitialColor = initialColor;
            _inputReader = Channel.CreateUnbounded<long>();
            _computer = new IntCodeComputer(_inputReader);
            _stateMachine = new StateMachine(Direction.Up);
            _visitedPoints = new Dictionary<Point, Color>();
        }

        public async Task<Dictionary<Point, Color>> Run(List<long> intCode)
        {
            Point initialPoint = new Point(0, 0);
            Color initialColor = InitialColor;

            Point nextPoint = initialPoint;

            bool nextDirectionOutput = false;
            await _inputReader.Writer.WriteAsync((int)initialColor).ConfigureAwait(false);

            await foreach (var item in _computer.RunIntCodeProgram(intCode))
            {
                if (item != 0 && item != 1)
                {
                    throw new SolvingException("IntCode computer is expected to return boolean values");
                }

                if (nextDirectionOutput)
                {
                    nextPoint = await MovetoNextPanel(nextPoint, item).ConfigureAwait(false);
                }
                else
                {
                    PaintPanel(nextPoint, item);
                }

                nextDirectionOutput ^= true;
            }

            return _visitedPoints;
        }

        private async Task<Point> MovetoNextPanel(Point nextPoint, long item)
        {
            Direction dir = _stateMachine.ChangeState(isClockWise: item != 0);

            //Point lastPoint = VisitedPoints.Last();       // Do NOT do this
            Point lastPoint = nextPoint;

            nextPoint = (dir switch
            {
                Direction.Up => new Point(lastPoint.X, lastPoint.Y - 1),
                Direction.Down => new Point(lastPoint.X, lastPoint.Y + 1),
                Direction.Right => new Point(lastPoint.X + 1, lastPoint.Y),
                Direction.Left => new Point(lastPoint.X - 1, lastPoint.Y),
                _ => throw new SolvingException($"Unhandled direction: {dir}")
            });

            Color existingColor = _visitedPoints.TryGetValue(nextPoint, out Color knownColor)
                ? knownColor
                : InitialColor;

            await _inputReader.Writer.WriteAsync((int)existingColor).ConfigureAwait(false);

            return nextPoint;
        }

        private void PaintPanel(Point nextPoint, long item)
        {
            _visitedPoints[nextPoint] = (Color)item;
        }
    }

    public enum Direction
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3
    }

    public enum Color
    {
        Black = 0,
        White = 1
    }

    public class StateMachine
    {
        public Direction State { get; private set; }

        public StateMachine(Direction initialDirection)
        {
            State = initialDirection;
        }

        public Direction ChangeState(bool isClockWise)
        {
            int newState = isClockWise
                ? (int)State + 1
                : (int)State - 1;

            if (newState > 3)
            {
                newState = 0;
            }
            else if (newState < 0)
            {
                newState = 3;
            }

            State = (Direction)(newState);

            return State;
        }
    }
}
